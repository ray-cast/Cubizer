using System.Collections;
using System.Collections.Generic;

namespace Cubizer
{
	public class VoxelModelListIDEnumerable : IEnumerable
	{
		private List<VoxelPrimitive> _array;
		private int _instanceID;

		public VoxelModelListIDEnumerable(List<VoxelPrimitive> array, int instanceID)
		{
			_array = array;
			_instanceID = instanceID;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public VoxelModelListIDEnum GetEnumerator()
		{
			return new VoxelModelListIDEnum(_array, _instanceID);
		}
	}
}