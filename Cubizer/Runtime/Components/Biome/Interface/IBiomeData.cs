using Cubizer.Chunk;

namespace Cubizer.Biome
{
	public interface IBiomeData
	{
		ChunkPrimer OnBuildChunk(CubizerBehaviour terrain, int x, int y, int z);
	}
}