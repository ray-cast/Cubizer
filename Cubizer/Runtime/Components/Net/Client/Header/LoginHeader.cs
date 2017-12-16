using Cubizer.Net.Protocol;
using Cubizer.Net.Protocol.Login.Clientbound;

namespace Cubizer.Net.Client.Header
{
	public sealed class LoginHeader : IPacketHeader
	{
		public void OnDispatchIncomingPacket(IPacketSerializable packet)
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
		}

		private void InvokeLoginDisconnect(LoginDisconnect packet)
		{
		}

		private void InvokeSetCompression(SetCompression packet)
		{
		}

		private void InvokeEncryptionRequest(EncryptionRequest packet)
		{
		}
	}
}