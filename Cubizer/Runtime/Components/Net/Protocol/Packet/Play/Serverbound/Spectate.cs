using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Serverbound
{
	[Packet(Packet)]
	public class Spectate : IPacketSerializable
	{
		public const int Packet = 0x1E;

		public byte[] targetPlayer;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new Spectate();
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out targetPlayer, 16);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(targetPlayer, 0, 16);
		}
	}
}