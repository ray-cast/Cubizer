namespace Cubizer.Protocol
{
	public interface IClientProtocol
	{
		void DispatchIncomingPacket(UncompressedPacket packet);
	}
}