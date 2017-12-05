using System;
using System.Net.Sockets;
using System.Text;

using UnityEngine;

namespace Cubizer
{
	public class ClientProtocolDefault : IClientProtocol
	{
		public bool ConnectRequire(NetworkStream stream)
		{
			Byte[] sendBytes = Encoding.ASCII.GetBytes("Connect require");
			stream.Write(sendBytes, 0, sendBytes.Length);
			return true;
		}

		public bool DispatchIncomingPacket(byte[] buffer, int length)
		{
			string msg = Encoding.ASCII.GetString(buffer, 0, length);
			Debug.Log("Data：" + msg + ".Length:[" + length + "byte]");
			return true;
		}
	}
}