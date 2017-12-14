using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class Animation : IPacketSerializable
	{
		public const int Packet = 0x06;

		public uint entityID;
		public byte animation;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new Animation();
		}

		public void Deserialize(NetworkReader br)
		{
			br.ReadVarInt(out entityID);
			br.Read(out animation);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarInt(entityID);
			bw.Write(animation);
		}
	}
}