using Cubizer.Chunk;

namespace Cubizer.Biome
{
	public sealed class BiomeDataNull : IBiomeData
	{
		public ChunkPrimer OnBuildChunk(CubizerBehaviour context, int x, int y, int z)
		{
			return null;
		}
	}
}