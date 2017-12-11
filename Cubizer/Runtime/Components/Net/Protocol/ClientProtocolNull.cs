namespace Cubizer.Protocol
{
	public sealed class ClientProtocolNull : IClientProtocol
	{
		public void DispatchIncomingPacket(UncompressedPacket packet)
		{
		}
	}
}