using System;

using Cubizer.Time;
using Cubizer.Net.Client;
using Cubizer.Net.Protocol.Play.Serverbound;

namespace Cubizer.Net
{
	public sealed partial class ClientComponent
	{
		private partial class PacketListener : IPacketListener
		{
			private TimeComponent _time;

			public void PacketListenerPlayCtor()
			{
				_time = client.GetComponent<TimeComponent>() as TimeComponent;
			}

			void IPacketListener.OnAdvancements(Protocol.Play.Clientbound.Advancements packet)
			{
			}

			void IPacketListener.OnAnimation(Protocol.Play.Clientbound.Animation packet)
			{
			}

			void IPacketListener.OnAttachEntity(Protocol.Play.Clientbound.AttachEntity packet)
			{
			}

			void IPacketListener.OnBlockAction(Protocol.Play.Clientbound.BlockAction packet)
			{
			}

			void IPacketListener.OnBlockBreakAnimation(Protocol.Play.Clientbound.BlockBreakAnimation packet)
			{
			}

			void IPacketListener.OnBlockChange(Protocol.Play.Clientbound.BlockChange packet)
			{
			}

			void IPacketListener.OnBossBar(Protocol.Play.Clientbound.BossBar packet)
			{
			}

			void IPacketListener.OnCamera(Protocol.Play.Clientbound.Camera packet)
			{
			}

			void IPacketListener.OnChangeGameState(Protocol.Play.Clientbound.ChangeGameState packet)
			{
			}

			void IPacketListener.OnChatMessage(Protocol.Play.Clientbound.ChatMessage packet)
			{
			}

			void IPacketListener.OnChunkData(Protocol.Play.Clientbound.ChunkData packet)
			{
			}

			void IPacketListener.OnChunkUnload(Protocol.Play.Clientbound.ChunkUnload packet)
			{
			}

			void IPacketListener.OnCloseWindow(Protocol.Play.Clientbound.CloseWindow packet)
			{
			}

			void IPacketListener.OnCollectItem(Protocol.Play.Clientbound.CollectItem packet)
			{
			}

			void IPacketListener.OnCombatEvent(Protocol.Play.Clientbound.CombatEvent packet)
			{
			}

			void IPacketListener.OnConfirmTransaction(Protocol.Play.Clientbound.ConfirmTransaction packet)
			{
			}

			void IPacketListener.OnDestroyEntities(Protocol.Play.Clientbound.DestroyEntities packet)
			{
			}

			void IPacketListener.OnDisconnect(Protocol.Play.Clientbound.Disconnect packet)
			{
			}

			void IPacketListener.OnDisplayScoreboard(Protocol.Play.Clientbound.DisplayScoreboard packet)
			{
			}

			void IPacketListener.OnEffect(Protocol.Play.Clientbound.Effect packet)
			{
			}

			void IPacketListener.OnEntity(Protocol.Play.Clientbound.Entity packet)
			{
			}

			void IPacketListener.OnEntityEffect(Protocol.Play.Clientbound.EntityEffect packet)
			{
			}

			void IPacketListener.OnEntityEquipment(Protocol.Play.Clientbound.EntityEquipment packet)
			{
			}

			void IPacketListener.OnEntityHeadLook(Protocol.Play.Clientbound.EntityHeadLook packet)
			{
			}

			void IPacketListener.OnEntityLook(Protocol.Play.Clientbound.EntityLook packet)
			{
			}

			void IPacketListener.OnEntityLookAndRelativeMove(Protocol.Play.Clientbound.EntityLookAndRelativeMove packet)
			{
			}

			void IPacketListener.OnEntityMetadata(Protocol.Play.Clientbound.EntityMetadata packet)
			{
			}

			void IPacketListener.OnEntityProperties(Protocol.Play.Clientbound.EntityProperties packet)
			{
			}

			void IPacketListener.OnEntityRelativeMove(Protocol.Play.Clientbound.EntityRelativeMove packet)
			{
			}

			void IPacketListener.OnEntityStatus(Protocol.Play.Clientbound.EntityStatus packet)
			{
			}

			void IPacketListener.OnEntityTeleport(Protocol.Play.Clientbound.EntityTeleport packet)
			{
			}

			void IPacketListener.OnEntityVelocity(Protocol.Play.Clientbound.EntityVelocity packet)
			{
			}

			void IPacketListener.OnExplosion(Protocol.Play.Clientbound.Explosion packet)
			{
			}

			void IPacketListener.OnHeldItemChange(Protocol.Play.Clientbound.HeldItemChange packet)
			{
			}

			void IPacketListener.OnJoinGame(Protocol.Play.Clientbound.JoinGame packet)
			{
			}

			void IPacketListener.OnKeepAlive(Protocol.Play.Clientbound.KeepAlive packet)
			{
				client._client.SendOutcomingPacket(new KeepAlive { keepAliveID = (uint)new Random().Next() });
			}

			void IPacketListener.OnMap(Protocol.Play.Clientbound.Map packet)
			{
			}

			void IPacketListener.OnMultiBlockChange(Protocol.Play.Clientbound.MultiBlockChange packet)
			{
			}

			void IPacketListener.OnNamedSoundEffect(Protocol.Play.Clientbound.NamedSoundEffect packet)
			{
			}

			void IPacketListener.OnOpenSignEditor(Protocol.Play.Clientbound.OpenSignEditor packet)
			{
			}

			void IPacketListener.OnOpenWindow(Protocol.Play.Clientbound.OpenWindow packet)
			{
			}

			void IPacketListener.OnParticle(Protocol.Play.Clientbound.Particle packet)
			{
			}

			void IPacketListener.OnPlayerAbilities(Protocol.Play.Clientbound.PlayerAbilities packet)
			{
			}

			void IPacketListener.OnPlayerListHeaderAndFooter(Protocol.Play.Clientbound.PlayerListHeaderAndFooter packet)
			{
			}

			void IPacketListener.OnPlayerListItem(Protocol.Play.Clientbound.PlayerListItem packet)
			{
			}

			void IPacketListener.OnPlayerPositionAndLook(Protocol.Play.Clientbound.PlayerPositionAndLook packet)
			{
			}

			void IPacketListener.OnPluginMessage(Protocol.Play.Clientbound.PluginMessage packet)
			{
			}

			void IPacketListener.OnRemoveEntityEffect(Protocol.Play.Clientbound.RemoveEntityEffect packet)
			{
			}

			void IPacketListener.OnResourcePackSend(Protocol.Play.Clientbound.ResourcePackSend packet)
			{
			}

			void IPacketListener.OnRespawn(Protocol.Play.Clientbound.Respawn packet)
			{
			}

			void IPacketListener.OnScoreboardObjective(Protocol.Play.Clientbound.ScoreboardObjective packet)
			{
			}

			void IPacketListener.OnSelectAdvancementTab(Protocol.Play.Clientbound.SelectAdvancementTab packet)
			{
			}

			void IPacketListener.OnServerDifficulty(Protocol.Play.Clientbound.ServerDifficulty packet)
			{
			}

			void IPacketListener.OnSetCooldown(Protocol.Play.Clientbound.SetCooldown packet)
			{
			}

			void IPacketListener.OnSetExperience(Protocol.Play.Clientbound.SetExperience packet)
			{
			}

			void IPacketListener.OnSetPassengers(Protocol.Play.Clientbound.SetPassengers packet)
			{
			}

			void IPacketListener.OnSetSlot(Protocol.Play.Clientbound.SetSlot packet)
			{
			}

			void IPacketListener.OnSoundEffect(Protocol.Play.Clientbound.SoundEffect packet)
			{
			}

			void IPacketListener.OnSpawnExperienceOrb(Protocol.Play.Clientbound.SpawnExperienceOrb packet)
			{
			}

			void IPacketListener.OnSpawnGlobalEntity(Protocol.Play.Clientbound.SpawnGlobalEntity packet)
			{
			}

			void IPacketListener.OnSpawnMob(Protocol.Play.Clientbound.SpawnMob packet)
			{
			}

			void IPacketListener.OnSpawnObject(Protocol.Play.Clientbound.SpawnObject packet)
			{
			}

			void IPacketListener.OnSpawnPainting(Protocol.Play.Clientbound.SpawnPainting packet)
			{
			}

			void IPacketListener.OnSpawnPlayer(Protocol.Play.Clientbound.SpawnPlayer packet)
			{
			}

			void IPacketListener.OnSpawnPosition(Protocol.Play.Clientbound.SpawnPosition packet)
			{
			}

			void IPacketListener.OnStatistics(Protocol.Play.Clientbound.Statistics packet)
			{
			}

			void IPacketListener.OnTabComplete(Protocol.Play.Clientbound.TabComplete packet)
			{
			}

			void IPacketListener.OnTeams(Protocol.Play.Clientbound.Teams packet)
			{
			}

			void IPacketListener.OnTimeUpdate(Protocol.Play.Clientbound.TimeUpdate packet)
			{
				_time.worldAge = packet.worldAge;
				_time.timeOfDay = packet.timeOfDay;
			}

			void IPacketListener.OnTitle(Protocol.Play.Clientbound.Title packet)
			{
			}

			void IPacketListener.OnUnlockRecipes(Protocol.Play.Clientbound.UnlockRecipes packet)
			{
			}

			void IPacketListener.OnUpdateBlockEntity(Protocol.Play.Clientbound.UpdateBlockEntity packet)
			{
			}

			void IPacketListener.OnUpdateHealth(Protocol.Play.Clientbound.UpdateHealth packet)
			{
			}

			void IPacketListener.OnUpdateScore(Protocol.Play.Clientbound.UpdateScore packet)
			{
			}

			void IPacketListener.OnUseBed(Protocol.Play.Clientbound.UseBed packet)
			{
			}

			void IPacketListener.OnVehicleMove(Protocol.Play.Clientbound.VehicleMove packet)
			{
			}

			void IPacketListener.OnWindowItems(Protocol.Play.Clientbound.WindowItems packet)
			{
			}

			void IPacketListener.OnWindowProperty(Protocol.Play.Clientbound.WindowProperty packet)
			{
			}

			void IPacketListener.OnWorldBorder(Protocol.Play.Clientbound.WorldBorder packet)
			{
			}
		}
	}
}