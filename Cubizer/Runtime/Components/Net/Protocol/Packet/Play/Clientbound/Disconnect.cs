using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public sealed class Disconnect : IPacketSerializable
	{
		public const int Packet = 0x1A;

		public string reason;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarString(reason);
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out reason);
		}

		public object Clone()
		{
			return new Disconnect();
		}
	}
}