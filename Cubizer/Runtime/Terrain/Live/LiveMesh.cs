using UnityEngine;

namespace Cubizer
{
	public struct LiveMesh
	{
		public Vector3[] vertices;
		public Vector3[] normals;
		public Vector2[] uv;
		public int[] indices;

		public Mesh mesh
		{
			get
			{
				Mesh mesh = new Mesh();
				mesh.vertices = vertices;
				mesh.normals = normals;
				mesh.uv = uv;
				mesh.triangles = indices;

				return mesh;
			}
		}

		public LiveMesh(int verticesCount, int indicesCount)
		{
			vertices = new Vector3[verticesCount];
			normals = new Vector3[verticesCount];
			uv = new Vector2[verticesCount];
			indices = new int[indicesCount];
		}
	}
}