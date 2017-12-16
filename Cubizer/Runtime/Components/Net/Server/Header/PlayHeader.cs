using Cubizer.Net.Protocol;
using Cubizer.Net.Protocol.Play.Serverbound;

namespace Cubizer.Net.Server.Header
{
	public sealed class PlayHeader : IPacketHeader
	{
		public void OnDispatchIncomingPacket(IPacketSerializable packet)
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
		}

		private void InvokePrepareCraftingGrid(PrepareCraftingGrid packet)
		{
		}

		private void InvokeTabComplete(TabComplete packet)
		{
		}

		private void InvokeChatMessage(ChatMessage packet)
		{
		}

		private void InvokeClientStatus(ClientStatus packet)
		{
		}

		private void InvokeClientSettings(ClientSettings packet)
		{
		}

		private void InvokeConfirmTransaction(ConfirmTransaction packet)
		{
		}

		private void InvokeEnchantItem(EnchantItem packet)
		{
		}

		private void InvokeClickWindow(ClickWindow packet)
		{
		}

		private void InvokeCloseWindow(CloseWindow packet)
		{
		}

		private void InvokePluginMessage(PluginMessage packet)
		{
		}

		private void InvokeUseEntity(UseEntity packet)
		{
		}

		private void InvokeKeepAlive(KeepAlive packet)
		{
		}

		private void InvokePlayer(Player packet)
		{
		}

		private void InvokePlayerPosition(PlayerPosition packet)
		{
		}

		private void InvokePlayerPositionAndLook(PlayerPositionAndLook packet)
		{
		}

		private void InvokePlayerLook(PlayerLook packet)
		{
		}

		private void InvokeVehicleMove(VehicleMove packet)
		{
		}

		private void InvokeSteerBoat(SteerBoat packet)
		{
		}

		private void InvokePlayerAbilities(PlayerAbilities packet)
		{
		}

		private void InvokePlayerDigging(PlayerDigging packet)
		{
		}

		private void InvokeEntityAction(EntityAction packet)
		{
		}

		private void InvokeSteerVehicle(SteerVehicle packet)
		{
		}

		private void InvokeCraftingBookData(CraftingBookData packet)
		{
		}

		private void InvokeResourcePackStatus(ResourcePackStatus packet)
		{
		}

		private void InvokeAdvancementTab(AdvancementTab packet)
		{
		}

		private void InvokeHeldItemChange(HeldItemChange packet)
		{
		}

		private void InvokeCreativeInventoryAction(CreativeInventoryAction packet)
		{
		}

		private void InvokeUpdateSign(UpdateSign packet)
		{
		}

		private void InvokeAnimation(Animation packet)
		{
		}

		private void InvokeSpectate(Spectate packet)
		{
		}

		private void InvokePlayerBlockPlacement(PlayerBlockPlacement packet)
		{
		}

		private void InvokeUseItem(UseItem packet)
		{
		}
	}
}