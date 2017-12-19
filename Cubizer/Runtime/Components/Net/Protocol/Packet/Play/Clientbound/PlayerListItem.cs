using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class PlayerListItem : IPacketSerializable
	{
		public const int Packet = 0x2D;

		public uint action;

		public uint numberOfPlayers;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new PlayerListItem();
		}

		public void Deserialize(NetworkReader br)
		{
			br.ReadVarInt(out action);
			br.ReadVarInt(out numberOfPlayers);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarInt(action);
			bw.WriteVarInt(numberOfPlayers);
		}
	}
}