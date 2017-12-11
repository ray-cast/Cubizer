namespace Cubizer.Protocol
{
	public sealed class ClientProtocolNull : IPacketRouter
	{
		public void DispatchIncomingPacket(UncompressedPacket packet)
		{
		}
	}
}