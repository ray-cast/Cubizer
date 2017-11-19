using UnityEngine;

namespace Cubizer
{
	[SelectionBase]
	[DisallowMultipleComponent]
	public abstract class IChunkData : MonoBehaviour
	{
		public abstract ChunkPrimer chunk { set; get; }

		public abstract void OnUpdateChunk();
	}
}