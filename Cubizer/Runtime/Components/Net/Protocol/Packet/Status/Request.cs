using System.IO;
using System.Reflection;

namespace Cubizer.Protocol.Status
{
	[Packet(0x00)]
	public sealed class Request : IPacketSerializable
	{
		public static readonly Request Empty = new Request();

		public uint packId
		{
			get
			{
				var typeInfo = this.GetType().GetTypeInfo();
				var attr = typeInfo.GetCustomAttribute<PacketAttribute>();
				return attr.id;
			}
		}

		public static Request Deserialize(ref BinaryReader br)
		{
			return Empty;
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

		public static Response Deserialize(ref BinaryReader br)
		{
			return new Response
			{
				response = br.ReadString()
			};
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(response);
		}
	}
}