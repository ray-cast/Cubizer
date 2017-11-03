using UnityEngine;

namespace Cubizer
{
	public class ChunkUtility
	{
		public static short CalcChunkPos(float x, int size)
		{
			return (short)Mathf.FloorToInt(x / (float)size);
		}

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

		public static int HashInt(int x, int y, int z)
		{
			return _hash_int(x) ^ _hash_int(y) ^ _hash_int(z);
		}
	}
}