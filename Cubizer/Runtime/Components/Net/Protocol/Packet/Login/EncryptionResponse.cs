using System.Reflection;

using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Login.Serverbound
{
	[Packet(Packet)]
	public sealed class EncryptionResponse : IPacketSerializable
	{
		public const int Packet = 0x01;

		public byte[] secret;
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

		public object Clone()
		{
			return new EncryptionResponse();
		}
	}
}