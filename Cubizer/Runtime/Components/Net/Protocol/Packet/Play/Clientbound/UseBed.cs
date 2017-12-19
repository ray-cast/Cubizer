using Cubizer.Net.Protocol.Struct;
using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class UseBed : IPacketSerializable
	{
		public const int Packet = 0x2F;

		public uint entityID;
		public Vector3Int localtion;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new UseBed();
		}

		public void Deserialize(NetworkReader br)
		{
			br.ReadVarInt(out entityID);
			br.ReadPos(out localtion);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarInt(entityID);
			bw.WritePos(localtion);
		}
	}
}