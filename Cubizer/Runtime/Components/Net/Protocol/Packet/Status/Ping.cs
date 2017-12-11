using System.IO;
using System.Reflection;

namespace Cubizer.Protocol.Status
{
	[Packet(0x01)]
	public sealed class Ping : IPacketSerializable
	{
		public long Payload;

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
			Payload = br.ReadInt64();
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(Payload);
		}
	}

	[Packet(0x01)]
	public sealed class Pong : IPacketSerializable
	{
		public long Payload;

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
			Payload = br.ReadInt64();
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(Payload);
		}
	}
}