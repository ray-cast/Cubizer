using System.Collections.Generic;

using UnityEngine;

namespace Cubizer
{
	[AddComponentMenu("Cubizer/ChunkData")]
	public class ChunkData : IChunkData
	{
		private ChunkPrimer _chunk;

		public override ChunkPrimer chunk
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

		public void Start()
		{
			Debug.Assert(transform.parent != null);

			if (_chunk != null)
			{
				if (_chunk.voxels.count > 0)
					this.OnUpdateChunk();
			}
		}

		public void OnDrawGizmos()
		{
			if (chunk == null)
				return;

			if (chunk.voxels != null || chunk.voxels.count > 0)
			{
				var bound = chunk.voxels.bound;

				Vector3 pos = transform.position;
				pos.x += (bound.x - 1) * 0.5f;
				pos.y += (bound.y - 1) * 0.5f;
				pos.z += (bound.z - 1) * 0.5f;

				Gizmos.color = Color.black;
				Gizmos.DrawWireCube(pos, new Vector3(bound.x, bound.y, bound.z));
			}
		}

		public override void OnUpdateChunk()
		{
			for (int i = 0; i < transform.childCount; i++)
				Destroy(transform.GetChild(i).gameObject);

			if (_chunk == null || _chunk.voxels.count == 0)
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

					var controller = material.userdata as LiveBehaviourBase;
					if (controller == null)
						continue;

					controller.OnBuildChunk(gameObject, model, it.Value);
				}
			}
		}
	}
}