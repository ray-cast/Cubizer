using UnityEngine;

namespace Cubizer
{
	[SelectionBase]
	[DisallowMultipleComponent]
	public abstract class IChunkData : MonoBehaviour
	{
		public abstract bool dirty
		{
			get; set;
		}

		public abstract ChunkPrimer chunk
		{
			get; set;
		}

		public abstract CubizerContext context
		{
			get;
		}

		public abstract void OnBuildChunk();
	}
}