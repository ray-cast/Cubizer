using UnityEngine;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cubizer
{
	[AddComponentMenu("Cubizer/LiveCubeTileBehaviour")]
	public class LiveCubeTileBehaviour : LiveBehaviour
	{
		private readonly static Vector3[,] _vertices = new Vector3[6, 4]
		{
			{ new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, +0.5f), new Vector3(-0.5f, +0.5f, -0.5f), new Vector3(-0.5f, +0.5f, +0.5f) },
			{ new Vector3(+0.5f, -0.5f, -0.5f), new Vector3(+0.5f, -0.5f, +0.5f), new Vector3(+0.5f, +0.5f, -0.5f), new Vector3(+0.5f, +0.5f, +0.5f) },
			{ new Vector3(-0.5f, +0.5f, -0.5f), new Vector3(-0.5f, +0.5f, +0.5f), new Vector3(+0.5f, +0.5f, -0.5f), new Vector3(+0.5f, +0.5f, +0.5f) },
			{ new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, -0.5f, +0.5f), new Vector3(+0.5f, -0.5f, -0.5f), new Vector3(+0.5f, -0.5f, +0.5f) },
			{ new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(-0.5f, +0.5f, -0.5f), new Vector3(+0.5f, -0.5f, -0.5f), new Vector3(+0.5f, +0.5f, -0.5f) },
			{ new Vector3(-0.5f, -0.5f, +0.5f), new Vector3(-0.5f, +0.5f, +0.5f), new Vector3(+0.5f, -0.5f, +0.5f), new Vector3(+0.5f, +0.5f, +0.5f) }
		};

		private readonly static Vector3[] _normals = new Vector3[6]
		{
			new Vector3(-1, 0, 0),
			new Vector3(+1, 0, 0),
			new Vector3(0, +1, 0),
			new Vector3(0, -1, 0),
			new Vector3(0, 0, -1),
			new Vector3(0, 0, +1)
		};

		private readonly static Vector2Int[,] _uvs = new Vector2Int[6, 4]
		{
			{ new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) },
			{ new Vector2Int(1, 0), new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(0, 1) },
			{ new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int(1, 1), new Vector2Int(1, 0) },
			{ new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, 1) },
			{ new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 0), new Vector2Int(1, 1) },
			{ new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 0), new Vector2Int(0, 1) }
		};

		private readonly static int[,] _indices = new int[6, 6]
		{
			{ 0, 3, 2, 0, 1, 3 },
			{ 0, 3, 1, 0, 2, 3 },
			{ 0, 3, 2, 0, 1, 3 },
			{ 0, 3, 1, 0, 2, 3 },
			{ 0, 3, 2, 0, 1, 3 },
			{ 0, 3, 1, 0, 2, 3 }
		};

		[SerializeField] public int tileSize = 1;
		[SerializeField] public int tilePadding = 2048;
		[SerializeField] public int[] tiles = new int[] { 0, 0, 0, 0, 0, 0 };

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
				_task = Task.Run(() => { LiveMesh data; BuildBlocks(context, out data); return data; });
				_task.GetAwaiter().OnCompleted(() =>
				{
					BuildGameObject(context, _task.Result);
				});
			}
			else
			{
				LiveMesh data;
				BuildBlocks(context, out data);
				BuildGameObject(context, data);
			}
		}

		private void BuildBlocks(ChunkDataContext context, out LiveMesh mesh)
		{
			var writeCount = 0;
			mesh = new LiveMesh(context.faceCount * 4, context.faceCount * 6);

			foreach (VoxelPrimitive it in context.model.GetEnumerator(this.material.GetInstanceID()))
			{
				Vector3 translate, scale;
				it.GetTranslateScale(out translate, out scale);

				float s = tilePadding > 0 ? 1.0f / tilePadding : 0;

				float a = s;
				float b = 1.0f / tileSize - a;

				for (int i = 0; i < 6; i++)
				{
					if (!it.faces[i])
						continue;

					float du = (tiles[i] % tileSize) / (float)tileSize;
					float dv = (tiles[i] / (float)tileSize) / tileSize;

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
						mesh.uv[n].x = du + (_uvs[i, k].x > 0 ? b : a);
						mesh.uv[n].y = dv + (_uvs[i, k].y > 0 ? b : a);
					}

					for (int j = writeCount * 6, k = 0; k < 6; k++, j++)
						mesh.indices[j] = writeCount * 4 + _indices[i, k];

					writeCount++;
				}
			}
		}

		private void BuildGameObject(ChunkDataContext context, LiveMesh data)
		{
			if (data.indices.Length > 0)
			{
				var actors = new GameObject(this.name);
				actors.isStatic = this.gameObject.isStatic;
				actors.tag = gameObject.tag;
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

				if (physicMaterial)
				{
					var meshCollider = actors.AddComponent<MeshCollider>();
					meshCollider.sharedMesh = mesh;
					meshCollider.material = physicMaterial;
				}
			}
		}
	}
}