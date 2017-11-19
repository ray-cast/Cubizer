using UnityEngine;

namespace Cubizer
{
	public class TerrainDelegates
	{
		public delegate void OnSaveData(GameObject chunk);

		public delegate bool OnLoadData(int x, int y, int z, out ChunkPrimer chunk);

		public OnSaveData onSaveChunkData;
		public OnLoadData onLoadChunkData;
	}
}