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

		public virtual void OnBuildChunk(Terrain terrain, ChunkPrimer chunk)
		{
			if (_chunkGenerator != null)
				_chunkGenerator.OnCreateChunk(chunk);
		}
	}
}