using UnityEngine;

namespace Cubizer.Protocol
{
	public class ServerProtocol : IPacketRouter
	{
		public void DispatchIncomingPacket(UncompressedPacket packet)
		{
			Debug.Log("Packet：" + packet.packetId + ".Length:[" + packet.data.Count + "byte]");
		}
	}
}