using UnityEngine;
using System.Collections;

namespace Chunk
{
	public abstract class ChunkTerrainGenerator : MonoBehaviour
	{
		public abstract void OnCreateChunk(ChunkTree map);
	}
}