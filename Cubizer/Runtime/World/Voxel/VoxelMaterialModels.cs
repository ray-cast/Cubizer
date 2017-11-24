using System;
using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public struct VoxelMaterialModels
	{
		[SerializeField]
		public bool merge;

		[SerializeField]
		public bool transparent;

		[NonSerialized]
		public object userdata;

		public void Reset()
		{
			merge = true;
			transparent = false;
			userdata = null;
		}
	}
}