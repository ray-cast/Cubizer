using System.Collections.Generic;

namespace Cubizer
{
	public class VoxelMaterialManager : IVoxelMaterialManager
	{
		private List<VoxelMaterial> _lives = new List<VoxelMaterial>();
		private Dictionary<string, int> _liveIndex = new Dictionary<string, int>();

		public List<VoxelMaterial> lives
		{
			get { return _lives; }
		}

		public VoxelMaterial CreateMaterial(string name, VoxelMaterialModels models)
		{
			UnityEngine.Debug.Assert(!System.String.IsNullOrEmpty(name));

			if (_liveIndex.ContainsKey(name))
				throw new System.Exception(string.Format("Material has been created ({0})", name));

			var newMaterial = new VoxelMaterial(name, models, _lives.Count + 1)
			{
				is_merge = models.merge,
				is_transparent = models.transparent
			};

			_liveIndex.Add(name, _lives.Count + 1);
			_lives.Add(newMaterial);

			return newMaterial;
		}

		public VoxelMaterial GetMaterial(string name)
		{
			UnityEngine.Debug.Assert(!System.String.IsNullOrEmpty(name));

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