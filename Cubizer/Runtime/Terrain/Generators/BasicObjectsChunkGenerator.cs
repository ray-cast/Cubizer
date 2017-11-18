using Cubizer;
using Cubizer.Math;

namespace Cubizer
{
	public class BasicObjectsChunkGenerator : IChunkGenerator
	{
		private BasicObjectsMaterials _materials;
		private BasicObjectsParams _params;

		public BasicObjectsChunkGenerator(BasicObjectsParams parameters, BasicObjectsMaterials materials)
		{
			_params = parameters;
			_materials = materials;

			if (_params.isGenTree && materials.tree == null)
				UnityEngine.Debug.LogError("Please drag a Tree into Hierarchy View.");
			if (_params.isGenTree && materials.treeLeaf == null)
				UnityEngine.Debug.LogError("Please drag a TreeLeaf into Hierarchy View.");
			if (_params.isGenGrass && materials.sand == null)
				UnityEngine.Debug.LogError("Please drag a Sand into Hierarchy View.");
			if (_params.isGenFlower && materials.flower == null)
				UnityEngine.Debug.LogError("Please drag a Flower into Hierarchy View.");
			if (_params.isGenWeed && materials.weed == null)
				UnityEngine.Debug.LogError("Please drag a Weed into Hierarchy View.");
			if (_params.isGenObsidian && materials.obsidian == null)
				UnityEngine.Debug.LogError("Please drag a Obsidian into Hierarchy View.");
			if (_params.isGenWater && materials.water == null)
				UnityEngine.Debug.LogError("Please drag a Water into Hierarchy View.");
			if (_params.isGenCloud && materials.cloud == null)
				UnityEngine.Debug.LogError("Please drag a Cloud into Hierarchy View.");
			if (_params.isGenSoil && materials.soil == null)
				UnityEngine.Debug.LogError("Please drag a Soil into Hierarchy View.");
		}

		public ChunkPrimer BuildPlaneOnly(Terrain terrain, short x, short y, short z)
		{
			var size = terrain.profile.terrain.settings.chunkSize;
			var map = new ChunkPrimer(size, x, y, z, size * size * _params.floorBase);

			for (byte ix = 0; ix < map.voxels.bound.x; ix++)
			{
				for (byte iz = 0; iz < map.voxels.bound.z; iz++)
				{
					for (byte iy = 0; iy < _params.floorBase; iy++)
						map.voxels.Set(ix, iy, iz, _materials.grass);
				}
			}

			return map;
		}

		public void buildTree(ChunkPrimer map, byte ix, byte iz, byte h)
		{
			for (int iy = h + 3; iy < h + 8; iy++)
			{
				for (int ox = -3; ox <= 3; ox++)
				{
					for (int oz = -3; oz <= 3; oz++)
					{
						int d = (ox * ox) + (oz * oz) + (iy - (h + 4)) * (iy - (h + 4));
						if (d < 11)
							map.voxels.Set((byte)(ix + ox), (byte)iy, (byte)(iz + oz), _materials.treeLeaf);
					}
				}
			}

			for (byte iy = h; iy < h + 7; iy++)
				map.voxels.Set(ix, iy, iz, _materials.tree);
		}

		public void BuildGrass(ChunkPrimer map, byte ix, byte iz, int dx, int dz, VoxelMaterial main, out float f, out byte h)
		{
			f = Noise.simplex2(_params.grass.loopX * dx, _params.grass.loopZ * dz, _params.grass.octaves, _params.grass.persistence, _params.grass.lacunarity);
			f = (f * (f * _params.floorHeightLismit + _params.floorBase));

			h = System.Math.Max((byte)1, (byte)f);

			if (_params.isGenGrass || _params.isGenSand)
			{
				if (_params.isGenGrass && _params.isGenSand)
				{
					UnityEngine.Random.InitState(ix ^ iz * h);

					for (byte iy = 0; iy < h; iy++)
						map.voxels.Set(ix, iy, iz, UnityEngine.Random.value > _params.thresholdSand ? _materials.grass : _materials.sand);
				}
				else
				{
					var waterHeight = _params.floorBase - _params.floorHeightLismit * 0.2f;
					if (h < waterHeight)
					{
						for (byte iy = 0; iy < h; iy++)
							map.voxels.Set(ix, iy, iz, _materials.dirt);
					}
					else
					{
						for (byte iy = 0; iy < h - 1; iy++)
							map.voxels.Set(ix, iy, iz, _materials.dirt);

						map.voxels.Set(ix, (byte)(h - 1), iz, main);
					}
				}
			}
		}

		public ChunkPrimer Buildland(Terrain terrain, short x, short y, short z, VoxelMaterial main)
		{
			var size = terrain.profile.terrain.settings.chunkSize;
			var map = new ChunkPrimer(size, x, y, z, size * size * _params.floorBase);

			int offsetX = x * map.voxels.bound.x;
			int offsetZ = z * map.voxels.bound.z;

			for (byte ix = 0; ix < map.voxels.bound.x; ix++)
			{
				for (byte iz = 0; iz < map.voxels.bound.z; iz++)
				{
					int dx = offsetX + ix;
					int dz = offsetZ + iz;

					float f; byte h;
					this.BuildGrass(map, ix, iz, dx, dz, main, out f, out h);

					var waterHeight = (byte)(_params.floorBase - _params.floorHeightLismit * 0.2f);
					if (_params.isGenWater && h <= waterHeight)
					{
						for (byte iy = h; iy <= waterHeight; iy++)
							map.voxels.Set(ix, iy, iz, _materials.water);
					}
					else
					{
						if (f > waterHeight && f < (waterHeight + 0.1))
							map.voxels.Set(ix, (byte)(h - 1), iz, _materials.sand);

						if (_params.isGenWeed && Noise.simplex2(
								_params.weeds.loopX * dx,
								_params.weeds.loopZ * dz,
								_params.weeds.octaves,
								_params.weeds.persistence,
								_params.weeds.lacunarity) > _params.weeds.threshold)
						{
							UnityEngine.Random.InitState(ix * iz);
							map.voxels.Set(ix, h, iz, _materials.weed[(int)System.Math.Round(UnityEngine.Random.value * (_materials.weed.Length - 1))]);
						}
						else if (_params.isGenFlower && Noise.simplex2(
								_params.flowers.loopX * dx,
								_params.flowers.loopZ * dz,
								_params.flowers.octaves,
								_params.flowers.persistence,
								_params.flowers.lacunarity) > _params.flowers.threshold)
						{
							UnityEngine.Random.InitState(ix * iz);
							map.voxels.Set(ix, h, iz, _materials.flower[(int)System.Math.Round(UnityEngine.Random.value * (_materials.flower.Length - 1))]);
						}
						else if (_params.isGenTree && h < map.voxels.bound.y - 8)
						{
							if (ix > 3 && ix < map.voxels.bound.y - 3 && iz > 3 && iz < map.voxels.bound.y - 3)
							{
								if (Noise.simplex2(
									_params.tree.loopX * dx,
									_params.tree.loopZ * dz,
									_params.tree.octaves,
									_params.tree.persistence,
									_params.tree.lacunarity) > _params.tree.threshold)
								{
									this.buildTree(map, ix, iz, h);
								}
							}
						}
					}
				}
			}

			return map;
		}

		public ChunkPrimer BuildClouds(Terrain terrain, short x, short y, short z)
		{
			var map = new ChunkPrimer(terrain.profile.terrain.settings.chunkSize, x, y, z);

			int offsetX = x * map.voxels.bound.x;
			int offsetY = y * map.voxels.bound.y;
			int offsetZ = z * map.voxels.bound.z;

			for (int ix = 0; ix < map.voxels.bound.x; ix++)
			{
				for (int iz = 0; iz < map.voxels.bound.y; iz++)
				{
					int dx = offsetX + ix;
					int dz = offsetZ + iz;

					for (int iy = 0; iy < 8; iy++)
					{
						int dy = offsetY + iy;

						if (Noise.simplex3(dx * 0.01f, dy * 0.1f, dz * 0.01f, 8, 0.5f, 2) > _params.thresholdCloud)
							map.voxels.Set((byte)ix, (byte)iy, (byte)iz, _materials.cloud);
					}
				}
			}

			return map;
		}

		public ChunkPrimer buildObsidian(Terrain terrain, short x, short y, short z)
		{
			var size = terrain.profile.terrain.settings.chunkSize;
			var map = new ChunkPrimer(size, x, y, z, size * size * 8);

			for (byte ix = 0; ix < map.voxels.bound.x; ix++)
			{
				for (byte iz = 0; iz < map.voxels.bound.z; iz++)
				{
					for (byte iy = 0; iy < 8; iy++)
						map.voxels.Set(ix, iy, iz, _materials.obsidian);
				}
			}

			return map;
		}

		public ChunkPrimer OnCreateChunk(Terrain terrain, short x, short y, short z)
		{
			switch (_params.layer)
			{
				case BasicObjectBiomeType.Plane:
					return BuildPlaneOnly(terrain, x, y, z);

				case BasicObjectBiomeType.Clound:
					return BuildClouds(terrain, x, y, z);

				case BasicObjectBiomeType.Grassland:
					return Buildland(terrain, x, y, z, _materials.grass);

				case BasicObjectBiomeType.Forest:
					return Buildland(terrain, x, y, z, _materials.grass);

				case BasicObjectBiomeType.Sandyland:
					return Buildland(terrain, x, y, z, _materials.sand);

				default:
					return null;
			}
		}
	}
}