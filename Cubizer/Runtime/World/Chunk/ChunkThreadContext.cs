namespace Cubizer
{
	public class ChunkThreadData
	{
		public readonly int x;
		public readonly int y;
		public readonly int z;

		public readonly IPlayerListener player;
		public readonly IBiomeData biome;
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