namespace Cubizer
{
	public class ChunkGeneratorNull : IChunkGenerator
	{
		public ChunkPrimer OnCreateChunk(Terrain terrain, short x, short y, short z)
		{
			return null;
		}
	}
}