using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

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

		private readonly Queue<LiveMesh> _queue = new Queue<LiveMesh>();

		public override void OnBuildChunk(ChunkDataContext context)
		{
			if (context.async)
				StartCoroutine("BuildChunkWithCoroutine", context);
			else
			{
				var data = BuildBlocks(context);
				BuildGameObject(context, data);
			}
		}

		private LiveMesh BuildBlocks(ChunkDataContext context)
		{
			var writeCount = 0;
			var mesh = new LiveMesh(context.faceCount * 4, context.faceCount * 6);

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

			return mesh;
		}

		private void BuildGameObject(ChunkDataContext context, LiveMesh data)
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

		public IEnumerator BuildChunkWithCoroutine(ChunkDataContext context)
		{
			Func<Task> task = async () =>
			{
				await Task.Run(() =>
				{
					var data = BuildBlocks(context);

					lock (_queue)
					{
						_queue.Enqueue(data);
					}
				});
			};

			task();

			yield return new WaitWhile(() => !(_queue.Count > 0));

			lock (_queue)
			{
				BuildGameObject(context, _queue.Dequeue());
			}
		}
	}
}