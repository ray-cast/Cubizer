using System;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class CubizerProfile : ScriptableObject
	{
		#pragma warning disable 0169 // "field x is never used"

		public TerrainModel terrain = new TerrainModel();
		public ChunkManagerModels chunk = new ChunkManagerModels();
		public BiomeManagerModels biome = new BiomeManagerModels();
		public LiveManagerModels lives = new LiveManagerModels();
	}
}