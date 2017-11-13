namespace Cubizer
{
	public class BiomeDataNull : BiomeData
	{
		public void Awake()
		{
			this.chunkGenerator = new ChunkGeneratorNull();
		}
	}
}