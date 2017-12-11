using System.IO;

namespace Cubizer.Protocol.Login
{
	[Packet(0x00)]
	public sealed class LoginStart : ISerializablePacket
	{
		public string name;

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(name);
		}

		public void Deserialize(ref BinaryReader br)
		{
			name = br.ReadString();
		}
	}

	[Packet(0x00)]
	public sealed class LoginDisconnect : ISerializablePacket
	{
		public string reason;

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(reason);
		}

		public void Deserialize(ref BinaryReader br)
		{
			reason = br.ReadString();
		}
	}

	[Packet(0x02)]
	public sealed class LoginSuccess : ISerializablePacket
	{
		public string UUID;

		public string username;

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(UUID);
			bw.Write(username);
		}

		public void Deserialize(ref BinaryReader br)
		{
			UUID = br.ReadString();
			username = br.ReadString();
		}
	}
}