using UnityEngine;
using System.Collections;

namespace Cubizer
{
	public class BiomeData : MonoBehaviour
	{
		private Terrain _terrain;

		private ChunkGenerator _chunkGenerator;

		public Terrain terrain
		{
			set { _terrain = value; }
			get { return _terrain; }
		}

		public ChunkGenerator chunkGenerator
		{
			set { _chunkGenerator = value; }
			get { return _chunkGenerator; }
		}

		public virtual void OnBuildChunk(ChunkPrimer chunk)
		{
			Debug.Assert(_terrain != null);

			if (_chunkGenerator == null)
				throw new System.Exception("The chunk generator of biome cannot be null");

			_chunkGenerator.OnCreateChunk(chunk);
		}
	}
}