using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public sealed class ChunkUnload : IPacketSerializable
	{
		public const int Packet = 0x1D;

		public int chunkX;
		public int chunkZ;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(chunkX);
			bw.Write(chunkZ);
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out chunkX);
			br.Read(out chunkZ);
		}

		public object Clone()
		{
			return new ChunkUnload();
		}
	}
}