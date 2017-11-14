namespace Cubizer
{
	public class BiomeData
	{
		private ChunkGenerator _chunkGenerator;

		public ChunkGenerator chunkGenerator
		{
			get { return _chunkGenerator; }
		}

		public BiomeData(ChunkGenerator generator)
		{
			_chunkGenerator = generator;
		}

		public virtual ChunkPrimer OnBuildChunk(Terrain terrain, short x, short y, short z)
		{
			return _chunkGenerator != null ? _chunkGenerator.OnCreateChunk(terrain, x, y, z) : null;
		}
	}
}