using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Cubizer/BasicObjectsBiome")]
	public class BasicObjectsBiomeGenerator : BiomeGenerator
	{
		public LiveBehaviour _materialGrass;
		public LiveBehaviour _materialSand;
		public LiveBehaviour _materialTree;
		public LiveBehaviour _materialTreeLeaf;
		public LiveBehaviour _materialFlower;
		public LiveBehaviour _materialWeed;
		public LiveBehaviour _materialObsidian;
		public LiveBehaviour _materialWater;
		public LiveBehaviour _materialCloud;
		public LiveBehaviour _materialSoil;

		public int layerGrass = 0;
		public int layerCloud = 3;
		public int layerObsidian = -10;

		public bool _isGenPlaneOnly = false;

		private BiomeData[] _biomeDatas;

		private static Dictionary<string, BasicObjectsParams> biomeParams = new Dictionary<string, BasicObjectsParams>
		{
			{ "None", new BasicObjectsParams { layer = BasicObjectBiomeType.Space } },
			{ "Plane", new BasicObjectsParams { layer = BasicObjectBiomeType.Plane } },
			{ "Cloud", new BasicObjectsParams { layer = BasicObjectBiomeType.Clound } },
			{ "Grassland", new BasicObjectsParams { layer = BasicObjectBiomeType.Grassland } }
		};

		public void Start()
		{
			var materials = new BasicObjectsMaterials();
			if (_materialGrass != null)
				materials.grass = _materialGrass.material;
			if (_materialSand != null)
				materials.sand = _materialSand.material;
			if (_materialTree != null)
				materials.tree = _materialTree.material;
			if (_materialTreeLeaf != null)
				materials.treeLeaf = _materialTreeLeaf.material;
			if (_materialFlower != null)
				materials.flower = _materialFlower.material;
			if (_materialWeed != null)
				materials.weed = _materialWeed.material;
			if (_materialObsidian != null)
				materials.obsidian = _materialObsidian.material;
			if (_materialWater != null)
				materials.water = _materialWater.material;
			if (_materialCloud != null)
				materials.cloud = _materialCloud.material;
			if (_materialCloud != null)
				materials.soil = _materialSoil.material;

			_biomeDatas = new BiomeData[biomeParams.Count];

			var written = 0;

			foreach (var param in biomeParams)
			{
				var biomeData = new BiomeData(new BasicObjectsChunkGenerator(param.Value, materials));
				_biomeDatas[written++] = biomeData;
			}
		}

		public override BiomeData OnBuildBiome(short x, short y, short z)
		{
			if (_isGenPlaneOnly)
			{
				if (y == layerGrass)
					return _biomeDatas[(int)BasicObjectBiomeType.Plane];
				else
					return _biomeDatas[(int)BasicObjectBiomeType.Space];
			}
			else
			{
				if (y == layerGrass)
					return _biomeDatas[(int)BasicObjectBiomeType.Grassland];
				else if (y == layerCloud)
					return _biomeDatas[(int)BasicObjectBiomeType.Clound];
				else
					return _biomeDatas[(int)BasicObjectBiomeType.Space];
			}
		}
	}
}