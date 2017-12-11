using UnityEngine;

namespace Cubizer.Protocol
{
	public sealed class ClientProtocol : IClientProtocol
	{
		public void DispatchIncomingPacket(UncompressedPacket packet)
		{
			Debug.Log("Packet：" + packet.packetId + ".Length:[" + packet.length + "byte]");
		}
	}
}