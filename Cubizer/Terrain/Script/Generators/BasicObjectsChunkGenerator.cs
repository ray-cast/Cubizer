using Cubizer;
using Cubizer.Math;

namespace Cubizer
{
	public class BasicObjectsChunkGenerator : ChunkGenerator
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

		public void BuildPlaneOnly(ChunkPrimer map)
		{
			byte h = (byte)_params.floorBase;
			for (byte x = 0; x < map.voxels.bound.x; x++)
			{
				for (byte z = 0; z < map.voxels.bound.z; z++)
				{
					for (byte y = 0; y < h; y++)
						map.voxels.Set(x, y, z, _materials.grass);
				}
			}
		}

		public void BuildGrassland(ChunkPrimer map)
		{
			var pos = map.position;

			int offsetX = pos.x * map.voxels.bound.x;
			int offsetY = pos.y * map.voxels.bound.y;
			int offsetZ = pos.z * map.voxels.bound.z;

			for (byte x = 0; x < map.voxels.bound.x; x++)
			{
				for (byte z = 0; z < map.voxels.bound.z; z++)
				{
					int dx = offsetX + x;
					int dz = offsetZ + z;

					float f = Noise.simplex2(dx * 0.01f, dz * 0.01f, 4, 0.4f, 2);

					byte h = (byte)(f * (f * _params.floorHeightLismit + _params.floorBase));

					if (_params.isGenGrass)
					{
						for (byte y = 0; y < h; y++)
							map.voxels.Set(x, y, z, _materials.grass);
					}

					var waterHeight = _params.floorBase - _params.floorHeightLismit * 0.2f;
					if (_params.isGenWater && h < waterHeight)
					{
						for (byte y = h; y <= _params.floorBase - _params.floorHeightLismit * 0.2f; y++)
							map.voxels.Set(x, y, z, _materials.water);
					}
					else
					{
						if (h == waterHeight)
						{
							if (f > 0.34 && f < 0.365)
								map.voxels.Set(x, (byte)(h - 1), z, _materials.sand);
						}

						if (_params.isGenWeed && Noise.simplex2(-dx * 0.1f, dz * 0.1f, 4, 0.8f, 2) > 0.7)
							map.voxels.Set(x, h, z, _materials.weed);
						else if (_params.isGenFlower && Noise.simplex2(dx * 0.05f, -dz * 0.05f, 4, 0.8f, 2) > 0.75)
							map.voxels.Set(x, h, z, _materials.flower);
						else if (_params.isGenTree && h < map.voxels.bound.y - 8)
						{
							if (x > 3 && x < map.voxels.bound.y - 3 && z > 3 && z < map.voxels.bound.y - 3)
							{
								if (Noise.simplex2(dx, dz, 6, 0.5f, 2) > 0.84)
								{
									for (int y = h + 3; y < h + 8; y++)
									{
										for (int ox = -3; ox <= 3; ox++)
										{
											for (int oz = -3; oz <= 3; oz++)
											{
												int d = (ox * ox) + (oz * oz) + (y - (h + 4)) * (y - (h + 4));
												if (d < 11)
													map.voxels.Set((byte)(x + ox), (byte)y, (byte)(z + oz), _materials.treeLeaf);
											}
										}
									}

									for (byte y = h; y < h + 7; y++)
										map.voxels.Set(x, y, z, _materials.tree);
								}
							}
						}
					}
				}
			}
		}

		public void BuildClouds(ChunkPrimer map)
		{
			var pos = map.position;

			int offsetX = pos.x * map.voxels.bound.x;
			int offsetY = pos.y * map.voxels.bound.y;
			int offsetZ = pos.z * map.voxels.bound.z;

			for (int x = 0; x < map.voxels.bound.x; x++)
			{
				for (int z = 0; z < map.voxels.bound.y; z++)
				{
					int dx = offsetX + x;

					int dz = offsetZ + z;

					for (int y = 0; y < 8; y++)
					{
						int dy = offsetY + y;

						if (Noise.simplex3(dx * 0.01f, dy * 0.1f, dz * 0.01f, 8, 0.5f, 2) > 0.75)
							map.voxels.Set((byte)x, (byte)y, (byte)z, _materials.cloud);
					}
				}
			}
		}

		public void buildObsidian(ChunkPrimer map)
		{
			for (byte x = 0; x < map.voxels.bound.x; x++)
			{
				for (byte z = 0; z < map.voxels.bound.z; z++)
				{
					for (byte y = 0; y < 8; y++)
						map.voxels.Set(x, y, z, _materials.obsidian);
				}
			}
		}

		public override void OnCreateChunk(ChunkPrimer map)
		{
			switch (_params.layer)
			{
				case BasicObjectBiomeType.Plane:
					this.BuildPlaneOnly(map);
					break;

				case BasicObjectBiomeType.Clound:
					this.BuildClouds(map);
					break;

				case BasicObjectBiomeType.Grassland:
					this.BuildGrassland(map);
					break;
			}
		}
	}
}