using UnityEngine;

namespace Cubizer
{
	[AddComponentMenu("Cubizer/LiveMeshBehaviour")]
	public class LiveMeshBehaviour : LiveBehaviour
	{
		public Mesh _mesh;

		public override int GetVerticesCount(int faceCount)
		{
			return (int)((faceCount / 6) * _mesh.vertexCount);
		}

		public override int GetIndicesCount(int faceCount)
		{
			return (int)((faceCount / 6) * _mesh.triangles.Length);
		}

		public override bool OnUpdateChunk(ref ChunkPrimer map, System.Byte x, System.Byte y, System.Byte z)
		{
			return false;
		}

		public override void OnBuildBlock(ref TerrainMesh mesh, ref int index, Vector3 pos, Vector3 scale, VoxelVisiableFaces faces)
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

		public override void OnBuildComponents(GameObject gameObject, Mesh mesh)
		{
			var renderer = this.GetComponent<MeshRenderer>();
			if (renderer != null)
			{
				gameObject.AddComponent<MeshFilter>().mesh = mesh;
				var clone = gameObject.AddComponent<MeshRenderer>();
				clone.material = renderer.material;
				clone.receiveShadows = renderer.receiveShadows;
				clone.shadowCastingMode = renderer.shadowCastingMode;
				renderer = clone;
			}

			var collider = this.GetComponent<Collider>();
			if (collider != null)
			{
				var type = collider.GetType();
				if (type == typeof(MeshCollider))
					gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
			}
		}

		public void Start()
		{
			if (_mesh == null)
				UnityEngine.Debug.LogError("Please drag a Mesh into Hierarchy View.");
		}
	}
}