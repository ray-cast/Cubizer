namespace Cubizer
{
	public interface IVoxelMaterialManager
	{
		VoxelMaterial RegisterMaterial(string name, VoxelMaterial material);

		VoxelMaterial GetMaterial(string name);

		VoxelMaterial GetMaterial(int id);

		VoxelMaterial[] GetMaterials();
	}
}