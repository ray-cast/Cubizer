namespace Cubizer
{
	public class BiomeDataNull : IBiomeData
	{
		public virtual ChunkPrimer OnBuildChunk(CubizerBehaviour terrain, short x, short y, short z)
		{
			return null;
		}
	}
}