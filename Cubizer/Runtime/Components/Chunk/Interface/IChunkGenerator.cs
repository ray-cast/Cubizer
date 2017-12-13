namespace Cubizer.Chunk
{
	public interface IChunkGenerator
	{
		ChunkPrimer OnCreateChunk(CubizerBehaviour terrain, int x, int y, int z);
	}
}