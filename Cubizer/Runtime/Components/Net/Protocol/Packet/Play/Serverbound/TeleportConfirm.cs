using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Serverbound
{
	[Packet(Packet)]
	public class TeleportConfirm : IPacketSerializable
	{
		public const int Packet = 0x0;

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
			return new TeleportConfirm();
		}

		public void Deserialize(NetworkReader br)
		{
			br.ReadVarInt(out teleportID);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarInt(teleportID);
		}
	}
}