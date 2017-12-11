namespace Cubizer.Protocol
{
	public interface IPacketSerializable
	{
		uint packId
		{
			get;
		}

		void Serialize(NetworkWrite bw);
	}
}