using Cubizer.Chunk;

namespace Cubizer.Biome
{
	public class BiomeDataNull : IBiomeData
	{
		public virtual ChunkPrimer OnBuildChunk(CubizerBehaviour context, int x, int y, int z)
		{
			return null;
		}
	}
}