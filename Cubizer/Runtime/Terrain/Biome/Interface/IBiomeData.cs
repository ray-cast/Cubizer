namespace Cubizer
{
	public interface IBiomeData
	{
		ChunkPrimer OnBuildChunk(CubizerBehaviour terrain, short x, short y, short z);
	}
}