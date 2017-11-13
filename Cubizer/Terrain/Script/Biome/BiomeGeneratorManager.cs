using System;

using UnityEngine;

namespace Cubizer
{
	[ExecuteInEditMode]
	[AddComponentMenu("Cubizer/BiomeGeneratorManager")]
	public class BiomeGeneratorManager : MonoBehaviour
	{
		[SerializeField]
		private Terrain _terrain;

		private BiomeDataNull _biomeNull = new BiomeDataNull();
		private BiomeDataManager _biomes = new BiomeDataManager();

		public Terrain terrain
		{
			get { return _terrain; }
		}

		public BiomeDataManager biomes
		{
			get { return _biomes; }
		}

		public void Awake()
		{
			if (_terrain == null)
				Debug.LogError("Please assign a Terrain in Inspector.");
		}

		public void OnBiomeEnable(BiomeGenerator biome)
		{
			biome.terrain = _terrain;
		}

		public void OnBiomeDisable(BiomeGenerator biome)
		{
		}

		public BiomeData buildBiome(short x, short y, short z)
		{
			Debug.Assert(_biomes != null);

			for (int i = 0; i < transform.childCount; i++)
			{
				var transformChild = transform.GetChild(i);
				var biome = transformChild.GetComponent<BiomeGenerator>().OnBuildBiome(x, y, z);
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
			BiomeData biomeData = null;
			if (!_biomes.Get(x, y, z, ref biomeData))
				biomeData = this.buildBiome(x, y, z);

			if (biomeData.chunkGenerator != null)
			{
				var biomes = new Vector3Int[6];
				biomes[0] = new Vector3Int(x - 1, y, z);
				biomes[1] = new Vector3Int(x, y - 1, z);
				biomes[2] = new Vector3Int(x, y, z - 1);
				biomes[3] = new Vector3Int(x + 1, y, z);
				biomes[4] = new Vector3Int(x, y + 1, z);
				biomes[5] = new Vector3Int(x, y, z + 1);

				for (int j = 0; j < 6; j++)
				{
					if (_biomes.Exists(biomes[j].x, biomes[j].y, biomes[j].z))
						continue;

					BiomeData biomeTemp = null;
					for (int i = 0; i < transform.childCount; i++)
					{
						var biomeGenerator = transform.GetChild(i).GetComponent<BiomeGenerator>();

						biomeTemp = biomeGenerator.OnBuildBiome((short)biomes[j].x, (short)biomes[j].y, (short)biomes[j].z);
						if (biomeTemp != null)
							break;
					}

					if (biomeTemp == null)
						biomeTemp = _biomeNull;

					_biomes.Set((short)biomes[j].x, (short)biomes[j].y, (short)biomes[j].z, biomeTemp);
				}
			}

			var chunk = biomeData.OnBuildChunk(_terrain, x, y, z);
			if (chunk == null)
				chunk = new ChunkPrimer(_terrain.chunkSize, _terrain.chunkSize, _terrain.chunkSize, x, y, z);

			return chunk;
		}
	}
}