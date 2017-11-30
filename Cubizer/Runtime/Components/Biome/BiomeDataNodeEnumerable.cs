using System.Collections;

namespace Cubizer
{
	public class BiomeDataNodeEnumerable<_Tx, _Ty> : IEnumerable
		where _Tx : struct
		where _Ty : class
	{
		private readonly BiomeDataNode<_Tx, _Ty>[] _array;

		public BiomeDataNodeEnumerable(BiomeDataNode<_Tx, _Ty>[] array)
		{
			_array = array;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public BiomeDataNodeEnum<_Tx, _Ty> GetEnumerator()
		{
			return new BiomeDataNodeEnum<_Tx, _Ty>(_array);
		}
	}
}