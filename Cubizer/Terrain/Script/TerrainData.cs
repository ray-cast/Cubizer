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
		private Dictionary<LiveBehaviour, List<Math.Vector3<byte>>> _chunkEntitiesDynamic;

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
			_chunkEntitiesDynamic = new Dictionary<LiveBehaviour, List<Math.Vector3<byte>>>();
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

			if (_chunkEntitiesDynamic != null && _chunkEntitiesDynamic.Count > 0)
			{
				StopCoroutine("OnUpdateEntities");
				_chunkEntitiesDynamic.Clear();
			}

			var model = _chunk.CreateVoxelModel(VoxelCullMode.Culled);
			if (model != null)
			{
				var entities = new Dictionary<int, int>();
				if (model.CalcFaceCountAsAllocate(ref entities) == 0)
					return;

				foreach (var it in entities)
				{
					var controller = VoxelMaterialManager.GetInstance().GetMaterial(it.Key).userdata as ILiveBehaviour;
					if (controller == null)
						continue;

					controller.OnBuildChunk(gameObject, model, it.Value);
				}

				if (_chunkEntitiesDynamic.Count > 0)
					StartCoroutine("OnUpdateEntities");
			}
		}

		public IEnumerator OnUpdateEntities()
		{
			yield return new WaitForSeconds(_repeatRateUpdate);

			if (_chunkEntitiesDynamic != null)
			{
				StartCoroutine("OnUpdateEntities");
			}
		}
	}
}