using System;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class VoxelMaterialParams
	{
		[SerializeField]
		public string name;

		[SerializeField]
		public object userdata;

		[SerializeField]
		public bool dynamic = false;

		[SerializeField]
		public bool transparent = false;

		[SerializeField]
		public bool merge = false;
	}
}