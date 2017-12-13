using System.Collections;

namespace Cubizer.Chunk
{
	public sealed class ChunkDataNodeEnumerable<_Tx, _Ty> : IEnumerable
		where _Tx : struct
		where _Ty : class
	{
		private readonly ChunkDataNode<_Tx, _Ty>[] _array;

		public ChunkDataNodeEnumerable(ChunkDataNode<_Tx, _Ty>[] array)
		{
			_array = array;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}

		public ChunkDataNodeEnum<_Tx, _Ty> GetEnumerator()
		{
			return new ChunkDataNodeEnum<_Tx, _Ty>(_array);
		}
	}
}