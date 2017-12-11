using System.IO;
using System.Reflection;

namespace Cubizer.Protocol.Handshake
{
	[Packet(0x00)]
	public sealed class Handshake : IPacketSerializable
	{
		public uint version;
		public string address;
		public ushort port;
		public NextStateType nextState;

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

		public Handshake(uint version, string address, ushort port, NextStateType next)
		{
			this.version = version;
			this.address = address;
			this.port = port;
			this.nextState = next;
		}

		public void Deserialize(ref BinaryReader br)
		{
			version = br.ReadUInt32();
			address = br.ReadString();
			port = br.ReadUInt16();

			var state = (NextStateType)br.ReadUInt32();
			switch (state)
			{
				case NextStateType.Status:
					nextState = NextStateType.Status;
					break;

				case NextStateType.Login:
					nextState = NextStateType.Login;
					break;

				default:
					nextState = NextStateType.Invalid;
					break;
			}
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarInt(version);
			bw.Write(address);
			bw.Write(port);
			bw.Write((int)nextState);
		}
	}
}