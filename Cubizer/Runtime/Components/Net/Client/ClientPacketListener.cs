using Cubizer.Net.Protocol.Login.Clientbound;
using Cubizer.Net.Protocol.Play.Clientbound;
using Cubizer.Net.Protocol.Status.Clientbound;

namespace Cubizer.Net.Client
{
	public class ClientPacketListener : IPacketListener
	{
		public virtual void OnAdvancements(Advancements packet)
		{
		}
		public virtual void OnAnimation(Animation packet)
		{
		}
		public virtual void OnAttachEntity(AttachEntity packet)
		{
		}
		public virtual void OnBlockAction(BlockAction packet)
		{
		}
		public virtual void OnBlockBreakAnimation(BlockBreakAnimation packet)
		{
		}
		public virtual void OnBlockChange(BlockChange packet)
		{
		}
		public virtual void OnBossBar(BossBar packet)
		{
		}
		public virtual void OnCamera(Camera packet)
		{
		}
		public virtual void OnChangeGameState(ChangeGameState packet)
		{
		}
		public virtual void OnChatMessage(ChatMessage packet)
		{
		}
		public virtual void OnChunkData(ChunkData packet)
		{
		}
		public virtual void OnChunkUnload(ChunkUnload packet)
		{
		}
		public virtual void OnCloseWindow(CloseWindow packet)
		{
		}
		public virtual void OnCollectItem(CollectItem packet)
		{
		}
		public virtual void OnCombatEvent(CombatEvent packet)
		{
		}
		public virtual void OnConfirmTransaction(ConfirmTransaction packet)
		{
		}
		public virtual void OnDestroyEntities(DestroyEntities packet)
		{
		}
		public virtual void OnDisconnect(Disconnect packet)
		{
		}
		public virtual void OnDisplayScoreboard(DisplayScoreboard packet)
		{
		}
		public virtual void OnEffect(Effect packet)
		{
		}
		public virtual void OnEncryptionRequest(EncryptionRequest packet)
		{
		}
		public virtual void OnEntity(Entity packet)
		{
		}
		public virtual void OnEntityEffect(EntityEffect packet)
		{
		}
		public virtual void OnEntityEquipment(EntityEquipment packet)
		{
		}
		public virtual void OnEntityHeadLook(EntityHeadLook packet)
		{
		}
		public virtual void OnEntityLook(EntityLook packet)
		{
		}
		public virtual void OnEntityLookAndRelativeMove(EntityLookAndRelativeMove packet)
		{
		}
		public virtual void OnEntityMetadata(EntityMetadata packet)
		{
		}
		public virtual void OnEntityProperties(EntityProperties packet)
		{
		}
		public virtual void OnEntityRelativeMove(EntityRelativeMove packet)
		{
		}
		public virtual void OnEntityStatus(EntityStatus packet)
		{
		}
		public virtual void OnEntityTeleport(EntityTeleport packet)
		{
		}
		public virtual void OnEntityVelocity(EntityVelocity packet)
		{
		}
		public virtual void OnExplosion(Explosion packet)
		{
		}
		public virtual void OnHeldItemChange(HeldItemChange packet)
		{
		}
		public virtual void OnJoinGame(JoinGame packet)
		{
		}
		public virtual void OnKeepAlive(KeepAlive packet)
		{
		}
		public virtual void OnLoginDisconnect(LoginDisconnect packet)
		{
		}
		public virtual void OnLoginSuccess(LoginSuccess packet)
		{
		}
		public virtual void OnMap(Map packet)
		{
		}
		public virtual void OnMultiBlockChange(MultiBlockChange packet)
		{
		}
		public virtual void OnNamedSoundEffect(NamedSoundEffect packet)
		{
		}
		public virtual void OnOpenSignEditor(OpenSignEditor packet)
		{
		}
		public virtual void OnOpenWindow(OpenWindow packet)
		{
		}
		public virtual void OnParticle(Particle packet)
		{
		}
		public virtual void OnPlayerAbilities(PlayerAbilities packet)
		{
		}
		public virtual void OnPlayerListHeaderAndFooter(PlayerListHeaderAndFooter packet)
		{
		}
		public virtual void OnPlayerListItem(PlayerListItem packet)
		{
		}
		public virtual void OnPlayerPositionAndLook(PlayerPositionAndLook packet)
		{
		}
		public virtual void OnPluginMessage(PluginMessage packet)
		{
		}
		public virtual void OnPong(Pong packet)
		{
		}
		public virtual void OnRemoveEntityEffect(RemoveEntityEffect packet)
		{
		}
		public virtual void OnResourcePackSend(ResourcePackSend packet)
		{
		}
		public virtual void OnRespawn(Respawn packet)
		{
		}
		public virtual void OnResponse(Response packet)
		{
		}
		public virtual void OnScoreboardObjective(ScoreboardObjective packet)
		{
		}
		public virtual void OnSelectAdvancementTab(SelectAdvancementTab packet)
		{
		}
		public virtual void OnServerDifficulty(ServerDifficulty packet)
		{
		}
		public virtual void OnSetCompression(SetCompression packet)
		{
		}
		public virtual void OnSetCooldown(SetCooldown packet)
		{
		}
		public virtual void OnSetExperience(SetExperience packet)
		{
		}
		public virtual void OnSetPassengers(SetPassengers packet)
		{
		}
		public virtual void OnSetSlot(SetSlot packet)
		{
		}
		public virtual void OnSoundEffect(SoundEffect packet)
		{
		}
		public virtual void OnSpawnExperienceOrb(SpawnExperienceOrb packet)
		{
		}
		public virtual void OnSpawnGlobalEntity(SpawnGlobalEntity packet)
		{
		}
		public virtual void OnSpawnMob(SpawnMob packet)
		{
		}
		public virtual void OnSpawnObject(SpawnObject packet)
		{
		}
		public virtual void OnSpawnPainting(SpawnPainting packet)
		{
		}
		public virtual void OnSpawnPlayer(SpawnPlayer packet)
		{
		}
		public virtual void OnSpawnPosition(SpawnPosition packet)
		{
		}
		public virtual void OnStatistics(Statistics packet)
		{
		}
		public virtual void OnTabComplete(TabComplete packet)
		{
		}
		public virtual void OnTeams(Teams packet)
		{
		}
		public virtual void OnTimeUpdate(TimeUpdate packet)
		{
		}
		public virtual void OnTitle(Title packet)
		{
		}
		public virtual void OnUnlockRecipes(UnlockRecipes packet)
		{
		}
		public virtual void OnUpdateBlockEntity(UpdateBlockEntity packet)
		{
		}
		public virtual void OnUpdateHealth(UpdateHealth packet)
		{
		}
		public virtual void OnUpdateScore(UpdateScore packet)
		{
		}
		public virtual void OnUseBed(UseBed packet)
		{
		}
		public virtual void OnVehicleMove(VehicleMove packet)
		{
		}
		public virtual void OnWindowItems(WindowItems packet)
		{
		}
		public virtual void OnWindowProperty(WindowProperty packet)
		{
		}
		public virtual void OnWorldBorder(WorldBorder packet)
		{
		}
	}
}