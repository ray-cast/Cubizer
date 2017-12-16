using Cubizer.Net.Protocol.Status.Serverbound;
using Cubizer.Net.Protocol.Handshakes.Serverbound;
using Cubizer.Net.Protocol.Login.Serverbound;
using Cubizer.Net.Protocol.Play.Serverbound;

namespace Cubizer.Net.Server
{
	public interface IPacketListener
	{
		void OnPing(Ping packet);
		void OnRequest(Request packet);

		void OnHandshake(Handshake handshake);

		void OnLoginStart(LoginStart packet);
		void OnEncryptionResponse(EncryptionResponse packet);
		void OnTeleportConfirm(TeleportConfirm packet);
		void OnPrepareCraftingGrid(PrepareCraftingGrid packet);
		void OnTabComplete(TabComplete packet);
		void OnChatMessage(ChatMessage packet);
		void OnClientStatus(ClientStatus packet);
		void OnClientSettings(ClientSettings packet);
		void OnConfirmTransaction(ConfirmTransaction packet);
		void OnEnchantItem(EnchantItem packet);
		void OnClickWindow(ClickWindow packet);
		void OnCloseWindow(CloseWindow packet);
		void OnPluginMessage(PluginMessage packet);
		void OnUseEntity(UseEntity packet);
		void OnKeepAlive(KeepAlive packet);
		void OnPlayer(Player packet);
		void OnPlayerPosition(PlayerPosition packet);
		void OnPlayerPositionAndLook(PlayerPositionAndLook packet);
		void OnPlayerLook(PlayerLook packet);
		void OnVehicleMove(VehicleMove packet);
		void OnSteerBoat(SteerBoat packet);
		void OnPlayerAbilities(PlayerAbilities packet);
		void OnPlayerDigging(PlayerDigging packet);
		void OnEntityAction(EntityAction packet);
		void OnSteerVehicle(SteerVehicle packet);
		void OnCraftingBookData(CraftingBookData packet);
		void OnResourcePackStatus(ResourcePackStatus packet);
		void OnAdvancementTab(AdvancementTab packet);
		void OnHeldItemChange(HeldItemChange packet);
		void OnCreativeInventoryAction(CreativeInventoryAction packet);
		void OnUpdateSign(UpdateSign packet);
		void OnAnimation(Animation packet);
		void OnSpectate(Spectate packet);
		void OnPlayerBlockPlacement(PlayerBlockPlacement packet);
		void OnUseItem(UseItem packet);
	}
}