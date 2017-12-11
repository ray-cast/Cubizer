using System.Net.Sockets;

namespace Cubizer.Protocol
{
	public sealed class ServerProtocolNull : IServerProtocol
	{
		public void DispatchIncomingPacket(NetworkStream stream, UncompressedPacket packet)
		{
		}
	}
}