using System.Net.Sockets;

using Cubizer.Net.Protocol;

namespace Cubizer.Net.Server
{
	public delegate void OnStartTcpListener();
	public delegate void OnStopTcpListener();

	public delegate void OnIncomingClient(TcpClient client);
	public delegate void OnIncomingClientSession(ServerSession session);

	public delegate void OnOutcomingClientSession(ServerSession session);

	public delegate void OnDispatchInvalidPacket(SessionStatus status, UncompressedPacket data);
	public delegate void OnDispatchIncomingPacket(SessionStatus status, IPacketSerializable data);
}