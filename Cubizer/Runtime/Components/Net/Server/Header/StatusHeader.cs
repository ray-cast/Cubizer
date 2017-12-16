using Cubizer.Net.Protocol;
using Cubizer.Net.Protocol.Status.Serverbound;

namespace Cubizer.Net.Server.Header
{
	public sealed class StatusHeader : IPacketHeader
	{
		public void OnDispatchIncomingPacket(IPacketSerializable packet)
		{
			switch (packet.packetId)
			{
				case Ping.Packet:
					this.InvokePing(packet as Ping);
					break;

				case Request.Packet:
					this.InvokeRequest(packet as Request);
					break;
			}
		}

		private void InvokePing(Ping packet)
		{
		}

		private void InvokeRequest(Request packet)
		{
		}
	}
}