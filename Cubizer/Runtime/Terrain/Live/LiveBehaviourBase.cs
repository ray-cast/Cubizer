using UnityEngine;

namespace Cubizer
{
	public abstract class LiveBehaviourBase : MonoBehaviour
	{
		public abstract void OnBuildChunk(GameObject parent, IVoxelModel model, int faceCount);
	}
}