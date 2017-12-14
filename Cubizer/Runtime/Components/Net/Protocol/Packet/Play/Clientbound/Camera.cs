using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class Camera : IPacketSerializable
	{
		public const int Packet = 0x39;

		public uint cameraID;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new Camera();
		}

		public void Deserialize(NetworkReader br)
		{
			br.ReadVarInt(out cameraID);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(cameraID);
		}
	}
}