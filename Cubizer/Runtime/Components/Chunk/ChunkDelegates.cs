namespace Cubizer.Chunk
{
	public delegate void OnDestroyData(ChunkPrimer chunk);
	public delegate void OnBlockEvent(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel);

	public delegate void OnLoadDataBefore(int x, int y, int z, ref ChunkPrimer chunk);
	public delegate void OnLoadDataAfter(ChunkPrimer chunk);

	public class ChunkDelegates
	{
		public OnDestroyData OnDestroyChunk;

		public OnLoadDataBefore OnLoadChunkBefore;
		public OnLoadDataAfter OnLoadChunkAfter;

		public OnBlockEvent OnAddBlockBefore;
		public OnBlockEvent OnAddBlockAfter;

		public OnBlockEvent OnRemoveBlockBefore;
		public OnBlockEvent OnRemoveBlockAfter;
	}
}