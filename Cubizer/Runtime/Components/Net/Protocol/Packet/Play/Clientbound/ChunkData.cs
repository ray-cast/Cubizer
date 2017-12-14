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
			chunkX = br.ReadInt32();
			chunkZ = br.ReadInt32();
			groundUpContinuous = br.ReadBoolean();
			primaryBitMask = br.ReadVarInt();
			data = br.ReadBytes();
			entities = br.ReadBytes();
		}

		public object Clone()
		{
			return new Disconnect();
		}
	}
}