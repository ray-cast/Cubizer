using System.Collections;

namespace Cubizer
{
	public sealed class VoxelDataNodeEnumerable<_Tx, _Ty> : IEnumerable
		where _Tx : struct
		where _Ty : class
	{
		private readonly VoxelDataNode<_Tx, _Ty>[] _array;

		public VoxelDataNodeEnumerable(VoxelDataNode<_Tx, _Ty>[] array)
		{
			_array = array;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public VoxelDataNodeEnum<_Tx, _Ty> GetEnumerator()
		{
			return new VoxelDataNodeEnum<_Tx, _Ty>(_array);
		}
	}
}