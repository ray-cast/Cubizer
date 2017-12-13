using System.Reflection;

using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Encryption
{
	[Packet(0x01)]
	public sealed class EncryptionRequest : IPacketSerializable
	{
		public string serverID;
		public byte[] key;
		public byte[] token;

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
			bw.Write(serverID, 20);

			bw.WriteVarInt(key.Length);
			bw.Write(key);

			bw.WriteVarInt(token.Length);
			bw.Write(token);
		}

		public void Deserialize(NetworkReader br)
		{
			this.serverID = br.ReadString(20);
			this.key = br.ReadBytes();
			this.token = br.ReadBytes();
		}
	}

	[Packet(0x01)]
	public sealed class EncryptionResponse : IPacketSerializable
	{
		public byte[] secret;
		public byte[] token;

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
			bw.WriteVarInt((uint)secret.Length);
			bw.Write(secret);

			bw.WriteVarInt((uint)token.Length);
			bw.Write(token);
		}

		public void Deserialize(NetworkReader br)
		{
			this.secret = br.ReadBytes();
			this.token = br.ReadBytes();
		}
	}
}