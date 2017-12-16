using Cubizer.Net.Protocol;
using Cubizer.Net.Protocol.Login.Serverbound;

namespace Cubizer.Net.Server
{
	public partial class ServerPacketRouter
	{
		public void DispatchLoginPacket(IPacketSerializable packet)
		{
			switch (packet.packetId)
			{
				case LoginStart.Packet:
					this.InvokeLoginStart(packet as LoginStart);
					break;

				case EncryptionResponse.Packet:
					this.InvokeEncryptionResponse(packet as EncryptionResponse);
					break;
			}
		}

		private void InvokeLoginStart(LoginStart packet)
		{
			packetListener.OnLoginStart(packet);
		}

		private void InvokeEncryptionResponse(EncryptionResponse packet)
		{
			packetListener.OnEncryptionResponse(packet);
		}
	}
}