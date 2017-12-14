using System;
using System.IO;

using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Handshake.Serverbound
{
	[Packet(Packet)]
	public sealed class Handshake : IPacketSerializable
	{
		public const int Packet = 0x0;

		public uint version;
		public string address;
		public ushort port;
		public SessionStatus nextState;

		public uint packetId
		{
			get
			{
				return Packet;
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

		void IPacketSerializable.Deserialize(NetworkReader br)
		{
			uint status;

			br.ReadVarInt(out version);
			br.ReadVarString(out address);
			br.Read(out port);
			br.ReadVarInt(out status);

			switch (status)
			{
				case (int)SessionStatus.Handshaking:
					nextState = SessionStatus.Handshaking;
					break;

				case (int)SessionStatus.Status:
					nextState = SessionStatus.Status;
					break;

				case (int)SessionStatus.Login:
					nextState = SessionStatus.Login;
					break;

				case (int)SessionStatus.Play:
					nextState = SessionStatus.Play;
					break;

				default:
					throw new InvalidDataException($"Invalid session status:{(int)status}");
			}
		}

		void IPacketSerializable.Serialize(NetworkWrite bw)
		{
			bw.WriteVarInt(version);
			bw.WriteVarString(address);
			bw.Write(port);
			bw.WriteVarInt((uint)nextState);
		}

		object ICloneable.Clone()
		{
			return new Handshake();
		}
	}
}