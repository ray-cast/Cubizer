using UnityEngine;

namespace Cubizer
{
	public abstract class IBiomeGenerator : MonoBehaviour
	{
		public abstract IBiomeData OnBuildBiome(short x, short y, short z);
	}
}