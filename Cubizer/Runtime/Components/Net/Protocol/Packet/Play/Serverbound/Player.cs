using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Serverbound
{
	[Packet(Packet)]
	public class Player : IPacketSerializable
	{
		public const int Packet = 0x0D;

		public bool onGround;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new Player();
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out onGround);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(onGround);
		}
	}
}