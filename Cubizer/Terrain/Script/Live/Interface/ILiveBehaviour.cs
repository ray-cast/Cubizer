using UnityEngine;

namespace Cubizer
{
	public interface ILiveBehaviour
	{
		void OnBuildChunkObject(GameObject parent, IVoxelModel model, int faceCount);
	}
}