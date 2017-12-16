using Cubizer.Net.Protocol.Status.Serverbound;
using Cubizer.Net.Protocol.Handshakes.Serverbound;
using Cubizer.Net.Protocol.Login.Serverbound;
using Cubizer.Net.Protocol.Play.Serverbound;

namespace Cubizer.Net.Server
{
	public class ServerPacketListener : IPacketListener
	{
		public virtual void OnPing(Ping packet)
		{
		}
		public virtual void OnRequest(Request packet)
		{
		}

		public virtual void OnHandshake(Handshake handshake)
		{
		}

		public virtual void OnLoginStart(LoginStart packet)
		{
		}
		public virtual void OnEncryptionResponse(EncryptionResponse packet)
		{
		}
		public virtual void OnTeleportConfirm(TeleportConfirm packet)
		{
		}
		public virtual void OnPrepareCraftingGrid(PrepareCraftingGrid packet)
		{
		}
		public virtual void OnTabComplete(TabComplete packet)
		{
		}
		public virtual void OnChatMessage(ChatMessage packet)
		{
		}
		public virtual void OnClientStatus(ClientStatus packet)
		{
		}
		public virtual void OnClientSettings(ClientSettings packet)
		{
		}
		public virtual void OnConfirmTransaction(ConfirmTransaction packet)
		{
		}
		public virtual void OnEnchantItem(EnchantItem packet)
		{
		}
		public virtual void OnClickWindow(ClickWindow packet)
		{
		}
		public virtual void OnCloseWindow(CloseWindow packet)
		{
		}
		public virtual void OnPluginMessage(PluginMessage packet)
		{
		}
		public virtual void OnUseEntity(UseEntity packet)
		{
		}
		public virtual void OnKeepAlive(KeepAlive packet)
		{
		}
		public virtual void OnPlayer(Player packet)
		{
		}
		public virtual void OnPlayerPosition(PlayerPosition packet)
		{
		}
		public virtual void OnPlayerPositionAndLook(PlayerPositionAndLook packet)
		{
		}
		public virtual void OnPlayerLook(PlayerLook packet)
		{
		}
		public virtual void OnVehicleMove(VehicleMove packet)
		{
		}
		public virtual void OnSteerBoat(SteerBoat packet)
		{
		}
		public virtual void OnPlayerAbilities(PlayerAbilities packet)
		{
		}
		public virtual void OnPlayerDigging(PlayerDigging packet)
		{
		}
		public virtual void OnEntityAction(EntityAction packet)
		{
		}
		public virtual void OnSteerVehicle(SteerVehicle packet)
		{
		}
		public virtual void OnCraftingBookData(CraftingBookData packet)
		{
		}
		public virtual void OnResourcePackStatus(ResourcePackStatus packet)
		{
		}
		public virtual void OnAdvancementTab(AdvancementTab packet)
		{
		}
		public virtual void OnHeldItemChange(HeldItemChange packet)
		{
		}
		public virtual void OnCreativeInventoryAction(CreativeInventoryAction packet)
		{
		}
		public virtual void OnUpdateSign(UpdateSign packet)
		{
		}
		public virtual void OnAnimation(Animation packet)
		{
		}
		public virtual void OnSpectate(Spectate packet)
		{
		}
		public virtual void OnPlayerBlockPlacement(PlayerBlockPlacement packet)
		{
		}
		public virtual void OnUseItem(UseItem packet)
		{
		}
	}
}