using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class ChatMessage : IPacketSerializable
	{
		public const int Packet = 0x0F;

		public string jsonData;
		public byte type;

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
			br.Read(out jsonData);
			br.Read(out type);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarString(jsonData);
			bw.Write(type);
		}
	}
}