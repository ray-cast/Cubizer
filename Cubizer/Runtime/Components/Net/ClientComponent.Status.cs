using System;

using Cubizer.Net.Client;

namespace Cubizer.Net
{
	public sealed partial class ClientComponent
	{
		private partial class PacketListener : IPacketListener
		{
			void IPacketListener.OnPong(Protocol.Status.Clientbound.Pong packet)
			{
				throw new NotImplementedException();
			}

			void IPacketListener.OnResponse(Protocol.Status.Clientbound.Response packet)
			{
				throw new NotImplementedException();
			}
		}
	}
}