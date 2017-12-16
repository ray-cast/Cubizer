using Cubizer.Net.Protocol;
using Cubizer.Net.Protocol.Handshakes.Serverbound;

namespace Cubizer.Net.Server.Header
{
	public sealed class HandshakesHeader : IPacketHeader
	{
		public void OnDispatchIncomingPacket(IPacketSerializable packet)
		{
			switch (packet.packetId)
			{
				case Handshake.Packet:
					this.InvokeHandshake(packet as Handshake);
					break;
			}
		}

		public void InvokeHandshake(Handshake packet)
		{
		}
	}
}