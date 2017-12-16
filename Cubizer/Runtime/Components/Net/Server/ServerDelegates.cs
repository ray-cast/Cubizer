using Cubizer.Net.Protocol;

namespace Cubizer.Net.Server
{
	public delegate void OnStartTcpListener();
	public delegate void OnStopTcpListener();

	public delegate void OnIncomingClient(System.Net.Sockets.TcpClient client);
	public delegate void OnIncomingClientSession(ServerSession session);

	public delegate void OnOutcomingClientSession(ServerSession session);

	public delegate void OnDispatchInvalidPacketDelegate(SessionStatus status, UncompressedPacket data);
	public delegate void OnDispatchIncomingPacketDelegate(SessionStatus status, IPacketSerializable data);
}