using System.Net.Sockets;

namespace Cubizer
{
	public sealed class ServerTcpDelegates
	{
		public delegate void OnStartTcpListener();
		public delegate void OnStopTcpListener();

		public delegate void OnIncomingClient(TcpClient client);
		public delegate void OnIncomingClientSession(ClientSession session);

		public delegate void OnOutcomingClientSession(ClientSession session);

		public OnStartTcpListener onStartTcpListener;
		public OnStopTcpListener onStopTcpListener;

		public OnIncomingClient onIncomingClient;
		public OnIncomingClientSession onIncomingClientSession;

		public OnOutcomingClientSession onOutcomingClientSession;
	}
}