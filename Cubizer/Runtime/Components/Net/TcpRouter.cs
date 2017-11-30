using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

		private void DispatchIncomingClient(TcpClient tcpClient, CancellationToken cancellationToken)
		{
			try
			{
				Debug.Log($"Incoming connection from {tcpClient.Client.RemoteEndPoint}.");

				var session = new ClientSession(tcpClient);
				session.Start(cancellationToken);

				_sessions.Add(session);
			}
			catch (Exception e)
			{
				Debug.Log("This thread has a except：" + e.Message);
				throw e;
			}
		}

		public Task Start(CancellationToken cancellationToken)
		{
			Debug.Assert(_task == null);

			_task = Task.Run(async () =>
			{
				try
				{
					Debug.Log("Starting Listener...");

					cancellationToken.Register(Dispose);

					_listener.Start();

					while (!cancellationToken.IsCancellationRequested)
						this.DispatchIncomingClient(await _listener.AcceptTcpClientAsync(), cancellationToken);

					_listener.Stop();

					Debug.Log("Stop Listener...");
				}
				catch (Exception e)
				{
					Debug.Log("This thread has a except：" + e.Message);
					throw e;
				}
			});

			return _task;
		}

		public void Dispose()
		{
			if (!_task.IsCompleted)
			{
				foreach (var sessions in _sessions)
					sessions.Dispose();

				if (_listener.Server.IsBound)
				{
					using (TcpClient tcpClient = new TcpClient())
					{
						tcpClient.Connect(_address, _port);

						using (var stream = tcpClient.GetStream())
						{
							Byte[] sendBytes = Encoding.ASCII.GetBytes("Exit");
							stream.Write(sendBytes, 0, sendBytes.Length);
						}
					}
				}

				_task.Wait();
				_task = null;
			}
		}
	}
}