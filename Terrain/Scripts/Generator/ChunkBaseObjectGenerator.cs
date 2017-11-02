using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace Chunk
{
	using ChunkPos = System.Byte;
	using ChunkTranslate = ChunkPosition<System.Byte>;

	[Serializable]
	public class ChunkGrass : ChunkEntity
	{
		public ChunkGrass()
			: base("Grass")
		{
		}
	}

	[Serializable]
	public class ChunkItemTree : ChunkEntity
	{
		public ChunkItemTree()
			: base("Tree")
		{
		}
	}

	[Serializable]
	public class ChunkTreeLeaf : ChunkEntity
	{
		public ChunkTreeLeaf()
			: base("TreeLeaf")
		{
		}
	}

	[Serializable]
	public class ChunkFlower : ChunkEntity
	{
		public ChunkFlower()
			: base("Flower", true, false, true)
		{
		}

		public override void OnCreateBlock(ref ChunkMesh mesh, ref int index, VisiableFaces faces, ChunkTranslate position, float scale)
		{
			CreatePlantMesh(ref mesh, ref index, faces, position, scale);
		}
	}

	[Serializable]
	public class ChunkWeed : ChunkEntity
	{
		public ChunkWeed()
			: base("Weed", true, false, true)
		{
		}

		public override void OnCreateBlock(ref ChunkMesh mesh, ref int index, VisiableFaces faces, ChunkTranslate position, float scale)
		{
			CreatePlantMesh(ref mesh, ref index, faces, position, scale);
		}
	}

	[Serializable]
	public class ChunkGroundFog : ChunkEntity
	{
		public ChunkGroundFog()
			: base("GroundFog", true)
		{
		}
	}

	[Serializable]
	public class ChunkObsidian : ChunkEntity
	{
		public ChunkObsidian()
			: base("Obsidian")
		{
		}
	}

	[Serializable]
	public class ChunkCloud : ChunkEntity
	{
		public ChunkCloud()
			: base("Cloud", false, false)
		{
		}
	}

	[Serializable]
	public class ChunkWater : ChunkEntity
	{
		public ChunkWater()
			: base("WaterHigh", true, true)
		{
		}

		public override bool Update(ref ChunkTree map, ChunkTranslate translate)
		{
			if (translate.y > 1)
			{
				var translateNow = new ChunkPosition<System.Byte>(translate.x, (byte)(translate.y - 1), translate.z);
				if (map.Set(translateNow, this, false))
				{
					map.Set(translate, null);
					return true;
				}
			}

			return false;
		}
	}

	[Serializable]
	public class ChunkBaseObjectGenerator : ChunkTerrainGenerator
	{
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

		private int _chunkSize = ChunkSetting.CHUNK_SIZE;

		private ChunkEntity _entityGrass;
		private ChunkEntity _entityTree;
		private ChunkEntity _entityTreeLeaf;
		private ChunkEntity _entityFlower;
		private ChunkEntity _entityWeed;
		private ChunkEntity _entityObsidian;
		private ChunkEntity _entityWater;
		private ChunkEntity _entityCloud;

		public void Start()
		{
			_entityGrass = new ChunkGrass();
			_entityTree = new ChunkItemTree();
			_entityTreeLeaf = new ChunkTreeLeaf();
			_entityFlower = new ChunkFlower();
			_entityWeed = new ChunkWeed();
			_entityObsidian = new ChunkObsidian();
			_entityWater = new ChunkWater();
			_entityCloud = new ChunkCloud();
		}

		public override void OnCreateChunk(ChunkTree map)
		{
			var pos = map.position;

			int offsetX = pos.x * _chunkSize;
			int offsetY = pos.y * _chunkSize;
			int offsetZ = pos.z * _chunkSize;

			float layer = pos.y;

			if (_isGenPlaneOnly)
			{
				if (layer == _layerGrass)
				{
					byte h = (byte)_floorBase;
					for (byte x = 0; x < _chunkSize; x++)
					{
						for (byte z = 0; z < _chunkSize; z++)
						{
							for (byte y = 0; y < h; y++)
								map.Set(x, y, z, _entityGrass);
						}
					}
				}

				return;
			}

			if (layer == _layerGrass)
			{
				for (byte x = 0; x < _chunkSize; x++)
				{
					for (byte z = 0; z < _chunkSize; z++)
					{
						int dx = offsetX + x;
						int dz = offsetZ + z;

						float f = ChunkUtility.simplex2(dx * 0.01f, dz * 0.01f, 4, 0.5f, 2);

						byte h = (byte)(f * (f * _floorHeightLismit + _floorBase));

						if (_isGenGrass)
						{
							for (byte y = 0; y < h; y++)
								map.Set(x, y, z, _entityGrass);
						}

						if (_isGenWater && h <= _floorBase - _floorHeightLismit * 0.2f)
						{
							for (byte y = h; y <= _floorBase - _floorHeightLismit * 0.2f; y++)
								map.Set(x, y, z, _entityWater);
						}
						else
						{
							if (_isGenWeed && ChunkUtility.simplex2(-dx * 0.1f, dz * 0.1f, 4, 0.8f, 2) > 0.7)
								map.Set(x, h, z, _entityWeed);
							else if (_isGenFlower && ChunkUtility.simplex2(dx * 0.05f, -dz * 0.05f, 4, 0.8f, 2) > 0.75)
								map.Set(x, h, z, _entityFlower);
							else if (_isGenTree && h < _chunkSize - 8)
							{
								if (x > 3 && x < _chunkSize - 3 && z > 3 && z < _chunkSize - 3)
								{
									if (ChunkUtility.simplex2(dx, dz, 6, 0.5f, 2) > 0.84)
									{
										for (int y = h + 3; y < h + 8; y++)
										{
											for (int ox = -3; ox <= 3; ox++)
											{
												for (int oz = -3; oz <= 3; oz++)
												{
													int d = (ox * ox) + (oz * oz) + (y - (h + 4)) * (y - (h + 4));
													if (d < 11)
														map.Set((byte)(x + ox), (byte)y, (byte)(z + oz), _entityTreeLeaf);
												}
											}
										}

										for (byte y = h; y < h + 7; y++)
											map.Set(x, y, z, _entityTree);
									}
								}
							}
						}
					}
				}
			}

			if (_isGenCloud && layer == _layerCloud)
			{
				for (int x = 0; x < _chunkSize; x++)
				{
					for (int z = 0; z < _chunkSize; z++)
					{
						int dx = offsetX + x;
						int dz = offsetZ + z;

						for (int y = 0; y < 8; y++)
						{
							int dy = offsetY + y;

							if (ChunkUtility.simplex3(dx * 0.01f, dy * 0.1f, dz * 0.01f, 8, 0.5f, 2) > 0.75)
								map.Set((byte)x, (byte)y, (byte)z, _entityCloud);
						}
					}
				}
			}

			if (_isGenObsidian && layer == _layerObsidian)
			{
				for (byte x = 0; x < _chunkSize; x++)
				{
					for (byte z = 0; z < _chunkSize; z++)
					{
						for (byte y = 0; y < 8; y++)
							map.Set(x, y, z, _entityObsidian);
					}
				}
			}
		}
	}
}