using System.Collections;

namespace Cubizer
{
	public class VoxelDataNodeEnumerable<_Tx, _Ty> : IEnumerable
		where _Tx : struct
		where _Ty : class
	{
		private VoxelDataNode<_Tx, _Ty>[] _array;

		public VoxelDataNodeEnumerable(VoxelDataNode<_Tx, _Ty>[] array)
		{
			_array = array;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}

		public VoxelDataNodeEnum<_Tx, _Ty> GetEnumerator()
		{
			return new VoxelDataNodeEnum<_Tx, _Ty>(_array);
		}
	}
}