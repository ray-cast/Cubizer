using Cubizer.Net.Protocol.Status.Clientbound;
using Cubizer.Net.Protocol.Login.Clientbound;
using Cubizer.Net.Protocol.Play.Clientbound;

namespace Cubizer.Net.Client
{
	public interface IPacketListener
	{
		void OnPong(Pong packet);
		void OnResponse(Response packet);

		void OnLoginSuccess(LoginSuccess packet);
		void OnLoginDisconnect(LoginDisconnect packet);
		void OnSetCompression(SetCompression packet);
		void OnEncryptionRequest(EncryptionRequest packet);

		void OnSpawnObject(SpawnObject packet);
		void OnSpawnExperienceOrb(SpawnExperienceOrb packet);
		void OnSpawnGlobalEntity(SpawnGlobalEntity packet);
		void OnSpawnMob(SpawnMob packet);
		void OnSpawnPainting(SpawnPainting packet);
		void OnSpawnPlayer(SpawnPlayer packet);
		void OnAnimation(Animation packet);
		void OnStatistics(Statistics packet);
		void OnBlockBreakAnimation(BlockBreakAnimation packet);
		void OnUpdateBlockEntity(UpdateBlockEntity packet);
		void OnBlockAction(BlockAction packet);
		void OnBlockChange(BlockChange packet);
		void OnBossBar(BossBar packet);
		void OnServerDifficulty(ServerDifficulty packet);
		void OnTabComplete(TabComplete packet);
		void OnChatMessage(ChatMessage packet);
		void OnMultiBlockChange(MultiBlockChange packet);
		void OnConfirmTransaction(ConfirmTransaction packet);
		void OnCloseWindow(CloseWindow packet);
		void OnOpenWindow(OpenWindow packet);
		void OnWindowItems(WindowItems packet);
		void OnWindowProperty(WindowProperty packet);
		void OnSetSlot(SetSlot packet);
		void OnSetCooldown(SetCooldown packet);
		void OnPluginMessage(PluginMessage packet);
		void OnNamedSoundEffect(NamedSoundEffect packet);
		void OnDisconnect(Disconnect packet);
		void OnEntityStatus(EntityStatus packet);
		void OnExplosion(Explosion packet);
		void OnChunkUnload(ChunkUnload packet);
		void OnChangeGameState(ChangeGameState packet);
		void OnKeepAlive(KeepAlive packet);
		void OnChunkData(ChunkData packet);
		void OnEffect(Effect packet);
		void OnParticle(Particle packet);
		void OnJoinGame(JoinGame packet);
		void OnMap(Map packet);
		void OnEntity(Entity packet);
		void OnEntityRelativeMove(EntityRelativeMove packet);
		void OnEntityLookAndRelativeMove(EntityLookAndRelativeMove packet);
		void OnEntityLook(EntityLook packet);
		void OnVehicleMove(VehicleMove packet);
		void OnOpenSignEditor(OpenSignEditor packet);
		void OnPlayerAbilities(PlayerAbilities packet);
		void OnCombatEvent(CombatEvent packet);
		void OnPlayerListItem(PlayerListItem packet);
		void OnPlayerPositionAndLook(PlayerPositionAndLook packet);
		void OnUseBed(UseBed packet);
		void OnUnlockRecipes(UnlockRecipes packet);
		void OnDestroyEntities(DestroyEntities packet);
		void OnRemoveEntityEffect(RemoveEntityEffect packet);
		void OnResourcePackSend(ResourcePackSend packet);
		void OnRespawn(Respawn packet);
		void OnEntityHeadLook(EntityHeadLook packet);
		void OnSelectAdvancementTab(SelectAdvancementTab packet);
		void OnWorldBorder(WorldBorder packet);
		void OnCamera(Camera packet);
		void OnHeldItemChange(HeldItemChange packet);
		void OnDisplayScoreboard(DisplayScoreboard packet);
		void OnEntityMetadata(EntityMetadata packet);
		void OnAttachEntity(AttachEntity packet);
		void OnEntityVelocity(EntityVelocity packet);
		void OnEntityEquipment(EntityEquipment packet);
		void OnSetExperience(SetExperience packet);
		void OnUpdateHealth(UpdateHealth packet);
		void OnScoreboardObjective(ScoreboardObjective packet);
		void OnSetPassengers(SetPassengers packet);
		void OnTeams(Teams packet);
		void OnUpdateScore(UpdateScore packet);
		void OnSpawnPosition(SpawnPosition packet);
		void OnTimeUpdate(TimeUpdate packet);
		void OnTitle(Title packet);
		void OnSoundEffect(SoundEffect packet);
		void OnPlayerListHeaderAndFooter(PlayerListHeaderAndFooter packet);
		void OnCollectItem(CollectItem packet);
		void OnEntityTeleport(EntityTeleport packet);
		void OnAdvancements(Advancements packet);
		void OnEntityProperties(EntityProperties packet);
		void OnEntityEffect(EntityEffect packet);
	}
}