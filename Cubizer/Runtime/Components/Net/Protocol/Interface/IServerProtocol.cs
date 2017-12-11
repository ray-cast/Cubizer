using System.Net.Sockets;

namespace Cubizer.Protocol
{
	public interface IServerProtocol
	{
		void DispatchIncomingPacket(NetworkStream stream, UncompressedPacket packet);
	}
}