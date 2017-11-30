using UnityEngine;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Cubizer
{
	public class ClientSession : IDisposable
	{
		private bool _isQuitRequest = false;

		private readonly TcpClient _tcpClient;

		public ClientSession(TcpClient client)
		{
			_tcpClient = client;
		}

		~ClientSession()
		{
			_isQuitRequest = true;
			_tcpClient.Close();
		}

		public Task Start()
		{
			return Task.Run(async () =>
			{
				using (var stream = _tcpClient.GetStream())
				{
					byte[] buffer = new byte[8192];

					while (!_isQuitRequest)
					{
						int byteRead = await stream.ReadAsync(buffer, 0, buffer.Length);
						if (byteRead == 0)
						{
							Debug.Log("Disconnect...");
							break;
						}

						this.OnMessage(buffer, byteRead);
					}
				}
			});
		}

		public void Dispose()
		{
			_isQuitRequest = true;
			_tcpClient.Dispose();
		}

		public virtual void OnMessage(byte[] buffer, int length)
		{
			string msg = Encoding.Unicode.GetString(buffer, 0, length);
			Debug.Log("Data：" + msg + ".Length:[" + length + "byte]");
		}
	}
}