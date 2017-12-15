using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class Advancements : IPacketSerializable
	{
		public const int Packet = 0x4C;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public void Serialize(NetworkWrite bw)
		{
		}

		public void Deserialize(NetworkReader br)
		{
		}

		public object Clone()
		{
			return new Advancements();
		}
	}
}