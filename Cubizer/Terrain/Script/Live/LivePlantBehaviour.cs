using UnityEngine;

namespace Cubizer
{
	[RequireComponent(typeof(MeshRenderer))]
	[AddComponentMenu("Cubizer/LivePlantBehaviour")]
	public class LivePlantBehaviour : LiveBehaviour
	{
		private MeshRenderer _renderer;

		public static Vector3[,] _positions = new Vector3[4, 4]
		{
			{ new Vector3( 0.0f, -0.5f, -0.5f), new Vector3( 0.0f, -0.5f, +0.5f), new Vector3( 0.0f, +0.5f, -0.5f), new Vector3( 0.0f, +0.5f, +0.5f)},
			{ new Vector3( 0.0f, -0.5f, -0.5f), new Vector3( 0.0f, -0.5f, +0.5f), new Vector3( 0.0f, +0.5f, -0.5f), new Vector3( 0.0f, +0.5f, +0.5f)},
			{ new Vector3(-0.5f, -0.5f,  0.0f), new Vector3(-0.5f, +0.5f,  0.0f), new Vector3(+0.5f, -0.5f,  0.0f), new Vector3(+0.5f, +0.5f,  0.0f)},
			{ new Vector3(-0.5f, -0.5f,  0.0f), new Vector3(-0.5f, +0.5f,  0.0f), new Vector3(+0.5f, -0.5f,  0.0f), new Vector3(+0.5f, +0.5f,  0.0f)}
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

		public void Start()
		{
			_renderer = GetComponent<MeshRenderer>();
		}

		public int GetVerticesCount(int faceCount)
		{
			return (faceCount / 6) * 16;
		}

		public int GetIndicesCount(int faceCount)
		{
			return (faceCount / 6) * 24;
		}

		public static void CreatePlantMesh(ref Vector3[] vertices, ref Vector3[] normals, ref Vector2[] uv, ref int[] triangles, ref int index, Vector3 translate, Vector3 scale)
		{
			for (int i = 0; i < 4; i++)
			{
				for (int n = index * 4, k = 0; k < 4; k++, n++)
				{
					Vector3 v = _positions[i, k];
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

		public override void OnBuildChunkObject(GameObject parent, IVoxelModel model, int faceCount)
		{
			var writeCount = 0;
			var data = new TerrainMesh(GetVerticesCount(faceCount), GetIndicesCount(faceCount));

			foreach (VoxelPrimitive it in model.GetEnumerator(this.material.GetInstanceID()))
			{
				Vector3 pos, scale;
				it.GetTranslateScale(out pos, out scale);
				OnBuildBlock(ref data, ref writeCount, pos, scale, it.faces);
			}

			if (data.triangles.Length > 0)
			{
				Mesh mesh = new Mesh();
				mesh.vertices = data.vertices;
				mesh.normals = data.normals;
				mesh.uv = data.uv;
				mesh.triangles = data.triangles;

				var actors = new GameObject(this.name);
				actors.isStatic = parent.isStatic;
				actors.layer = parent.layer;
				actors.transform.parent = parent.transform;
				actors.transform.position = parent.transform.position;

				OnBuildComponents(actors, mesh);
			}
		}

		public void OnBuildBlock(ref TerrainMesh mesh, ref int index, Vector3 pos, Vector3 scale, VoxelVisiableFaces faces)
		{
			CreatePlantMesh(ref mesh.vertices, ref mesh.normals, ref mesh.uv, ref mesh.triangles, ref index, pos, scale);
		}

		public void OnBuildComponents(GameObject gameObject, Mesh mesh)
		{
			gameObject.AddComponent<MeshFilter>().mesh = mesh;

			if (_renderer != null)
			{
				var clone = gameObject.AddComponent<MeshRenderer>();
				clone.material = _renderer.material;
				clone.receiveShadows = _renderer.receiveShadows;
				clone.shadowCastingMode = _renderer.shadowCastingMode;

				_renderer = clone;

				var lod = this.GetComponent<LODGroup>();
				if (lod != null)
				{
					var lods = lod.GetLODs();
					for (int i = 0; i < lods.Length; i++)
					{
						if (lods[i].renderers.Length > 0)
							lods[i].renderers[0] = _renderer;
					}

					gameObject.AddComponent<LODGroup>().SetLODs(lods);
				}
			}
		}
	}
}