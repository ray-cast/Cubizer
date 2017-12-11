using System.Net.Sockets;

namespace Cubizer.Protocol
{
	public sealed class ClientProtocolNull : IClientProtocol
	{
		public bool ConnectRequire(NetworkStream stream)
		{
			return true;
		}

		public bool DispatchIncomingPacket(UncompressedPacket packet)
		{
			return true;
		}
	}
}