using UnityEngine;
using System.Collections;

namespace Cubizer
{
	public abstract class TerrainGenerator : MonoBehaviour
	{
		public abstract void OnCreateChunk(ChunkTree map);
	}
}