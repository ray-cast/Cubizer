using UnityEngine;

namespace Cubizer
{
	[SelectionBase]
	[DisallowMultipleComponent]
	public abstract class IChunkData : MonoBehaviour
	{
		public abstract ChunkPrimer chunk
		{
			get; set;
		}

		public abstract IChunkDataManager chunkManager
		{
			get; internal set;
		}

		public abstract void OnBuildChunk();
	}
}