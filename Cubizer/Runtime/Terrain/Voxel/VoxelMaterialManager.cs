using System.Diagnostics;
using System.Collections.Generic;

using UnityEngine;

namespace Cubizer
{
	public sealed class VoxelMaterialManager : IVoxelMaterialManager
	{
		private static List<VoxelMaterial> _lives = new List<VoxelMaterial>();
		private static Dictionary<string, int> _liveIndex = new Dictionary<string, int>();

		private static readonly VoxelMaterialManager instance = new VoxelMaterialManager();

		public static Dictionary<string, int> lives
		{
			get { return _liveIndex; }
		}

		private VoxelMaterialManager()
		{
		}

		public static VoxelMaterialManager GetInstance()
		{
			return instance;
		}

		public VoxelMaterial CreateMaterial(string name, VoxelMaterialParams setting)
		{
			System.Diagnostics.Debug.Assert(!System.String.IsNullOrEmpty(name));

			if (_liveIndex.ContainsKey(name))
				throw new System.Exception(string.Format("Material has been created ({0})", name));

			var newMaterial = new VoxelMaterial(name, _lives.Count + 1)
			{
				is_merge = setting.merge,
				is_transparent = setting.transparent
			};

			_liveIndex.Add(name, _lives.Count + 1);
			_lives.Add(newMaterial);

			return newMaterial;
		}

		public VoxelMaterial GetMaterial(string name)
		{
			System.Diagnostics.Debug.Assert(!System.String.IsNullOrEmpty(name));

			int index = 0;
			if (!_liveIndex.TryGetValue(name, out index))
				throw new System.Exception(string.Format("Material not found ({0})", name));

			return _lives[index - 1];
		}

		public VoxelMaterial GetMaterial(int id)
		{
			if (id <= 0 && id <= (_lives.Count + 1))
				throw new System.Exception(string.Format("Out of index ({0})", id));
			return _lives[id - 1];
		}

		public VoxelMaterial[] GetMaterialAll()
		{
			VoxelMaterial[] materials = new VoxelMaterial[_lives.Count];

			for (int i = 0; i < _lives.Count; i++)
				materials[i] = _lives[i];

			return materials;
		}

		public void Dispose()
		{
			_lives.Clear();
			_liveIndex.Clear();
		}
	}
}