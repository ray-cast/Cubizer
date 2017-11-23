using System;

namespace Cubizer
{
	public interface IVoxelMaterialManager : IDisposable
	{
		VoxelMaterial CreateMaterial(string name, VoxelMaterialModels models);

		VoxelMaterial GetMaterial(string name);

		VoxelMaterial GetMaterial(int id);

		VoxelMaterial[] GetMaterialAll();
	}
}