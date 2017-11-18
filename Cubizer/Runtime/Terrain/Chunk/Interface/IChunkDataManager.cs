using System.Collections;

namespace Cubizer
{
	interface IChunkDataManager
	{
		bool Load(string path);
		bool Save(string path);

		bool Set(int x, int y, int z, ChunkPrimer value);
		bool Get(int x, int y, int z, out ChunkPrimer chunk);

		int Count();

		void GC();

		IEnumerable GetEnumerator();
	}
}