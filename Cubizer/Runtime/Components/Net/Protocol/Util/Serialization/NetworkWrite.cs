using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using Cubizer.Net.Protocol.Struct;
using Cubizer.Net.Protocol.Extensions;

namespace Cubizer.Net.Protocol.Serialization
{
	public class NetworkWrite : BinaryWriter
	{
		public NetworkWrite(Stream stream)
			: base(stream)
		{
		}

		public NetworkWrite(Stream stream, Encoding encoding)
			: base(stream, encoding)
		{
		}

		public NetworkWrite(Stream stream, Encoding encoding, bool leaveOpen)
			: base(stream, encoding, leaveOpen)
		{
		}

		public override void Write(bool value) => base.Write(value);
		public override void Write(byte value) => base.Write(value);
		public override void Write(sbyte value) => base.Write(value);
		public override void Write(char value) => base.Write(value);
		public override void Write(string value) => base.Write(value);
		public override void Write(byte[] value) => base.Write(value);
		public override void Write(byte[] value, int index, int count) => base.Write(value, index, count);
		public override void Write(char[] value, int index, int count) => base.Write(value, index, count);
		public override void Write(short value) => base.Write(value.ToBigEndian());
		public override void Write(int value) => base.Write(value.ToBigEndian());
		public override void Write(uint value) => this.WriteVarInt(value);
		public override void Write(long value) => base.Write(value.ToBigEndian());
		public override void Write(ushort value) => base.Write(value.ToBigEndian());
		public override void Write(ulong value) => base.Write(value.ToBigEndian());
		public override void Write(float value) => base.Write(BitConverter.ToUInt32(BitConverter.GetBytes(value), 0).ToBigEndian());
		public override void Write(double value) => base.Write(BitConverter.DoubleToInt64Bits(value).ToBigEndian());
		public void Write(Guid value) => base.Write(value.ToByteArray());

		public void WriteVarInt(int value)
		{
			this.WriteVarInt((uint)value);
		}

		public void WriteVarInt(uint value)
		{
			do
			{
				byte temp = (byte)(value & 0x7F);
				value >>= 7;
				if (value != 0)
					temp |= 0x80;

				base.Write(temp);
			}
			while (value != 0);
		}

		public void WriteVarLong(long value)
		{
			do
			{
				byte temp = (byte)(value & 0x7F);

				value >>= 7;
				if (value != 0)
					temp |= 0x80;

				base.Write(temp);
			} while (value != 0);
		}

		public void WriteVarString(string value, int maxLength = short.MaxValue)
		{
			var bytes = Encoding.UTF8.GetBytes(value);
			if (bytes.Length <= maxLength)
			{
				this.WriteVarInt((uint)bytes.Length);
				base.Write(bytes);
			}
			else
			{
				throw new InvalidDataException($"String is too big .length:{value.Length}.");
			}
		}

		public void WriteVarBytes(byte[] value)
		{
			this.WriteVarInt((uint)value.Length);
			this.Write(value);
		}

		public void WritePos(Vector3Int value)
		{
			this.Write((ulong)((uint)value.x & 0x3FFFFFF) << 38 | (ulong)((uint)value.y & 0xFFF) << 26 | (ulong)((uint)value.z & 0x3FFFFFF));
		}

		public void WritePakcets<T>(IReadOnlyList<T> array)
			where T : IPacketSerializable
		{
			foreach (var it in array)
				it.Serialize(this);
		}
	}
}