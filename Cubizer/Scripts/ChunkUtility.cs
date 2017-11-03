using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Linq.Expressions;

using UnityEngine;

namespace Cubizer
{
	public class ChunkUtility
	{
		public static short CalcChunkPos(float x, int size)
		{
			return (short)Mathf.FloorToInt(x / (float)size);
		}

		private static int _hash_int(int key)
		{
			key = ~key + (key << 15);
			key = key ^ (key >> 12);
			key = key + (key << 2);
			key = key ^ (key >> 4);
			key = key * 2057;
			key = key ^ (key >> 16);
			return key;
		}

		public static int HashInt(int x, int y, int z)
		{
			return _hash_int(x) ^ _hash_int(y) ^ _hash_int(z);
		}

		public static string SaveMeshAsOBJ(MeshFilter mf, Vector3 scale)
		{
			Mesh mesh = mf.sharedMesh;
			Material[] sharedMaterials = mf.GetComponent<Renderer>().sharedMaterials;
			Vector2 textureOffset = mf.GetComponent<Renderer>().sharedMaterial.GetTextureOffset("_MainTex");
			Vector2 textureScale = mf.GetComponent<Renderer>().sharedMaterial.GetTextureScale("_MainTex");

			StringBuilder stringBuilder = new StringBuilder().Append("mtllib design.mtl")
				.Append("\n")
				.Append("g ")
				.Append(mf.name)
				.Append("\n");

			Vector3[] vertices = mesh.vertices;
			for (int i = 0; i < vertices.Length; i++)
			{
				Vector3 vector = vertices[i];
				stringBuilder.Append(string.Format("v {0} {1} {2}\n", vector.x * scale.x, vector.y * scale.y, vector.z * scale.z));
			}

			stringBuilder.Append("\n");

			Dictionary<int, int> dictionary = new Dictionary<int, int>();

			if (mesh.subMeshCount > 1)
			{
				int[] triangles = mesh.GetTriangles(1);

				for (int j = 0; j < triangles.Length; j += 3)
				{
					if (!dictionary.ContainsKey(triangles[j]))
						dictionary.Add(triangles[j], 1);

					if (!dictionary.ContainsKey(triangles[j + 1]))
						dictionary.Add(triangles[j + 1], 1);

					if (!dictionary.ContainsKey(triangles[j + 2]))
						dictionary.Add(triangles[j + 2], 1);
				}
			}

			for (int num = 0; num != mesh.uv.Length; num++)
			{
				Vector2 vector2 = Vector2.Scale(mesh.uv[num], textureScale) + textureOffset;

				if (dictionary.ContainsKey(num))
					stringBuilder.Append(string.Format("vt {0} {1}\n", mesh.uv[num].x, mesh.uv[num].y));
				else
					stringBuilder.Append(string.Format("vt {0} {1}\n", vector2.x, vector2.y));
			}

			for (int k = 0; k < mesh.subMeshCount; k++)
			{
				stringBuilder.Append("\n");

				if (k == 0)
					stringBuilder.Append("usemtl ").Append("Material_design").Append("\n");

				if (k == 1)
					stringBuilder.Append("usemtl ").Append("Material_logo").Append("\n");

				int[] triangles2 = mesh.GetTriangles(k);

				for (int l = 0; l < triangles2.Length; l += 3)
					stringBuilder.Append(string.Format("f {0}/{0} {1}/{1} {2}/{2}\n", triangles2[l] + 1, triangles2[l + 2] + 1, triangles2[l + 1] + 1));
			}

			return stringBuilder.ToString();
		}

		public static void SaveMeshAsObjFile(string path, MeshFilter mf, Vector3 scale)
		{
			StreamWriter streamWriter = new StreamWriter(path);
			streamWriter.Write(SaveMeshAsOBJ(mf, new Vector3(-1f, 1f, 1f)));
			streamWriter.Close();
		}
	}
}