using UnityEngine;

namespace Cubizer
{
	public abstract class IPlayer : MonoBehaviour
	{
		public abstract Camera player
		{
			get;
		}

		public abstract PlayerModel model
		{
			get;
		}
	}
}