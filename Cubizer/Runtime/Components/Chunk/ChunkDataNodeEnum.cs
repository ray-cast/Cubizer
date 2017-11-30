using System.Collections;

namespace Cubizer
{
	public class ChunkDataNodeEnum<_Tx, _Ty> : IEnumerator
		where _Tx : struct
		where _Ty : class
	{
		private int position = -1;
		private readonly ChunkDataNode<_Tx, _Ty>[] _array;

		public ChunkDataNodeEnum(ChunkDataNode<_Tx, _Ty>[] list)
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

		public ChunkDataNode<_Tx, _Ty> Current
		{
			get
			{
				return _array[position];
			}
		}
	}
}