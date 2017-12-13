using System.Reflection;

using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Status
{
	[Packet(0x01)]
	public sealed class Ping : IPacketSerializable
	{
		public long payload;

		public uint packId
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
	}

	[Packet(0x01)]
	public sealed class Pong : IPacketSerializable
	{
		public long payload;

		public uint packId
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
	}
}