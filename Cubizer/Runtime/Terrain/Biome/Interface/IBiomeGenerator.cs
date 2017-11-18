namespace Cubizer
{
	public interface IBiomeGenerator
	{
		IBiomeData OnBuildBiome(short x, short y, short z);
	}
}