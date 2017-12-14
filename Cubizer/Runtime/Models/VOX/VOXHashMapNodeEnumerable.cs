using System.Collections;

namespace Cubizer.Models
{
	public sealed class VOXHashMapNodeEnumerable<_Tx> : IEnumerable
		where _Tx : struct
	{
		private VOXHashMapNode<_Tx>[] _array;

		public VOXHashMapNodeEnumerable(VOXHashMapNode<_Tx>[] array)
		{
			_array = array;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}

		public VOXHashMapNodeEnum<_Tx> GetEnumerator()
		{
			return new VOXHashMapNodeEnum<_Tx>(_array);
		}
	}
}