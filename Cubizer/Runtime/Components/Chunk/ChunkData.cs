using System.Collections.Generic;

using UnityEngine;

namespace Cubizer.Chunk
{
	public class ChunkData : IChunkData
	{
		private bool _dirty;

		private ChunkPrimer _chunk;

		public override bool dirty
		{
			get
			{
				return _dirty;
			}
			internal set
			{
				_dirty = value;
			}
		}

		public override ChunkPrimer chunk
		{
			get
			{
				return _chunk;
			}
			set
			{
				Debug.Assert(_chunk != value);

				if (_chunk != null)
					_chunk.OnChunkChange -= OnBuildChunk;

				_chunk = value;

				if (_chunk != null)
					_chunk.OnChunkChange += OnBuildChunk;
			}
		}

		public void OnDrawGizmos()
		{
			if (chunk == null)
				return;

			if (chunk.voxels != null || chunk.voxels.Count > 0)
			{
				var bound = chunk.voxels.Bound;

				Vector3 pos = transform.position;
				pos.x += (bound.x - 1) * 0.5f;
				pos.y += (bound.y - 1) * 0.5f;
				pos.z += (bound.z - 1) * 0.5f;

				Gizmos.color = Color.black;
				Gizmos.DrawWireCube(pos, new Vector3(bound.x, bound.y, bound.z));
			}
		}

		private void doBuildChunk(bool is_async)
		{
			for (int i = 0; i < transform.childCount; i++)
				Destroy(transform.GetChild(i).gameObject);

			if (_chunk == null || _chunk.voxels.Count == 0)
				return;

			var model = _chunk.CreateVoxelModel(VoxelCullMode.Culled);
			if (model != null)
			{
				var entities = new Dictionary<int, int>();
				if (model.CalcFaceCountAsAllocate(ref entities) == 0)
					return;

				foreach (var it in entities)
				{
					var material = VoxelMaterialManager.GetInstance().GetMaterial(it.Key);
					if (material == null)
						continue;

					var controller = material.Userdata as ILiveBehaviour;
					if (controller == null)
						continue;

					var context = new ChunkDataContext(this, model, it.Value, is_async);
					controller.OnBuildChunk(context);
				}
			}
		}

		public void OnBuildChunk()
		{
			doBuildChunk(false);
		}

		public void OnBuildChunkAsync()
		{
			doBuildChunk(true);
		}

		public void Init(ChunkPrimer chunk)
		{
			Debug.Assert(_chunk == null);

			_chunk = chunk;
			_chunk.dirty = _dirty = false;
			_chunk.OnChunkChange += OnBuildChunk;

			this.OnBuildChunkAsync();
		}
	}
}