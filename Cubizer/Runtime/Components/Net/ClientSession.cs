using UnityEngine;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cubizer
{
	public class ClientSession : IDisposable
	{
		private bool _isQuitRequest = false;

		private readonly byte[] buffer = new byte[8192];
		private readonly TcpClient _tcpClient;

		public ClientSession(TcpClient client)
		{
			_tcpClient = client;
		}

		~ClientSession()
		{
			_tcpClient.Close();
		}

		public Task Start(CancellationToken cancellationToken)
		{
			return Task.Run(async () =>
			{
				using (var stream = _tcpClient.GetStream())
				{
					try
					{
						do
						{
							await DispatchIncomingPacket(stream);
						}
						while (!cancellationToken.IsCancellationRequested);
					}
					catch (Exception e)
					{
						Debug.Log(e.Message);
					}
				}
			});
		}

		private async Task DispatchIncomingPacket(NetworkStream stream)
		{
			int byteRead = await stream.ReadAsync(buffer, 0, buffer.Length);
			if (byteRead > 0)
				DispatchIncomingPacket(buffer, byteRead);
		}

		private void DispatchIncomingPacket(byte[] buffer, int length)
		{
			string msg = Encoding.ASCII.GetString(buffer, 0, length);
			Debug.Log("Data：" + msg + ".Length:[" + length + "byte]");
		}

		public void Dispose()
		{
			_tcpClient.Dispose();
		}
	}
}