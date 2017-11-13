using System;
using System.Collections.Generic;

using UnityEngine;

namespace Cubizer
{
	public class VoxelCruncherCulled : IVoxelCruncherStrategy
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

				bool f1 = (instanceID[0] == null) ? true : instanceID[0].name != name ? true : false;
				bool f2 = (instanceID[1] == null) ? true : instanceID[1].name != name ? true : false;
				bool f3 = (instanceID[2] == null) ? true : instanceID[2].name != name ? true : false;
				bool f4 = (instanceID[3] == null) ? true : instanceID[3].name != name ? true : false;
				bool f5 = (instanceID[4] == null) ? true : instanceID[4].name != name ? true : false;
				bool f6 = (instanceID[5] == null) ? true : instanceID[5].name != name ? true : false;

				if (material.is_merge)
				{
					if (x == 0) f1 = false;
					if (z == 0) f5 = false;
					if (x + 1 == bound.x) f2 = false;
					if (z + 1 == bound.z) f6 = false;
				}

				faces.left = f1;
				faces.right = f2;
				faces.bottom = f3;
				faces.top = f4;
				faces.front = f5;
				faces.back = f6;
			}
			else
			{
				bool f1 = (instanceID[0] == null) ? true : instanceID[0].is_transparent ? true : false;
				bool f2 = (instanceID[1] == null) ? true : instanceID[1].is_transparent ? true : false;
				bool f3 = (instanceID[2] == null) ? true : instanceID[2].is_transparent ? true : false;
				bool f4 = (instanceID[3] == null) ? true : instanceID[3].is_transparent ? true : false;
				bool f5 = (instanceID[4] == null) ? true : instanceID[4].is_transparent ? true : false;
				bool f6 = (instanceID[5] == null) ? true : instanceID[5].is_transparent ? true : false;

				faces.left = f1;
				faces.right = f2;
				faces.bottom = f3;
				faces.top = f4;
				faces.front = f5;
				faces.back = f6;
			}

			if (!material.is_merge)
			{
				bool all = faces.left | faces.right | faces.bottom | faces.top | faces.front | faces.back;

				faces.left = all;
				faces.right = all;
				faces.bottom = all;
				faces.top = all;
				faces.front = all;
				faces.back = all;
			}

			return faces.left | faces.right | faces.bottom | faces.top | faces.front | faces.back;
		}

		public VoxelModel CalcVoxelCruncher(VoxelData<VoxelMaterial> voxels)
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

			var array = new VoxelPrimitive[crunchers.Count];

			int numbers = 0;
			foreach (var it in crunchers)
				array[numbers++] = it;

			return new VoxelModel(array);
		}
	}
}