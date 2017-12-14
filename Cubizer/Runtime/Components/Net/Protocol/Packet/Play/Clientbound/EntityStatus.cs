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
			bw.Write(entityStatus);
		}

		public void Deserialize(NetworkReader br)
		{
			entityID = br.ReadInt32();
			entityStatus = br.ReadString();
		}

		public object Clone()
		{
			return new Disconnect();
		}
	}
}