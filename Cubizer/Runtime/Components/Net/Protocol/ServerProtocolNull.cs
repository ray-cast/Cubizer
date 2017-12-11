namespace Cubizer.Protocol
{
	public sealed class ServerProtocolNull : IServerProtocol
	{
		public void DispatchIncomingPacket(UncompressedPacket packet)
		{
		}
	}
}