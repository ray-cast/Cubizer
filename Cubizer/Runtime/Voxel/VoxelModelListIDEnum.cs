using System.Collections;
using System.Collections.Generic;

namespace Cubizer
{
	public class VoxelModelListIDEnum : IEnumerator
	{
		private int position = -1;
		private readonly int _instanceID = 0;
		private readonly List<VoxelPrimitive> _array;

		public VoxelModelListIDEnum(List<VoxelPrimitive> list, int instanceID)
		{
			_array = list;
			_instanceID = instanceID;
		}

		public bool MoveNext()
		{
			var length = _array.Count;
			for (position++; position < length; position++)
			{
				if (_array[position].material.GetInstanceID() != _instanceID)
					continue;
				break;
			}

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