using UnityEngine;

namespace Cubizer
{
	[RequireComponent(typeof(MeshRenderer))]
	[AddComponentMenu("Cubizer/LiveCubeTileBehaviour")]
	public class LiveCubeTileBehaviour : LiveBehaviour
	{
		public int tileSize = 1;
		public int tilePadding = 2048;

		public int[] tiles = new int[] { 0, 0, 0, 0, 0, 0 };

		public bool collide = true;

		private MeshRenderer _renderer;

		private static Vector3[,] _positions = new Vector3[6, 4]
		{
			{ new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, +0.5f), new Vector3(-0.5f, +0.5f, -0.5f), new Vector3(-0.5f, +0.5f, +0.5f) },
			{ new Vector3(+0.5f, -0.5f, -0.5f), new Vector3(+0.5f, -0.5f, +0.5f), new Vector3(+0.5f, +0.5f, -0.5f), new Vector3(+0.5f, +0.5f, +0.5f) },
			{ new Vector3(-0.5f, +0.5f, -0.5f), new Vector3(-0.5f, +0.5f, +0.5f), new Vector3(+0.5f, +0.5f, -0.5f), new Vector3(+0.5f, +0.5f, +0.5f) },
			{ new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, +0.5f), new Vector3(+0.5f, -0.5f, -0.5f), new Vector3(+0.5f, -0.5f, +0.5f) },
			{ new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, +0.5f, -0.5f), new Vector3(+0.5f, -0.5f, -0.5f), new Vector3(+0.5f, +0.5f, -0.5f) },
			{ new Vector3(-0.5f, -0.5f, +0.5f), new Vector3(-0.5f, +0.5f, +0.5f), new Vector3(+0.5f, -0.5f, +0.5f), new Vector3(+0.5f, +0.5f, +0.5f) }
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

		private static Vector2Int[,] _uvs = new Vector2Int[6, 4]
		{
			{ new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },
			{ new Vector2Int(1, 0), new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(0, 1) },
			{ new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(1, 0) },
			{ new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, 1) },
			{ new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, 1) },
			{ new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 0), new Vector2Int(0, 1) }
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

		public void Start()
		{
			_renderer = GetComponent<MeshRenderer>();
		}

		public override bool OnUpdateChunk(ref ChunkPrimer map, System.Byte x, System.Byte y, System.Byte z)
		{
			return false;
		}

		public override void OnBuildBlock(ref TerrainMesh mesh, ref int index, Vector3 translate, Vector3 scale, VoxelVisiableFaces faces)
		{
			bool[] visiable = new bool[] { faces.left, faces.right, faces.top, faces.bottom, faces.front, faces.back };

			float s = tilePadding > 0 ? 1.0f / tilePadding : 0;

			float a = s;
			float b = 1.0f / tileSize - a;

			for (int i = 0; i < 6; i++)
			{
				if (!visiable[i])
					continue;

				float du = (tiles[i] % tileSize) / (float)tileSize;
				float dv = (tiles[i] / (float)tileSize) / tileSize;

				for (int n = index * 4, k = 0; k < 4; k++, n++)
				{
					Vector3 v = _positions[i, k];
					v.x *= scale.x;
					v.y *= scale.y;
					v.z *= scale.z;
					v.x += translate.x;
					v.y += translate.y;
					v.z += translate.z;

					mesh.vertices[n] = v;
					mesh.normals[n] = _normals[i];
					mesh.uv[n].x = du + (_uvs[i, k].x > 0 ? b : a);
					mesh.uv[n].y = dv + (_uvs[i, k].y > 0 ? b : a);
				}

				for (int j = index * 6, k = 0; k < 6; k++, j++)
					mesh.triangles[j] = index * 4 + _indices[i, k];

				index++;
			}
		}

		public override void OnBuildComponents(GameObject gameObject, Mesh mesh)
		{
			if (_renderer != null)
			{
				var clone = gameObject.AddComponent<MeshRenderer>();
				clone.material = _renderer.material;
				clone.receiveShadows = _renderer.receiveShadows;
				clone.shadowCastingMode = _renderer.shadowCastingMode;
			}

			gameObject.AddComponent<MeshFilter>().mesh = mesh;

			if (collide)
				gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
		}
	}
}