using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Status.Clientbound
{
	[Packet(Packet)]
	public sealed class Pong : IPacketSerializable
	{
		public const int Packet = 0x01;

		public long payload;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out payload);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(payload);
		}

		public object Clone()
		{
			return new Pong();
		}
	}
}