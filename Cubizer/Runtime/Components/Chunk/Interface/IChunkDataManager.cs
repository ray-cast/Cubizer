using System.Collections;

namespace Cubizer
{
	public interface IChunkDataManager
	{
		int Count { get; }

		bool Load(string path);
		bool Save(string path);

		void Set(int x, int y, int z, ChunkPrimer value);
		bool Get(int x, int y, int z, out ChunkPrimer chunk);

		void GC();

		IEnumerable GetEnumerator();
	}
}