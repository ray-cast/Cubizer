namespace Cubizer
{
	public class VoxelCruncherStupid : IVoxelCruncherStrategy
	{
		public VoxelModel CalcVoxelCruncher(VoxelData<VoxelMaterial> map)
		{
			var crunchers = new VoxelPrimitive[map.count];

			var n = 0;
			var faces = new VoxelVisiableFaces(true, true, true, true, true, true);

			foreach (var it in map.GetEnumerator())
			{
				var x = it.position.x;
				var y = it.position.y;
				var z = it.position.z;
				var c = it.value;

				crunchers[n++] = new VoxelPrimitive(x, x, z, z, y, y, faces, c);
			}

			return new VoxelModel(crunchers);
		}
	}
}