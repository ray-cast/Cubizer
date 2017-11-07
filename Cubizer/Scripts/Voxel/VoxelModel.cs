using UnityEngine;

namespace Cubizer
{
	public class VoxelModel<VoxelMaterial> where VoxelMaterial : class
	{
		private static Vector3[,] _positions = new Vector3[6, 4]
		{
				{ new Vector3(-1, -1, -1), new Vector3(-1, -1, +1), new Vector3(-1, +1, -1), new Vector3(-1, +1, +1) },
				{ new Vector3(+1, -1, -1), new Vector3(+1, -1, +1), new Vector3(+1, +1, -1), new Vector3(+1, +1, +1) },
				{ new Vector3(-1, +1, -1), new Vector3(-1, +1, +1), new Vector3(+1, +1, -1), new Vector3(+1, +1, +1) },
				{ new Vector3(-1, -1, -1), new Vector3(-1, -1, +1), new Vector3(+1, -1, -1), new Vector3(+1, -1, +1) },
				{ new Vector3(-1, -1, -1), new Vector3(-1, +1, -1), new Vector3(+1, -1, -1), new Vector3(+1, +1, -1) },
				{ new Vector3(-1, -1, +1), new Vector3(-1, +1, +1), new Vector3(+1, -1, +1), new Vector3(+1, +1, +1) }
		};

		private static Vector3[] _normals = new Vector3[6]
		{
				new Vector3(-1, 0, 0),
				new Vector3(+1, 0, 0),
				new Vector3(0, +1, 0),
				new Vector3(0, -1, 0),
				new Vector3(0, 0, -1),
				new Vector3(0, 0, +1)
		};

		private static Vector2[,] _uvs = new Vector2[6, 4]
		{
				{ new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) },
				{ new Vector2(1, 0), new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 1) },
				{ new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 1), new Vector2(1, 0) },
				{ new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) },
				{ new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) },
				{ new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 0), new Vector2(0, 1) }
		};

		private static int[,] _indices = new int[6, 6]
		{
				{ 0, 3, 2, 0, 1, 3 },
				{ 0, 3, 1, 0, 2, 3 },
				{ 0, 3, 2, 0, 1, 3 },
				{ 0, 3, 1, 0, 2, 3 },
				{ 0, 3, 2, 0, 1, 3 },
				{ 0, 3, 1, 0, 2, 3 }
		};

		public VoxelCruncher<VoxelMaterial>[] voxels;

		public VoxelModel(VoxelCruncher<VoxelMaterial>[] array)
		{
			voxels = array;
		}

		public static void CreateCubeMesh(ref Vector3[] vertices, ref Vector3[] normals, ref Vector2[] uv, ref int[] triangles, ref int index, VoxelVisiableFaces faces, Vector3 translate, Vector3 scale)
		{
			bool[] visiable = new bool[] { faces.left, faces.right, faces.top, faces.bottom, faces.front, faces.back };

			for (int i = 0; i < 6; i++)
			{
				if (!visiable[i])
					continue;

				for (int n = index * 4, k = 0; k < 4; k++, n++)
				{
					Vector3 v = _positions[i, k] * 0.5f;
					v.x *= scale.x;
					v.y *= scale.y;
					v.z *= scale.z;
					v.x += translate.x;
					v.y += translate.y;
					v.z += translate.z;

					vertices[n] = v;
					normals[n] = _normals[i];
					uv[n] = _uvs[i, k];
				}

				for (int j = index * 6, k = 0; k < 6; k++, j++)
					triangles[j] = index * 4 + _indices[i, k];

				index++;
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

			public static void CreatePlantMesh(ref Vector3[] vertices, ref Vector3[] normals, ref Vector2[] uv, ref int[] triangles, ref int index, Vector3 translate, Vector3 scale)
			{
				for (int i = 0; i < 4; i++)
				{
					for (int n = index * 4, k = 0; k < 4; k++, n++)
					{
						Vector3 v = _positions[i, k] * 0.5f;
						v.x *= scale.x;
						v.y *= scale.y;
						v.z *= scale.z;
						v.x += translate.x;
						v.y += translate.y;
						v.z += translate.z;

						vertices[n] = v;
						normals[n] = _normals[i];
						uv[n] = _uvs[i, k];
					}

					for (int j = index * 6, k = 0; k < 6; k++, j++)
						triangles[j] = index * 4 + _indices[i, k];

					index++;
				}
			}
		}
	}
}