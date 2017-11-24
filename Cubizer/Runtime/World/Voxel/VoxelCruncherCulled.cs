using System;
using System.Collections.Generic;

using UnityEngine;

namespace Cubizer
{
	public class VoxelCruncherCulled : IVoxelCruncher
	{
		private static VoxelMaterial[] instanceID = new VoxelMaterial[6] { null, null, null, null, null, null };

		public bool GetVisiableFaces(VoxelMaterial[,,] map, Vector3Int bound, int x, int y, int z, VoxelMaterial material, out VoxelVisiableFaces faces)
		{
			for (int i = 0; i < 6; i++)
				instanceID[i] = null;

			if (x >= 1) instanceID[0] = map[(byte)(x - 1), y, z];
			if (y >= 1) instanceID[2] = map[x, (byte)(y - 1), z];
			if (z >= 1) instanceID[4] = map[x, y, (byte)(z - 1)];
			if (x < bound.x - 1) instanceID[1] = map[(byte)(x + 1), y, z];
			if (y < bound.y - 1) instanceID[3] = map[x, (byte)(y + 1), z];
			if (z < bound.z - 1) instanceID[5] = map[x, y, (byte)(z + 1)];

			if (material.is_transparent)
			{
				var name = material.name;

				faces.left = (instanceID[0] == null) ? true : instanceID[0].name != name ? true : false;
				faces.right = (instanceID[1] == null) ? true : instanceID[1].name != name ? true : false;
				faces.bottom = (instanceID[2] == null) ? true : instanceID[2].name != name ? true : false;
				faces.top = (instanceID[3] == null) ? true : instanceID[3].name != name ? true : false;
				faces.front = (instanceID[4] == null) ? true : instanceID[4].name != name ? true : false;
				faces.back = (instanceID[5] == null) ? true : instanceID[5].name != name ? true : false;

				if (material.is_merge)
				{
					if (x == 0) faces.left = false;
					if (z == 0) faces.front = false;
					if (x + 1 == bound.x) faces.right = false;
					if (z + 1 == bound.z) faces.back = false;
				}
			}
			else
			{
				faces.left = (instanceID[0] == null) ? true : instanceID[0].is_transparent ? true : false;
				faces.right = (instanceID[1] == null) ? true : instanceID[1].is_transparent ? true : false;
				faces.bottom = (instanceID[2] == null) ? true : instanceID[2].is_transparent ? true : false;
				faces.top = (instanceID[3] == null) ? true : instanceID[3].is_transparent ? true : false;
				faces.front = (instanceID[4] == null) ? true : instanceID[4].is_transparent ? true : false;
				faces.back = (instanceID[5] == null) ? true : instanceID[5].is_transparent ? true : false;
			}

			if (!material.is_merge)
				faces = new VoxelVisiableFaces(faces.any);

			return faces.any;
		}

		public IVoxelModel CalcVoxelCruncher(VoxelData<VoxelMaterial> voxels)
		{
			var map = new VoxelMaterial[voxels.bound.x, voxels.bound.y, voxels.bound.z];

			for (int i = 0; i < voxels.bound.x; ++i)
			{
				for (int j = 0; j < voxels.bound.y; ++j)
					for (int k = 0; k < voxels.bound.z; ++k)
						map[i, j, k] = null;
			}

			foreach (var it in voxels.GetEnumerator())
				map[it.position.x, it.position.y, it.position.z] = it.value;

			var crunchers = new List<VoxelPrimitive>();
			var bound = new Vector3Int(voxels.bound.x, voxels.bound.y, voxels.bound.z);

			foreach (var it in voxels.GetEnumerator())
			{
				var x = it.position.x;
				var y = it.position.y;
				var z = it.position.z;
				var c = it.value;

				VoxelVisiableFaces faces;
				if (!GetVisiableFaces(map, bound, x, y, z, c, out faces))
					continue;

				crunchers.Add(new VoxelPrimitive(x, x, y, y, z, z, faces, c));
			}

			return new VoxelModelList(crunchers);
		}
	}
}