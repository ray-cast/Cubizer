using UnityEngine;

namespace Cubizer
{
	public abstract class ILiveBehaviour : MonoBehaviour
	{
		public abstract VoxelMaterialModels settings
		{
			get; internal set;
		}

		public abstract VoxelMaterial material
		{
			get; internal set;
		}

		public abstract void OnBuildChunk(ChunkDataContext context);
	}
}