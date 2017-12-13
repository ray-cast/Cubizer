using Cubizer.Net.Protocol;

namespace Cubizer.Net.Client
{
	public delegate void OnStartTcpListener();
	public delegate void OnStopTcpListener();

	public delegate void OnDispatchInvalidPacket(SessionStatus status, UncompressedPacket data);
	public delegate void OnDispatchIncomingPacket(SessionStatus status, IPacketSerializable data);
}