using Cubizer.Net.Protocol;
using Cubizer.Net.Protocol.Play.Serverbound;

namespace Cubizer.Net.Server
{
	public partial class ServerPacketRouter
	{
		public void DispatchPlayPacket(IPacketSerializable packet)
		{
			switch (packet.packetId)
			{
				case TeleportConfirm.Packet:
					this.InvokeTeleportConfirm(packet as TeleportConfirm);
					break;

				case PrepareCraftingGrid.Packet:
					this.InvokePrepareCraftingGrid(packet as PrepareCraftingGrid);
					break;

				case TabComplete.Packet:
					this.InvokeTabComplete(packet as TabComplete);
					break;

				case ChatMessage.Packet:
					this.InvokeChatMessage(packet as ChatMessage);
					break;

				case ClientStatus.Packet:
					this.InvokeClientStatus(packet as ClientStatus);
					break;

				case ClientSettings.Packet:
					this.InvokeClientSettings(packet as ClientSettings);
					break;

				case ConfirmTransaction.Packet:
					this.InvokeConfirmTransaction(packet as ConfirmTransaction);
					break;

				case EnchantItem.Packet:
					this.InvokeEnchantItem(packet as EnchantItem);
					break;

				case ClickWindow.Packet:
					this.InvokeClickWindow(packet as ClickWindow);
					break;

				case CloseWindow.Packet:
					this.InvokeCloseWindow(packet as CloseWindow);
					break;

				case PluginMessage.Packet:
					this.InvokePluginMessage(packet as PluginMessage);
					break;

				case UseEntity.Packet:
					this.InvokeUseEntity(packet as UseEntity);
					break;

				case KeepAlive.Packet:
					this.InvokeKeepAlive(packet as KeepAlive);
					break;

				case Player.Packet:
					this.InvokePlayer(packet as Player);
					break;

				case PlayerPosition.Packet:
					this.InvokePlayerPosition(packet as PlayerPosition);
					break;

				case PlayerPositionAndLook.Packet:
					this.InvokePlayerPositionAndLook(packet as PlayerPositionAndLook);
					break;

				case PlayerLook.Packet:
					this.InvokePlayerLook(packet as PlayerLook);
					break;

				case VehicleMove.Packet:
					this.InvokeVehicleMove(packet as VehicleMove);
					break;

				case SteerBoat.Packet:
					this.InvokeSteerBoat(packet as SteerBoat);
					break;

				case PlayerAbilities.Packet:
					this.InvokePlayerAbilities(packet as PlayerAbilities);
					break;

				case PlayerDigging.Packet:
					this.InvokePlayerDigging(packet as PlayerDigging);
					break;

				case EntityAction.Packet:
					this.InvokeEntityAction(packet as EntityAction);
					break;

				case SteerVehicle.Packet:
					this.InvokeSteerVehicle(packet as SteerVehicle);
					break;

				case CraftingBookData.Packet:
					this.InvokeCraftingBookData(packet as CraftingBookData);
					break;

				case ResourcePackStatus.Packet:
					this.InvokeResourcePackStatus(packet as ResourcePackStatus);
					break;

				case AdvancementTab.Packet:
					this.InvokeAdvancementTab(packet as AdvancementTab);
					break;

				case HeldItemChange.Packet:
					this.InvokeHeldItemChange(packet as HeldItemChange);
					break;

				case CreativeInventoryAction.Packet:
					this.InvokeCreativeInventoryAction(packet as CreativeInventoryAction);
					break;

				case UpdateSign.Packet:
					this.InvokeUpdateSign(packet as UpdateSign);
					break;

				case Animation.Packet:
					this.InvokeAnimation(packet as Animation);
					break;

				case Spectate.Packet:
					this.InvokeSpectate(packet as Spectate);
					break;

				case PlayerBlockPlacement.Packet:
					this.InvokePlayerBlockPlacement(packet as PlayerBlockPlacement);
					break;

				case UseItem.Packet:
					this.InvokeUseItem(packet as UseItem);
					break;
			}
		}

		private void InvokeTeleportConfirm(TeleportConfirm packet)
		{
			packetListener.OnTeleportConfirm(packet);
		}

		private void InvokePrepareCraftingGrid(PrepareCraftingGrid packet)
		{
			packetListener.OnPrepareCraftingGrid(packet);
		}

		private void InvokeTabComplete(TabComplete packet)
		{
			packetListener.OnTabComplete(packet);
		}

		private void InvokeChatMessage(ChatMessage packet)
		{
			packetListener.OnChatMessage(packet);
		}

		private void InvokeClientStatus(ClientStatus packet)
		{
			packetListener.OnClientStatus(packet);
		}

		private void InvokeClientSettings(ClientSettings packet)
		{
			packetListener.OnClientSettings(packet);
		}

		private void InvokeConfirmTransaction(ConfirmTransaction packet)
		{
			packetListener.OnConfirmTransaction(packet);
		}

		private void InvokeEnchantItem(EnchantItem packet)
		{
			packetListener.OnEnchantItem(packet);
		}

		private void InvokeClickWindow(ClickWindow packet)
		{
			packetListener.OnClickWindow(packet);
		}

		private void InvokeCloseWindow(CloseWindow packet)
		{
			packetListener.OnCloseWindow(packet);
		}

		private void InvokePluginMessage(PluginMessage packet)
		{
			packetListener.OnPluginMessage(packet);
		}

		private void InvokeUseEntity(UseEntity packet)
		{
			packetListener.OnUseEntity(packet);
		}

		private void InvokeKeepAlive(KeepAlive packet)
		{
			packetListener.OnKeepAlive(packet);
		}

		private void InvokePlayer(Player packet)
		{
			packetListener.OnPlayer(packet);
		}

		private void InvokePlayerPosition(PlayerPosition packet)
		{
			packetListener.OnPlayerPosition(packet);
		}

		private void InvokePlayerPositionAndLook(PlayerPositionAndLook packet)
		{
			packetListener.OnPlayerPositionAndLook(packet);
		}

		private void InvokePlayerLook(PlayerLook packet)
		{
			packetListener.OnPlayerLook(packet);
		}

		private void InvokeVehicleMove(VehicleMove packet)
		{
			packetListener.OnVehicleMove(packet);
		}

		private void InvokeSteerBoat(SteerBoat packet)
		{
			packetListener.OnSteerBoat(packet);
		}

		private void InvokePlayerAbilities(PlayerAbilities packet)
		{
			packetListener.OnPlayerAbilities(packet);
		}

		private void InvokePlayerDigging(PlayerDigging packet)
		{
			packetListener.OnPlayerDigging(packet);
		}

		private void InvokeEntityAction(EntityAction packet)
		{
			packetListener.OnEntityAction(packet);
		}

		private void InvokeSteerVehicle(SteerVehicle packet)
		{
			packetListener.OnSteerVehicle(packet);
		}

		private void InvokeCraftingBookData(CraftingBookData packet)
		{
			packetListener.OnCraftingBookData(packet);
		}

		private void InvokeResourcePackStatus(ResourcePackStatus packet)
		{
			packetListener.OnResourcePackStatus(packet);
		}

		private void InvokeAdvancementTab(AdvancementTab packet)
		{
			packetListener.OnAdvancementTab(packet);
		}

		private void InvokeHeldItemChange(HeldItemChange packet)
		{
			packetListener.OnHeldItemChange(packet);
		}

		private void InvokeCreativeInventoryAction(CreativeInventoryAction packet)
		{
			packetListener.OnCreativeInventoryAction(packet);
		}

		private void InvokeUpdateSign(UpdateSign packet)
		{
			packetListener.OnUpdateSign(packet);
		}

		private void InvokeAnimation(Animation packet)
		{
			packetListener.OnAnimation(packet);
		}

		private void InvokeSpectate(Spectate packet)
		{
			packetListener.OnSpectate(packet);
		}

		private void InvokePlayerBlockPlacement(PlayerBlockPlacement packet)
		{
			packetListener.OnPlayerBlockPlacement(packet);
		}

		private void InvokeUseItem(UseItem packet)
		{
			packetListener.OnUseItem(packet);
		}
	}
}