using System;
using System.IO;
using System.Text;

namespace Cubizer.Protocol
{
	public class UncompressedPacket
	{
		public uint packetId;

		public ArraySegment<byte> data;

		public UncompressedPacket()
		{
			this.packetId = 0;
		}

		public UncompressedPacket(uint id, ArraySegment<byte> data)
		{
			this.packetId = id;
			this.data = data;
		}

		public void Serialize(Stream stream)
		{
			var length = (uint)data.Count + sizeof(uint);

			using (var bw = new NetworkWrite(stream, Encoding.UTF8, true))
			{
				bw.WriteVarInt(length);
				bw.WriteVarInt(packetId);
				bw.BaseStream.Write(data.Array, data.Offset, data.Count);
				bw.Flush();
			}
		}

		public int Deserialize(Stream stream, int maxLength = ushort.MaxValue)
		{
			using (var br = new BinaryReader(stream, Encoding.UTF8, true))
			{
				var length = br.ReadInt32();

				if (length > sizeof(uint) && length < maxLength)
				{
					packetId = br.ReadUInt32();

					data = new ArraySegment<byte>(new byte[length - sizeof(uint)]);
					return stream.Read(data.Array, data.Offset, data.Count);
				}

				return length;
			}
		}
	}
}