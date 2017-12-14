using UnityEngine;

namespace Cubizer.Models
{
	public interface IVOXCruncherStrategy
	{
		VOXModel CalcVoxelCruncher(VoxData chunk, Color32[] palette);
	}
}