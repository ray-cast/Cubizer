using System;
using System.Net.Sockets;
using System.Text;

using UnityEngine;

namespace Cubizer.Protocol
{
	public sealed class ClientProtocolDefault : IClientProtocol
	{
		public bool ConnectRequire(NetworkStream stream)
		{
			Byte[] sendBytes = Encoding.ASCII.GetBytes("Connect require");
			stream.Write(sendBytes, 0, sendBytes.Length);
			return true;
		}

		public bool DispatchIncomingPacket(UncompressedPacket packet)
		{
			Debug.Log("Packet：" + packet.packetId + ".Length:[" + packet.length + "byte]");
			return true;
		}
	}
}