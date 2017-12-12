namespace Cubizer.Protocol
{
	public sealed class PacketCompress : IPacketCompress
	{
		public CompressedPacket Compress(UncompressedPacket packet)
		{
			return new CompressedPacket(packet.packetId, packet.data);
		}

		public UncompressedPacket Decompress(CompressedPacket packet)
		{
			return new UncompressedPacket(packet.packetId, packet.data);
		}
	}
}