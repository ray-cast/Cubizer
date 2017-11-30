using UnityEngine;

namespace Cubizer
{
	public interface IPlayer
	{
		Camera player
		{
			get;
		}

		PlayerModel model
		{
			get;
		}
	}
}