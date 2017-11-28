namespace Cubizer
{
	public struct ChunkDataContext
	{
		public bool async;

		public int faceCount;

		public IChunkData parent;
		public IVoxelModel model;
	}
}