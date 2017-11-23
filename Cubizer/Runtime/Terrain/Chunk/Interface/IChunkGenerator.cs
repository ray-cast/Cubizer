namespace Cubizer
{
	public interface IChunkGenerator
	{
		ChunkPrimer OnCreateChunk(CubizerBehaviour terrain, int x, int y, int z);
	}
}