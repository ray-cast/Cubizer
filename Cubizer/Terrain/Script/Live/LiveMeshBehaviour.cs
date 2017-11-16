using UnityEngine;

namespace Cubizer
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[AddComponentMenu("Cubizer/LiveMeshBehaviour")]
	public class LiveMeshBehaviour : LiveBehaviour
	{
		private MeshFilter _meshFilter;
		private MeshRenderer _meshRenderer;
		private LODGroup _lodGroup;

		public bool collide = true;
		public PhysicMaterial physicMaterial;

		public void Start()
		{
			_lodGroup = GetComponent<LODGroup>();
			_meshFilter = GetComponent<MeshFilter>();
			_meshRenderer = GetComponent<MeshRenderer>();
		}

		public override void OnBuildChunkObject(GameObject parent, IVoxelModel model, int faceCount)
		{
			foreach (VoxelPrimitive it in model.GetEnumerator(this.material.GetInstanceID()))
			{
				Vector3 pos, scale;
				it.GetTranslateScale(out pos, out scale);

				var actors = new GameObject(this.name);
				actors.isStatic = parent.isStatic;
				actors.layer = parent.layer;
				actors.transform.parent = parent.transform;
				actors.transform.position = parent.transform.position + pos + transform.localPosition;
				actors.transform.localScale = transform.localScale;

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

				if (collide && _lodGroup.enabled)
				{
					var meshCollider = actors.AddComponent<MeshCollider>();
					meshCollider.sharedMesh = _meshFilter.mesh;
					meshCollider.material = physicMaterial;
				}
			}
		}
	}
}