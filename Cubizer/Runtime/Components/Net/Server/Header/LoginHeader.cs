using Cubizer.Net.Protocol;
using Cubizer.Net.Protocol.Login.Serverbound;

namespace Cubizer.Net.Server.Header
{
	public sealed class LoginHeader : IPacketHeader
	{
		public void OnDispatchIncomingPacket(IPacketSerializable packet)
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
		}

		private void InvokeEncryptionResponse(EncryptionResponse packet)
		{
		}
	}
}