using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Cubizer.Math;

namespace Cubizer
{
	[AddComponentMenu("Cubizer/LiveMeshBehaviour")]
	public class LiveMeshBehaviour : LiveBehaviour
	{
		public Mesh _mesh;

		public override uint GetVerticesCount(uint faceCount)
		{
			return (uint)((faceCount / 6) * _mesh.vertexCount);
		}

		public override uint GetIndicesCount(uint faceCount)
		{
			return (uint)((faceCount / 6) * _mesh.triangles.Length);
		}

		public override bool OnUpdateChunk(ref ChunkData map, System.Byte x, System.Byte y, System.Byte z)
		{
			return false;
		}

		public override void OnCreateBlock(ref TerrainMesh mesh, ref int index, Vector3 pos, Vector3 scale, VoxelVisiableFaces faces)
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

		public override void Start()
		{
			if (_mesh == null)
				UnityEngine.Debug.LogError("Please drag a Mesh into Hierarchy View.");

			this.RegisterDefaultMaterial();
		}
	}
}