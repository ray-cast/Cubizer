using System.Collections.Generic;

using UnityEngine;

namespace Cubizer.Models
{
	using VOXMaterial = System.Int32;

	public sealed class VOXCruncherCulled : IVOXCruncherStrategy
	{
		public static bool GetVisiableFaces(VOXMaterial[,,] map, Vector3Int bound, int x, int y, int z, VOXMaterial material, Color32[] palette, out VOXVisiableFaces faces)
		{
			VOXMaterial[] instanceID = new VOXMaterial[6] { VOXMaterial.MaxValue, VOXMaterial.MaxValue, VOXMaterial.MaxValue, VOXMaterial.MaxValue, VOXMaterial.MaxValue, VOXMaterial.MaxValue };

			if (x >= 1) instanceID[0] = map[(byte)(x - 1), y, z];
			if (y >= 1) instanceID[2] = map[x, (byte)(y - 1), z];
			if (z >= 1) instanceID[4] = map[x, y, (byte)(z - 1)];
			if (x <= bound.x) instanceID[1] = map[(byte)(x + 1), y, z];
			if (y <= bound.y) instanceID[3] = map[x, (byte)(y + 1), z];
			if (z <= bound.z) instanceID[5] = map[x, y, (byte)(z + 1)];

			var alpha = palette[material].a;
			if (alpha < 255)
			{
				bool f1 = (instanceID[0] == VOXMaterial.MaxValue) ? true : palette[instanceID[0]].a != alpha ? true : false;
				bool f2 = (instanceID[1] == VOXMaterial.MaxValue) ? true : palette[instanceID[1]].a != alpha ? true : false;
				bool f3 = (instanceID[2] == VOXMaterial.MaxValue) ? true : palette[instanceID[2]].a != alpha ? true : false;
				bool f4 = (instanceID[3] == VOXMaterial.MaxValue) ? true : palette[instanceID[3]].a != alpha ? true : false;
				bool f5 = (instanceID[4] == VOXMaterial.MaxValue) ? true : palette[instanceID[4]].a != alpha ? true : false;
				bool f6 = (instanceID[5] == VOXMaterial.MaxValue) ? true : palette[instanceID[5]].a != alpha ? true : false;

				faces.left = f1;
				faces.right = f2;
				faces.bottom = f3;
				faces.top = f4;
				faces.front = f5;
				faces.back = f6;
			}
			else
			{
				bool f1 = (instanceID[0] == VOXMaterial.MaxValue) ? true : palette[instanceID[0]].a < 255 ? true : false;
				bool f2 = (instanceID[1] == VOXMaterial.MaxValue) ? true : palette[instanceID[1]].a < 255 ? true : false;
				bool f3 = (instanceID[2] == VOXMaterial.MaxValue) ? true : palette[instanceID[2]].a < 255 ? true : false;
				bool f4 = (instanceID[3] == VOXMaterial.MaxValue) ? true : palette[instanceID[3]].a < 255 ? true : false;
				bool f5 = (instanceID[4] == VOXMaterial.MaxValue) ? true : palette[instanceID[4]].a < 255 ? true : false;
				bool f6 = (instanceID[5] == VOXMaterial.MaxValue) ? true : palette[instanceID[5]].a < 255 ? true : false;

				faces.left = f1;
				faces.right = f2;
				faces.bottom = f3;
				faces.top = f4;
				faces.front = f5;
				faces.back = f6;
			}

			return faces.left | faces.right | faces.bottom | faces.top | faces.front | faces.back;
		}

		public VOXModel CalcVoxelCruncher(VoxData chunk, Color32[] palette)
		{
			var crunchers = new List<VOXCruncher>();
			var bound = new Vector3Int(chunk.x, chunk.y, chunk.z);

			for (int i = 0; i < chunk.x; ++i)
			{
				for (int j = 0; j < chunk.y; ++j)
				{
					for (int k = 0; k < chunk.z; ++k)
					{
						var c = chunk.voxels[i, j, k];
						if (c != int.MaxValue)
						{
							VOXVisiableFaces faces;
							if (!GetVisiableFaces(chunk.voxels, bound, i, j, k, c, palette, out faces))
								continue;

							crunchers.Add(new VOXCruncher((byte)i, (byte)i, (byte)j, (byte)j, (byte)k, (byte)k, faces, c));
						}
					}
				}
			}

			return new VOXModel(crunchers);
		}
	}
}