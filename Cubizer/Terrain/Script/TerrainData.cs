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
						_chunk.onChunkChange -= OnUpdateChunk;

					_chunk = value;

					if (_chunk != null)
						_chunk.onChunkChange += OnUpdateChunk;
				}
			}
			get
			{
				return _chunk;
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
					terrain.chunks.Set(chunk.position.x, chunk.position.y, chunk.position.z, chunk);

				if (_chunk.voxels.count > 0)
					this.OnUpdateChunk();
			}
		}

		public void OnDestroy()
		{
			if (transform.parent != null)
			{
				var terrain = transform.parent.GetComponent<Terrain>();
				if (terrain != null)
					terrain.chunks.Set(chunk.position.x, chunk.position.y, chunk.position.z, null);
			}
		}

		public void OnDrawGizmos()
		{
			if (chunk == null)
				return;

			if (chunk.voxels == null || chunk.voxels.count == 0)
				return;

			var bound = chunk.voxels.bound;

			Vector3 pos = transform.position;
			pos.x += (bound.x - 1) * 0.5f;
			pos.y += (bound.y - 1) * 0.5f;
			pos.z += (bound.z - 1) * 0.5f;

			Gizmos.color = Color.black;
			Gizmos.DrawWireCube(pos, new Vector3(bound.x, bound.y, bound.z));
		}

		public void OnUpdateChunk()
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

			var model = _chunk.CreateVoxelModel(VoxelCullMode.Culled);
			if (model == null)
				return;

			var entities = new Dictionary<string, int>();
			if (model.CalcFaceCountAsAllocate(ref entities) == 0)
				return;

			foreach (var entity in entities)
			{
				var controller = VoxelMaterialManager.GetInstance().GetMaterial(entity.Key).userdata as LiveBehaviour;
				if (controller == null)
					continue;

				var numVertices = controller.GetVerticesCount(entity.Value);
				var numIndices = controller.GetIndicesCount(entity.Value);
				if (numVertices == 0 || numIndices == 0)
					continue;

				var data = new TerrainMesh();
				data.vertices = new Vector3[numVertices];
				data.normals = new Vector3[numVertices];
				data.uv = new Vector2[numVertices];
				data.triangles = new int[numIndices];

				var writeCount = 0;
				foreach (VoxelPrimitive it in model.GetEnumerator())
				{	
					if (it.material.name != entity.Key)
						continue;

					Vector3 pos, scale;
					it.GetTranslateScale(out pos, out scale);
					controller.OnBuildBlock(ref data, ref writeCount, pos, scale, it.faces);
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

					controller.OnBuildComponents(gameObject, mesh);
				}
			}

			if (_chunkEntitiesDynamic.Count > 0)
				StartCoroutine("OnUpdateEntities");
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
					this.OnUpdateChunk();

				StartCoroutine("OnUpdateEntities");
			}
		}
	}
}