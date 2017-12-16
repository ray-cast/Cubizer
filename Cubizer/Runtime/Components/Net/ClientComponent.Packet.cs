using Cubizer.Net.Client;

namespace Cubizer.Net
{
	public sealed partial class ClientComponent
	{
		private partial class PacketListener : IPacketListener
		{
			private readonly ClientComponent client;

			public PacketListener(ClientComponent client)
			{
				this.client = client;

				PacketListenerPlayCtor();
			}
		}
	}
}