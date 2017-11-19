namespace Cubizer
{
	public interface IChunkGenerator
	{
		ChunkPrimer OnCreateChunk(CubizerBehaviour terrain, short x, short y, short z);
	}
}