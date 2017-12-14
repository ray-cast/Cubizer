using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class SelectAdvancementTab : IPacketSerializable
	{
		public const int Packet = 0x37;

		public bool hasID;
		public string tabID;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new SelectAdvancementTab();
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out hasID);
			br.ReadVarString(out tabID);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(hasID);
			bw.WriteVarString(tabID);
		}
	}
}