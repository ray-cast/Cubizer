using System.Collections;
using System.Collections.Generic;

namespace Cubizer
{
	public class VoxelModelListEnumerable: IEnumerable
	{
		private List<VoxelPrimitive> _array;

		public VoxelModelListEnumerable(List<VoxelPrimitive> array)
		{
			_array = array;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}

		public VoxelModelListEnum GetEnumerator()
		{
			return new VoxelModelListEnum(_array);
		}
	}
}