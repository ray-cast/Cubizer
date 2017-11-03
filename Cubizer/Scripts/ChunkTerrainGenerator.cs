using UnityEngine;
using System.Collections;

namespace Cubizer
{
	public abstract class ChunkTerrainGenerator : MonoBehaviour
	{
		public abstract void OnCreateChunk(ChunkTree map);
	}
}