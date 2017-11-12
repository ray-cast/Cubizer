using UnityEngine;
using System.Collections;

namespace Cubizer
{
	public abstract class TerrainBiomeGenerator : MonoBehaviour
	{
		[SerializeField]
		private ChunkGenerator _chunkGenerator;

		public abstract void OnBuildBiome();
	}
}