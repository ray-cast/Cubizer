using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Serverbound
{
	[Packet(Packet)]
	public class ChatMessage : IPacketSerializable
	{
		public const int Packet = 0x02;

		public string message;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new ChatMessage();
		}

		public void Deserialize(NetworkReader br)
		{
			br.ReadVarString(out message);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarString(message);
		}
	}
}