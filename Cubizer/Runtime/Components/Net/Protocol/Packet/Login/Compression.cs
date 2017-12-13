using System.Reflection;

using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Clientbound.Login
{
	[Packet(0x03)]
	public sealed class SetCompression : IPacketSerializable
	{
		public int threshold;

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
			bw.Write(threshold);
		}

		public void Deserialize(NetworkReader br)
		{
			threshold = br.ReadInt32();
		}
	}
}