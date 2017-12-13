namespace Cubizer.Net.Protocol.Extensions
{
	internal static class BitConvertExtensions
	{
		public static ushort ToBigEndian(this short value)
		{
			return ((ushort)value).ToBigEndian();
		}

		public static ushort ToBigEndian(this ushort value)
		{
			return (ushort)((value >> 8) | (((byte)value) << 8));
		}

		public static uint ToBigEndian(this int value)
		{
			return ((uint)value).ToBigEndian();
		}

		public static uint ToBigEndian(this uint value)
		{
			return
				((value) >> 24) |
				((value & 0x00FF0000) >> 8) |
				((value & 0x0000FF00) << 8) |
				((value & 0x000000FF) << 24);
		}

		public static ulong ToBigEndian(this long value)
		{
			return ((ulong)value).ToBigEndian();
		}

		public static ulong ToBigEndian(this ulong value)
		{
			return
				((value) >> 56) |
				((value & 0x00FF000000000000) >> 40) |
				((value & 0x0000FF0000000000) >> 24) |
				((value & 0x000000FF00000000) >> 08) |
				((value & 0x00000000FF000000) << 08) |
				((value & 0x0000000000FF0000) << 24) |
				((value & 0x000000000000FF00) << 40) |
				((value & 0x00000000000000FF) << 56);
		}
	}
}