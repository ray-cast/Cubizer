using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Status.Clientbound
{
	[Packet(Packet)]
	public sealed class Response : IPacketSerializable
	{
		public const int Packet = 0x00;

		public string response;

		public uint packetId
		{
			get
			{
				return packetId;
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

		public object Clone()
		{
			return new Response();
		}
	}
}