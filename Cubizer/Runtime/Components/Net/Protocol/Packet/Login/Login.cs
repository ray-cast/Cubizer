using System.Reflection;

using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Login
{
	[Packet(0x00)]
	public sealed class LoginStart : IPacketSerializable
	{
		public string name;

		public uint packId
		{
			get
			{
				var typeInfo = this.GetType().GetTypeInfo();
				var attr = typeInfo.GetCustomAttribute<PacketAttribute>();
				return attr.id;
			}
		}

		public LoginStart(string name)
		{
			this.name = name;
		}

		public void Deserialize(NetworkReader br)
		{
			name = br.ReadString();
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(name);
		}
	}

	[Packet(0x00)]
	public sealed class LoginDisconnect : IPacketSerializable
	{
		public string reason;

		public uint packId
		{
			get
			{
				var typeInfo = this.GetType().GetTypeInfo();
				var attr = typeInfo.GetCustomAttribute<PacketAttribute>();
				return attr.id;
			}
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(reason);
		}

		public void Deserialize(NetworkReader br)
		{
			reason = br.ReadString();
		}
	}

	[Packet(0x02)]
	public sealed class LoginSuccess : IPacketSerializable
	{
		public string UUID;
		public string username;

		public uint packId
		{
			get
			{
				var typeInfo = this.GetType().GetTypeInfo();
				var attr = typeInfo.GetCustomAttribute<PacketAttribute>();
				return attr.id;
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
	}
}