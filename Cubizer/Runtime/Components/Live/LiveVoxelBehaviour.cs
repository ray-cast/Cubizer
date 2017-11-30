using UnityEngine;

namespace Cubizer
{
	[AddComponentMenu("Cubizer/LiveVoxelBehaviour")]
	public class LiveVoxelBehaviour : LiveBehaviour
	{
		private Mesh _mesh;
		private Material _meshMaterial;
		private LOD[] _lods;

		public int _LOD = 0;
		public Shader _shader;
		public TextAsset _asset;
		public PhysicMaterial _physicMaterial;

		public void Start()
		{
			if (_shader == null)
				Debug.LogError("Please assign a shader on the inspector");

			if (_asset == null || _asset.bytes.Length == 0)
				Debug.LogError("Please assign a .vox file on the inspector");

			try
			{
				var vox = Model.VoxFileImport.Load(_asset.bytes);
				if (vox != null)
				{
					var model = Model.VoxFileImport.LoadVoxelFileAsGameObject(this.name, vox, _LOD, _shader);

					_mesh = model.GetComponentInChildren<MeshFilter>().sharedMesh;

					var lodGroup = model.GetComponentInChildren<LODGroup>();
					if (lodGroup != null)
						_lods = lodGroup.GetLODs();

					var meshRenderer = model.GetComponentInChildren<MeshRenderer>();
					if (meshRenderer != null)
						_meshMaterial = meshRenderer.sharedMaterial;

					DestroyImmediate(model);
				}
			}
			catch (System.Exception e)
			{
				Debug.LogError(e.Message);
			}
		}

		public override void OnBuildChunk(ChunkDataContext context)
		{
			if (_mesh == null)
				return;

			foreach (VoxelPrimitive it in context.model.GetEnumerator(this.material.GetInstanceID()))
			{
				Vector3 pos, scale;
				it.GetTranslateScale(out pos, out scale);

				var actors = new GameObject(this.name);
				actors.isStatic = gameObject.isStatic;
				actors.tag = gameObject.tag;
				actors.layer = gameObject.layer;
				actors.transform.parent = context.parent.transform;
				actors.transform.localPosition = transform.position + pos;
				actors.transform.localRotation = transform.rotation;
				actors.transform.localScale = Vector3.Scale(transform.localScale, scale);

				actors.AddComponent<MeshFilter>().sharedMesh = _mesh;

				if (_meshMaterial)
				{
					var clone = actors.AddComponent<MeshRenderer>();
					clone.material = _meshMaterial;

					if (_lods != null)
					{
						LOD[] lods = new LOD[_lods.Length];

						for (int i = 0; i < _lods.Length; i++)
						{
							if (lods[i].renderers.Length > 0)
								lods[i].renderers[0] = clone;
						}

						actors.AddComponent<LODGroup>().SetLODs(_lods);
					}
				}

				if (_physicMaterial != null)
				{
					var meshCollider = actors.AddComponent<MeshCollider>();
					meshCollider.sharedMesh = _mesh;
					meshCollider.material = _physicMaterial;
				}
			}
		}
	}
}