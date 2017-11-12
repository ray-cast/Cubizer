using System;

using UnityEngine;

namespace Cubizer
{
	[ExecuteInEditMode]
	[AddComponentMenu("Cubizer/BiomeManager")]
	public class BiomeManager : MonoBehaviour
	{
		private Terrain _terrain;

		private BiomeGeneratorManager _biomes;

		public Terrain terrain
		{
			set
			{
				if (_terrain != value)
				{
					_terrain = value;
					UpdateTerrain();
				}
			}
			get
			{
				return _terrain;
			}
		}

		public BiomeGeneratorManager biomes
		{
			set
			{
				_biomes = value;
			}
			get
			{
				return _biomes;
			}
		}

		public void Start()
		{
			_biomes = new BiomeGeneratorManager();
		}

		private void UpdateTerrain()
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				var transformChild = transform.GetChild(i);
				transformChild.GetComponent<BiomeGenerator>().terrain = _terrain;
			}
		}

		public void OnBiomeEnable(BiomeGenerator biome)
		{
			biome.terrain = _terrain;
		}

		public void OnBiomeDisable(BiomeGenerator biome)
		{
		}

		public BiomeGenerator buildBiome(short x, short y, short z)
		{
			Debug.Assert(_biomes != null);

			var length = transform.childCount;

			for (int i = 0; i < length; i++)
			{
				var transformChild = transform.GetChild(i);
				var biome = transformChild.GetComponent<BiomeGenerator>().OnBuildBiome(x, y, z);
				if (biome != null)
				{
					_biomes.Set(x, y, z, biome);
					return biome;
				}
			}

			return null;
		}
	}
}