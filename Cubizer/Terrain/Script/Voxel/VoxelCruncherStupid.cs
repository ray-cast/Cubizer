using System.Collections.Generic;

namespace Cubizer
{
	public class VoxelCruncherStupid : IVoxelCruncherStrategy
	{
		private static VoxelVisiableFaces faces = new VoxelVisiableFaces();

		public VoxelModel CalcVoxelCruncher(VoxelData<VoxelMaterial> map)
		{
			var crunchers = new List<VoxelPrimitive>(map.count);

			foreach (var it in map.GetEnumerator())
			{
				var x = it.position.x;
				var y = it.position.y;
				var z = it.position.z;
				var c = it.value;

				crunchers.Add(new VoxelPrimitive(x, x, z, z, y, y, faces, c));
			}

			return new VoxelModel(crunchers);
		}
	}
}