using System.Reflection;

using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Login.Clientbound
{
	[Packet(Packet)]
	public sealed class EncryptionRequest : IPacketSerializable
	{
		public const int Packet = 0x01;

		public string serverID;
		public byte[] key;
		public byte[] token;

		public uint packetId
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

		public object Clone()
		{
			return new EncryptionRequest();
		}
	}
}