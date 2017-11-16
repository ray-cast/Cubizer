using UnityEngine;

namespace Cubizer
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	[AddComponentMenu("Cubizer/LiveMeshBehaviour")]
	public class LiveMeshBehaviour : LiveBehaviour
	{
		private Mesh _mesh;
		private MeshRenderer _meshRenderer;

		public bool collide = true;
		public PhysicMaterial physicMaterial;

		public void Start()
		{
			_mesh = GetComponent<MeshFilter>().sharedMesh;
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

				actors.AddComponent<MeshFilter>().sharedMesh = _mesh;

				if (_meshRenderer != null)
				{
					var clone = actors.AddComponent<MeshRenderer>();
					clone.material = _meshRenderer.material;
					clone.receiveShadows = _meshRenderer.receiveShadows;
					clone.shadowCastingMode = _meshRenderer.shadowCastingMode;
				}

				if (collide)
				{
					var meshCollider = actors.AddComponent<MeshCollider>();
					meshCollider.sharedMesh = _mesh;
					meshCollider.material = physicMaterial;
				}
			}
		}

		public void OnBuildBlock(ref TerrainMesh mesh, ref int index, Vector3 pos, Vector3 scale, VoxelVisiableFaces faces)
		{
			var startVertices = _mesh.vertexCount * index;
			var startIndices = _mesh.triangles.Length * index;

			for (int i = startVertices, j = 0; i < startVertices + _mesh.vertexCount; i++, j++)
			{
				Vector3 v;
				v.x = _mesh.vertices[j].x * scale.x + pos.x;
				v.y = _mesh.vertices[j].y * scale.y + pos.y;
				v.z = _mesh.vertices[j].z * scale.z + pos.z;

				mesh.vertices[i] = v;
			}

			_mesh.normals.CopyTo(mesh.normals, startVertices);
			_mesh.uv.CopyTo(mesh.uv, startVertices);
			_mesh.triangles.CopyTo(mesh.triangles, startVertices);

			if (startIndices != 0)
			{
				for (int i = startIndices; i < startIndices + _mesh.triangles.Length; i++)
					mesh.triangles[i] += startVertices;
			}

			index++;
		}
	}
}