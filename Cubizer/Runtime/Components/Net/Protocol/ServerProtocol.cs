using System.Threading.Tasks;
using UnityEngine;

namespace Cubizer.Protocol
{
	public sealed class ServerProtocol : IPacketRouter
	{
		Task IPacketRouter.DispatchIncomingPacket(UncompressedPacket packet)
		{
			Debug.Log("Packet：" + packet.packetId + ".Length:[" + packet.data.Count + "byte]");
			return Task.CompletedTask;
		}
	}
}