namespace Cubizer
{
	public class ChunkGeneratorNull : IChunkGenerator
	{
		public ChunkPrimer OnCreateChunk(CubizerBehaviour terrain, int x, int y, int z)
		{
			return null;
		}
	}
}