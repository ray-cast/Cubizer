using UnityEngine;

namespace Cubizer
{
	public abstract class ILiveBehaviour : MonoBehaviour
	{
		public abstract void OnBuildChunk(GameObject parent, IVoxelModel model, int faceCount);
	}
}