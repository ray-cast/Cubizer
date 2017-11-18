using System;
using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class CubizerProfile : ScriptableObject
	{
		#pragma warning disable 0169 // "field x is never used"

		public TerrainModel terrain = new TerrainModel();
		public ChunkDataModels chunk = new ChunkDataModels();
		public BiomeGeneratorModels biome = new BiomeGeneratorModels();
		public LiveModels lives = new LiveModels();
	}
}