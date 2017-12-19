using Cubizer.Net.Protocol.Extensions;
using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Serverbound
{
	[Packet(Packet)]
	public class UseItem : IPacketSerializable
	{
		public const int Packet = 0x20;

		public Hand hand;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new UseItem();
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out hand);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(hand);
		}
	}
}