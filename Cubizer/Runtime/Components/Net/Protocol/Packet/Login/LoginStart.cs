﻿using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Login.Serverbound
{
	[Packet(Packet)]
	public sealed class LoginStart : IPacketSerializable
	{
		public const int Packet = 0x00;

		public string name;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public LoginStart()
		{
		}

		public LoginStart(string name)
		{
			this.name = name;
		}

		public void Deserialize(NetworkReader br)
		{
			br.ReadVarString(out name);
		}

		public void Serialize(NetworkWrite bw)
		{
			bw.WriteVarString(name);
		}

		public object Clone()
		{
			return new LoginStart();
		}
	}
}