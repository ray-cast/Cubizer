using UnityEngine;

namespace Cubizer.Chunk
{
	[SelectionBase]
	[DisallowMultipleComponent]
	public abstract class IChunkData : MonoBehaviour
	{
		public abstract bool dirty
		{
			get; internal set;
		}

		public abstract ChunkPrimer chunk
		{
			get; set;
		}
	}
}