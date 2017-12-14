using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Login.Serverbound
{
	[Packet(Packet)]
	public sealed class SetCompression : IPacketSerializable
	{
		public const int Packet = 0x03;

		public int threshold;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(threshold);
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out threshold);
		}

		public object Clone()
		{
			return new SetCompression();
		}
	}
}