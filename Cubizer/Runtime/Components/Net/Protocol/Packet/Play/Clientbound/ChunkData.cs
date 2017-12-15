using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public sealed class ChunkData : IPacketSerializable
	{
		public const int Packet = 0x20;

		public int chunkX;
		public int chunkZ;
		public bool groundUpContinuous;
		public uint primaryBitMask;
		public byte[] data;
		public byte[] entities;

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
			bw.Write(groundUpContinuous);
			bw.WriteVarInt(primaryBitMask);
			bw.WriteVarInt(data.Length);
			bw.Write(data);
			bw.WriteVarInt(entities.Length);
			bw.Write(entities);
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out chunkX);
			br.Read(out chunkZ);
			br.Read(out groundUpContinuous);
			br.ReadVarInt(out primaryBitMask);
			br.ReadVarBytes(out data, int.MaxValue);
			br.ReadVarBytes(out entities);
		}

		public object Clone()
		{
			return new ChunkData();
		}
	}
}