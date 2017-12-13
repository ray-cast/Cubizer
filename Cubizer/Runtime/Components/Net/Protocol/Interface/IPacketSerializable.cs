using System;

using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Protocol
{
	public interface IPacketSerializable : ICloneable
	{
		uint packetId
		{
			get;
		}

		void Deserialize(NetworkReader br);
		void Serialize(NetworkWrite bw);
	}
}