using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Cubizer
{
	public class VoxelMaterialManager
	{
		private static List<VoxelMaterial> _lives = new List<VoxelMaterial>();
		private static Dictionary<string, int> _liveIndex = new Dictionary<string, int>();

		public static Dictionary<string, int> materials
		{
			get { return _liveIndex; }
		}

		public static VoxelMaterial CreateMaterial(string name)
		{
			Debug.Assert(!System.String.IsNullOrEmpty(name));

			if (_liveIndex.ContainsKey(name))
				return null;

			var material = new VoxelMaterial(name, _lives.Count);
			_liveIndex.Add(name, _lives.Count);
			_lives.Add(material);

			return material;
		}

		public static VoxelMaterial GetMaterial(string name)
		{
			Debug.Assert(!System.String.IsNullOrEmpty(name));

			if (_liveIndex.ContainsKey(name))
				return _lives[_liveIndex[name]];

			return null;
		}

		public static VoxelMaterial GetMaterial(int id)
		{
			return _lives.Count <= id ? null : _lives[id];
		}
	}
}