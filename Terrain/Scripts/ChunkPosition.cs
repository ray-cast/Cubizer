using System;

namespace Chunk
{
	[Serializable]
	public struct ChunkPosition<_Tx> where _Tx : struct
	{
		public _Tx x;
		public _Tx y;
		public _Tx z;

		public ChunkPosition(_Tx xx, _Tx yy, _Tx zz)
		{
			x = xx;
			y = yy;
			z = zz;
		}
	}
}