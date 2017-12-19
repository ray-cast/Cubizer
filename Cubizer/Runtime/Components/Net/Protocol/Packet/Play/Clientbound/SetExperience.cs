using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class SetExperience : IPacketSerializable
	{
		public const int Packet = 0x3F;

		public float experienceBar;
		public uint level;
		public uint totalExperience;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new SetExperience();
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out experienceBar);
			br.ReadVarInt(out level);
			br.ReadVarInt(out totalExperience);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(experienceBar);
			bw.WriteVarInt(level);
			bw.WriteVarInt(totalExperience);
		}
	}
}