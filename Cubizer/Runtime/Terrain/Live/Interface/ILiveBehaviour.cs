using UnityEngine;

namespace Cubizer
{
	public interface ILiveBehaviour
	{
		void OnBuildChunk(GameObject parent, IVoxelModel model, int faceCount);
	}
}