using System;
using System.Net.Sockets;
using System.Text;

using UnityEngine;

namespace Cubizer
{
	public class ServerProtocolDefault : IServerProtocol
	{
		public bool DispatchIncomingPacket(NetworkStream stream, byte[] buffer, int length)
		{
			string msg = Encoding.ASCII.GetString(buffer, 0, length);
			Debug.Log("Data：" + msg + ".Length:[" + length + "byte]");
			stream.Write(buffer, 0, length);
			return true;
		}
	}
}