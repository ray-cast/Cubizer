using Cubizer.Net.Protocol;
using Cubizer.Net.Protocol.Status.Clientbound;

namespace Cubizer.Net.Client
{
	public partial class ClientPacketRouter
	{
		private void DispatchStatusPacket(IPacketSerializable packet)
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
			packetListener.OnPong(packet);
		}

		private void InvokeResponse(Response packet)
		{
			packetListener.OnResponse(packet);
		}
	}
}