using System.Collections.Generic;

namespace Cubizer.Models
{
	public sealed class VOXModel
	{
		public List<VOXCruncher> voxels;

		public VOXModel(List<VOXCruncher> array)
		{
			voxels = array;
		}
	}
}