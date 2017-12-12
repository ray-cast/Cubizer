using System.IO;
using System.Reflection;

namespace Cubizer.Protocol.Status
{
	[Packet(0x00)]
	public sealed class Request : IPacketSerializable
	{
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
		}

		public void Serialize(NetworkWrite bw)
		{
		}
	}

	[Packet(0x00)]
	public sealed class Response : IPacketSerializable
	{
		public string response;

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
			response = br.ReadString();
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(response);
		}
	}
}