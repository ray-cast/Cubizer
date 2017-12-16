using Cubizer.Net.Protocol;
using Cubizer.Net.Protocol.Login.Clientbound;

namespace Cubizer.Net.Client
{
	public partial class ClientPacketRouter
	{
		private void DispatchLoginPacket(IPacketSerializable packet)
		{
			switch (packet.packetId)
			{
				case LoginSuccess.Packet:
					InvokeLoginSuccess(packet as LoginSuccess);
					break;

				case LoginDisconnect.Packet:
					InvokeLoginDisconnect(packet as LoginDisconnect);
					break;

				case SetCompression.Packet:
					InvokeSetCompression(packet as SetCompression);
					break;

				case EncryptionRequest.Packet:
					InvokeEncryptionRequest(packet as EncryptionRequest);
					break;
			}
		}

		private void InvokeLoginSuccess(LoginSuccess packet)
		{
			packetListener.OnLoginSuccess(packet);
		}

		private void InvokeLoginDisconnect(LoginDisconnect packet)
		{
			packetListener.OnLoginDisconnect(packet);
		}

		private void InvokeSetCompression(SetCompression packet)
		{
			packetListener.OnSetCompression(packet);
		}

		private void InvokeEncryptionRequest(EncryptionRequest packet)
		{
			packetListener.OnEncryptionRequest(packet);
		}
	}
}