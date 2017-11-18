namespace Cubizer
{
	public interface IVoxelMaterialManager
	{
		VoxelMaterial CreateMaterial(string name);

		VoxelMaterial GetMaterial(string name);

		VoxelMaterial GetMaterial(int id);

		VoxelMaterial[] GetMaterials();
	}
}