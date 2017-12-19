using System.Threading.Tasks;
using System.Collections.Generic;

using Cubizer.Net.Protocol;
using Cubizer.Net.Protocol.Serialization;
using Cubizer.Net.Protocol.Status.Clientbound;
using Cubizer.Net.Protocol.Login.Clientbound;
using Cubizer.Net.Protocol.Play.Clientbound;

namespace Cubizer.Net.Client
{
	public partial class ClientPacketRouter : IPacketRouter
	{
		public SessionStatus status { get; set; }

		public OnDispatchIncomingPacketDelegate onDispatchIncomingPacket { get; set; }
		public OnDispatchInvalidPacketDelegate onDispatchInvalidPacket { get; set; }

		private readonly IPacketListener packetListener;
		private readonly List<IPacketSerializable>[] list = new List<IPacketSerializable>[(int)SessionStatus.MaxEnum];

		public ClientPacketRouter(IPacketListener listener = null)
		{
			packetListener = listener ?? new ClientPacketListenerNull();

			list[(int)SessionStatus.Status] = new List<IPacketSerializable>(2);
			list[(int)SessionStatus.Status].Add(new Pong());
			list[(int)SessionStatus.Status].Add(new Response());

			list[(int)SessionStatus.Login] = new List<IPacketSerializable>(3);
			list[(int)SessionStatus.Login].Add(new LoginDisconnect());
			list[(int)SessionStatus.Login].Add(new EncryptionRequest());
			list[(int)SessionStatus.Login].Add(new LoginSuccess());
			list[(int)SessionStatus.Login].Add(new SetCompression());

			list[(int)SessionStatus.Play] = new List<IPacketSerializable>(80);
			list[(int)SessionStatus.Play].Add(new SpawnObject());
			list[(int)SessionStatus.Play].Add(new SpawnExperienceOrb());
			list[(int)SessionStatus.Play].Add(new SpawnGlobalEntity());
			list[(int)SessionStatus.Play].Add(new SpawnMob());
			list[(int)SessionStatus.Play].Add(new SpawnPainting());
			list[(int)SessionStatus.Play].Add(new SpawnPlayer());
			list[(int)SessionStatus.Play].Add(new Animation());
			list[(int)SessionStatus.Play].Add(new Statistics());
			list[(int)SessionStatus.Play].Add(new BlockBreakAnimation());
			list[(int)SessionStatus.Play].Add(new UpdateBlockEntity());
			list[(int)SessionStatus.Play].Add(new BlockAction());
			list[(int)SessionStatus.Play].Add(new BlockChange());
			list[(int)SessionStatus.Play].Add(new BossBar());
			list[(int)SessionStatus.Play].Add(new ServerDifficulty());
			list[(int)SessionStatus.Play].Add(new TabComplete());
			list[(int)SessionStatus.Play].Add(new ChatMessage());
			list[(int)SessionStatus.Play].Add(new MultiBlockChange());
			list[(int)SessionStatus.Play].Add(new ConfirmTransaction());
			list[(int)SessionStatus.Play].Add(new CloseWindow());
			list[(int)SessionStatus.Play].Add(new OpenWindow());
			list[(int)SessionStatus.Play].Add(new WindowItems());
			list[(int)SessionStatus.Play].Add(new WindowProperty());
			list[(int)SessionStatus.Play].Add(new SetSlot());
			list[(int)SessionStatus.Play].Add(new SetCooldown());
			list[(int)SessionStatus.Play].Add(new PluginMessage());
			list[(int)SessionStatus.Play].Add(new NamedSoundEffect());
			list[(int)SessionStatus.Play].Add(new Disconnect());
			list[(int)SessionStatus.Play].Add(new EntityStatus());
			list[(int)SessionStatus.Play].Add(new Explosion());
			list[(int)SessionStatus.Play].Add(new ChunkUnload());
			list[(int)SessionStatus.Play].Add(new ChangeGameState());
			list[(int)SessionStatus.Play].Add(new KeepAlive());
			list[(int)SessionStatus.Play].Add(new ChunkData());
			list[(int)SessionStatus.Play].Add(new Effect());
			list[(int)SessionStatus.Play].Add(new Particle());
			list[(int)SessionStatus.Play].Add(new JoinGame());
			list[(int)SessionStatus.Play].Add(new Map());
			list[(int)SessionStatus.Play].Add(new Entity());
			list[(int)SessionStatus.Play].Add(new EntityRelativeMove());
			list[(int)SessionStatus.Play].Add(new EntityLookAndRelativeMove());
			list[(int)SessionStatus.Play].Add(new EntityLook());
			list[(int)SessionStatus.Play].Add(new VehicleMove());
			list[(int)SessionStatus.Play].Add(new OpenSignEditor());
			list[(int)SessionStatus.Play].Add(new CraftRecipeResponse());
			list[(int)SessionStatus.Play].Add(new PlayerAbilities());
			list[(int)SessionStatus.Play].Add(new CombatEvent());
			list[(int)SessionStatus.Play].Add(new PlayerListItem());
			list[(int)SessionStatus.Play].Add(new PlayerPositionAndLook());
			list[(int)SessionStatus.Play].Add(new UseBed());
			list[(int)SessionStatus.Play].Add(new UnlockRecipes());
			list[(int)SessionStatus.Play].Add(new DestroyEntities());
			list[(int)SessionStatus.Play].Add(new RemoveEntityEffect());
			list[(int)SessionStatus.Play].Add(new ResourcePackSend());
			list[(int)SessionStatus.Play].Add(new Respawn());
			list[(int)SessionStatus.Play].Add(new EntityHeadLook());
			list[(int)SessionStatus.Play].Add(new SelectAdvancementTab());
			list[(int)SessionStatus.Play].Add(new WorldBorder());
			list[(int)SessionStatus.Play].Add(new Camera());
			list[(int)SessionStatus.Play].Add(new HeldItemChange());
			list[(int)SessionStatus.Play].Add(new DisplayScoreboard());
			list[(int)SessionStatus.Play].Add(new EntityMetadata());
			list[(int)SessionStatus.Play].Add(new AttachEntity());
			list[(int)SessionStatus.Play].Add(new EntityVelocity());
			list[(int)SessionStatus.Play].Add(new EntityEquipment());
			list[(int)SessionStatus.Play].Add(new SetExperience());
			list[(int)SessionStatus.Play].Add(new UpdateHealth());
			list[(int)SessionStatus.Play].Add(new ScoreboardObjective());
			list[(int)SessionStatus.Play].Add(new SetPassengers());
			list[(int)SessionStatus.Play].Add(new Teams());
			list[(int)SessionStatus.Play].Add(new UpdateScore());
			list[(int)SessionStatus.Play].Add(new SpawnPosition());
			list[(int)SessionStatus.Play].Add(new TimeUpdate());
			list[(int)SessionStatus.Play].Add(new Title());
			list[(int)SessionStatus.Play].Add(new SoundEffect());
			list[(int)SessionStatus.Play].Add(new PlayerListHeaderAndFooter());
			list[(int)SessionStatus.Play].Add(new CollectItem());
			list[(int)SessionStatus.Play].Add(new EntityTeleport());
			list[(int)SessionStatus.Play].Add(new Advancements());
			list[(int)SessionStatus.Play].Add(new EntityProperties());
			list[(int)SessionStatus.Play].Add(new EntityEffect());
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

		private void OnDispatchInvalidPacket(SessionStatus status, UncompressedPacket packet)
		{
		}

		private void OnDispatchIncomingPacket(SessionStatus status, IPacketSerializable packet)
		{
			switch (status)
			{
				case SessionStatus.Status:
					this.DispatchStatusPacket(packet);
					break;

				case SessionStatus.Login:
					this.DispatchLoginPacket(packet);
					break;

				case SessionStatus.Play:
					this.DispatchPlayPacket(packet);
					break;
			}
		}
	}
}