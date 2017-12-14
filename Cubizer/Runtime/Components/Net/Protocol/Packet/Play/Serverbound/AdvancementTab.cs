using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Serverbound
{
	[Packet(Packet)]
	public class AdvancementTab : IPacketSerializable
	{
		public const int Packet = 0x19;

		public uint action;
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
			return new AdvancementTab();
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out action);
			br.ReadVarString(out tabID);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(action);
			bw.WriteVarString(tabID);
		}
	}
}