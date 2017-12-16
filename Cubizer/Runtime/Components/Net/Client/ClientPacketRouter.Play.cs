using Cubizer.Net.Protocol;
using Cubizer.Net.Protocol.Play.Clientbound;

namespace Cubizer.Net.Client
{
	public partial class ClientPacketRouter
	{
		private void DispatchPlayPacket(IPacketSerializable packet)
		{
			switch (packet.packetId)
			{
				case SpawnObject.Packet:
					this.InvokeSpawnObject(packet as SpawnObject);
					break;

				case SpawnExperienceOrb.Packet:
					this.InvokeSpawnExperienceOrb(packet as SpawnExperienceOrb);
					break;

				case SpawnGlobalEntity.Packet:
					this.InvokeSpawnGlobalEntity(packet as SpawnGlobalEntity);
					break;

				case SpawnMob.Packet:
					this.InvokeSpawnMob(packet as SpawnMob);
					break;

				case SpawnPainting.Packet:
					this.InvokeSpawnPainting(packet as SpawnPainting);
					break;

				case SpawnPlayer.Packet:
					this.InvokeSpawnPlayer(packet as SpawnPlayer);
					break;

				case Animation.Packet:
					this.InvokeAnimation(packet as Animation);
					break;

				case Statistics.Packet:
					this.InvokeStatistics(packet as Statistics);
					break;

				case BlockBreakAnimation.Packet:
					this.InvokeBlockBreakAnimation(packet as BlockBreakAnimation);
					break;

				case UpdateBlockEntity.Packet:
					this.InvokeUpdateBlockEntity(packet as UpdateBlockEntity);
					break;

				case BlockAction.Packet:
					this.InvokeBlockAction(packet as BlockAction);
					break;

				case BlockChange.Packet:
					this.InvokeBlockChange(packet as BlockChange);
					break;

				case BossBar.Packet:
					this.InvokeBossBar(packet as BossBar);
					break;

				case ServerDifficulty.Packet:
					this.InvokeServerDifficulty(packet as ServerDifficulty);
					break;

				case TabComplete.Packet:
					this.InvokeTabComplete(packet as TabComplete);
					break;

				case ChatMessage.Packet:
					this.InvokeChatMessage(packet as ChatMessage);
					break;

				case MultiBlockChange.Packet:
					this.InvokeMultiBlockChange(packet as MultiBlockChange);
					break;

				case ConfirmTransaction.Packet:
					this.InvokeConfirmTransaction(packet as ConfirmTransaction);
					break;

				case CloseWindow.Packet:
					this.InvokeCloseWindow(packet as CloseWindow);
					break;

				case OpenWindow.Packet:
					this.InvokeOpenWindow(packet as OpenWindow);
					break;

				case WindowItems.Packet:
					this.InvokeWindowItems(packet as WindowItems);
					break;

				case WindowProperty.Packet:
					this.InvokeWindowProperty(packet as WindowProperty);
					break;

				case SetSlot.Packet:
					this.InvokeSetSlot(packet as SetSlot);
					break;

				case SetCooldown.Packet:
					this.InvokeSetCooldown(packet as SetCooldown);
					break;

				case PluginMessage.Packet:
					this.InvokePluginMessage(packet as PluginMessage);
					break;

				case NamedSoundEffect.Packet:
					this.InvokeNamedSoundEffect(packet as NamedSoundEffect);
					break;

				case Disconnect.Packet:
					this.InvokeDisconnect(packet as Disconnect);
					break;

				case EntityStatus.Packet:
					this.InvokeEntityStatus(packet as EntityStatus);
					break;

				case Explosion.Packet:
					this.InvokeExplosion(packet as Explosion);
					break;

				case ChunkUnload.Packet:
					this.InvokeChunkUnload(packet as ChunkUnload);
					break;

				case ChangeGameState.Packet:
					this.InvokeChangeGameState(packet as ChangeGameState);
					break;

				case KeepAlive.Packet:
					this.InvokeKeepAlive(packet as KeepAlive);
					break;

				case ChunkData.Packet:
					this.InvokeChunkData(packet as ChunkData);
					break;

				case Effect.Packet:
					this.InvokeEffect(packet as Effect);
					break;

				case Particle.Packet:
					this.InvokeParticle(packet as Particle);
					break;

				case JoinGame.Packet:
					this.InvokeJoinGame(packet as JoinGame);
					break;

				case Map.Packet:
					this.InvokeMap(packet as Map);
					break;

				case Entity.Packet:
					this.InvokeEntity(packet as Entity);
					break;

				case EntityRelativeMove.Packet:
					this.InvokeEntityRelativeMove(packet as EntityRelativeMove);
					break;

				case EntityLookAndRelativeMove.Packet:
					this.InvokeEntityLookAndRelativeMove(packet as EntityLookAndRelativeMove);
					break;

				case EntityLook.Packet:
					this.InvokeEntityLook(packet as EntityLook);
					break;

				case VehicleMove.Packet:
					this.InvokeVehicleMove(packet as VehicleMove);
					break;

				case OpenSignEditor.Packet:
					this.InvokeOpenSignEditor(packet as OpenSignEditor);
					break;

				case PlayerAbilities.Packet:
					this.InvokePlayerAbilities(packet as PlayerAbilities);
					break;

				case CombatEvent.Packet:
					this.InvokeCombatEvent(packet as CombatEvent);
					break;

				case PlayerListItem.Packet:
					this.InvokePlayerListItem(packet as PlayerListItem);
					break;

				case PlayerPositionAndLook.Packet:
					this.InvokePlayerPositionAndLook(packet as PlayerPositionAndLook);
					break;

				case UseBed.Packet:
					this.InvokeUseBed(packet as UseBed);
					break;

				case UnlockRecipes.Packet:
					this.InvokeUnlockRecipes(packet as UnlockRecipes);
					break;

				case DestroyEntities.Packet:
					this.InvokeDestroyEntities(packet as DestroyEntities);
					break;

				case RemoveEntityEffect.Packet:
					this.InvokeRemoveEntityEffect(packet as RemoveEntityEffect);
					break;

				case ResourcePackSend.Packet:
					this.InvokeResourcePackSend(packet as ResourcePackSend);
					break;

				case Respawn.Packet:
					this.InvokeRespawn(packet as Respawn);
					break;

				case EntityHeadLook.Packet:
					this.InvokeEntityHeadLook(packet as EntityHeadLook);
					break;

				case SelectAdvancementTab.Packet:
					this.InvokeSelectAdvancementTab(packet as SelectAdvancementTab);
					break;

				case WorldBorder.Packet:
					this.InvokeWorldBorder(packet as WorldBorder);
					break;

				case Camera.Packet:
					this.InvokeCamera(packet as Camera);
					break;

				case HeldItemChange.Packet:
					this.InvokeHeldItemChange(packet as HeldItemChange);
					break;

				case DisplayScoreboard.Packet:
					this.InvokeDisplayScoreboard(packet as DisplayScoreboard);
					break;

				case EntityMetadata.Packet:
					this.InvokeEntityMetadata(packet as EntityMetadata);
					break;

				case AttachEntity.Packet:
					this.InvokeAttachEntity(packet as AttachEntity);
					break;

				case EntityVelocity.Packet:
					this.InvokeEntityVelocity(packet as EntityVelocity);
					break;

				case EntityEquipment.Packet:
					this.InvokeEntityEquipment(packet as EntityEquipment);
					break;

				case SetExperience.Packet:
					this.InvokeSetExperience(packet as SetExperience);
					break;

				case UpdateHealth.Packet:
					this.InvokeUpdateHealth(packet as UpdateHealth);
					break;

				case ScoreboardObjective.Packet:
					this.InvokeScoreboardObjective(packet as ScoreboardObjective);
					break;

				case SetPassengers.Packet:
					this.InvokeSetPassengers(packet as SetPassengers);
					break;

				case Teams.Packet:
					this.InvokeTeams(packet as Teams);
					break;

				case UpdateScore.Packet:
					this.InvokeUpdateScore(packet as UpdateScore);
					break;

				case SpawnPosition.Packet:
					this.InvokeSpawnPosition(packet as SpawnPosition);
					break;

				case TimeUpdate.Packet:
					this.InvokeTimeUpdate(packet as TimeUpdate);
					break;

				case Title.Packet:
					this.InvokeTitle(packet as Title);
					break;

				case SoundEffect.Packet:
					this.InvokeSoundEffect(packet as SoundEffect);
					break;

				case PlayerListHeaderAndFooter.Packet:
					this.InvokePlayerListHeaderAndFooter(packet as PlayerListHeaderAndFooter);
					break;

				case CollectItem.Packet:
					this.InvokeCollectItem(packet as CollectItem);
					break;

				case EntityTeleport.Packet:
					this.InvokeEntityTeleport(packet as EntityTeleport);
					break;

				case Advancements.Packet:
					this.InvokeAdvancements(packet as Advancements);
					break;

				case EntityProperties.Packet:
					this.InvokeEntityProperties(packet as EntityProperties);
					break;

				case EntityEffect.Packet:
					this.InvokeEntityEffect(packet as EntityEffect);
					break;
			}
		}

		private void InvokeSpawnObject(SpawnObject packet)
		{
			packetListener.OnSpawnObject(packet);
		}

		private void InvokeSpawnExperienceOrb(SpawnExperienceOrb packet)
		{
			packetListener.OnSpawnExperienceOrb(packet);
		}

		private void InvokeSpawnGlobalEntity(SpawnGlobalEntity packet)
		{
			packetListener.OnSpawnGlobalEntity(packet);
		}

		private void InvokeSpawnMob(SpawnMob packet)
		{
			packetListener.OnSpawnMob(packet);
		}

		private void InvokeSpawnPainting(SpawnPainting packet)
		{
			packetListener.OnSpawnPainting(packet);
		}

		private void InvokeSpawnPlayer(SpawnPlayer packet)
		{
			packetListener.OnSpawnPlayer(packet);
		}

		private void InvokeAnimation(Animation packet)
		{
			packetListener.OnAnimation(packet);
		}

		private void InvokeStatistics(Statistics packet)
		{
			packetListener.OnStatistics(packet);
		}

		private void InvokeBlockBreakAnimation(BlockBreakAnimation packet)
		{
			packetListener.OnBlockBreakAnimation(packet);
		}

		private void InvokeUpdateBlockEntity(UpdateBlockEntity packet)
		{
			packetListener.OnUpdateBlockEntity(packet);
		}

		private void InvokeBlockAction(BlockAction packet)
		{
			packetListener.OnBlockAction(packet);
		}

		private void InvokeBlockChange(BlockChange packet)
		{
			packetListener.OnBlockChange(packet);
		}

		private void InvokeBossBar(BossBar packet)
		{
			packetListener.OnBossBar(packet);
		}

		private void InvokeServerDifficulty(ServerDifficulty packet)
		{
			packetListener.OnServerDifficulty(packet);
		}

		private void InvokeTabComplete(TabComplete packet)
		{
			packetListener.OnTabComplete(packet);
		}

		private void InvokeChatMessage(ChatMessage packet)
		{
			packetListener.OnChatMessage(packet);
		}

		private void InvokeMultiBlockChange(MultiBlockChange packet)
		{
			packetListener.OnMultiBlockChange(packet);
		}

		private void InvokeConfirmTransaction(ConfirmTransaction packet)
		{
			packetListener.OnConfirmTransaction(packet);
		}

		private void InvokeCloseWindow(CloseWindow packet)
		{
			packetListener.OnCloseWindow(packet);
		}

		private void InvokeOpenWindow(OpenWindow packet)
		{
			packetListener.OnOpenWindow(packet);
		}

		private void InvokeWindowItems(WindowItems packet)
		{
			packetListener.OnWindowItems(packet);
		}

		private void InvokeWindowProperty(WindowProperty packet)
		{
			packetListener.OnWindowProperty(packet);
		}

		private void InvokeSetSlot(SetSlot packet)
		{
			packetListener.OnSetSlot(packet);
		}

		private void InvokeSetCooldown(SetCooldown packet)
		{
			packetListener.OnSetCooldown(packet);
		}

		private void InvokePluginMessage(PluginMessage packet)
		{
			packetListener.OnPluginMessage(packet);
		}

		private void InvokeNamedSoundEffect(NamedSoundEffect packet)
		{
			packetListener.OnNamedSoundEffect(packet);
		}

		private void InvokeDisconnect(Disconnect packet)
		{
			packetListener.OnDisconnect(packet);
		}

		private void InvokeEntityStatus(EntityStatus packet)
		{
			packetListener.OnEntityStatus(packet);
		}

		private void InvokeExplosion(Explosion packet)
		{
			packetListener.OnExplosion(packet);
		}

		private void InvokeChunkUnload(ChunkUnload packet)
		{
			packetListener.OnChunkUnload(packet);
		}

		private void InvokeChangeGameState(ChangeGameState packet)
		{
			packetListener.OnChangeGameState(packet);
		}

		private void InvokeKeepAlive(KeepAlive packet)
		{
			packetListener.OnKeepAlive(packet);
		}

		private void InvokeChunkData(ChunkData packet)
		{
			packetListener.OnChunkData(packet);
		}

		private void InvokeEffect(Effect packet)
		{
			packetListener.OnEffect(packet);
		}

		private void InvokeParticle(Particle packet)
		{
			packetListener.OnParticle(packet);
		}

		private void InvokeJoinGame(JoinGame packet)
		{
			packetListener.OnJoinGame(packet);
		}

		private void InvokeMap(Map packet)
		{
			packetListener.OnMap(packet);
		}

		private void InvokeEntity(Entity packet)
		{
			packetListener.OnEntity(packet);
		}

		private void InvokeEntityRelativeMove(EntityRelativeMove packet)
		{
			packetListener.OnEntityRelativeMove(packet);
		}

		private void InvokeEntityLookAndRelativeMove(EntityLookAndRelativeMove packet)
		{
			packetListener.OnEntityLookAndRelativeMove(packet);
		}

		private void InvokeEntityLook(EntityLook packet)
		{
			packetListener.OnEntityLook(packet);
		}

		private void InvokeVehicleMove(VehicleMove packet)
		{
			packetListener.OnVehicleMove(packet);
		}

		private void InvokeOpenSignEditor(OpenSignEditor packet)
		{
			packetListener.OnOpenSignEditor(packet);
		}

		private void InvokePlayerAbilities(PlayerAbilities packet)
		{
			packetListener.OnPlayerAbilities(packet);
		}

		private void InvokeCombatEvent(CombatEvent packet)
		{
			packetListener.OnCombatEvent(packet);
		}

		private void InvokePlayerListItem(PlayerListItem packet)
		{
			packetListener.OnPlayerListItem(packet);
		}

		private void InvokePlayerPositionAndLook(PlayerPositionAndLook packet)
		{
			packetListener.OnPlayerPositionAndLook(packet);
		}

		private void InvokeUseBed(UseBed packet)
		{
			packetListener.OnUseBed(packet);
		}

		private void InvokeUnlockRecipes(UnlockRecipes packet)
		{
			packetListener.OnUnlockRecipes(packet);
		}

		private void InvokeDestroyEntities(DestroyEntities packet)
		{
			packetListener.OnDestroyEntities(packet);
		}

		private void InvokeRemoveEntityEffect(RemoveEntityEffect packet)
		{
			packetListener.OnRemoveEntityEffect(packet);
		}

		private void InvokeResourcePackSend(ResourcePackSend packet)
		{
			packetListener.OnResourcePackSend(packet);
		}

		private void InvokeRespawn(Respawn packet)
		{
			packetListener.OnRespawn(packet);
		}

		private void InvokeEntityHeadLook(EntityHeadLook packet)
		{
			packetListener.OnEntityHeadLook(packet);
		}

		private void InvokeSelectAdvancementTab(SelectAdvancementTab packet)
		{
			packetListener.OnSelectAdvancementTab(packet);
		}

		private void InvokeWorldBorder(WorldBorder packet)
		{
			packetListener.OnWorldBorder(packet);
		}

		private void InvokeCamera(Camera packet)
		{
			packetListener.OnCamera(packet);
		}

		private void InvokeHeldItemChange(HeldItemChange packet)
		{
			packetListener.OnHeldItemChange(packet);
		}

		private void InvokeDisplayScoreboard(DisplayScoreboard packet)
		{
			packetListener.OnDisplayScoreboard(packet);
		}

		private void InvokeEntityMetadata(EntityMetadata packet)
		{
			packetListener.OnEntityMetadata(packet);
		}

		private void InvokeAttachEntity(AttachEntity packet)
		{
			packetListener.OnAttachEntity(packet);
		}

		private void InvokeEntityVelocity(EntityVelocity packet)
		{
			packetListener.OnEntityVelocity(packet);
		}

		private void InvokeEntityEquipment(EntityEquipment packet)
		{
			packetListener.OnEntityEquipment(packet);
		}

		private void InvokeSetExperience(SetExperience packet)
		{
			packetListener.OnSetExperience(packet);
		}

		private void InvokeUpdateHealth(UpdateHealth packet)
		{
			packetListener.OnUpdateHealth(packet);
		}

		private void InvokeScoreboardObjective(ScoreboardObjective packet)
		{
			packetListener.OnScoreboardObjective(packet);
		}

		private void InvokeSetPassengers(SetPassengers packet)
		{
			packetListener.OnSetPassengers(packet);
		}

		private void InvokeTeams(Teams packet)
		{
			packetListener.OnTeams(packet);
		}

		private void InvokeUpdateScore(UpdateScore packet)
		{
			packetListener.OnUpdateScore(packet);
		}

		private void InvokeSpawnPosition(SpawnPosition packet)
		{
			packetListener.OnSpawnPosition(packet);
		}

		private void InvokeTimeUpdate(TimeUpdate packet)
		{
			packetListener.OnTimeUpdate(packet);
		}

		private void InvokeTitle(Title packet)
		{
			packetListener.OnTitle(packet);
		}

		private void InvokeSoundEffect(SoundEffect packet)
		{
			packetListener.OnSoundEffect(packet);
		}

		private void InvokePlayerListHeaderAndFooter(PlayerListHeaderAndFooter packet)
		{
			packetListener.OnPlayerListHeaderAndFooter(packet);
		}

		private void InvokeCollectItem(CollectItem packet)
		{
			packetListener.OnCollectItem(packet);
		}

		private void InvokeEntityTeleport(EntityTeleport packet)
		{
			packetListener.OnEntityTeleport(packet);
		}

		private void InvokeAdvancements(Advancements packet)
		{
			packetListener.OnAdvancements(packet);
		}

		private void InvokeEntityProperties(EntityProperties packet)
		{
			packetListener.OnEntityProperties(packet);
		}

		private void InvokeEntityEffect(EntityEffect packet)
		{
			packetListener.OnEntityEffect(packet);
		}
	}
}