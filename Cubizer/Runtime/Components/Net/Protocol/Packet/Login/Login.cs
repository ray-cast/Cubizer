using System.Reflection;

using Cubizer.Protocol.Serialization;

namespace Cubizer.Protocol.Login
{
	#region server

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

	[Packet(0x03)]
	public sealed class SetCompression : IPacketSerializable
	{
		public int threshold;

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
			bw.Write(threshold);
		}

		public void Deserialize(NetworkReader br)
		{
			threshold = br.ReadInt32();
		}
	}

	#endregion server

	#region client

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

	#endregion client
}