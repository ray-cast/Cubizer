namespace Cubizer.Protocol
{
	public sealed class ServerProtocolNull : IPacketRouter
	{
		public void DispatchIncomingPacket(UncompressedPacket packet)
		{
		}
	}
}