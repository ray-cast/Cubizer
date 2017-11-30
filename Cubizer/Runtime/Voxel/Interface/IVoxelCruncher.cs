namespace Cubizer
{
	public interface IVoxelCruncher
	{
		IVoxelModel CalcVoxelCruncher(VoxelData<VoxelMaterial> map);
	}
}