using System.Threading.Tasks;
using System.Collections.Generic;

using Cubizer.Net.Protocol;
using Cubizer.Net.Protocol.Serialization;
using Cubizer.Net.Protocol.Status.Clientbound;
using Cubizer.Net.Protocol.Login.Clientbound;

namespace Cubizer.Net.Client
{
	public sealed class ClientPacketRouter : IPacketRouter
	{
		private readonly List<IPacketSerializable>[] list = new List<IPacketSerializable>[(int)SessionStatus.MaxEnum];

		public SessionStatus status { get; set; }

		public OnDispatchIncomingPacket onDispatchIncomingPacket { get; set; }
		public OnDispatchInvalidPacket onDispatchInvalidPacket { get; set; }

		public ClientPacketRouter()
		{
			list[(int)SessionStatus.Status] = new List<IPacketSerializable>(2);
			list[(int)SessionStatus.Status].Add(new Pong());
			list[(int)SessionStatus.Status].Add(new Response());

			list[(int)SessionStatus.Login] = new List<IPacketSerializable>(3);
			list[(int)SessionStatus.Login].Add(new LoginDisconnect());
			list[(int)SessionStatus.Login].Add(new EncryptionRequest());
			list[(int)SessionStatus.Login].Add(new LoginSuccess());
		}

		Task IPacketRouter.DispatchIncomingPacket(UncompressedPacket packet)
		{
			switch (status)
			{
				case SessionStatus.Status:
				case SessionStatus.Login:
				case SessionStatus.Play:
					{
						IPacketSerializable pack = null;

						foreach (var it in list[(int)status])
						{
							if (it.packetId == packet.packetId)
							{
								pack = it.Clone() as IPacketSerializable;
								break;
							}
						}

						if (pack != null)
						{
							using (var br = new NetworkReader(packet.data))
								pack.Deserialize(br);

							onDispatchIncomingPacket?.Invoke(status, pack);
						}
						else
						{
							onDispatchInvalidPacket?.Invoke(status, packet);
						}
					}
					break;
			}

			return Task.CompletedTask;
		}
	}
}