using UnityEngine;

namespace Cubizer
{
	public class TerrainDelegates
	{
		public delegate void OnSaveData(GameObject chunk);

		public delegate bool OnLoadData(Vector3Int position, out ChunkData chunk);
	}
}