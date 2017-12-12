namespace Cubizer.Protocol
{
	internal static class SizeofExtensions
	{
		public static byte Sizeof(this sbyte value) => sizeof(sbyte);
		public static byte Sizeof(this short value) => sizeof(short);
		public static byte Sizeof(this int value) => sizeof(int);
		public static byte Sizeof(this long value) => sizeof(long);
		public static byte Sizeof(this byte value) => sizeof(byte);
		public static byte Sizeof(this ushort value) => sizeof(ushort);
		public static byte Sizeof(this uint value) => sizeof(uint);
		public static byte Sizeof(this ulong value) => sizeof(ulong);
		public static byte Sizeof(this decimal value) => sizeof(decimal);
		public static byte Sizeof(this float value) => sizeof(float);
		public static byte Sizeof(this double value) => sizeof(double);

		public static byte SizeofBytes(this short value)
		{
			byte cout = 0;
			do { value >>= 7; cout++; } while (value != 0);
			return cout;
		}

		public static byte SizeofBytes(this int value)
		{
			byte cout = 0;
			do { value >>= 7; cout++; } while (value != 0);
			return cout;
		}

		public static byte SizeofBytes(this long value)
		{
			byte cout = 0;
			do { value >>= 7; cout++; } while (value != 0);
			return cout;
		}

		public static byte SizeofBytes(this ushort value)
		{
			byte cout = 0;
			do { value >>= 7; cout++; } while (value != 0);
			return cout;
		}

		public static byte SizeofBytes(this uint value)
		{
			byte cout = 0;
			do { value >>= 7; cout++; } while (value != 0);
			return cout;
		}

		public static byte SizeofBytes(this ulong value)
		{
			byte cout = 0;
			do { value >>= 7; cout++; } while (value != 0);
			return cout;
		}
	}
}