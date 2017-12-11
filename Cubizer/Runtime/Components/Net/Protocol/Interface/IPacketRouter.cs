namespace Cubizer.Protocol
{
	public interface IPacketRouter
	{
		void DispatchIncomingPacket(UncompressedPacket packet);
	}
}