using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class UpdateHealth : IPacketSerializable
	{
		public const int Packet = 0x40;

		public float health;
		public uint food;
		public float foodSaturation;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new UpdateHealth();
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out health);
			br.ReadVarInt(out food);
			br.Read(out foodSaturation);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(health);
			bw.WriteVarInt(food);
			bw.Write(foodSaturation);
		}
	}
}