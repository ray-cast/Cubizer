using System.Net.Sockets;

namespace Cubizer
{
	public interface IServerProtocol
	{
		bool DispatchIncomingPacket(NetworkStream stream, byte[] buffer, int length);
	}
}