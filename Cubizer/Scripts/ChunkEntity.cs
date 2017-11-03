using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

using Cubizer.Math;

namespace Cubizer
{
	using ChunkVector3 = Vector3Int;
	using ChunkMap = ChunkTree;
	using ChunkPos = System.Byte;
	using ChunkPosition = Cubizer.Math.Vector3<System.Byte>;

	[Serializable]
	public class ChunkEntity : ICloneable
	{
		private bool _actor;
		private bool _dynamic;
		private bool _transparent;

		private string _name;
		private Material _material;

		public bool is_actor { get { return _actor; } }
		public bool is_dynamic { get { return _dynamic; } }
		public bool is_transparent { get { return _transparent; } }

		public string name { set { _name = value; } get { return _name; } }
		public Material material { set { _material = value; } get { return _material; } }

		public ChunkEntity()
		{
			_actor = false;
			_dynamic = false;
			_transparent = false;
		}

		public ChunkEntity(string name, Material material, bool transparent = false, bool dynamic = false, bool actor = false)
		{
			_name = name;
			_material = material;
			_actor = actor;
			_dynamic = dynamic;
			_transparent = transparent;
		}

		public struct Cube
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

			public static void CreateCubeMesh(ref ChunkMesh mesh, ref int index, VisiableFaces faces, ChunkPosition translate, float scale)
			{
				bool[] visiable = new bool[] { faces.left, faces.right, faces.top, faces.bottom, faces.front, faces.back };

				for (int i = 0; i < 6; i++)
				{
					if (!visiable[i])
						continue;

					for (int j = 0; j < 6; j++, index++)
					{
						int k = _indices[i, j];

						Vector3 v = _positions[i, k] * scale * 0.5f;
						v.x += translate.x;
						v.y += translate.y;
						v.z += translate.z;

						mesh.vertices[index] = v;
						mesh.normals[index] = _normals[i];
						mesh.uv[index] = _uvs[i, k];
						mesh.triangles[index] = index;
					}
				}
			}

			public static void CreateCubeMesh(ref ChunkMesh mesh, ref int index, VisiableFaces faces, ChunkPosition translate, uint palette, float scale)
			{
				bool[] visiable = new bool[] { faces.left, faces.right, faces.top, faces.bottom, faces.front, faces.back };

				float s = 1.0f / 16.0f;
				float a = 0 + 1.0f / 32.0f;
				float b = s - 1.0f / 32.0f;

				for (int i = 0; i < 6; i++)
				{
					if (!visiable[i])
						continue;

					for (int j = 0; j < 6; j++, index++)
					{
						int k = _indices[i, j];

						Vector3 v = _positions[i, k] * scale * 0.5f;
						v.x += translate.x;
						v.y += translate.y;
						v.z += translate.z;

						float du = (palette % 16) * s;
						float dv = (palette / 16) * s;

						Vector2 uv;
						uv.x = du + (_uvs[i, k].x > 0 ? b : a);
						uv.y = dv + (_uvs[i, k].y > 0 ? b : a);

						mesh.vertices[index] = v;
						mesh.normals[index] = _normals[i];
						mesh.uv[index] = uv;
						mesh.triangles[index] = index;
					}
				}
			}

			public static void CreateCubeMesh(ref ChunkMesh mesh, ref int index, VisiableFaces faces, ChunkPosition translate, Color32 color, float scale)
			{
				bool[] visiable = new bool[] { faces.left, faces.right, faces.top, faces.bottom, faces.front, faces.back };

				for (int i = 0; i < 6; i++)
				{
					if (!visiable[i])
						continue;

					for (int j = 0; j < 6; j++, index++)
					{
						int k = _indices[i, j];

						Vector3 v = _positions[i, k] * scale * 0.5f;
						v.x += translate.x;
						v.y += translate.y;
						v.z += translate.z;

						mesh.colors[index] = color;
						mesh.vertices[index] = v;
						mesh.normals[index] = _normals[i];
						mesh.uv[index] = _uvs[i, k];
						mesh.triangles[index] = index;
					}
				}
			}
		}

		public struct Plant
		{
			public static Vector3[,] _positions = new Vector3[4, 4]
			{
				{ new Vector3( 0, -1, -1), new Vector3( 0, -1, +1), new Vector3( 0, +1, -1), new Vector3( 0, +1, +1)},
				{ new Vector3( 0, -1, -1), new Vector3( 0, -1, +1), new Vector3( 0, +1, -1), new Vector3( 0, +1, +1)},
				{ new Vector3(-1, -1,  0), new Vector3(-1, +1,  0), new Vector3(+1, -1,  0), new Vector3(+1, +1,  0)},
				{ new Vector3(-1, -1,  0), new Vector3(-1, +1,  0), new Vector3(+1, -1,  0), new Vector3(+1, +1,  0)}
			};

			public static Vector3[] _normals = new Vector3[4]
			{
				new Vector3(-1, 0, 0),
				new Vector3(+1, 0, 0),
				new Vector3(0, 0, -1),
				new Vector3(0, 0, +1)
			};

			public static Vector2[,] _uvs = new Vector2[4, 4]
			{
				{ new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)},
				{ new Vector2(1, 0), new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 1)},
				{ new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1)},
				{ new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 0), new Vector2(0, 1)}
			};

			public static int[,] _indices = new int[4, 6]
			{
				{0, 3, 2, 0, 1, 3},
				{0, 3, 1, 0, 2, 3},
				{0, 3, 2, 0, 1, 3},
				{0, 3, 1, 0, 2, 3}
			};

			public static void CreatePlantMesh(ref ChunkMesh mesh, ref int index, VisiableFaces faces, ChunkPosition translate, float scale)
			{
				for (int i = 0; i < 4; i++)
				{
					for (int k = 0; k < 6; k++, index++)
					{
						int j = _indices[i, k];

						Vector3 v = _positions[i, j] * 0.5f * scale;
						v.x += translate.x;
						v.y += translate.y;
						v.z += translate.z;

						mesh.vertices[index] = v;
						mesh.normals[index] = _normals[i];
						mesh.uv[index] = _uvs[i, j];
						mesh.triangles[index] = index;
					}
				}
			}
		}

		public static void CreateCubeMesh(ref ChunkMesh mesh, ref int index, VisiableFaces faces, ChunkPosition translate, float scale = 1.0f)
		{
			Cube.CreateCubeMesh(ref mesh, ref index, faces, translate, scale);
		}

		public static void CreateCubeMesh(ref ChunkMesh mesh, ref int index, VisiableFaces faces, ChunkPosition translate, uint palette, float scale = 1.0f)
		{
			Cube.CreateCubeMesh(ref mesh, ref index, faces, translate, palette, scale);
		}

		public static void CreateCubeMesh(ref ChunkMesh mesh, ref int index, VisiableFaces faces, ChunkPosition translate, Color32 color, float scale = 1.0f)
		{
			Cube.CreateCubeMesh(ref mesh, ref index, faces, translate, color, scale);
		}

		public static void CreatePlantMesh(ref ChunkMesh mesh, ref int index, VisiableFaces faces, ChunkPosition translate, float scale = 1.0f)
		{
			Plant.CreatePlantMesh(ref mesh, ref index, faces, translate, scale);
		}

		public object Clone()
		{
			object obj = null;

			BinaryFormatter inputFormatter = new BinaryFormatter();
			MemoryStream inputStream;

			using (inputStream = new MemoryStream())
			{
				inputFormatter.Serialize(inputStream, this);
			}

			using (MemoryStream outputStream = new MemoryStream(inputStream.ToArray()))
			{
				BinaryFormatter outputFormatter = new BinaryFormatter();
				obj = outputFormatter.Deserialize(outputStream);
			}

			return obj;
		}

		public object Instance()
		{
			return this.MemberwiseClone();
		}

		public virtual void OnCreateBlock(ref ChunkMesh mesh, ref int index, VisiableFaces faces, ChunkPosition position, float scale = 1.0f)
		{
			CreateCubeMesh(ref mesh, ref index, faces, position, scale);
		}

		public virtual bool OnUpdateChunk(ref ChunkMap map, ChunkPosition translate)
		{
			return false;
		}
	}
}