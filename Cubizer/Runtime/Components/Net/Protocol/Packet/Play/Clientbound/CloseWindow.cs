using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class CloseWindow : IPacketSerializable
	{
		public const int Packet = 0x12;

		public byte windowID;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new CloseWindow();
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out windowID);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(windowID);
		}
	}
}