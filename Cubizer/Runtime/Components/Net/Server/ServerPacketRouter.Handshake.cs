using Cubizer.Net.Protocol;
using Cubizer.Net.Protocol.Handshakes.Serverbound;

namespace Cubizer.Net.Server
{
	public partial class ServerPacketRouter
	{
		public void DispatchHandshakingPacket(IPacketSerializable packet)
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
			packetListener.OnHandshake(packet);
		}
	}
}