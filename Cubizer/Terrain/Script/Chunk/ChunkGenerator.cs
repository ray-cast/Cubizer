namespace Cubizer
{
	public abstract class ChunkGenerator
	{
		public abstract ChunkPrimer OnCreateChunk(Terrain terrain, short x, short y, short z);
	}
}