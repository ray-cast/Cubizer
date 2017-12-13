using System.Reflection;
using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Serverbound.Handshake
{
	[Packet(0x00)]
	public sealed class Handshake : IPacketSerializable
	{
		public uint version;
		public string address;
		public ushort port;
		public SessionStatus nextState;

		public uint packId
		{
			get
			{
				var typeInfo = this.GetType().GetTypeInfo();
				var attr = typeInfo.GetCustomAttribute<PacketAttribute>();
				return attr.id;
			}
		}

		public Handshake()
		{
		}

		public Handshake(uint version, string address, ushort port, SessionStatus next)
		{
			this.version = version;
			this.address = address;
			this.port = port;
			this.nextState = next;
		}

		public void Deserialize(NetworkReader br)
		{
			version = br.ReadUInt32();
			address = br.ReadString();
			port = br.ReadUInt16();

			var state = (SessionStatus)br.ReadUInt32();
			switch (state)
			{
				case SessionStatus.Status:
					nextState = SessionStatus.Status;
					break;

				case SessionStatus.Login:
					nextState = SessionStatus.Login;
					break;

				default:
					nextState = SessionStatus.Invalid;
					break;
			}
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarInt(version);
			bw.Write(address);
			bw.Write(port);
			bw.WriteVarInt((uint)nextState);
		}
	}
}