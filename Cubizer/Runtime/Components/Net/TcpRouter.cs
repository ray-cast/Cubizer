using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Cubizer
{
	public class TcpRouter : IDisposable
	{
		private readonly int _port;
		private readonly IPAddress _address;
		private readonly TcpListener _listener;
		private readonly List<ClientSession> _sessions;

		private Task _task;
		private bool _isQuitRequest = false;

		public TcpRouter(string ip, int port)
		{
			_address = IPAddress.Parse(ip);
			_port = port;
			_listener = new TcpListener(_address, _port);
			_sessions = new List<ClientSession>();
		}

		~TcpRouter()
		{
			this.Dispose();
		}

		private void DispatchIncomingClient(TcpClient tcpClient)
		{
			try
			{
				var session = new ClientSession(tcpClient);
				session.Start();

				_sessions.Add(session);
			}
			catch (Exception e)
			{
				Debug.Log("This thread has a except：" + e.Message);
				throw e;
			}
		}

		public void Start()
		{
			_task = Task.Run(async () =>
			{
				try
				{
					Debug.Log("Starting Listener...");

					_listener.Start();

					while (!_isQuitRequest)
					{
						var tcpClient = await _listener.AcceptTcpClientAsync();
						if (!_isQuitRequest)
							this.DispatchIncomingClient(tcpClient);
					}

					_listener.Stop();

					Debug.Log("Stop Listener...");
				}
				catch (Exception e)
				{
					Debug.Log("This thread has a except：" + e.Message);
					throw e;
				}
			});
		}

		public void Dispose()
		{
			if (!_task.IsCompleted)
			{
				_isQuitRequest = true;

				foreach (var sessions in _sessions)
					sessions.Dispose();

				using (TcpClient tcpClient = new TcpClient())
				{
					tcpClient.Connect(_address, _port);

					using (var stream = tcpClient.GetStream())
					{
						Byte[] sendBytes = Encoding.ASCII.GetBytes("exit");
						stream.Write(sendBytes, 0, sendBytes.Length);
					}
				}

				_task.Wait();
			}
		}
	}
}