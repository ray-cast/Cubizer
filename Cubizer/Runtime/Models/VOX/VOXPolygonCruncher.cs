using UnityEngine;

namespace Cubizer.Models
{
	public sealed class VOXPolygonCruncher
	{
		public static VOXModel CalcVoxelCruncher(VoxData chunk, Color32[] palette, VOXCruncherMode mode)
		{
			switch (mode)
			{
				case VOXCruncherMode.Stupid:
					return new VOXCruncherStupid().CalcVoxelCruncher(chunk, palette);

				case VOXCruncherMode.Culled:
					return new VOXCruncherCulled().CalcVoxelCruncher(chunk, palette);

				case VOXCruncherMode.Greedy:
					return new VOXCruncherGreedy().CalcVoxelCruncher(chunk, palette);

				default:
					return null;
			}
		}
	}
}