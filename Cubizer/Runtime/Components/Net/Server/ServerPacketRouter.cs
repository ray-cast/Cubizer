using System.Threading.Tasks;
using System.Collections.Generic;

using Cubizer.Net.Protocol;
using Cubizer.Net.Protocol.Serialization;
using Cubizer.Net.Protocol.Login.Serverbound;
using Cubizer.Net.Protocol.Status.Serverbound;
using Cubizer.Net.Protocol.Handshake.Serverbound;

namespace Cubizer.Net.Server
{
	public sealed class ServerPacketRouter : IPacketRouter
	{
		private readonly List<IPacketSerializable>[] list = new List<IPacketSerializable>[(int)SessionStatus.MaxEnum];

		public SessionStatus status { get; set; }

		public OnDispatchIncomingPacket onDispatchIncomingPacket { get; set; }
		public OnDispatchInvalidPacket onDispatchInvalidPacket { get; set; }

		public ServerPacketRouter()
		{
			list[(int)SessionStatus.Handshaking] = new List<IPacketSerializable>();
			list[(int)SessionStatus.Handshaking].Add(new Handshake());

			list[(int)SessionStatus.Status] = new List<IPacketSerializable>();
			list[(int)SessionStatus.Status].Add(new Request());
			list[(int)SessionStatus.Status].Add(new Ping());

			list[(int)SessionStatus.Login] = new List<IPacketSerializable>();
			list[(int)SessionStatus.Login].Add(new LoginStart());
			list[(int)SessionStatus.Login].Add(new EncryptionResponse());
			list[(int)SessionStatus.Login].Add(new SetCompression());
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