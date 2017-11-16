using UnityEngine;

namespace Cubizer
{
	[ExecuteInEditMode]
	[AddComponentMenu("Cubizer/BiomeGeneratorManager")]
	public class BiomeGeneratorManager : MonoBehaviour
	{
		[SerializeField]
		private Terrain _terrain;

		private IBiomeData _biomeNull = new BiomeDataNull();
		private IBiomeDataManager _biomes = new BiomeDataManager();

		public Terrain terrain
		{
			get { return _terrain; }
		}

		public IBiomeDataManager biomes
		{
			get { return _biomes; }
		}

		public void Awake()
		{
			if (_terrain == null)
				Debug.LogError("Please assign a Terrain in Inspector.");
		}

		public IBiomeData buildBiome(short x, short y, short z)
		{
			Debug.Assert(_biomes != null);

			for (int i = 0; i < transform.childCount; i++)
			{
				var transformChild = transform.GetChild(i);
				var biome = transformChild.GetComponent<IBiomeGenerator>().OnBuildBiome(x, y, z);
				if (biome != null)
				{
					_biomes.Set(x, y, z, biome);
					return biome;
				}
			}

			_biomes.Set(x, y, z, _biomeNull);
			return _biomeNull;
		}

		public ChunkPrimer buildChunk(short x, short y, short z)
		{
			IBiomeData biomeData;
			if (!_biomes.Get(x, y, z, out biomeData))
				biomeData = this.buildBiome(x, y, z);

			var chunk = biomeData.OnBuildChunk(_terrain, x, y, z);
			if (chunk == null)
				chunk = new ChunkPrimer(_terrain.chunkSize, _terrain.chunkSize, _terrain.chunkSize, x, y, z);

			return chunk;
		}
	}
}