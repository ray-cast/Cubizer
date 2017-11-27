using UnityEngine;

namespace Cubizer
{
	public interface IPlayerListener
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