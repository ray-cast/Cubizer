using UnityEngine;

namespace Cubizer
{
	[AddComponentMenu("Cubizer/MeshFilter")]
	public sealed class CubizerMeshFilter : MonoBehaviour
	{
		public int _LOD = 0;
		public Shader _shader;
		public TextAsset _asset;

		public void Start()
		{
			try
			{
				if (_asset == null)
					Debug.LogError("Please assign a .vox file on the inspector");

				if (_shader == null)
					Debug.LogError("Please assign a shader on the inspector");

				var vox = Model.VoxFileImport.Load(_asset.bytes);
				if (vox != null)
					Model.VoxFileImport.LoadVoxelFileAsGameObject(gameObject, vox, _LOD, _shader);
			}
			catch (System.Exception e)
			{
				Debug.LogError(e.Message);
			}
		}
	}
}