using UnityEngine;

namespace Cubizer.Protocol
{
	public class ServerProtocol : IServerProtocol
	{
		public void DispatchIncomingPacket(UncompressedPacket packet)
		{
			Debug.Log("Packet：" + packet.packetId + ".Length:[" + packet.length + "byte]");
		}
	}
}