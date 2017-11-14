using System.Collections;
using System.Collections.Generic;

namespace Cubizer
{
	public class VoxelModelList : IVoxelModel
	{
		public List<VoxelPrimitive> voxels;

		public VoxelModelList(List<VoxelPrimitive> array)
		{
			voxels = array;
		}

		public int CalcFaceCountAsAllocate(ref Dictionary<string, int> entities)
		{
			foreach (var it in voxels)
			{
				int facesCount = it.faces.count;
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

		public IEnumerable GetEnumerator()
		{
			if (voxels == null)
				throw new System.ApplicationException("GetEnumerator: Empty data");

			return new VoxelModelListEnumerable(voxels);
		}
	}
}