using System.IO;
using System.Reflection;

namespace Cubizer.Protocol.Status
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

		public void Deserialize(ref BinaryReader br)
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

		public void Deserialize(ref BinaryReader br)
		{
			payload = br.ReadInt64();
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(payload);
		}
	}
}