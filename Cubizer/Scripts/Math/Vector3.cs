using System;

namespace Cubizer
{
	namespace Math
	{
		[Serializable]
		public struct Vector3<_Tx> where _Tx : struct
		{
			public _Tx x;
			public _Tx y;
			public _Tx z;

			public Vector3(_Tx xx, _Tx yy, _Tx zz)
			{
				x = xx;
				y = yy;
				z = zz;
			}
		}
	}
}