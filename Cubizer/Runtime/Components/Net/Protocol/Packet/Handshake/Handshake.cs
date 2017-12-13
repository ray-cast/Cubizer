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
			version = br.ReadUInt32();
			address = br.ReadString();
			port = br.ReadUInt16();

			var status = (SessionStatus)br.ReadUInt32();
			switch (status)
			{
				case SessionStatus.Handshaking:
					nextState = SessionStatus.Handshaking;
					break;

				case SessionStatus.Status:
					nextState = SessionStatus.Status;
					break;

				case SessionStatus.Login:
					nextState = SessionStatus.Login;
					break;

				case SessionStatus.Play:
					nextState = SessionStatus.Play;
					break;

				default:
					throw new InvalidDataException($"Invalid session status:{(int)status}");
			}
		}

		void IPacketSerializable.Serialize(NetworkWrite bw)
		{
			bw.WriteVarInt(version);
			bw.Write(address);
			bw.Write(port);
			bw.WriteVarInt((uint)nextState);
		}

		object ICloneable.Clone()
		{
			return new Handshake();
		}
	}
}