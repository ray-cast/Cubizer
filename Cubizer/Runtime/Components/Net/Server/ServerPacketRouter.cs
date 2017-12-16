using System.Threading.Tasks;
using System.Collections.Generic;

using Cubizer.Net.Protocol;
using Cubizer.Net.Protocol.Serialization;
using Cubizer.Net.Protocol.Play.Serverbound;
using Cubizer.Net.Protocol.Login.Serverbound;
using Cubizer.Net.Protocol.Status.Serverbound;
using Cubizer.Net.Protocol.Handshakes.Serverbound;
using Cubizer.Net.Server.Header;

namespace Cubizer.Net.Server
{
	public class ServerPacketRouter : IPacketRouter
	{
		public SessionStatus status { get; set; }

		public OnDispatchIncomingPacketDelegate onDispatchIncomingPacket { get; set; }
		public OnDispatchInvalidPacketDelegate onDispatchInvalidPacket { get; set; }

		private readonly IPacketHeader _statusHeader = new StatusHeader();
		private readonly IPacketHeader _loginHeader = new LoginHeader();
		private readonly IPacketHeader _playHeader = new PlayHeader();

		private readonly List<IPacketSerializable>[] list = new List<IPacketSerializable>[(int)SessionStatus.MaxEnum];

		public ServerPacketRouter()
		{
			list[(int)SessionStatus.Handshaking] = new List<IPacketSerializable>(1);
			list[(int)SessionStatus.Handshaking].Add(new Handshake());

			list[(int)SessionStatus.Status] = new List<IPacketSerializable>(2);
			list[(int)SessionStatus.Status].Add(new Request());
			list[(int)SessionStatus.Status].Add(new Ping());

			list[(int)SessionStatus.Login] = new List<IPacketSerializable>(3);
			list[(int)SessionStatus.Login].Add(new LoginStart());
			list[(int)SessionStatus.Login].Add(new EncryptionResponse());

			list[(int)SessionStatus.Play] = new List<IPacketSerializable>(33);
			list[(int)SessionStatus.Play].Add(new TeleportConfirm());
			list[(int)SessionStatus.Play].Add(new TabComplete());
			list[(int)SessionStatus.Play].Add(new ChatMessage());
			list[(int)SessionStatus.Play].Add(new ClientStatus());
			list[(int)SessionStatus.Play].Add(new ClientSettings());
			list[(int)SessionStatus.Play].Add(new ConfirmTransaction());
			list[(int)SessionStatus.Play].Add(new EnchantItem());
			list[(int)SessionStatus.Play].Add(new ClickWindow());
			list[(int)SessionStatus.Play].Add(new CloseWindow());
			list[(int)SessionStatus.Play].Add(new PluginMessage());
			list[(int)SessionStatus.Play].Add(new UseEntity());
			list[(int)SessionStatus.Play].Add(new KeepAlive());
			list[(int)SessionStatus.Play].Add(new Player());
			list[(int)SessionStatus.Play].Add(new PlayerPosition());
			list[(int)SessionStatus.Play].Add(new PlayerPositionAndLook());
			list[(int)SessionStatus.Play].Add(new PlayerLook());
			list[(int)SessionStatus.Play].Add(new VehicleMove());
			list[(int)SessionStatus.Play].Add(new SteerBoat());
			list[(int)SessionStatus.Play].Add(new CraftRecipeRequest());
			list[(int)SessionStatus.Play].Add(new PlayerAbilities());
			list[(int)SessionStatus.Play].Add(new PlayerDigging());
			list[(int)SessionStatus.Play].Add(new EntityAction());
			list[(int)SessionStatus.Play].Add(new SteerVehicle());
			list[(int)SessionStatus.Play].Add(new CraftingBookData());
			list[(int)SessionStatus.Play].Add(new ResourcePackStatus());
			list[(int)SessionStatus.Play].Add(new AdvancementTab());
			list[(int)SessionStatus.Play].Add(new HeldItemChange());
			list[(int)SessionStatus.Play].Add(new CreativeInventoryAction());
			list[(int)SessionStatus.Play].Add(new UpdateSign());
			list[(int)SessionStatus.Play].Add(new Animation());
			list[(int)SessionStatus.Play].Add(new Spectate());
			list[(int)SessionStatus.Play].Add(new PlayerBlockPlacement());
			list[(int)SessionStatus.Play].Add(new UseItem());
		}

		public Task DispatchIncomingPacket(UncompressedPacket packet)
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
							try
							{
								using (var br = new NetworkReader(packet.data))
									pack.Deserialize(br);

								this.InvokeDispatchIncomingPacket(status, pack);
							}
							catch (System.NotImplementedException e)
							{
								UnityEngine.Debug.LogException(e);
							}
						}
						else
						{
							this.InvokeDispatchInvalidPacket(status, packet);
						}
					}
					break;
			}

			return Task.CompletedTask;
		}

		private void InvokeDispatchInvalidPacket(SessionStatus status, UncompressedPacket packet)
		{
			if (onDispatchInvalidPacket != null)
				onDispatchInvalidPacket(status, packet);

			this.OnDispatchInvalidPacket(status, packet);
		}

		private void InvokeDispatchIncomingPacket(SessionStatus status, IPacketSerializable packet)
		{
			if (onDispatchIncomingPacket != null)
				onDispatchIncomingPacket(status, packet);

			this.OnDispatchIncomingPacket(status, packet);
		}

		protected virtual void OnDispatchInvalidPacket(SessionStatus status, UncompressedPacket packet)
		{
		}

		protected virtual void OnDispatchIncomingPacket(SessionStatus status, IPacketSerializable packet)
		{
			switch (status)
			{
				case SessionStatus.Status:
					_statusHeader.OnDispatchIncomingPacket(packet);
					break;

				case SessionStatus.Login:
					_loginHeader.OnDispatchIncomingPacket(packet);
					break;

				case SessionStatus.Play:
					_playHeader.OnDispatchIncomingPacket(packet);
					break;
			}
		}
	}
}