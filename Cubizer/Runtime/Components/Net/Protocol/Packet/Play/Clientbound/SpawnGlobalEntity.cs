using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class SpawnGlobalEntity : IPacketSerializable
	{
		public const int Packet = 0x02;

		public uint entityID;
		public byte type;
		public double x;
		public double y;
		public double z;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new SpawnGlobalEntity();
		}

		public void Deserialize(NetworkReader br)
		{
			br.ReadVarInt(out entityID);
			br.Read(out type);
			br.Read(out x);
			br.Read(out y);
			br.Read(out z);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarInt(entityID);
			bw.Write(type);
			bw.Write(x);
			bw.Write(y);
			bw.Write(z);
		}
	}
}