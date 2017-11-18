using System.Collections;
using System.Collections.Generic;

namespace Cubizer
{
	public class VoxelModelListEnum : IEnumerator
	{
		private int position = -1;
		private List<VoxelPrimitive> _array;

		public VoxelModelListEnum(List<VoxelPrimitive> list)
		{
			_array = list;
		}

		public bool MoveNext()
		{
			position++;
			return position < _array.Count;
		}

		public void Reset()
		{
			position = -1;
		}

		object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}

		public VoxelPrimitive Current
		{
			get
			{
				return _array[position];
			}
		}
	}
}