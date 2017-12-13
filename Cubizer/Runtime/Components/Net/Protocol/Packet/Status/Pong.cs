using System.Reflection;

using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Status.Clientbound
{
	[Packet(Packet)]
	public sealed class Pong : IPacketSerializable
	{
		public const int Packet = 0x01;

		public long payload;

		public uint packetId
		{
			get
			{
				var typeInfo = this.GetType().GetTypeInfo();
				var attr = typeInfo.GetCustomAttribute<PacketAttribute>();
				return attr.id;
			}
		}

		public void Deserialize(NetworkReader br)
		{
			payload = br.ReadInt64();
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(payload);
		}

		public object Clone()
		{
			return new Pong();
		}
	}
}