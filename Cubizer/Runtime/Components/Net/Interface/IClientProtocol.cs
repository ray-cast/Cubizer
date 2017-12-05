using System.Net.Sockets;

namespace Cubizer
{
	public interface IClientProtocol
	{
		bool ConnectRequire(NetworkStream stream);

		bool DispatchIncomingPacket(byte[] buffer, int length);
	}
}