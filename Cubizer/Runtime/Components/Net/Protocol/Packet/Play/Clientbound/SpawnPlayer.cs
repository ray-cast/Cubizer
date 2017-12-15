using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class SpawnPlayer : IPacketSerializable
	{
		public const int Packet = 0x05;

		public uint entityID;
		public byte playerUUID;
		public double x;
		public double y;
		public double z;
		public byte yaw;
		public byte pitch;
		public byte metaData;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new SpawnPlayer();
		}

		public void Deserialize(NetworkReader br)
		{
			br.ReadVarInt(out entityID);
			br.Read(out playerUUID);
			br.Read(out x);
			br.Read(out y);
			br.Read(out z);
			br.Read(out yaw);
			br.Read(out pitch);
			br.Read(out metaData);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarInt(entityID);
			bw.Write(playerUUID);
			bw.Write(x);
			bw.Write(y);
			bw.Write(z);
			bw.Write(yaw);
			bw.Write(pitch);
			bw.Write(metaData);
		}
	}
}