using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using Cubizer.Protocol.Extensions;

namespace Cubizer.Protocol.Serialization
{
	public class NetworkReader : BinaryReader
	{
		public NetworkReader(Stream stream)
			: base(stream)
		{
		}

		public NetworkReader(Stream stream, Encoding encoding)
			: base(stream, encoding)
		{
		}

		public NetworkReader(Stream stream, Encoding encoding, bool leaveOpen)
			: base(stream, encoding, leaveOpen)
		{
		}

		public override bool ReadBoolean() => base.ReadBoolean();
		public override byte ReadByte() => base.ReadByte();
		public override sbyte ReadSByte() => base.ReadSByte();
		public override byte[] ReadBytes(int count) => base.ReadBytes(count);
		public override short ReadInt16() => (Int16)base.ReadInt16().ToBigEndian();
		public override int ReadInt32() => (int)base.ReadInt32().ToBigEndian();
		public override long ReadInt64() => (long)base.ReadInt64().ToBigEndian();
		public override ushort ReadUInt16() => base.ReadUInt16().ToBigEndian();
		public override uint ReadUInt32() => this.ReadVarInt();
		public override ulong ReadUInt64() => base.ReadUInt64().ToBigEndian();
		public override float ReadSingle() => BitConverter.ToSingle(BitConverter.GetBytes(base.ReadUInt32().ToBigEndian()), 0);
		public override double ReadDouble() => BitConverter.Int64BitsToDouble((long)base.ReadInt64().ToBigEndian());

		public Guid ReadGuid()
		{
			Guid guid;
			if (!Guid.TryParse(base.ReadString(), out guid))
				throw new InvalidDataException("Invalid Guid");

			return guid;
		}

		public uint ReadVarInt()
		{
			byte numRead = 0;
			return ReadVarInt(out numRead);
		}

		public uint ReadVarInt(out byte numRead)
		{
			numRead = 0;
			int result = 0;
			byte read;
			do
			{
				read = ReadByte();
				int value = (read & 0x7F);
				result |= (value << (7 * numRead));

				numRead++;
				if (numRead > 5)
				{
					throw new InvalidDataException("VarInt is too big");
				}
			} while ((read & 0x80) != 0);

			return (uint)result;
		}

		public ulong ReadVarLong()
		{
			byte numRead = 0;
			return ReadVarLong(out numRead);
		}

		public ulong ReadVarLong(out byte numRead)
		{
			numRead = 0;
			ulong result = 0;
			uint read;
			do
			{
				read = ReadByte();
				uint value = (read & 0x7F);
				result |= (value << (7 * numRead));

				numRead++;
				if (numRead > 10)
				{
					throw new InvalidDataException("VarLong is too big");
				}
			} while ((read & 0x80) != 0);

			return result;
		}

		public override string ReadString()
		{
			var length = (int)this.ReadUInt32();
			if (length <= short.MaxValue)
			{
				var bytes = base.ReadBytes(length);
				return Encoding.UTF8.GetString(bytes);
			}
			else
			{
				throw new InvalidDataException($"String is too big .length:{length}.");
			}
		}

		public void ReadPakcets<T>(IReadOnlyList<T> array)
			where T : IPacketSerializable
		{
			foreach (var it in array)
				it.Deserialize(this);
		}
	}
}