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

		public int CalcFaceCountAsAllocate(ref Dictionary<string, int> entities)
		{
			foreach (var it in voxels)
			{
				int facesCount = 0;
				if (it.faces.left) facesCount++;
				if (it.faces.right) facesCount++;
				if (it.faces.top) facesCount++;
				if (it.faces.bottom) facesCount++;
				if (it.faces.front) facesCount++;
				if (it.faces.back) facesCount++;

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