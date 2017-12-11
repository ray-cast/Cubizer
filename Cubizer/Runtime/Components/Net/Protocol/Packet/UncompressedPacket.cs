using System;
using System.IO;
using System.Text;

namespace Cubizer.Protocol
{
	public class UncompressedPacket
	{
		public uint length;
		public uint packetId;

		public ArraySegment<byte> data;

		public UncompressedPacket()
		{
			this.length = 0;
			this.packetId = 0;
		}

		public UncompressedPacket(uint length, uint id, ArraySegment<byte> data)
		{
			this.length = length;
			this.packetId = id;
			this.data = data;
		}

		public void Serialize(Stream stream)
		{
			length = (uint)data.Count + sizeof(uint);

			using (var bw = new NetworkWrite(stream, Encoding.UTF8, true))
			{
				bw.WriteVarInt(length);
				bw.WriteVarInt(packetId);
				bw.Flush();

				stream.Write(data.Array, data.Offset, data.Count);
			}
		}

		public int Deserialize(Stream stream, int maxLength = ushort.MaxValue)
		{
			using (var br = new BinaryReader(stream, Encoding.UTF8, true))
			{
				length = br.ReadUInt32();
				packetId = br.ReadUInt32();
			}

			if (length > sizeof(uint) && length < maxLength)
			{
				data = new ArraySegment<byte>(new byte[length - sizeof(uint)]);
				return stream.Read(data.Array, data.Offset, data.Count);
			}

			return 0;
		}
	}
}