namespace Cubizer
{
	public interface IChunkGenerator
	{
		ChunkPrimer OnCreateChunk(Terrain terrain, short x, short y, short z);
	}
}