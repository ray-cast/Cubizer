using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class SpawnPainting : IPacketSerializable
	{
		public const int Packet = 0x04;

		public uint entityID;
		public byte entityUUID;
		public string title;
		public byte[] position;
		public byte direction;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new SpawnPainting();
		}

		public void Deserialize(NetworkReader br)
		{
			br.ReadVarInt(out entityID);
			br.Read(out entityUUID);
			br.ReadVarString(out title);
			br.Read(out position, 8);
			br.Read(out direction);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarInt(entityID);
			bw.Write(entityUUID);
			bw.Write(title);
			bw.Write(position, 0, 8);
			bw.Write(direction);
		}
	}
}