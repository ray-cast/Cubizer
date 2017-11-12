using UnityEngine;

namespace Cubizer
{
	public abstract class ChunkGenerator : MonoBehaviour
	{
		public abstract void OnCreateChunk(ChunkData map);
	}
}