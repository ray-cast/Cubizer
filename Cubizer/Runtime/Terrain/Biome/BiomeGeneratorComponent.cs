using UnityEngine;

namespace Cubizer
{
	public class BiomeGeneratorComponent : CubizerComponent<BiomeGeneratorModels>
	{
		private GameObject _biomeObject;

		public override bool active
		{
			get { return true; }
		}

		public IBiomeDataManager biomes
		{
			get { return model.settings.biomes; }
		}

		public override void OnEnable()
		{
			_biomeObject = new GameObject("TerrainBiomes");

			foreach (var it in model.settings.biomeGenerators)
			{
				if (it != null)
				{
					var gameObject = GameObject.Instantiate(it.gameObject);
					gameObject.name = it.name;
					gameObject.transform.parent = _biomeObject.transform;
				}
			}
		}

		public IBiomeData buildBiome(short x, short y, short z)
		{
			var transform = _biomeObject.transform;
			var transformCount = transform.childCount;

			for (int i = 0; i < transformCount; i++)
			{
				var biome = transform.GetChild(i).GetComponent<IBiomeGenerator>().OnBuildBiome(x, y, z);
				if (biome != null)
				{
					model.settings.biomes.Set(x, y, z, biome);
					return biome;
				}
			}

			model.settings.biomes.Set(x, y, z, model.settings.biomeNull);
			return model.settings.biomeNull;
		}

		public ChunkPrimer buildChunk(short x, short y, short z)
		{
			IBiomeData biomeData;
			if (!model.settings.biomes.Get(x, y, z, out biomeData))
				biomeData = this.buildBiome(x, y, z);

			var chunk = biomeData.OnBuildChunk(context.terrain, x, y, z);
			if (chunk == null)
				chunk = new ChunkPrimer(context.profile.terrain.settings.chunkSize, x, y, z);

			return chunk;
		}
	}
}