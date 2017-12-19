using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class WindowItems : IPacketSerializable
	{
		public const int Packet = 0x14;

		public byte windowID;
		public short count;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new WindowItems();
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out windowID);
			br.Read(out count);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(windowID);
			bw.Write(count);
		}
	}
}