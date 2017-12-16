namespace Cubizer.Chunk
{
	public sealed class ChunkGeneratorNull : IChunkGenerator
	{
		public ChunkPrimer OnCreateChunk(CubizerBehaviour terrain, int x, int y, int z)
		{
			return null;
		}
	}
}