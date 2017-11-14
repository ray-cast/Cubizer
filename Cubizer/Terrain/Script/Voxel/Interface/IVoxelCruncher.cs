namespace Cubizer
{
	public interface IVoxelCruncher
	{
		VoxelModel CalcVoxelCruncher(VoxelData<VoxelMaterial> map);
	}
}