using Cubizer.Net.Protocol;
using Cubizer.Net.Protocol.Status.Serverbound;

namespace Cubizer.Net.Server
{
	public partial class ServerPacketRouter
	{
		public void DispatchStatusPacket(IPacketSerializable packet)
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
			packetListener.OnPing(packet);
		}

		private void InvokeRequest(Request packet)
		{
			packetListener.OnRequest(packet);
		}
	}
}