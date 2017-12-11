using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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

			using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
			{
				bw.Write(length);
				bw.Write(packetId);
				bw.Flush();
			}

			stream.Write(data.Array, data.Offset, data.Count);
		}

		public async Task SerializeAsync(Stream stream)
		{
			length = (uint)data.Count + sizeof(uint);

			using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
			{
				bw.Write(length);
				bw.Write(packetId);
				bw.Flush();
			}

			await stream.WriteAsync(data.Array, data.Offset, data.Count);
		}

		public int Deserialize(Stream stream)
		{
			using (var br = new BinaryReader(stream, Encoding.UTF8, true))
			{
				length = br.ReadUInt32();
				packetId = br.ReadUInt32();
			}

			if (length > 0)
			{
				data = new ArraySegment<byte>(new byte[length + sizeof(uint)]);
				return stream.Read(data.Array, data.Offset, data.Count);
			}

			return 0;
		}

		public async Task<int> DeserializeAsync(Stream stream)
		{
			using (var br = new BinaryReader(stream, Encoding.UTF8, true))
			{
				length = br.ReadUInt32();
				packetId = br.ReadUInt32();
			}

			if (length > 0)
			{
				data = new ArraySegment<byte>(new byte[length + sizeof(uint)]);
				return await stream.ReadAsync(data.Array, data.Offset, data.Count);
			}

			return 0;
		}
	}
}