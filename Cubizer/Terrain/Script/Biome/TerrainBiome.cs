using UnityEngine;
using System.Collections;

namespace Cubizer
{
	public class TerrainBiome : MonoBehaviour
	{
		private Terrain _terrain;

		public Terrain terrain
		{
			set { _terrain = value; }
			get { return _terrain; }
		}
	}
}