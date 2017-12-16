using UnityEngine;

namespace Cubizer
{
	public interface IPlayer
	{
		Camera player
		{
			get;
		}

		PlayerModels model
		{
			get;
		}
	}
}