using UnityEngine;

namespace Cubizer
{
	[AddComponentMenu("Cubizer/LiveVoxelBehaviour")]
	public class LiveVoxelBehaviour : LiveBehaviour
	{
		private MeshFilter _meshFilter;
		private MeshRenderer _meshRenderer;
		private MeshCollider _meshCollider;
		private LODGroup _lodGroup;

		public int _LOD = 0;
		public Shader _shader;
		public TextAsset _asset;

		public void Start()
		{
			if (_shader == null)
				Debug.LogError("Please assign a shader on the inspector");

			if (_asset == null)
				Debug.LogError("Please assign a .vox file on the inspector");

			try
			{
				var vox = Model.VoxFileImport.Load(_asset.bytes);
				if (vox != null)
					Model.VoxFileImport.LoadVoxelFileAsGameObject(gameObject, vox, _LOD, _shader);

				if (transform.childCount > 0)
				{
					var model = transform.GetChild(0);
					if (model)
					{
						model.gameObject.layer = this.gameObject.layer;

						_meshCollider = GetComponent<MeshCollider>();

						_lodGroup = model.GetComponent<LODGroup>();
						_meshFilter = model.GetComponent<MeshFilter>();
						_meshRenderer = model.GetComponent<MeshRenderer>();
					}
				}
			}
			catch (System.Exception e)
			{
				Debug.LogError(e.Message);
			}
		}

		public override void OnBuildChunk(IChunkData parent, IVoxelModel model, int faceCount)
		{
			if (_meshFilter == null)
				return;

			foreach (VoxelPrimitive it in model.GetEnumerator(this.material.GetInstanceID()))
			{
				Vector3 pos, scale;
				it.GetTranslateScale(out pos, out scale);

				var actors = new GameObject(this.name);
				actors.isStatic = gameObject.isStatic;
				actors.tag = gameObject.tag;
				actors.transform.parent = parent.transform;
				actors.transform.localPosition = transform.position + pos;
				actors.transform.localRotation = transform.rotation;
				actors.transform.localScale = Vector3.Scale(transform.localScale, scale);

				actors.AddComponent<MeshFilter>().sharedMesh = _meshFilter.sharedMesh;

				if (_meshRenderer != null && _meshRenderer.enabled)
				{
					var clone = actors.AddComponent<MeshRenderer>();
					clone.material = _meshRenderer.material;
					clone.receiveShadows = _meshRenderer.receiveShadows;
					clone.shadowCastingMode = _meshRenderer.shadowCastingMode;

					if (_lodGroup != null && _lodGroup.enabled)
					{
						var lods = _lodGroup.GetLODs();
						for (int i = 0; i < lods.Length; i++)
						{
							if (lods[i].renderers.Length > 0)
								lods[i].renderers[0] = clone;
						}

						actors.AddComponent<LODGroup>().SetLODs(lods);
					}
				}

				if (_meshCollider && _meshCollider.enabled)
				{
					var meshCollider = actors.AddComponent<MeshCollider>();
					meshCollider.sharedMesh = _meshCollider.sharedMesh ? _meshCollider.sharedMesh : _meshFilter.mesh;
					meshCollider.material = _meshCollider.material;
				}
			}
		}
	}
}