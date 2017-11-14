using System.Collections.Generic;

namespace Cubizer
{
	public class VoxelModel
	{
		public List<VoxelPrimitive> voxels;

		public VoxelModel(List<VoxelPrimitive> array)
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

		public static VoxelModel CreateVoxelModel(VoxelData<VoxelMaterial> map, VoxelCullMode mode)
		{
			UnityEngine.Debug.Assert(map != null);

			switch (mode)
			{
				case VoxelCullMode.Stupid:
					return new VoxelCruncherStupid().CalcVoxelCruncher(map);

				case VoxelCullMode.Culled:
					return new VoxelCruncherCulled().CalcVoxelCruncher(map);

				case VoxelCullMode.Greedy:
					return new VoxelCruncherGreedy().CalcVoxelCruncher(map);

				default:
					throw new System.Exception("Bad VoxelCullMode");
			}
		}
	}
}