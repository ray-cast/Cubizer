using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public sealed class EntityStatus : IPacketSerializable
	{
		public const int Packet = 0x1B;

		public int entityID;
		public string entityStatus;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(entityID);
			bw.WriteVarString(entityStatus);
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out entityID);
			br.Read(out entityStatus);
		}

		public object Clone()
		{
			return new EntityStatus();
		}
	}
}