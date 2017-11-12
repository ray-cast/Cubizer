namespace Cubizer
{
	public interface IVoxelCruncherStrategy
	{
		VoxelModel CalcVoxelCruncher(VoxelData<VoxelMaterial> map);
	}
}