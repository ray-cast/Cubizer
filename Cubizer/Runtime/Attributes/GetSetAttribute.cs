using System;
using UnityEngine;

namespace Cubizer
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	public sealed class GetSetAttribute : PropertyAttribute
	{
		public readonly string name;
		public bool dirty;

		public GetSetAttribute(string name)
		{
			this.name = name;
		}
	}
}