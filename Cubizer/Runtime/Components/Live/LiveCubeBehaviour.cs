using UnityEngine;

using System.Threading.Tasks;

using Cubizer.Chunk;

namespace Cubizer
{
	[AddComponentMenu("Cubizer/LiveCubeBehaviour")]
	public class LiveCubeBehaviour : LiveBehaviour
	{
		private static readonly Vector3[,] _vertices = new Vector3[6, 4]
		{
			{ new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, +0.5f), new Vector3(-0.5f, +0.5f, -0.5f), new Vector3(-0.5f, +0.5f, +0.5f) },
			{ new Vector3(+0.5f, -0.5f, -0.5f), new Vector3(+0.5f, -0.5f, +0.5f), new Vector3(+0.5f, +0.5f, -0.5f), new Vector3(+0.5f, +0.5f, +0.5f) },
			{ new Vector3(-0.5f, +0.5f, -0.5f), new Vector3(-0.5f, +0.5f, +0.5f), new Vector3(+0.5f, +0.5f, -0.5f), new Vector3(+0.5f, +0.5f, +0.5f) },
			{ new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, +0.5f), new Vector3(+0.5f, -0.5f, -0.5f), new Vector3(+0.5f, -0.5f, +0.5f) },
			{ new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, +0.5f, -0.5f), new Vector3(+0.5f, -0.5f, -0.5f), new Vector3(+0.5f, +0.5f, -0.5f) },
			{ new Vector3(-0.5f, -0.5f, +0.5f), new Vector3(-0.5f, +0.5f, +0.5f), new Vector3(+0.5f, -0.5f, +0.5f), new Vector3(+0.5f, +0.5f, +0.5f) }
		};

		private static readonly Vector3[] _normals = new Vector3[6]
		{
			new Vector3(-1, 0, 0),
			new Vector3(+1, 0, 0),
			new Vector3(0, +1, 0),
			new Vector3(0, -1, 0),
			new Vector3(0, 0, -1),
			new Vector3(0, 0, +1)
		};

		private static readonly Vector2[,] _uvs = new Vector2[6, 4]
		{
			{ new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) },
			{ new Vector2(1, 0), new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 1) },
			{ new Vector2(0, 1), new Vector2(0, 0), new Vector2(1, 1), new Vector2(1, 0) },
			{ new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) },
			{ new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) },
			{ new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 0), new Vector2(0, 1) }
		};

		private static readonly int[,] _indices = new int[6, 6]
		{
			{ 0, 3, 2, 0, 1, 3 },
			{ 0, 3, 1, 0, 2, 3 },
			{ 0, 3, 2, 0, 1, 3 },
			{ 0, 3, 1, 0, 2, 3 },
			{ 0, 3, 2, 0, 1, 3 },
			{ 0, 3, 1, 0, 2, 3 }
		};

		[SerializeField]
		public Material msehMaterial;

		[SerializeField]
		public PhysicMaterial physicMaterial;

		private Task<LiveMesh> _task;

		public void OnApplicationQuit()
		{
			if (_task != null)
				_task.Wait();
		}

		public override void OnBuildChunk(ChunkDataContext context)
		{
			if (context.async)
			{
				_task = Task.Run(() => { LiveMesh mesh = null; doBuildMesh(context, out mesh); return mesh; });
				_task.GetAwaiter().OnCompleted(() =>
				{
					doBuildGameObject(context, _task.Result);
				});
			}
			else
			{
				LiveMesh mesh;
				doBuildMesh(context, out mesh);
				doBuildGameObject(context, mesh);
			}
		}

		private void doBuildMesh(ChunkDataContext context, out LiveMesh mesh)
		{
			mesh = new LiveMesh(context.faceCount * 4, context.faceCount * 6);

			var writeCount = 0;

			foreach (VoxelPrimitive it in context.model.GetEnumerator(this.material.GetInstanceID()))
			{
				Vector3 translate, scale;
				it.GetTranslateScale(out translate, out scale);

				for (int i = 0; i < 6; i++)
				{
					if (!it.faces[i])
						continue;

					for (int n = writeCount * 4, k = 0; k < 4; k++, n++)
					{
						Vector3 v = _vertices[i, k];
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

					for (int j = writeCount * 6, k = 0; k < 6; k++, j++)
						mesh.indices[j] = writeCount * 4 + _indices[i, k];

					writeCount++;
				}
			}
		}

		private void doBuildGameObject(ChunkDataContext context, LiveMesh data)
		{
			if (data.indices.Length > 0)
			{
				var actors = new GameObject(this.name);
				actors.isStatic = this.gameObject.isStatic;
				actors.tag = this.gameObject.tag;
				actors.layer = this.gameObject.layer;
				actors.transform.parent = context.parent.transform;
				actors.transform.position = context.parent.transform.position;

				if (msehMaterial != null)
				{
					var clone = actors.AddComponent<MeshRenderer>();
					clone.material = msehMaterial;
				}

				var mesh = data.mesh;
				actors.AddComponent<MeshFilter>().mesh = mesh;

				if (physicMaterial != null)
				{
					var meshCollider = actors.AddComponent<MeshCollider>();
					meshCollider.sharedMesh = mesh;
					meshCollider.material = physicMaterial;
				}
			}
		}
	}
}