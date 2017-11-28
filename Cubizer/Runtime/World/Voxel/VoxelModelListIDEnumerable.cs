using System.Collections;
using System.Collections.Generic;

namespace Cubizer
{
	public class VoxelModelListIDEnumerable : IEnumerable
	{
		private readonly int _instanceID;
		private readonly List<VoxelPrimitive> _array;

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