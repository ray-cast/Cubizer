namespace Cubizer
{
	public interface IVoxelMaterialManager
	{
		VoxelMaterial CreateMaterial(string name, VoxelMaterialParams setting);

		VoxelMaterial GetMaterial(string name);

		VoxelMaterial GetMaterial(int id);

		VoxelMaterial[] GetMaterials();
	}
}