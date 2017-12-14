﻿using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol.Play.Serverbound
{
	[Packet(Packet)]
	public class VehicleMove : IPacketSerializable
	{
		public const int Packet = 0x10;

		public uint packetId
		{
			get
			{
				return Packet;
			}
		}

		public object Clone()
		{
			return new VehicleMove();
		}

		public void Deserialize(NetworkReader br)
		{
			throw new System.NotImplementedException();
		}

		public void Serialize(NetworkWrite bw)
		{
			throw new System.NotImplementedException();
		}
	}
}