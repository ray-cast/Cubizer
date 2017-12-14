using System.Collections;

namespace Cubizer.Models
{
	public sealed class VOXHashMapNodeEnum<_Tx> : IEnumerator
		where _Tx : struct
	{
		private int position = -1;
		private VOXHashMapNode<_Tx>[] _array;

		public VOXHashMapNodeEnum(VOXHashMapNode<_Tx>[] list)
		{
			_array = list;
		}

		public bool MoveNext()
		{
			var length = _array.Length;
			for (position++; position < length; position++)
			{
				if (_array[position] == null)
					continue;
				if (_array[position].is_empty())
					continue;
				break;
			}

			return position < _array.Length;
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

		public VOXHashMapNode<_Tx> Current
		{
			get
			{
				return _array[position];
			}
		}
	}
}