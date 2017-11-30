namespace Cubizer
{
	public interface IBiomeData
	{
		ChunkPrimer OnBuildChunk(CubizerBehaviour terrain, int x, int y, int z);
	}
}