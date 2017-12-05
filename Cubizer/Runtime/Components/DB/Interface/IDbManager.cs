using System;

namespace Cubizer
{
	public interface IDbManager : IDisposable
	{
		void LoadChunk(ChunkPrimer chunk, int x, int y, int z);

		void InsertBlock(int x, int y, int z, int xx, int yy, int zz, int ww);
	}
}