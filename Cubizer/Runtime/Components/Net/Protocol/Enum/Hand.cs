using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol
{
	public enum Hand : uint
	{
		Main = 0,
		Off = 1
	}

	namespace Extensions
	{
		public static class HandExtensions
		{
			public static void Read(this NetworkReader br, out Hand hand)
			{
				uint value;
				br.ReadVarInt(out value);

				switch (value)
				{
					case (int)Hand.Main:
						hand = Hand.Main;
						break;

					case (int)Hand.Off:
						hand = Hand.Off;
						break;

					default:
						throw new System.IO.InvalidDataException("Invalid Hand Enum");
				}
			}

			public static void Write(this NetworkWrite bw, Hand hand)
			{
				bw.WriteVarInt((uint)hand);
			}
		}
	}
}