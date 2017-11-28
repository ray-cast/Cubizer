using System.Collections;
using System.Collections.Generic;

namespace Cubizer
{
	public class VoxelModelListEnumerable : IEnumerable
	{
		private readonly List<VoxelPrimitive> _array;

		public VoxelModelListEnumerable(List<VoxelPrimitive> array)
		{
			_array = array;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public VoxelModelListEnum GetEnumerator()
		{
			return new VoxelModelListEnum(_array);
		}
	}
}