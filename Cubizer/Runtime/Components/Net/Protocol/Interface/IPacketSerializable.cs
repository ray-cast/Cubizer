namespace Cubizer.Protocol
{
	public interface IPacketSerializable
	{
		uint packId
		{
			get;
		}

		void Deserialize(NetworkReader br);
		void Serialize(NetworkWrite bw);
	}
}