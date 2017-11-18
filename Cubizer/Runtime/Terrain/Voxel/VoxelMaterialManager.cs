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

		public VoxelMaterial CreateMaterial(string name)
		{
			Debug.Assert(!System.String.IsNullOrEmpty(name));

			if (_liveIndex.ContainsKey(name))
				return null;

			var material = new VoxelMaterial(name, _lives.Count);
			_liveIndex.Add(name, _lives.Count);
			_lives.Add(material);

			return material;
		}

		public VoxelMaterial GetMaterial(string name)
		{
			Debug.Assert(!System.String.IsNullOrEmpty(name));

			if (!_liveIndex.ContainsKey(name))
				return null;
			return _lives[_liveIndex[name]];
		}

		public VoxelMaterial GetMaterial(int id)
		{
			return _lives.Count <= id ? null : _lives[id];
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