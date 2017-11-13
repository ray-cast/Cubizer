using UnityEngine;
using System.Collections;

namespace Cubizer
{
	public class BiomeData : MonoBehaviour
	{
		private ChunkGenerator _chunkGenerator;

		public ChunkGenerator chunkGenerator
		{
			set { _chunkGenerator = value; }
			get { return _chunkGenerator; }
		}

		public virtual ChunkPrimer OnBuildChunk(Terrain terrain, short x, short y, short z)
		{
			if (_chunkGenerator != null)
				return _chunkGenerator.OnCreateChunk(terrain, x, y, z);

			return null;
		}
	}
}