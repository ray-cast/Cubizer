using System.Collections.Generic;

namespace Cubizer
{
	public class VoxelCruncherStupid : IVoxelCruncher
	{
		private readonly static VoxelVisiableFaces faces = new VoxelVisiableFaces();

		public IVoxelModel CalcVoxelCruncher(VoxelData<VoxelMaterial> map)
		{
			var crunchers = new List<VoxelPrimitive>(map.Count);

			foreach (var it in map.GetEnumerator())
			{
				var x = it.position.x;
				var y = it.position.y;
				var z = it.position.z;
				var c = it.value;

				crunchers.Add(new VoxelPrimitive(x, x, z, z, y, y, faces, c));
			}

			return new VoxelModelList(crunchers);
		}
	}
}