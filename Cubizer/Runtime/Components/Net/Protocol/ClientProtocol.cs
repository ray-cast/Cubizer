using UnityEngine;

namespace Cubizer.Protocol
{
	public sealed class ClientProtocol : IPacketRouter
	{
		public void DispatchIncomingPacket(UncompressedPacket packet)
		{
			Debug.Log("Packet：" + packet.packetId + ".Length:[" + packet.data.Count + "byte]");
		}
	}
}