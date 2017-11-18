using UnityEngine;

namespace Cubizer
{
	public class CubizerProfile : ScriptableObject
	{
		#pragma warning disable 0169 // "field x is never used"

		public TerrainModel terrain = new TerrainModel();
	}
}