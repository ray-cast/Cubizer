using System;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class VoxelMaterialParams
	{
		[SerializeField]
		public bool dynamic = false;

		[SerializeField]
		public bool transparent = false;

		[SerializeField]
		public bool merge = true;
	}
}