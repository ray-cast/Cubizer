namespace Cubizer
{
	public class BiomeDataNull : IBiomeData
	{
		public virtual ChunkPrimer OnBuildChunk(CubizerBehaviour terrain, int x, int y, int z)
		{
			return null;
		}
	}
}