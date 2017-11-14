using UnityEngine;
using System.Collections;

namespace Cubizer
{
	public abstract class BiomeGenerator : MonoBehaviour, IBiomeGenerator
	{
		private Terrain _terrain;

		public Terrain terrain
		{
			get { return _terrain; }
		}

		public void InvokeDefaultOnEnable()
		{
			if (transform.parent != null)
				_terrain = transform.parent.GetComponent<BiomeGeneratorManager>().terrain;
		}

		public void OnEnable()
		{
			this.InvokeDefaultOnEnable();
		}

		public abstract IBiomeData OnBuildBiome(short x, short y, short z);
	}
}