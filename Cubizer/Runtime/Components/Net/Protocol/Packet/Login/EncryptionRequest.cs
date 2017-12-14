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
				return Packet;
			}
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarString(serverID, 20);
			bw.WriteVarBytes(key);
			bw.WriteVarBytes(token);
		}

		public void Deserialize(NetworkReader br)
		{
			br.ReadVarString(out this.serverID, 20);
			br.ReadVarBytes(out this.key);
			br.ReadVarBytes(out this.token);
		}

		public object Clone()
		{
			return new EncryptionRequest();
		}
	}
}