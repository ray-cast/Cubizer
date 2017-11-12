using UnityEngine;

namespace Cubizer
{
	public class ChunkGeneratorManager : MonoBehaviour
	{
		private Terrain _terrain;

		public Terrain terrain
		{
			set { _terrain = value; }
			get { return _terrain; }
		}
	}
}