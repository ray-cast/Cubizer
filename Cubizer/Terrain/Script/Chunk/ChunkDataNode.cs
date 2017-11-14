using System;

namespace Cubizer
{
	[Serializable]
	public class ChunkDataNode<_Tx, _Ty>
		where _Tx : struct
		where _Ty : class
	{
		public _Tx position;
		public _Ty value;

		public ChunkDataNode()
		{
			value = null;
		}

		public ChunkDataNode(_Tx x, _Ty value)
		{
			position = x;
			this.value = value;
		}

		public bool is_empty()
		{
			return value == null;
		}
	}
}