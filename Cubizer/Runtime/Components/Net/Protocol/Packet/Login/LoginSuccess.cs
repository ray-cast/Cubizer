using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Login.Clientbound
{
	[Packet(Packet)]
	public sealed class LoginSuccess : IPacketSerializable
	{
		public const int Packet = 0x02;

		public string UUID;
		public string username;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarString(UUID);
			bw.WriteVarString(username);
		}

		public void Deserialize(NetworkReader br)
		{
			br.ReadVarString(out UUID);
			br.ReadVarString(out username);
		}

		public object Clone()
		{
			return new LoginSuccess();
		}
	}
}