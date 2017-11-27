namespace Cubizer
{
	public class ChunkThreadData
	{
		public int x;
		public int y;
		public int z;

		public IPlayerListener player;
		public IBiomeData biome;
		public ChunkPrimer chunk;

		public ChunkThreadData(IPlayerListener playerData, IBiomeData biomeData, int xx, int yy, int zz)
		{
			x = xx;
			y = yy;
			z = zz;

			player = playerData;
			biome = biomeData;
		}
	}
}