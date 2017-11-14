using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Cubizer
{
	[SelectionBase]
	[DisallowMultipleComponent]
	[AddComponentMenu("Cubizer/TerrainData")]
	public class TerrainData : MonoBehaviour
	{
		private ChunkPrimer _chunk;

		private float _repeatRateUpdate = 2.0f;
		private Dictionary<LiveBehaviour, List<Math.Vector3<System.Byte>>> _chunkEntitiesDynamic;

		public ChunkPrimer chunk
		{
			set
			{
				if (_chunk != value)
				{
					if (_chunk != null)
						_chunk.onChunkChange -= UpdateChunk;

					_chunk = value;

					if (_chunk != null)
						_chunk.onChunkChange += UpdateChunk;
				}
			}
			get
			{
				return _chunk;
			}
		}

		public void UpdateChunk()
		{
			for (int i = 0; i < transform.childCount; i++)
				Destroy(transform.GetChild(i).gameObject);

			if (_chunk == null || _chunk.voxels.count == 0)
				return;

			if (_chunkEntitiesDynamic != null)
			{
				if (_chunkEntitiesDynamic.Count > 0)
				{
					StopCoroutine("OnUpdateEntities");
					_chunkEntitiesDynamic.Clear();
				}
			}

			var cruncher = VoxelCruncher.CalcVoxelCruncher(_chunk.voxels, VoxelCruncherMode.Culled);
			if (cruncher == null)
				return;

			var entities = new Dictionary<string, int>();
			if (cruncher.CalcFaceCountAsAllocate(ref entities) == 0)
				return;

			foreach (var entity in entities)
			{
				var controller = LiveResources.Load(entity.Key);
				if (controller == null)
					continue;

				var numVertices = controller.GetVerticesCount(entity.Value);
				if (numVertices == 0)
					continue;

				var numIndices = controller.GetIndicesCount(entity.Value);
				if (numIndices == 0)
					continue;

				var data = new TerrainMesh { vertices = new Vector3[numVertices], normals = new Vector3[numVertices], uv = new Vector2[numVertices], triangles = new int[numIndices] };

				var writeCount = 0;
				foreach (var it in cruncher.voxels)
				{
					if (it.material.name != entity.Key)
						continue;

					Vector3 pos, scale;
					it.GetTranslateScale(out pos, out scale);
					controller.OnCreateBlock(ref data, ref writeCount, pos, scale, it.faces);
				}

				if (data.vertices.Length >= 65000)
				{
					Debug.LogError("Mesh vertices is too large: chunk is " + chunk.position.x + " " + chunk.position.y + " " + chunk.position.z);
					continue;
				}

				if (data.triangles.Length > 0)
				{
					Mesh mesh = new Mesh();
					mesh.vertices = data.vertices;
					mesh.normals = data.normals;
					mesh.uv = data.uv;
					mesh.triangles = data.triangles;

					var gameObject = new GameObject(entity.Key);
					gameObject.isStatic = controller.gameObject.isStatic;
					gameObject.layer = controller.gameObject.layer;
					gameObject.transform.parent = this.transform;
					gameObject.transform.position = this.transform.position;

					var renderer = controller.GetComponent<MeshRenderer>();
					if (renderer != null)
					{
						gameObject.AddComponent<MeshFilter>().mesh = mesh;
						var clone = gameObject.AddComponent<MeshRenderer>();
						clone.material = renderer.material;
						clone.receiveShadows = renderer.receiveShadows;
						clone.shadowCastingMode = renderer.shadowCastingMode;
						renderer = clone;
					}

					var collider = controller.GetComponent<Collider>();
					if (collider != null)
					{
						var type = collider.GetType();
						if (type == typeof(MeshCollider))
							gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
					}

					var lod = controller.GetComponent<LODGroup>();
					if (lod != null)
					{
						var lods = lod.GetLODs();
						for (int i = 0; i < lods.Length; i++)
						{
							if (lods[i].renderers.Length > 0)
								lods[i].renderers[0] = renderer;
						}

						gameObject.AddComponent<LODGroup>().SetLODs(lods);
					}

					if (_chunkEntitiesDynamic.Count > 0)
						StartCoroutine("OnUpdateEntities");
				}
			}
		}

		public IEnumerator OnUpdateEntities()
		{
			yield return new WaitForSeconds(_repeatRateUpdate);

			if (_chunkEntitiesDynamic != null)
			{
				bool needUpdate = false;
				foreach (var it in _chunkEntitiesDynamic)
				{
					foreach (var pos in it.Value)
						needUpdate |= it.Key.OnUpdateChunk(ref _chunk, pos.x, pos.y, pos.z);
				}

				if (needUpdate)
					this.UpdateChunk();
				else
					StartCoroutine("OnUpdateEntities");
			}
		}

		public void Awake()
		{
			_chunkEntitiesDynamic = new Dictionary<LiveBehaviour, List<Math.Vector3<System.Byte>>>();
		}

		public void Start()
		{
			Debug.Assert(transform.parent != null);

			if (_chunk != null)
			{
				var terrain = transform.parent.GetComponent<Terrain>();
				if (terrain != null)
					terrain.chunks.Set(chunk.position, chunk);

				if (_chunk.voxels.count > 0)
					this.UpdateChunk();
			}
		}

		public void OnDestroy()
		{
			if (transform.parent != null)
			{
				var terrain = transform.parent.GetComponent<Terrain>();
				if (terrain != null)
					terrain.chunks.Set(chunk.position, null);
			}
		}

		public void OnDrawGizmos()
		{
			if (chunk == null)
				return;

			if (chunk.position.y == 0)
			{
				if (chunk.voxels == null)
					return;

				var bound = chunk.voxels.bound;

				Vector3 pos = transform.position;
				pos.x += (bound.x - 1) * 0.5f;
				pos.y += (bound.y - 1) * 0.5f;
				pos.z += (bound.z - 1) * 0.5f;

				Gizmos.color = Color.black;
				Gizmos.DrawWireCube(pos, new Vector3(bound.x, bound.y, bound.z));
			}
		}
	}
}