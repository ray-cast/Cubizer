namespace Cubizer.Protocol
{
	public class PacketCompress : IPacketCompress
	{
		public CompressedPacket Compress(UncompressedPacket packet)
		{
			return new CompressedPacket(packet.length, packet.packetId, packet.data);
		}

		public UncompressedPacket Decompress(CompressedPacket packet)
		{
			return new UncompressedPacket(packet.length, packet.packetId, packet.data);
		}
	}
}