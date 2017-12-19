using Cubizer.Chunk;

namespace Cubizer
{
	public class CubizerDelegates
	{
		public delegate void OnDestroyData(ChunkPrimer chunk);
		public delegate void OnBlockEvent(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel);

		public delegate void OnLoadDataBefore(int x, int y, int z, ref ChunkPrimer chunk);
		public delegate void OnLoadDataAfter(ChunkPrimer chunk);

		public delegate void OnConnectionPlayer(IPlayer player);
		public delegate void OnDisconnectPlayer(IPlayer player);

		public delegate void OnServerEvent();

		public OnDestroyData OnDestroyChunk;

		public OnLoadDataBefore OnLoadChunkBefore;
		public OnLoadDataAfter OnLoadChunkAfter;

		public OnBlockEvent OnAddBlockBefore;
		public OnBlockEvent OnAddBlockAfter;

		public OnBlockEvent OnRemoveBlockBefore;
		public OnBlockEvent OnRemoveBlockAfter;

		public OnServerEvent OnOpenServer;
		public OnServerEvent OnCloseServer;
	}
}