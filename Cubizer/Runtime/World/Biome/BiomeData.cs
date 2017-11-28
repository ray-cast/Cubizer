namespace Cubizer
{
	public class BiomeData : IBiomeData
	{
		public readonly IChunkGenerator _chunkGenerator;

		public BiomeData(IChunkGenerator generator)
		{
			_chunkGenerator = generator;
		}

		public ChunkPrimer OnBuildChunk(CubizerBehaviour terrain, int x, int y, int z)
		{
			return _chunkGenerator != null ? _chunkGenerator.OnCreateChunk(terrain, x, y, z) : null;
		}
	}
}