using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using Cubizer.Protocol.Extensions;

namespace Cubizer.Protocol.Serialization
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
		public override void Write(byte[] value) => base.Write(value);
		public override void Write(short value) => base.Write(value.ToBigEndian());
		public override void Write(int value) => base.Write(value.ToBigEndian());
		public override void Write(uint value) => this.WriteVarInt(value);
		public override void Write(long value) => base.Write(value.ToBigEndian());
		public override void Write(ushort value) => base.Write(value.ToBigEndian());
		public override void Write(ulong value) => base.Write(value.ToBigEndian());
		public override void Write(float value) => base.Write(BitConverter.ToUInt32(BitConverter.GetBytes(value), 0).ToBigEndian());
		public override void Write(double value) => base.Write(BitConverter.DoubleToInt64Bits(value).ToBigEndian());
		public void Write(Guid value) => base.Write(value.ToByteArray());

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

		public override void Write(string value)
		{
			var bytes = Encoding.UTF8.GetBytes(value);
			if (bytes.Length <= short.MaxValue)
			{
				this.WriteVarInt((uint)bytes.Length);
				base.Write(bytes);
			}
			else
			{
				throw new InvalidDataException($"String is too big .length:{value.Length}.");
			}
		}

		public void Write<T>(IReadOnlyList<T> array)
			where T : IPacketSerializable
		{
			foreach (var it in array)
				it.Serialize(this);
		}
	}
}