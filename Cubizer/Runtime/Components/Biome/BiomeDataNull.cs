using Cubizer.Chunk;

namespace Cubizer
{
	public class BiomeDataNull : IBiomeData
	{
		public virtual ChunkPrimer OnBuildChunk(CubizerBehaviour context, int x, int y, int z)
		{
			return null;
		}
	}
}