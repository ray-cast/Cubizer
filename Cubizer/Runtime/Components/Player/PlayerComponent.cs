using System;
using System.Collections.Generic;

namespace Cubizer.Players
{
	public sealed class PlayerComponent : CubizerComponent<PlayerManagerModels>
	{
		public List<IPlayer> players = new List<IPlayer>();

		public override bool active
		{
			get
			{
				return model.enabled;
			}
			set
			{
				model.enabled = value;
			}
		}

		public override void OnEnable()
		{
		}

		public void Connection(IPlayer player)
		{
			if (player != null)
			{
				if (players.Count < model.settings.maxPlayers)
					players.Add(player);
			}
			else
			{
				throw new ArgumentNullException("Connection() fail");
			}
		}

		public void Disconnect(IPlayer player)
		{
			if (player != null)
			{
				players.Remove(player);
			}
			else
			{
				throw new ArgumentNullException("Disconnect() fail");
			}
		}
	}
}