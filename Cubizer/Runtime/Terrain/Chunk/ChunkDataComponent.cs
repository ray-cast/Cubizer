using UnityEngine;

namespace Cubizer
{
	public class ChunkDataComponent : CubizerComponent<ChunkDataModels>
	{
		private GameObject _chunkObject;

		public int count
		{
			get { return _chunkObject != null ? _chunkObject.transform.childCount : 0; }
		}

		public override bool active
		{
			get { return true; }
		}

		public IChunkDataManager data
		{
			get { return model.settings.chunkManager; }
		}

		public override void OnEnable()
		{
			_chunkObject = new GameObject("TerrainChunks");
		}
	}
}