using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Serverbound
{
	[Packet(Packet)]
	public class KeepAlive : IPacketSerializable
	{
		public const int Packet = 0x0C;

		public long keepAliveID;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new KeepAlive();
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out keepAliveID);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(keepAliveID);
		}
	}
}