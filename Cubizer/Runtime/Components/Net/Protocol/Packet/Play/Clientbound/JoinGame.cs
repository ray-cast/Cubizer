using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Clientbound
{
	[Packet(Packet)]
	public class JoinGame : IPacketSerializable
	{
		public const int Packet = 0x23;

		public int entityID;
		public byte gameMode;
		public int dimension;
		public byte difficulty;
		public byte maxPlayers;
		public string levelType;
		public bool reducedDebugInfo;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new JoinGame();
		}

		public void Deserialize(NetworkReader br)
		{
			br.Read(out entityID);
			br.Read(out gameMode);
			br.Read(out dimension);
			br.Read(out difficulty);
			br.Read(out maxPlayers);
			br.Read(out levelType);
			br.Read(out reducedDebugInfo);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.Write(entityID);
			bw.Write(gameMode);
			bw.Write(dimension);
			bw.Write(difficulty);
			bw.Write(maxPlayers);
			bw.Write(levelType);
			bw.Write(reducedDebugInfo);
		}
	}
}