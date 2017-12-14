using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Login.Clientbound
{
	[Packet(Packet)]
	public sealed class LoginDisconnect : IPacketSerializable
	{
		public const int Packet = 0x00;

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
			br.ReadVarString(out reason);
		}

		public object Clone()
		{
			return new LoginDisconnect();
		}
	}
}