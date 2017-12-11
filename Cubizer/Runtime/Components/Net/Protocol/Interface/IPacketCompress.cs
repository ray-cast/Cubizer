namespace Cubizer.Protocol
{
	public interface IPacketCompress
	{
		CompressedPacket Compress(UncompressedPacket packet);

		UncompressedPacket Decompress(CompressedPacket packet);
	}
}