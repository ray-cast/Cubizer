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
			uint value;
			br.ReadVarInt(out value);

			switch (value)
			{
				case (int)Hand.Main:
					hand = Hand.Main;
					break;

				case (int)Hand.Off:
					hand = Hand.Off;
					break;

				default:
					throw new System.IO.InvalidDataException("Invalid Hand Enum");
			}
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarInt((uint)hand);
		}
	}
}