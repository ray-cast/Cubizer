namespace Cubizer
{
	public interface IBiomeData
	{
		ChunkPrimer OnBuildChunk(Terrain terrain, short x, short y, short z);
	}
}