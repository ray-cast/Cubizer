using System;

namespace Cubizer.Net.Protocol
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
	public sealed class PacketAttribute : Attribute
	{
		public uint id { get; }

		public PacketAttribute(uint id)
		{
			this.id = id;
		}
	}
}