using System;
using UnityEngine;

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

		public static class Vector3Extensions
		{
			private static int _hash_int(int key)
			{
				key = ~key + (key << 15);
				key = key ^ (key >> 12);
				key = key + (key << 2);
				key = key ^ (key >> 4);
				key = key * 2057;
				key = key ^ (key >> 16);
				return key;
			}

			public static int HashInt(this Vector3<byte> v)
			{
				return _hash_int(v.x) ^ _hash_int(v.y) ^ _hash_int(v.z);
			}

			public static int HashInt(this Vector3<short> v)
			{
				return _hash_int(v.x) ^ _hash_int(v.y) ^ _hash_int(v.z);
			}

			public static int HashInt(this Vector3<int> v)
			{
				return _hash_int(v.x) ^ _hash_int(v.y) ^ _hash_int(v.z);
			}

			public static void HashInt<T>(this Vector3<T> v) where T : struct
			{
				Debug.Assert(false);
			}
		}
	}
}