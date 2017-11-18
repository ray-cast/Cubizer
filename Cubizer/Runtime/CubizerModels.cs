using System;

namespace Cubizer
{
	[Serializable]
	public abstract class CubizerModel
	{
		public abstract void Reset();

		public virtual void OnValidate()
		{
		}
	}
}
