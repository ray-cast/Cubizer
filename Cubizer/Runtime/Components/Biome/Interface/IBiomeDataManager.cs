namespace Cubizer
{
	public interface IBiomeDataManager
	{
		bool Load(string path);
		bool Save(string path);

		bool Set(int x, int y, int z, IBiomeData value);
		bool Get(int x, int y, int z, out IBiomeData chunk);

		int Count();

		void GC();
	}
}