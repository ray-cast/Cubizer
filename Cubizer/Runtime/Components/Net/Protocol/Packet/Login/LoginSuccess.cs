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
			bw.Write(UUID);
			bw.Write(username);
		}

		public void Deserialize(NetworkReader br)
		{
			UUID = br.ReadString();
			username = br.ReadString();
		}

		public object Clone()
		{
			return new LoginSuccess();
		}
	}
}