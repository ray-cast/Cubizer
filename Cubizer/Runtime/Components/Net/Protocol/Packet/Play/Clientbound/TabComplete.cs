using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class TabComplete : IPacketSerializable
	{
		public const int Packet = 0x0E;

		public string matches;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new TabComplete();
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out matches);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarString(matches);
		}
	}
}