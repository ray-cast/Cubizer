using System.Net.Sockets;

namespace Cubizer
{
	public sealed class ServerProtocolNull : IServerProtocol
	{
		public bool DispatchIncomingPacket(NetworkStream stream, byte[] buffer, int length)
		{
			stream.Write(buffer, 0, length);
			return true;
		}
	}
}