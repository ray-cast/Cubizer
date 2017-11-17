using UnityEngine;

namespace Cubizer
{
	[AddComponentMenu("Cubizer/VOXMeshFilter")]
	public sealed class VOXMeshFilter : MonoBehaviour
	{
		public int _LOD = 0;
		public TextAsset _asset;

		public void Start()
		{
			try
			{
				if (_asset == null)
					Debug.LogError("Please assign a .vox file on the inspector");

				var vox = Model.VoxFileImport.Load(_asset.bytes);
				if (vox != null)
					Model.VoxFileImport.LoadVoxelFileAsGameObject(gameObject, vox, _LOD);

				var meshFilter = GetComponent<MeshFilter>();
			}
			catch (System.Exception e)
			{
				Debug.LogError(e.Message);
			}
		}
	}
}