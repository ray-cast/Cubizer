using System.Collections;
using System.Collections.Generic;

using Cubizer.Math;

using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Cubizer/BasicObjectsBiomeGenerator")]
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
		private BasicObjectsMaterials _materials;

		private static Dictionary<string, BasicObjectsParams> biomeParams = new Dictionary<string, BasicObjectsParams>
		{
			{ "None", new BasicObjectsParams { layer = BasicObjectBiomeType.Space } },
			{ "Plane", new BasicObjectsParams { layer = BasicObjectBiomeType.Plane } },
			{ "Cloud", new BasicObjectsParams { layer = BasicObjectBiomeType.Clound } },
			{ "Sea", new BasicObjectsParams { layer = BasicObjectBiomeType.Sea } },
			{ "Grassland", new BasicObjectsParams { layer = BasicObjectBiomeType.Grassland } },
			{ "Sandland", new BasicObjectsParams { layer = BasicObjectBiomeType.Sandyland, isGenSand = true, isGenFlower = false,  isGenTree = false } },
			{ "Forest", new BasicObjectsParams { layer = BasicObjectBiomeType.Forest } }
		};

		public void Start()
		{
			_materials = new BasicObjectsMaterials();
			if (_materialGrass != null)
				_materials.grass = _materialGrass.material;
			if (_materialSand != null)
				_materials.sand = _materialSand.material;
			if (_materialTree != null)
				_materials.tree = _materialTree.material;
			if (_materialTreeLeaf != null)
				_materials.treeLeaf = _materialTreeLeaf.material;
			if (_materialFlower != null)
				_materials.flower = _materialFlower.material;
			if (_materialWeed != null)
				_materials.weed = _materialWeed.material;
			if (_materialObsidian != null)
				_materials.obsidian = _materialObsidian.material;
			if (_materialWater != null)
				_materials.water = _materialWater.material;
			if (_materialCloud != null)
				_materials.cloud = _materialCloud.material;
			if (_materialCloud != null)
				_materials.soil = _materialSoil.material;

			_biomeDatas = new BiomeData[biomeParams.Count];

			var written = 0;

			foreach (var param in biomeParams)
			{
				var biomeData = new BiomeData(new BasicObjectsChunkGenerator(param.Value, _materials));
				_biomeDatas[written++] = biomeData;
			}
		}

		public BiomeData buildForest(float noise)
		{
			var noiseParams = new NoiseParams() 
			{
				octaves = 6,
				loopX = 1.0f,
				loopY = 1.0f,
				persistence = 0.5f,
				lacunarity = 2.0f,
				threshold = 0.87f - (0.52f - noise) * 10.0f,
			};

			var parameters = new BasicObjectsParams { layer = BasicObjectBiomeType.Forest, tree = noiseParams };
			return new BiomeData(new BasicObjectsChunkGenerator(parameters, _materials));
		}

		public BiomeData buildSandyland(float noise)
		{
			if (noise > 0.497)
			{
				var parameters = new BasicObjectsParams { layer = BasicObjectBiomeType.Sandyland, isGenSand = true, isGenFlower = false, isGenTree = false, thresholdSand = (0.5f - noise) * 1000.0f };
				return new BiomeData(new BasicObjectsChunkGenerator(parameters, _materials));
			}
			else
			{
				var parameters = new BasicObjectsParams { layer = BasicObjectBiomeType.Sandyland, isGenSand = true, isGenFlower = false, isGenTree = false, isGenGrass = false };
				return new BiomeData(new BasicObjectsChunkGenerator(parameters, _materials));
			}
		}

		public BiomeData buildGrassland(float noise)
		{
			if (noise > 0.485f)
			{
				var parameters = new BasicObjectsParams { layer = BasicObjectBiomeType.Sandyland, isGenSand = true, thresholdSand = (0.49f - 0.485f) * 50.0f };
				return new BiomeData(new BasicObjectsChunkGenerator(parameters, _materials));
			}
			else
			{
				var parameters = new BasicObjectsParams { layer = BasicObjectBiomeType.Grassland, };
				return new BiomeData(new BasicObjectsChunkGenerator(parameters, _materials));
			}
		}

		public override IBiomeData OnBuildBiome(short x, short y, short z)
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
				{
					var noise = Noise.simplex2(x * 0.006f, y * 0.006f, 4, 0.5f, 2);
					if (noise >= 0.51 && noise <= 0.52)
						return buildForest(noise);
					else if (noise >= 0.5)
						return _biomeDatas[(int)BasicObjectBiomeType.Grassland];
					else if (noise >= 0.49)
						return buildSandyland(noise);
					else
						return buildGrassland(noise);
				}
				else if (y == layerCloud)
					return _biomeDatas[(int)BasicObjectBiomeType.Clound];
				else
					return _biomeDatas[(int)BasicObjectBiomeType.Space];
			}
		}
	}
}