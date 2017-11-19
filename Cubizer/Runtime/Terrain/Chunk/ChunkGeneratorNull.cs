namespace Cubizer
{
	public class ChunkGeneratorNull : IChunkGenerator
	{
		public ChunkPrimer OnCreateChunk(CubizerBehaviour terrain, short x, short y, short z)
		{
			return null;
		}
	}
}