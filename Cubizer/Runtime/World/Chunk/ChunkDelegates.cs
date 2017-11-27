using UnityEngine;

namespace Cubizer
{
	public class ChunkDelegates
	{
		public delegate void OnSaveData(GameObject chunk);
		public delegate bool OnLoadData(int x, int y, int z, out ChunkPrimer chunk);
		public delegate bool OnBlockEvent(ChunkPrimer chunk, int x, int y, int z);
		public delegate bool OnCreatedChunk(ChunkPrimer chunk);

		public OnSaveData onSaveChunkData;
		public OnLoadData onLoadChunkData;
		public OnCreatedChunk onCreatedChunk;

		public OnBlockEvent onAddBlock;
		public OnBlockEvent onRemoveBlock;
	}
}