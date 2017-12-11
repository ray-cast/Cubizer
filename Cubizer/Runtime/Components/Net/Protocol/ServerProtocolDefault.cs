using System;
using System.Net.Sockets;
using System.Text;

namespace Cubizer.Protocol
{
	public class ServerProtocolDefault : IServerProtocol
	{
		public void DispatchIncomingPacket(NetworkStream stream, UncompressedPacket packet)
		{
		}
	}
}