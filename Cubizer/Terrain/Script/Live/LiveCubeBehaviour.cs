using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Cubizer
{
	[AddComponentMenu("Cubizer/LiveCubeBehaviour")]
	public class LiveCubeBehaviour : LiveBehaviour
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

		public override int GetVerticesCount(int faceCount)
		{
			return faceCount * 4;
		}

		public override int GetIndicesCount(int faceCount)
		{
			return faceCount * 6;
		}

		public override bool OnUpdateChunk(ref ChunkPrimer map, System.Byte x, System.Byte y, System.Byte z)
		{
			return false;
		}

		public override void OnCreateBlock(ref TerrainMesh mesh, ref int index, Vector3 translate, Vector3 scale, VoxelVisiableFaces faces)
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

					mesh.vertices[n] = v;
					mesh.normals[n] = _normals[i];
					mesh.uv[n] = _uvs[i, k];
				}

				for (int j = index * 6, k = 0; k < 6; k++, j++)
					mesh.triangles[j] = index * 4 + _indices[i, k];

				index++;
			}
		}
	}
}