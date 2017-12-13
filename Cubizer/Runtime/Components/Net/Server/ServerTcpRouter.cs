using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using UnityEngine;

using Cubizer.Net.Protocol;

namespace Cubizer.Net.Server
{
	public sealed class ServerTcpRouter : IDisposable
	{
		private readonly int _port;
		private readonly IPAddress _address;
		private readonly TcpListener _listener;
		private readonly IPacketRouter _protocol;
		private readonly List<ServerSession> _sessions = new List<ServerSession>();

		private Task _task;

		private int _sendTimeout = 0;
		private int _receiveTimeout = 0;

		public OnStartTcpListener onStartTcpListener { get; set; }
		public OnStopTcpListener onStopTcpListener { get; set; }

		public OnIncomingClient onIncomingClient { get; set; }
		public OnIncomingClientSession onIncomingClientSession { get; set; }

		public OnOutcomingClientSession onOutcomingClientSession { get; set; }

		public int count
		{
			get
			{
				return _sessions.Count;
			}
		}

		public int sessionsSendTimeout
		{
			set
			{
				if (_sendTimeout != value)
				{
					foreach (var it in _sessions)
						it.client.SendTimeout = value;

					_sendTimeout = value;
				}
			}
		}

		public int sessionsReceiveTimeout
		{
			set
			{
				if (_receiveTimeout != value)
				{
					foreach (var it in _sessions)
						it.client.ReceiveTimeout = value;

					_receiveTimeout = value;
				}
			}
		}

		public TcpListener listener
		{
			get
			{
				return _listener;
			}
		}

		public ServerTcpRouter(string ip, int port, IPacketRouter protocol)
		{
			Debug.Assert(protocol != null && !string.IsNullOrEmpty(ip));

			_port = port;
			_address = IPAddress.Parse(ip);
			_protocol = protocol;
			_listener = new TcpListener(_address, _port);
		}

		~ServerTcpRouter()
		{
			this.Dispose();
		}

		public Task Start(CancellationToken cancellationToken)
		{
			Debug.Assert(_task == null);

			_task = Task.Run(async () =>
			{
				try
				{
					if (onStartTcpListener != null)
						onStartTcpListener();

					_listener.Start();

					while (!cancellationToken.IsCancellationRequested)
						DispatchIncomingClient(await _listener.AcceptTcpClientAsync(), cancellationToken);
				}
				finally
				{
					if (onStopTcpListener != null)
						onStopTcpListener();

					_listener.Stop();
				}
			});

			return _task;
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
							using (var bw = new BinaryWriter(stream))
							{
								bw.Write(0);
							}
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
			if (cancellationToken.IsCancellationRequested)
				return;

			if (tcpClient.Connected)
			{
				if (onIncomingClient != null)
					onIncomingClient(tcpClient);

				var session = new ServerSession(tcpClient, _protocol);
				session.client.SendTimeout = _sendTimeout;
				session.client.ReceiveTimeout = _receiveTimeout;
				session.OnCompletion(OnCompletionSession);
				session.Start(cancellationToken);

				if (onIncomingClientSession != null)
					onIncomingClientSession(session);

				lock (_sessions) _sessions.Add(session);
			}
		}

		private void OnCompletionSession(ServerSession clientSession)
		{
			if (onOutcomingClientSession != null)
				onOutcomingClientSession(clientSession);

			lock (_sessions) _sessions.Remove(clientSession);
		}
	}
}