using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	public class ChunkGeneratorManager : MonoBehaviour
	{
		private Terrain _terrain;

		public Terrain terrain
		{
			set { _terrain = value; }
			get { return _terrain; }
		}

		public void OnGeneratorEnable(ChunkGenerator generator)
		{
		}

		public void OnGeneratorDisable(ChunkGenerator generator)
		{
		}
	}
}