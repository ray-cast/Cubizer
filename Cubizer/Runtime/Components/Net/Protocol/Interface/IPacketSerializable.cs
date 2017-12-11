namespace Cubizer.Protocol
{
	public interface ISerializablePacket
	{
		uint packId
		{
			get;
		}

		void Serialize(NetworkWrite bw);
	}
}