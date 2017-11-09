using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class VoxelMaterial
	{
		private bool _actor;
		private bool _dynamic;
		private bool _transparent;
		private bool _merge;

		private string _name;
		private string _material;

		public bool is_actor { get { return _actor; } }
		public bool is_dynamic { get { return _dynamic; } }
		public bool is_transparent { get { return _transparent; } }
		public bool is_merge { get { return _merge; } }

		public string name { set { _name = value; } get { return _name; } }
		public string material { set { _material = value; } get { return _material; } }
	}
}