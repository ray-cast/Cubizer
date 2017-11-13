using UnityEngine;
using System.Collections;

namespace Cubizer
{
	public abstract class BiomeGenerator : MonoBehaviour
	{
		private Terrain _terrain;

		public Terrain terrain
		{
			set { _terrain = value; }
			get { return _terrain; }
		}

		public void InvokeDefaultOnEnable()
		{
			if (transform.parent != null)
			{
				var biome = transform.parent.GetComponent<BiomeGeneratorManager>();
				biome.OnBiomeEnable(this);
			}
		}

		public void InvokeDefaultOnDisable()
		{
			if (transform.parent != null)
			{
				var biome = transform.parent.GetComponent<BiomeGeneratorManager>();
				biome.OnBiomeDisable(this);
			}
		}

		public void OnEnable()
		{
			this.InvokeDefaultOnEnable();
		}

		public void OnDestroy()
		{
			this.InvokeDefaultOnDisable();
		}

		public abstract BiomeData OnBuildBiome(short x, short y, short z);
	}
}