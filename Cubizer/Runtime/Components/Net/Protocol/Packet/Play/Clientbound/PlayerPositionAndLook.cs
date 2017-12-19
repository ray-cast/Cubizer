using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class PlayerPositionAndLook : IPacketSerializable
	{
		public const int Packet = 0x2E;

		public double x;
		public double y;
		public double z;
		public float yaw;
		public float pitch;
		public byte flags;
		public uint teleportID;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new PlayerPositionAndLook();
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out x);
			br.Read(out y);
			br.Read(out z);
			br.Read(out yaw);
			br.Read(out pitch);
			br.Read(out flags);
			br.ReadVarInt(out teleportID);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(x);
			bw.Write(y);
			bw.Write(z);
			bw.Write(yaw);
			bw.Write(pitch);
			bw.Write(flags);
			bw.WriteVarInt(teleportID);
		}
	}
}