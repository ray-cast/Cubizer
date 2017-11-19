namespace Cubizer
{
	public class BiomeData : IBiomeData
	{
		public IChunkGenerator _chunkGenerator;

		public BiomeData(IChunkGenerator generator)
		{
			_chunkGenerator = generator;
		}

		public ChunkPrimer OnBuildChunk(CubizerBehaviour terrain, short x, short y, short z)
		{
			return _chunkGenerator != null ? _chunkGenerator.OnCreateChunk(terrain, x, y, z) : null;
		}
	}
}