using UnityEngine;
using System.Collections;

namespace Cubizer
{
	public abstract class BiomeGenerator : MonoBehaviour
	{
		private Terrain _terrain;

		[SerializeField]
		private ChunkGenerator _chunkGenerator;

		public Terrain terrain
		{
			set { _terrain = value; }
			get { return _terrain; }
		}

		public ChunkGenerator chunkGenerator
		{
			set { _chunkGenerator = value; }
			get { return _chunkGenerator; }
		}

		public void InvokeDefaultOnEnable()
		{
			if (transform.parent != null)
			{
				var biome = transform.parent.GetComponent<BiomeManager>();
				biome.OnBiomeEnable(this);
			}
		}

		public void InvokeDefaultOnDisable()
		{
			if (transform.parent != null)
			{
				var biome = transform.parent.GetComponent<BiomeManager>();
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

		public virtual BiomeGenerator OnBuildBiome(short x, short y, short z)
		{
			return null;
		}
	}
}