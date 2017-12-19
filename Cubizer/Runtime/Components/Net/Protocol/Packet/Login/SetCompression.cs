using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Login.Clientbound
{
	[Packet(Packet)]
	public sealed class SetCompression : IPacketSerializable
	{
		public const int Packet = 0x03;

		public uint threshold;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarInt(threshold);
		}

		public void Deserialize(NetworkReader br)
		{
			br.ReadVarInt(out threshold);
		}

		public object Clone()
		{
			return new SetCompression();
		}
	}
}