using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Cubizer.Net.Protocol.Extensions;
using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol
{
	public sealed class CompressedPacket
	{
		public uint packetId;

		public ArraySegment<byte> data;

		public CompressedPacket()
		{
			this.packetId = 0;
		}

		public CompressedPacket(uint id, ArraySegment<byte> data)
		{
			this.packetId = id;
			this.data = data;
		}

		public void Serialize(Stream stream)
		{
			var length = (uint)data.Count + packetId.SizeofBytes();

			using (var bw = new NetworkWrite(stream, Encoding.UTF8, true))
			{
				bw.WriteVarInt(length);
				bw.WriteVarInt(packetId);
				bw.Flush();
			}

			stream.Write(data.Array, data.Offset, data.Count);
		}

		public async Task SerializeAsync(Stream stream)
		{
			var length = (uint)data.Count + packetId.SizeofBytes();

			using (var bw = new NetworkWrite(stream, Encoding.UTF8, true))
			{
				bw.WriteVarInt(length);
				bw.WriteVarInt(packetId);
				bw.Flush();
			}

			await stream.WriteAsync(data.Array, data.Offset, data.Count);
		}

		public int Deserialize(Stream stream, int maxLength = ushort.MaxValue)
		{
			using (var br = new NetworkReader(stream, Encoding.UTF8, true))
			{
				var length = (int)br.ReadVarInt();

				if (length > 0 && length < maxLength)
				{
					byte packLength;
					packetId = br.ReadVarInt(out packLength);

					data = new ArraySegment<byte>(new byte[length - packLength]);
					return stream.Read(data.Array, data.Offset, data.Count);
				}

				return length;
			}
		}

		public async Task<int> DeserializeAsync(Stream stream, int maxLength = ushort.MaxValue)
		{
			using (var br = new NetworkReader(stream, Encoding.UTF8, true))
			{
				var length = (int)br.ReadVarInt();

				if (length > 0 && length < maxLength)
				{
					byte packLength;
					packetId = br.ReadVarInt(out packLength);
					data = new ArraySegment<byte>(new byte[length - packLength]);

					return await stream.ReadAsync(data.Array, data.Offset, data.Count);
				}

				return length;
			}
		}
	}
}