namespace Cubizer
{
	public class BiomeDataNull : IBiomeData
	{
		public virtual ChunkPrimer OnBuildChunk(Terrain terrain, short x, short y, short z)
		{
			return null;
		}
	}
}