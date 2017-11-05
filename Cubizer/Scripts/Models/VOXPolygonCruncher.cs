using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace Cubizer
{
	namespace Model
	{
		public class VOXPolygonCruncher
		{
			public static Vector3[,] _positions = new Vector3[6, 4]
			{
				{ new Vector3(-1, -1, -1), new Vector3(-1, -1, +1), new Vector3(-1, +1, -1), new Vector3(-1, +1, +1) },
				{ new Vector3(+1, -1, -1), new Vector3(+1, -1, +1), new Vector3(+1, +1, -1), new Vector3(+1, +1, +1) },
				{ new Vector3(-1, +1, -1), new Vector3(-1, +1, +1), new Vector3(+1, +1, -1), new Vector3(+1, +1, +1) },
				{ new Vector3(-1, -1, -1), new Vector3(-1, -1, +1), new Vector3(+1, -1, -1), new Vector3(+1, -1, +1) },
				{ new Vector3(-1, -1, -1), new Vector3(-1, +1, -1), new Vector3(+1, -1, -1), new Vector3(+1, +1, -1) },
				{ new Vector3(-1, -1, +1), new Vector3(-1, +1, +1), new Vector3(+1, -1, +1), new Vector3(+1, +1, +1) }
			};

			public static Vector3[] _normals = new Vector3[6]
			{
				new Vector3(-1, 0, 0),
				new Vector3(+1, 0, 0),
				new Vector3(0, +1, 0),
				new Vector3(0, -1, 0),
				new Vector3(0, 0, -1),
				new Vector3(0, 0, +1)
			};

			public static Vector2[,] _uvs = new Vector2[6, 4]
			{
				{ new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) },
				{ new Vector2(1, 0), new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 1) },
				{ new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 1), new Vector2(1, 0) },
				{ new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) },
				{ new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) },
				{ new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 0), new Vector2(0, 1) }
			};

			public static int[,] _indices = new int[6, 6]
			{
				{ 0, 3, 2, 0, 1, 3 },
				{ 0, 3, 1, 0, 2, 3 },
				{ 0, 3, 2, 0, 1, 3 },
				{ 0, 3, 1, 0, 2, 3 },
				{ 0, 3, 2, 0, 1, 3 },
				{ 0, 3, 1, 0, 2, 3 }
			};

			public static float[,] _flipped = new float[6, 6]
			{
				{ 0, 1, 2, 1, 3, 2 },
				{ 0, 2, 1, 2, 3, 1 },
				{ 0, 1, 2, 1, 3, 2 },
				{ 0, 2, 1, 2, 3, 1 },
				{ 0, 1, 2, 1, 3, 2 },
				{ 0, 2, 1, 2, 3, 1 }
			};

			public class VoxelCruncher
			{
				public byte begin_x;
				public byte begin_y;
				public byte begin_z;

				public byte end_x;
				public byte end_y;
				public byte end_z;

				public int material;

				public VisiableFaces faces;

				public VoxelCruncher(byte begin_x, byte end_x, byte begin_y, byte end_y, byte begin_z, byte end_z, int _material)
				{
					this.begin_x = begin_x;
					this.begin_y = begin_y;
					this.begin_z = begin_z;
					this.end_x = end_x;
					this.end_y = end_y;
					this.end_z = end_z;

					material = _material;

					faces.left = true;
					faces.right = true;
					faces.top = true;
					faces.bottom = true;
					faces.front = true;
					faces.back = true;
				}

				public VoxelCruncher(byte begin_x, byte end_x, byte begin_y, byte end_y, byte begin_z, byte end_z, VisiableFaces _faces, int _material)
				{
					this.begin_x = begin_x;
					this.begin_y = begin_y;
					this.begin_z = begin_z;

					this.end_x = end_x;
					this.end_y = end_y;
					this.end_z = end_z;

					material = _material;
					faces = _faces;
				}
			}

			public static List<VoxelCruncher> CalcVoxelCruncher(ChunkMapByte3<ChunkEntity> map)
			{
				var listX = new List<VoxelCruncher>[map.bound.z, map.bound.y];

				for (int z = 0; z < map.bound.z; z++)
				{
					for (int y = 0; y < map.bound.y; y++)
					{
						for (int x = 0; x < map.bound.x; x++)
						{
							ChunkEntity entity = null;
							ChunkEntity entityLast = null;

							if (!map.Get((byte)x, (byte)y, (byte)z, ref entity))
								continue;

							if (entity == null)
								continue;

							entityLast = entity;

							int x_end = x;

							for (int xlast = x + 1; xlast < map.bound.x; xlast++)
							{
								if (!map.Get((byte)xlast, (byte)y, (byte)z, ref entity))
									break;

								if (entity.material != entityLast.material)
									break;

								x_end = xlast;
								entityLast = entity;
							}

							if (listX[z, y] == null)
								listX[z, y] = new List<VoxelCruncher>();

							listX[z, y].Add(new VoxelCruncher((byte)x, (byte)(x_end), (byte)y, (byte)(y), (byte)z, (byte)(z), entityLast.material));

							x = x_end;
						}
					}
				}

				for (int z = 0; z < map.bound.z; z++)
				{
					for (int y = 0; y < map.bound.y - 1; y++)
					{
						if (listX[z, y] == null || listX[z, y + 1] == null)
							continue;

						foreach (var cur in listX[z, y])
						{
							for (int h = y + 1; h < map.bound.y; h++)
							{
								if (listX[z, h] == null)
									break;

								foreach (var it in listX[z, h])
								{
									if (it.begin_x == cur.begin_x && it.end_x == cur.end_x)
									{
										if (it.material == cur.material)
										{
											listX[z, h].Remove(it);
											cur.end_y++;
											break;
										}
										else
										{
											h = ushort.MaxValue;
											break;
										}
									}

									if (it.begin_x > cur.begin_x)
									{
										h = ushort.MaxValue;
										break;
									}
								}
							}
						}
					}
				}

				for (int y = 0; y < map.bound.y; y++)
				{
					for (int z = 0; z < map.bound.z - 1; z++)
					{
						if (listX[z, y] == null || listX[z + 1, y] == null)
							continue;

						foreach (var cur in listX[z, y])
						{
							for (int d = z + 1; d < map.bound.z; d++)
							{
								if (listX[d, y] == null)
									break;

								foreach (var it in listX[d, y])
								{
									if (it.begin_x == cur.begin_x && it.end_x == cur.end_x && it.begin_y == cur.begin_y && it.end_y == cur.end_y)
									{
										if (it.material == cur.material)
										{
											listX[d, y].Remove(it);
											cur.end_z++;
											break;
										}
										else
										{
											d = ushort.MaxValue;
											break;
										}
									}

									if (it.begin_x > cur.begin_x || it.end_x > cur.end_x ||
										it.begin_y > cur.begin_y || it.end_y > cur.end_y ||
										it.begin_z > cur.begin_z || it.end_z > cur.end_z)
									{
										d = ushort.MaxValue;
										break;
									}
								}
							}
						}
					}
				}

				List<VoxelCruncher> list = new List<VoxelCruncher>();
				for (byte z = 0; z < map.bound.z; z++)
				{
					for (byte y = 0; y < map.bound.y; y++)
					{
						if (listX[z, y] == null)
							continue;

						foreach (var cur in listX[z, y])
							list.Add(cur);
					}
				}

				foreach (var it in list)
				{
					if (it.begin_x == it.end_x &&
						it.begin_y == it.end_y &&
						it.begin_z == it.end_z)
					{
						var x = it.begin_x;
						var y = it.begin_y;
						var z = it.begin_z;

						ChunkEntity[] instanceID = new ChunkEntity[6] { null, null, null, null, null, null };

						if (x >= 1) map.Get((byte)(x - 1), y, z, ref instanceID[0]);
						if (y >= 1) map.Get(x, (byte)(y - 1), z, ref instanceID[2]);
						if (z >= 1) map.Get(x, y, (byte)(z - 1), ref instanceID[4]);
						if (x <= (map.bound.x - 1)) map.Get((byte)(x + 1), y, z, ref instanceID[1]);
						if (y <= (map.bound.y - 1)) map.Get(x, (byte)(y + 1), z, ref instanceID[3]);
						if (z <= (map.bound.z - 1)) map.Get(x, y, (byte)(z + 1), ref instanceID[5]);

						bool f1 = (instanceID[0] == null) ? true : false;
						bool f2 = (instanceID[1] == null) ? true : false;
						bool f3 = (instanceID[2] == null) ? true : false;
						bool f4 = (instanceID[3] == null) ? true : false;
						bool f5 = (instanceID[4] == null) ? true : false;
						bool f6 = (instanceID[5] == null) ? true : false;

						it.faces.left = f1;
						it.faces.right = f2;
						it.faces.bottom = f3;
						it.faces.top = f4;
						it.faces.front = f5;
						it.faces.back = f6;
					}
					else if (it.begin_y == it.end_y && it.begin_z == it.end_z)
					{
						var x = it.begin_x;
						var y = it.begin_y;
						var z = it.begin_z;

						ChunkEntity[] instanceID = new ChunkEntity[2] { null, null };

						if (it.begin_x >= 1) map.Get((byte)(it.begin_x - 1), y, z, ref instanceID[0]);
						if (it.end_x <= (map.bound.x - 1)) map.Get((byte)(it.end_x + 1), y, z, ref instanceID[1]);

						it.faces.left = (instanceID[0] == null) ? true : false;
						it.faces.right = (instanceID[1] == null) ? true : false;
					}
					else if (it.begin_x == it.end_x && it.begin_z == it.end_z)
					{
						var x = it.begin_x;
						var y = it.begin_y;
						var z = it.begin_z;

						ChunkEntity[] instanceID = new ChunkEntity[2] { null, null };

						if (it.begin_y >= 1) map.Get(x, (byte)(it.begin_y - 1), z, ref instanceID[0]);
						if (it.end_y <= (map.bound.y - 1)) map.Get(x, (byte)(it.end_y + 1), z, ref instanceID[1]);

						it.faces.bottom = (instanceID[0] == null) ? true : false;
						it.faces.top = (instanceID[1] == null) ? true : false;
					}
					else if (it.begin_x == it.end_x && it.begin_y == it.end_y)
					{
						var x = it.begin_x;
						var y = it.begin_y;
						var z = it.begin_z;

						ChunkEntity[] instanceID = new ChunkEntity[2] { null, null };

						if (it.begin_z >= 1) map.Get(x, y, (byte)(it.begin_z - 1), ref instanceID[0]);
						if (it.end_z <= (map.bound.z - 1)) map.Get(x, y, (byte)(it.end_z + 1), ref instanceID[1]);

						it.faces.front = (instanceID[0] == null) ? true : false;
						it.faces.back = (instanceID[1] == null) ? true : false;
					}
				}

				return list;
			}

			public static List<VoxelCruncher> CalcVoxelCruncher(VoxFileChunkChild chunk)
			{
				var map = new ChunkMapByte3<ChunkEntity>(new Cubizer.Math.Vector3<int>(chunk.size.x, chunk.size.y, chunk.size.z), chunk.xyzi.voxelNums);

				for (int j = 0; j < chunk.xyzi.voxelNums * 4; j += 4)
				{
					var x = chunk.xyzi.voxels[j];
					var y = chunk.xyzi.voxels[j + 1];
					var z = chunk.xyzi.voxels[j + 2];
					var c = chunk.xyzi.voxels[j + 3];

					map.Set(x, y, z, new ChunkEntity("voxel", c));
				}

				return CalcVoxelCruncher(map);
			}
		}
	}
}