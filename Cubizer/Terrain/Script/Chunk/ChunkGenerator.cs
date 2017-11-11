using UnityEngine;

namespace Cubizer
{
	public abstract class ChunkGenerator : MonoBehaviour
	{
		private Terrain _terrain;

		public Terrain terrain
		{
			set { _terrain = value; }
			get { return _terrain; }
		}

		public abstract void OnCreateChunk(ChunkData map);
	}
}