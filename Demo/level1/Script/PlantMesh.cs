using UnityEngine;
using System.Collections;

using Cubizer;

public class PlantMesh : TerrainEntityBehaviour
{
	public static Vector3[,] _positions = new Vector3[4, 4]
	{
			{ new Vector3( 0, -1, -1), new Vector3( 0, -1, +1), new Vector3( 0, +1, -1), new Vector3( 0, +1, +1)},
			{ new Vector3( 0, -1, -1), new Vector3( 0, -1, +1), new Vector3( 0, +1, -1), new Vector3( 0, +1, +1)},
			{ new Vector3(-1, -1,  0), new Vector3(-1, +1,  0), new Vector3(+1, -1,  0), new Vector3(+1, +1,  0)},
			{ new Vector3(-1, -1,  0), new Vector3(-1, +1,  0), new Vector3(+1, -1,  0), new Vector3(+1, +1,  0)}
	};

	public static Vector3[] _normals = new Vector3[4]
	{
			new Vector3(-1, 0, 0),
			new Vector3(+1, 0, 0),
			new Vector3(0, 0, -1),
			new Vector3(0, 0, +1)
	};

	public static Vector2[,] _uvs = new Vector2[4, 4]
	{
			{ new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)},
			{ new Vector2(1, 0), new Vector2(0, 0), new Vector2(1, 1), new Vector2(0, 1)},
			{ new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1)},
			{ new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 0), new Vector2(0, 1)}
	};

	public static int[,] _indices = new int[4, 6]
	{
			{0, 3, 2, 0, 1, 3},
			{0, 3, 1, 0, 2, 3},
			{0, 3, 2, 0, 1, 3},
			{0, 3, 1, 0, 2, 3}
	};

	public override uint GetVerticesCount(uint faceCount)
	{
		return faceCount * 4 * 4;
	}

	public override uint GetIndicesCount(uint faceCount)
	{
		return faceCount * 6 * 4;
	}

	public static void CreatePlantMesh(ref Vector3[] vertices, ref Vector3[] normals, ref Vector2[] uv, ref int[] triangles, ref int index, Vector3 translate, Vector3 scale)
	{
		for (int i = 0; i < 4; i++)
		{
			for (int n = index * 4, k = 0; k < 4; k++, n++)
			{
				Vector3 v = _positions[i, k] * 0.5f;
				v.x *= scale.x;
				v.y *= scale.y;
				v.z *= scale.z;
				v.x += translate.x;
				v.y += translate.y;
				v.z += translate.z;

				vertices[n] = v;
				normals[n] = _normals[i];
				uv[n] = _uvs[i, k];
			}

			for (int j = index * 6, k = 0; k < 6; k++, j++)
				triangles[j] = index * 4 + _indices[i, k];

			index++;
		}
	}

	public override void OnCreateBlock(ref TerrainMesh mesh, ref int index, Vector3 pos, Vector3 scale, VoxelVisiableFaces faces)
	{
		CreatePlantMesh(ref mesh.vertices, ref mesh.normals, ref mesh.uv, ref mesh.triangles, ref index, pos, scale);
	}
}