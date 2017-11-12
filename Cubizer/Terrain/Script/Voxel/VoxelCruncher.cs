namespace Cubizer
{
	public class VoxelCruncher
	{
		public static VoxelModel CalcVoxelCruncher(VoxelData<VoxelMaterial> map, VoxelCruncherMode mode)
		{
			UnityEngine.Debug.Assert(map != null);

			switch (mode)
			{
				case VoxelCruncherMode.Stupid:
					return new VoxelCruncherStupid().CalcVoxelCruncher(map);

				case VoxelCruncherMode.Culled:
					return new VoxelCruncherCulled().CalcVoxelCruncher(map);

				case VoxelCruncherMode.Greedy:
					return new VoxelCruncherGreedy().CalcVoxelCruncher(map);

				default:
					throw new System.Exception("Bad VoxelCruncherMode");
			}
		}
	}
}