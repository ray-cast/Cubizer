using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Cubizer.Protocol
{
	public class NetworkWrite : IDisposable
	{
		public readonly BinaryWriter bw;

		public NetworkWrite(Stream stream)
		{
			bw = new BinaryWriter(stream);
		}

		public NetworkWrite(Stream stream, Encoding encoding, bool leaveOpen)
		{
			bw = new BinaryWriter(stream, encoding, leaveOpen);
		}

		public void Close() => bw.Close();
		public void Flush() => bw.Flush();
		public void Dispose() => bw.Dispose();

		public void Write(bool value) => bw.Write(value);
		public void Write(byte value) => bw.Write(value);
		public void Write(byte[] value) => bw.Write(value);
		public void Write(short value) => bw.Write(value.ToBigEndian());
		public void Write(int value) => bw.Write(value.ToBigEndian());
		public void Write(long value) => bw.Write(value.ToBigEndian());
		public void Write(ushort value) => bw.Write(value.ToBigEndian());
		public void Write(ulong value) => bw.Write(value.ToBigEndian());
		public void Write(float value) => bw.Write(BitConverter.ToUInt32(BitConverter.GetBytes(value), 0).ToBigEndian());
		public void Write(double value) => bw.Write(BitConverter.DoubleToInt64Bits(value).ToBigEndian());
		public void Write(Guid value) => bw.Write(value.ToByteArray());

		public void WriteVarInt(uint value)
		{
			do
			{
				byte temp = (byte)(value & 0x7F);
				value >>= 7;
				if (value != 0)
					temp |= 0x80;
				bw.Write(temp);
			}
			while (value != 0);
		}

		public void writeVarLong(long value)
		{
			do
			{
				byte temp = (byte)(value & 0x7F);

				value >>= 7;
				if (value != 0)
				{
					temp |= 0x80;
				}
				bw.Write(temp);
			} while (value != 0);
		}

		public void Write(string value)
		{
			var bytes = Encoding.UTF8.GetBytes(value);
			this.WriteVarInt((uint)bytes.Length);
			bw.Write(bytes);
		}

		public void Write<T>(IReadOnlyList<T> array)
			where T : ISerializablePacket
		{
			foreach (var item in array)
				item.Serialize(this);
		}
	}
}