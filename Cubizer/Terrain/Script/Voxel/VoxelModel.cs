using System.Collections;
using System.Collections.Generic;

namespace Cubizer
{
	public class VoxelModel
	{
		public VoxelPrimitive[] voxels;

		public VoxelModel(VoxelPrimitive[] array)
		{
			voxels = array;
		}

		public int CalcFaceCountAsAllocate(ref Dictionary<string, uint> entities)
		{
			foreach (var it in voxels)
			{
				bool[] visiable = new bool[] { it.faces.left, it.faces.right, it.faces.top, it.faces.bottom, it.faces.front, it.faces.back };

				uint facesCount = 0;
				for (int j = 0; j < 6; j++)
				{
					if (visiable[j])
						facesCount++;
				}

				if (facesCount > 0)
				{
					var name = it.material.name;
					if (!entities.ContainsKey(name))
						entities.Add(name, facesCount);
					else
						entities[name] += facesCount;
				}
			}

			return entities.Count;
		}
	}
}