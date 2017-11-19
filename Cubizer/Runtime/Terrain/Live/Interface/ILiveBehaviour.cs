using UnityEngine;

namespace Cubizer
{
	public abstract class ILiveBehaviour : MonoBehaviour
	{
		public abstract void OnBuildChunk(IChunkData parent, IVoxelModel model, int faceCount);
	}
}