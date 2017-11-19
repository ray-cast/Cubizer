using System.Collections;

namespace Cubizer
{
	public interface IChunkDataManager
	{
		bool Load(string path);
		bool Save(string path);

		void Set(int x, int y, int z, ChunkPrimer value);
		bool Get(int x, int y, int z, out ChunkPrimer chunk);

		int Count();

		void GC();

		IEnumerable GetEnumerator();
	}
}