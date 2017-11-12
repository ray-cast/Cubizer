using Cubizer;
using Cubizer.Math;

using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Cubizer/BasicObjectsGenerator")]
	public class BasicObjectsGenerator : ChunkGenerator
	{
		public LiveBehaviour _materialGrass;
		public LiveBehaviour _materialTree;
		public LiveBehaviour _materialTreeLeaf;
		public LiveBehaviour _materialFlower;
		public LiveBehaviour _materialWeed;
		public LiveBehaviour _materialObsidian;
		public LiveBehaviour _materialWater;
		public LiveBehaviour _materialCloud;

		public bool _isGenTree = true;
		public bool _isGenWater = true;
		public bool _isGenCloud = true;
		public bool _isGenFlower = true;
		public bool _isGenWeed = true;
		public bool _isGenGrass = true;
		public bool _isGenObsidian = true;
		public bool _isGenPlaneOnly = false;

		public int _floorBase = 10;
		public int _floorHeightLismit = 20;

		public int _layerGrass = 0;
		public int _layerCloud = 3;
		public int _layerObsidian = -10;

		private VoxelMaterial _entityGrass;
		private VoxelMaterial _entityTree;
		private VoxelMaterial _entityTreeLeaf;
		private VoxelMaterial _entityFlower;
		private VoxelMaterial _entityWeed;
		private VoxelMaterial _entityObsidian;
		private VoxelMaterial _entityWater;
		private VoxelMaterial _entityCloud;

		public void Start()
		{
			if (_materialGrass != null)
				_entityGrass = _materialGrass.material;
			if (_materialTree != null)
				_entityTree = _materialTree.material;
			if (_materialTreeLeaf != null)
				_entityTreeLeaf = _materialTreeLeaf.material;
			if (_materialFlower != null)
				_entityFlower = _materialFlower.material;
			if (_materialWeed != null)
				_entityWeed = _materialWeed.material;
			if (_materialObsidian != null)
				_entityObsidian = _materialObsidian.material;
			if (_materialWater != null)
				_entityWater = _materialWater.material;
			if (_materialCloud != null)
				_entityCloud = _materialCloud.material;

			if (_isGenTree && _entityTree == null)
				UnityEngine.Debug.LogError("Please drag a Tree into Hierarchy View.");
			if (_isGenTree && _entityTreeLeaf == null)
				UnityEngine.Debug.LogError("Please drag a TreeLeaf into Hierarchy View.");
			if ((_isGenGrass || _isGenPlaneOnly) && _entityGrass == null)
				UnityEngine.Debug.LogError("Please drag a Grass into Hierarchy View.");
			if (_isGenFlower && _entityFlower == null)
				UnityEngine.Debug.LogError("Please drag a Flower into Hierarchy View.");
			if (_isGenWeed && _entityWeed == null)
				UnityEngine.Debug.LogError("Please drag a Weed into Hierarchy View.");
			if (_isGenObsidian && _entityObsidian == null)
				UnityEngine.Debug.LogError("Please drag a Obsidian into Hierarchy View.");
			if (_isGenWater && _entityWater == null)
				UnityEngine.Debug.LogError("Please drag a Water into Hierarchy View.");
			if (_isGenCloud && _entityCloud == null)
				UnityEngine.Debug.LogError("Please drag a Cloud into Hierarchy View.");
		}

		public override void OnCreateChunk(ChunkData map)
		{
			var pos = map.position;

			int offsetX = pos.x * map.voxels.bound.x;
			int offsetY = pos.y * map.voxels.bound.y;
			int offsetZ = pos.z * map.voxels.bound.z;

			float layer = pos.y;

			if (_isGenPlaneOnly)
			{
				if (layer == _layerGrass)
				{
					byte h = (byte)_floorBase;
					for (byte x = 0; x < map.voxels.bound.x; x++)
					{
						for (byte z = 0; z < map.voxels.bound.z; z++)
						{
							for (byte y = 0; y < h; y++)
								map.voxels.Set(x, y, z, _entityGrass);
						}
					}
				}

				return;
			}

			if (layer == _layerGrass)
			{
				for (byte x = 0; x < map.voxels.bound.x; x++)
				{
					for (byte z = 0; z < map.voxels.bound.z; z++)
					{
						int dx = offsetX + x;
						int dz = offsetZ + z;

						float f = Noise.simplex2(dx * 0.01f, dz * 0.01f, 4, 0.4f, 2);

						byte h = (byte)(f * (f * _floorHeightLismit + _floorBase));

						if (_isGenGrass)
						{
							for (byte y = 0; y < h; y++)
								map.voxels.Set(x, y, z, _entityGrass);
						}

						if (_isGenWater && h <= _floorBase - _floorHeightLismit * 0.2f)
						{
							for (byte y = h; y <= _floorBase - _floorHeightLismit * 0.2f; y++)
								map.voxels.Set(x, y, z, _entityWater);
						}
						else
						{
							if (_isGenWeed && Noise.simplex2(-dx * 0.1f, dz * 0.1f, 4, 0.8f, 2) > 0.7)
								map.voxels.Set(x, h, z, _entityWeed);
							else if (_isGenFlower && Noise.simplex2(dx * 0.05f, -dz * 0.05f, 4, 0.8f, 2) > 0.75)
								map.voxels.Set(x, h, z, _entityFlower);
							else if (_isGenTree && h < map.voxels.bound.y - 8)
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
														map.voxels.Set((byte)(x + ox), (byte)y, (byte)(z + oz), _entityTreeLeaf);
												}
											}
										}

										for (byte y = h; y < h + 7; y++)
											map.voxels.Set(x, y, z, _entityTree);
									}
								}
							}
						}
					}
				}
			}

			if (_isGenCloud && layer == _layerCloud)
			{
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
								map.voxels.Set((byte)x, (byte)y, (byte)z, _entityCloud);
						}
					}
				}
			}

			if (_isGenObsidian && layer == _layerObsidian)
			{
				for (byte x = 0; x < map.voxels.bound.x; x++)
				{
					for (byte z = 0; z < map.voxels.bound.z; z++)
					{
						for (byte y = 0; y < 8; y++)
							map.voxels.Set(x, y, z, _entityObsidian);
					}
				}
			}
		}
	}
}