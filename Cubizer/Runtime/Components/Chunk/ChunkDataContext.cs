namespace Cubizer
{
	public struct ChunkDataContext
	{
		public readonly bool async;

		public readonly int faceCount;

		public readonly IChunkData parent;
		public readonly IVoxelModel model;

		public ChunkDataContext(IChunkData _parent, IVoxelModel _model, int _count, bool _async)
		{
			parent = _parent;
			model = _model;
			faceCount = _count;
			async = _async;
		}
	}
}