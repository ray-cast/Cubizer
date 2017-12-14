using Cubizer.Math;
using Cubizer.Chunk;

namespace Cubizer
{
	public class BasicObjectsChunkGenerator : IChunkGenerator
	{
		private BasicObjectsMaterials _materials;
		private BasicObjectsParams _params;

		private System.Random random = new System.Random();

		public BasicObjectsChunkGenerator(BasicObjectsParams parameters, BasicObjectsMaterials materials)
		{
			if (parameters.isGenTree && materials.tree == null)
				UnityEngine.Debug.LogError("Please drag a Tree into Hierarchy View.");
			if (parameters.isGenTree && materials.treeLeaf == null)
				UnityEngine.Debug.LogError("Please drag a TreeLeaf into Hierarchy View.");
			if (parameters.isGenGrass && materials.sand == null)
				UnityEngine.Debug.LogError("Please drag a Sand into Hierarchy View.");
			if (parameters.isGenFlower && materials.flower == null)
				UnityEngine.Debug.LogError("Please drag a Flower into Hierarchy View.");
			if (parameters.isGenWeed && materials.weed == null)
				UnityEngine.Debug.LogError("Please drag a Weed into Hierarchy View.");
			if (parameters.isGenObsidian && materials.obsidian == null)
				UnityEngine.Debug.LogError("Please drag a Obsidian into Hierarchy View.");
			if (parameters.isGenWater && materials.water == null)
				UnityEngine.Debug.LogError("Please drag a Water into Hierarchy View.");
			if (parameters.isGenCloud && materials.cloud == null)
				UnityEngine.Debug.LogError("Please drag a Cloud into Hierarchy View.");
			if (parameters.isGenSoil && materials.soil == null)
				UnityEngine.Debug.LogError("Please drag a Soil into Hierarchy View.");

			_params = parameters;
			_materials = materials;
		}

		public void BuildTree(ChunkPrimer map, byte ix, byte iz, byte h)
		{
			map.voxels.Set(ix, h, iz, _materials.trees[random.Next(0, _materials.trees.Length - 1)]);
		}

		public void BuildGrass(ChunkPrimer map, byte ix, byte iz, int dx, int dz, VoxelMaterial main, out float f, out byte h)
		{
			f = Noise.Simplex2(_params.grass.loopX * dx, _params.grass.loopZ * dz, _params.grass.octaves, _params.grass.persistence, _params.grass.lacunarity);
			f = (f * (f * _params.floorHeightLismit + _params.floorBase));

			h = System.Math.Max((byte)1, (byte)f);

			if (_params.isGenGrass || _params.isGenSand)
			{
				if (_params.isGenGrass && _params.isGenSand)
				{
					for (byte iy = 0; iy < h; iy++)
						map.voxels.Set(ix, iy, iz, random.Next() > _params.thresholdSand ? _materials.grass : _materials.sand);
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

		public ChunkPrimer Buildland(CubizerBehaviour terrain, int x, int y, int z, VoxelMaterial main)
		{
			var size = terrain.profile.chunk.settings.chunkSize;
			var map = new ChunkPrimer(size, x, y, z, size * size * _params.floorBase);

			int offsetX = x * map.voxels.Bound.x;
			int offsetZ = z * map.voxels.Bound.z;

			for (byte ix = 0; ix < map.voxels.Bound.x; ix++)
			{
				for (byte iz = 0; iz < map.voxels.Bound.z; iz++)
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

						if (_params.isGenWeed && Noise.Simplex2(
								_params.weeds.loopX * dx,
								_params.weeds.loopZ * dz,
								_params.weeds.octaves,
								_params.weeds.persistence,
								_params.weeds.lacunarity) > _params.weeds.threshold)
						{
							map.voxels.Set(ix, h, iz, _materials.weed[random.Next(0, _materials.weed.Length - 1)]);
						}
						else if (_params.isGenFlower && Noise.Simplex2(
								_params.flowers.loopX * dx,
								_params.flowers.loopZ * dz,
								_params.flowers.octaves,
								_params.flowers.persistence,
								_params.flowers.lacunarity) > _params.flowers.threshold)
						{
							map.voxels.Set(ix, h, iz, _materials.flower[random.Next(0, _materials.flower.Length - 1)]);
						}
						else if (_params.isGenTree && h < map.voxels.Bound.y - 8)
						{
							if (ix > 3 && ix < map.voxels.Bound.y - 3 && iz > 3 && iz < map.voxels.Bound.y - 3)
							{
								if (Noise.Simplex2(
									_params.tree.loopX * dx,
									_params.tree.loopZ * dz,
									_params.tree.octaves,
									_params.tree.persistence,
									_params.tree.lacunarity) > _params.tree.threshold)
								{
									this.BuildTree(map, ix, iz, h);
								}
							}
						}
					}
				}
			}

			return map;
		}

		public ChunkPrimer buildObsidian(CubizerBehaviour terrain, int x, int y, int z)
		{
			var size = terrain.profile.chunk.settings.chunkSize;
			var map = new ChunkPrimer(size, x, y, z, size * size * 8);

			for (byte ix = 0; ix < map.voxels.Bound.x; ix++)
			{
				for (byte iz = 0; iz < map.voxels.Bound.z; iz++)
				{
					for (byte iy = 0; iy < 8; iy++)
						map.voxels.Set(ix, iy, iz, _materials.obsidian);
				}
			}

			return map;
		}

		public ChunkPrimer OnCreateChunk(CubizerBehaviour terrain, int x, int y, int z)
		{
			switch (_params.layer)
			{
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