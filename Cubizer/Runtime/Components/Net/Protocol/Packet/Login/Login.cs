using System.IO;

namespace Cubizer.Protocol.Login
{
	[Packet(0x00)]
	public sealed class LoginStart : ISerializablePacket
	{
		public string name;

		public static LoginStart Deserialize(ref BinaryReader br)
		{
			return new LoginStart
			{
				name = br.ReadString(),
			};
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(name);
		}
	}

	[Packet(0x00)]
	public sealed class LoginDisconnect : ISerializablePacket
	{
		public string reason;

		public static LoginDisconnect Deserialize(ref BinaryReader br)
		{
			return new LoginDisconnect
			{
				reason = br.ReadString()
			};
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(reason);
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
	}
}