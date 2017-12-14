using System.Collections.Generic;

using UnityEngine;

namespace Cubizer.Models
{
	public sealed class VOXCruncherStupid : IVOXCruncherStrategy
	{
		public VOXModel CalcVoxelCruncher(VoxData chunk, Color32[] palette)
		{
			var crunchers = new List<VOXCruncher>();
			var faces = new VOXVisiableFaces(true, true, true, true, true, true);

			for (int i = 0; i < chunk.x; ++i)
			{
				for (int j = 0; j < chunk.y; ++j)
				{
					for (int k = 0; k < chunk.z; ++k)
					{
						var m = chunk.voxels[i, j, k];
						if (m != int.MaxValue)
							crunchers.Add(new VOXCruncher(i, i, j, j, k, k, faces, m));
					}
				}
			}

			return new VOXModel(crunchers);
		}
	}
}