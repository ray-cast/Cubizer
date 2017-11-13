namespace Cubizer
{
	public class ChunkGeneratorNull : ChunkGenerator
	{
		public override ChunkPrimer OnCreateChunk(Terrain terrain, short x, short y, short z)
		{
			return null;
		}
	}
}