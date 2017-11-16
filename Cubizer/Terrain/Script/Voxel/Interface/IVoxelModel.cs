using System.Collections;
using System.Collections.Generic;

namespace Cubizer
{
	public interface IVoxelModel
	{
		int CalcFaceCountAsAllocate(ref Dictionary<int, int> entities);

		IEnumerable GetEnumerator();

		IEnumerable GetEnumerator(int instanceID);
	}
}