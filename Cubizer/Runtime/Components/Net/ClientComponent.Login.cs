using System;

using Cubizer.Net.Client;
using Cubizer.Net.Protocol;

namespace Cubizer.Net
{
	public sealed partial class ClientComponent
	{
		private partial class PacketListener : IPacketListener
		{
			void IPacketListener.OnLoginSuccess(Protocol.Login.Clientbound.LoginSuccess packet)
			{
				client._clientRouter.status = SessionStatus.Play;
			}

			void IPacketListener.OnLoginDisconnect(Protocol.Login.Clientbound.LoginDisconnect packet)
			{
				client._clientRouter.status = SessionStatus.Login;
			}

			void IPacketListener.OnSetCompression(Protocol.Login.Clientbound.SetCompression packet)
			{
				throw new NotImplementedException();
			}

			void IPacketListener.OnEncryptionRequest(Protocol.Login.Clientbound.EncryptionRequest packet)
			{
				throw new NotImplementedException();
			}
		}
	}
}