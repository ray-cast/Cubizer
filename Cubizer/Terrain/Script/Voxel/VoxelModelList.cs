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

		public int CalcFaceCountAsAllocate(ref Dictionary<int, int> entities)
		{
			foreach (var it in voxels)
			{
				int facesCount = it.faces.count;
				if (facesCount > 0)
				{
					var id = it.material.GetInstanceID();
					if (!entities.ContainsKey(id))
						entities.Add(id, facesCount);
					else
						entities[id] += facesCount;
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