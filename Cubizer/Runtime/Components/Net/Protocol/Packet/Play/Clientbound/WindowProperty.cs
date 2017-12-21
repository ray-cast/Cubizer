using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class WindowProperty : IPacketSerializable
	{
		public const int Packet = 0x15;

		public byte windowID;
		public short property;
		public short value;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new WindowProperty();
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out windowID);
			br.Read(out property);
			br.Read(out value);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(windowID);
			bw.Write(property);
			bw.Write(value);
		}
	}
}