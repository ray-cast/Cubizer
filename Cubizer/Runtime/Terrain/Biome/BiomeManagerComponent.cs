using UnityEngine;

namespace Cubizer
{
	public class BiomeManagerComponent : CubizerComponent<BiomeGeneratorModels>
	{
		private GameObject _biomeObject;

		public int count
		{
			get { return _biomeObject != null ? _biomeObject.transform.childCount : 0; }
		}

		public override bool active
		{
			get { return true; }
		}

		public IBiomeDataManager biomes
		{
			get { return model.settings.biomeManager; }
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

		public IBiomeData buildBiomeIfNotExist(int x, int y, int z)
		{
			IBiomeData biomeData = null;
			if (this.biomes.Get(x, y, z, out biomeData))
				return biomeData;

			var transform = _biomeObject.transform;
			var transformCount = transform.childCount;

			for (int i = 0; i < transformCount; i++)
			{
				var biome = transform.GetChild(i).GetComponent<IBiomeGenerator>().OnBuildBiome((short)x, (short)y, (short)z);
				if (biome != null)
				{
					model.settings.biomeManager.Set(x, y, z, biome);
					return biome;
				}
			}

			model.settings.biomeManager.Set(x, y, z, model.settings.biomeNull);
			return model.settings.biomeNull;
		}
	}
}