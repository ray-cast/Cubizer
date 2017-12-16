using Cubizer.Net.Protocol;
using Cubizer.Net.Protocol.Status.Clientbound;

namespace Cubizer.Net.Client.Header
{
	public sealed class StatusHeader : IPacketHeader
	{
		public void OnDispatchIncomingPacket(IPacketSerializable packet)
		{
			switch (packet.packetId)
			{
				case Pong.Packet:
					this.InvokePong(packet as Pong);
					break;

				case Response.Packet:
					this.InvokeResponse(packet as Response);
					break;
			}
		}

		private void InvokePong(Pong packet)
		{
		}

		private void InvokeResponse(Response packet)
		{
		}
	}
}