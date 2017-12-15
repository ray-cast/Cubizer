using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Serverbound
{
	[Packet(Packet)]
	public class TabComplete : IPacketSerializable
	{
		public const int Packet = 0x02;

		public string text;
		public bool assumeCommand;
		public bool hasPosition;
		public byte[] lookedAtBlock;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new TabComplete();
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out text);
			br.Read(out assumeCommand);
			br.Read(out hasPosition);

			if (hasPosition) br.Read(out lookedAtBlock, 8);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarString(text);
			bw.Write(assumeCommand);
			bw.Write(hasPosition);

			if (hasPosition) bw.Write(lookedAtBlock);
		}
	}
}