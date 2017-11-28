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
		private MeshCollider _meshCollider;

		private LODGroup _lodGroup;

		public object _object;

		public void Start()
		{
			_lodGroup = GetComponent<LODGroup>();
			_meshFilter = GetComponent<MeshFilter>();
			_meshRenderer = GetComponent<MeshRenderer>();
			_meshCollider = GetComponent<MeshCollider>();
		}

		public override void OnBuildChunk(ChunkDataContext context)
		{
			foreach (VoxelPrimitive it in context.model.GetEnumerator(this.material.GetInstanceID()))
			{
				Vector3 pos, scale;
				it.GetTranslateScale(out pos, out scale);

				var actors = new GameObject(this.name);
				actors.isStatic = gameObject.isStatic;
				actors.tag = gameObject.tag;
				actors.layer = this.gameObject.layer;
				actors.transform.parent = context.parent.transform;
				actors.transform.localPosition = transform.position + pos;
				actors.transform.localRotation = transform.rotation;
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