using System;

namespace Cubizer
{
	public interface IVoxelMaterialManager : IDisposable
	{
		VoxelMaterial CreateMaterial(string name, VoxelMaterialParams setting);

		VoxelMaterial GetMaterial(string name);

		VoxelMaterial GetMaterial(int id);

		VoxelMaterial[] GetMaterialAll();
	}
}