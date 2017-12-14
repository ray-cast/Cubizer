using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class SpawnExperienceOrb : IPacketSerializable
	{
		public const int Packet = 0x01;

		public uint entityID;
		public double x;
		public double y;
		public double z;
		public short count;

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
			br.Read(out entityID);
			br.Read(out x);
			br.Read(out y);
			br.Read(out z);
			br.Read(out count);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(entityID);
			bw.Write(x);
			bw.Write(y);
			bw.Write(z);
			bw.Write(count);
		}
	}
}