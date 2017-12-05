using UnityEngine;

namespace Cubizer
{
	[SelectionBase]
	[DisallowMultipleComponent]
	public abstract class IChunkData : MonoBehaviour
	{
		public abstract bool Dirty
		{
			get; internal set;
		}

		public abstract ChunkPrimer Chunk
		{
			get; set;
		}
	}
}