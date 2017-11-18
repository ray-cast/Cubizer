using System.Diagnostics;
using System.Collections.Generic;

namespace Cubizer
{
	public sealed class VoxelMaterialManager : IVoxelMaterialManager
	{
		private static List<VoxelMaterial> _lives = new List<VoxelMaterial>();
		private static Dictionary<string, int> _liveIndex = new Dictionary<string, int>();

		public static Dictionary<string, int> lives
		{
			get { return _liveIndex; }
		}

		private static readonly VoxelMaterialManager instance = new VoxelMaterialManager();

		private VoxelMaterialManager()
		{
		}

		public static VoxelMaterialManager GetInstance()
		{
			return instance;
		}

		public VoxelMaterial CreateMaterial(string name, VoxelMaterialParams setting)
		{
			Debug.Assert(!System.String.IsNullOrEmpty(name));

			if (_liveIndex.ContainsKey(name))
				return null;

			var newMaterial = new VoxelMaterial(name, _lives.Count + 1);
			newMaterial.is_dynamic = setting.dynamic;
			newMaterial.is_merge = setting.merge;
			newMaterial.is_transparent = setting.transparent;

			_liveIndex.Add(name, _lives.Count + 1);
			_lives.Add(newMaterial);

			return newMaterial;
		}

		public VoxelMaterial GetMaterial(string name)
		{
			Debug.Assert(!System.String.IsNullOrEmpty(name));

			if (!_liveIndex.ContainsKey(name))
				return null;
			return _lives[_liveIndex[name] - 1];
		}

		public VoxelMaterial GetMaterial(int id)
		{
			return (id <= 0 && id <= (_lives.Count + 1)) ? null : _lives[id - 1];
		}

		public VoxelMaterial[] GetMaterials()
		{
			VoxelMaterial[] materials = new VoxelMaterial[_lives.Count];
			for (int i = 0; i < _lives.Count; i++)
				materials[i] = _lives[i];
			return materials;
		}
	}
}