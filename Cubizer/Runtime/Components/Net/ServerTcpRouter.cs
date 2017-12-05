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
	public class ServerTcpRouter : IDisposable
	{
		private readonly int _port;
		private readonly IPAddress _address;
		private readonly TcpListener _listener;
		private readonly List<ClientSession> _sessions;
		private readonly IServerProtocol _protocol;

		private Task _task;
		private Action _onIncomingClient;

		private int _sendTimeout = 0;
		private int _receiveTimeout = 0;

		public int Count
		{
			get
			{
				return _sessions != null ? _sessions.Count : 0;
			}
		}

		public int SendTimeout
		{
			set
			{
				if (_sendTimeout != value)
				{
					foreach (var it in _sessions)
						it.Client.SendTimeout = value;

					_sendTimeout = value;
				}
			}
		}

		public int ReceiveTimeout
		{
			set
			{
				if (_receiveTimeout != value)
				{
					foreach (var it in _sessions)
						it.Client.ReceiveTimeout = value;

					_receiveTimeout = value;
				}
			}
		}

		public ServerTcpRouter(IServerProtocol protocol, string ip, int port)
		{
			Debug.Assert(protocol != null && !string.IsNullOrEmpty(ip));

			_port = port;
			_address = IPAddress.Parse(ip);
			_protocol = protocol;
			_listener = new TcpListener(_address, _port);
			_sessions = new List<ClientSession>();
		}

		~ServerTcpRouter()
		{
			this.Dispose();
		}

		public Task Start(CancellationToken cancellationToken)
		{
			Debug.Assert(_task == null);

			cancellationToken.Register(Dispose);

			_task = Task.Run(async () =>
			{
				try
				{
					Debug.Log("Starting server listener...");

					_listener.Start();

					while (!cancellationToken.IsCancellationRequested)
						this.DispatchIncomingClient(await _listener.AcceptTcpClientAsync(), cancellationToken);

					_listener.Stop();
				}
				finally
				{
					Debug.Log("Stop server listener...");
				}
			});

			return _task;
		}

		public void OnIncomingClient(Action continuation)
		{
			_onIncomingClient = continuation;
		}

		public void Close()
		{
			if (!_task.IsCompleted)
			{
				foreach (var sessions in _sessions)
					sessions.Close();

				_sessions.Clear();

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

				try
				{
					_task.Wait();
				}
				catch (Exception)
				{
				}

				_task = null;
			}
		}

		public void Dispose()
		{
			this.Close();
		}

		private void DispatchIncomingClient(TcpClient tcpClient, CancellationToken cancellationToken)
		{
			Debug.Log($"Incoming connection from {tcpClient.Client.RemoteEndPoint}.");

			if (cancellationToken.IsCancellationRequested)
				return;

			if (tcpClient.Connected)
			{
				if (_onIncomingClient != null)
					_onIncomingClient.Invoke();

				var session = new ClientSession(tcpClient, _protocol);
				session.Client.SendTimeout = _sendTimeout;
				session.Client.ReceiveTimeout = _receiveTimeout;
				session.StartAsync(cancellationToken);

				_sessions.Add(session);
			}
		}

		public void Update()
		{
			foreach (var it in _sessions)
			{
				if (!it.Client.Connected)
					_sessions.Remove(it);
			}
		}
	}
}