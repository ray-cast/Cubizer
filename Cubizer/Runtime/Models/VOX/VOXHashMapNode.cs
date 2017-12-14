using System;

namespace Cubizer.Models
{
	using VOXMaterial = System.Int32;

	[Serializable]
	public sealed class VOXHashMapNode<_Tx>
		where _Tx : struct
	{
		public _Tx x;
		public _Tx y;
		public _Tx z;
		public VOXMaterial element;

		public VOXHashMapNode()
		{
			element = int.MaxValue;
		}

		public VOXHashMapNode(_Tx xx, _Tx yy, _Tx zz, VOXMaterial value)
		{
			x = xx;
			y = yy;
			z = zz;
			element = value;
		}

		public bool is_empty()
		{
			return element == int.MaxValue;
		}
	}
}