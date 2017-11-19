using System;

namespace Cubizer
{
	[Serializable]
	public struct VoxelMaterialParams
	{
		public bool merge;
		public bool transparent;

		public void Reset()
		{
			merge = true;
			transparent = false;
		}
	}
}