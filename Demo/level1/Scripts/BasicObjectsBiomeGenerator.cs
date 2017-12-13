using System.Collections;
using System.Collections.Generic;

using Cubizer.Math;
using Cubizer.Biome;

using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Cubizer/BasicObjectsBiomeGenerator")]
	public class BasicObjectsBiomeGenerator : BiomeGenerator
	{
		public GameObject _materialDirt;
		public GameObject _materialGrass;
		public GameObject _materialSand;
		public GameObject _materialTree;
		public GameObject _materialTreeLeaf;
		public GameObject _materialObsidian;
		public GameObject _materialWater;
		public GameObject _materialCloud;
		public GameObject _materialSoil;

		public GameObject[] _materialFlower;
		public GameObject[] _materialWeed;
		public GameObject[] _materialTrees;

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
			{ "Cloud", new BasicObjectsParams { layer = BasicObjectBiomeType.Cloud } },
			{ "Sea", new BasicObjectsParams { layer = BasicObjectBiomeType.Sea } },
			{ "Grassland", new BasicObjectsParams { layer = BasicObjectBiomeType.Grassland } },
			{ "Sandland", new BasicObjectsParams { layer = BasicObjectBiomeType.Sandyland, isGenSand = true, isGenFlower = false,  isGenTree = false } },
			{ "Forest", new BasicObjectsParams { layer = BasicObjectBiomeType.Forest } }
		};

		public void Start()
		{
			Math.Noise.simplex_seed(context.profile.terrain.settings.seed);

			_materials = new BasicObjectsMaterials();

			var materialFactor = context.materialFactory;
			if (_materialDirt != null)
				_materials.dirt = materialFactor.GetMaterial(_materialDirt.GetComponent<LiveBehaviour>().name);
			if (_materialGrass != null)
				_materials.grass = materialFactor.GetMaterial(_materialGrass.GetComponent<LiveBehaviour>().name);
			if (_materialSand != null)
				_materials.sand = materialFactor.GetMaterial(_materialSand.GetComponent<LiveBehaviour>().name);
			if (_materialTree != null)
				_materials.tree = materialFactor.GetMaterial(_materialTree.GetComponent<LiveBehaviour>().name);
			if (_materialTreeLeaf != null)
				_materials.treeLeaf = materialFactor.GetMaterial(_materialTreeLeaf.GetComponent<LiveBehaviour>().name);
			if (_materialObsidian != null)
				_materials.obsidian = materialFactor.GetMaterial(_materialObsidian.GetComponent<LiveBehaviour>().name);
			if (_materialWater != null)
				_materials.water = materialFactor.GetMaterial(_materialWater.GetComponent<LiveBehaviour>().name);
			if (_materialCloud != null)
				_materials.cloud = materialFactor.GetMaterial(_materialCloud.GetComponent<LiveBehaviour>().name);
			if (_materialCloud != null)
				_materials.soil = materialFactor.GetMaterial(_materialSoil.GetComponent<LiveBehaviour>().name);

			_materials.flower = new VoxelMaterial[_materialFlower.Length];
			for (int i = 0; i < _materialFlower.Length; i++)
				_materials.flower[i] = materialFactor.GetMaterial(_materialFlower[i].GetComponent<LiveBehaviour>().name);

			_materials.weed = new VoxelMaterial[_materialWeed.Length];
			for (int i = 0; i < _materialWeed.Length; i++)
				_materials.weed[i] = materialFactor.GetMaterial(_materialWeed[i].GetComponent<LiveBehaviour>().name);

			_materials.trees = new VoxelMaterial[_materialTrees.Length];
			for (int i = 0; i < _materialTrees.Length; i++)
				_materials.trees[i] = materialFactor.GetMaterial(_materialTrees[i].GetComponent<LiveBehaviour>().name);

			_biomeDatas = new BiomeData[biomeParams.Count];

			var written = 0;

			foreach (var param in biomeParams)
			{
				var biomeData = new BiomeData(new BasicObjectsChunkGenerator(param.Value, _materials));
				_biomeDatas[written++] = biomeData;
			}

			_biomeDatas[(int)BasicObjectBiomeType.Plane] = new BiomeData(new PlaneChunkGenerator(biomeParams["Plane"], _materials));
			_biomeDatas[(int)BasicObjectBiomeType.Cloud] = new BiomeData(new CloudChunkGenerator(biomeParams["Cloud"], _materials));
		}

		public BiomeData buildForest(float noise)
		{
			var noiseParams = new NoiseParams()
			{
				octaves = 6,
				loopX = 1.0f,
				loopZ = 1.0f,
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

		public override IBiomeData OnBuildBiome(int x, int y, int z)
		{
			Debug.Assert(_biomeDatas != null);

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
					return _biomeDatas[(int)BasicObjectBiomeType.Cloud];
				else
					return _biomeDatas[(int)BasicObjectBiomeType.Space];
			}
		}
	}
}