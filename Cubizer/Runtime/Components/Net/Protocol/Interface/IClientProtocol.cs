using System.Net.Sockets;

namespace Cubizer.Protocol
{
	public interface IClientProtocol
	{
		bool ConnectRequire(NetworkStream stream);

		bool DispatchIncomingPacket(UncompressedPacket packet);
	}
}