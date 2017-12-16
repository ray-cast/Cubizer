using Cubizer.Net.Protocol;

namespace Cubizer.Net.Client
{
	public delegate void OnStartTcpListener();
	public delegate void OnStopTcpListener();

	public delegate void OnDispatchInvalidPacketDelegate(SessionStatus status, UncompressedPacket packet);
	public delegate void OnDispatchIncomingPacketDelegate(SessionStatus status, IPacketSerializable packet);
}