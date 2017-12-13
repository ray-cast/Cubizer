namespace Cubizer.Net.Protocol
{
	public interface IPacketCompress
	{
		CompressedPacket Compress(UncompressedPacket packet);

		UncompressedPacket Decompress(CompressedPacket packet);
	}
}