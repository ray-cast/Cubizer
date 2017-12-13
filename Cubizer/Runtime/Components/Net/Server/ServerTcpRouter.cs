using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using UnityEngine;
using Cubizer.Protocol;

namespace Cubizer.Server
{
	public sealed class ServerTcpRouter : IDisposable
	{
		private readonly int _port;
		private readonly IPAddress _address;
		private readonly TcpListener _listener;
		private readonly IPacketRouter _protocol;
		private readonly List<ServerSession> _sessions = new List<ServerSession>();
		private readonly ServerTcpDelegates _events = new ServerTcpDelegates();

		private Task _task;

		private int _sessionSendTimeout = 0;
		private int _sessionReceiveTimeout = 0;

		public int sessionsCount
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
				if (_sessionSendTimeout != value)
				{
					foreach (var it in _sessions)
						it.client.SendTimeout = value;

					_sessionSendTimeout = value;
				}
			}
		}

		public int sessionsReceiveTimeout
		{
			set
			{
				if (_sessionReceiveTimeout != value)
				{
					foreach (var it in _sessions)
						it.client.ReceiveTimeout = value;

					_sessionReceiveTimeout = value;
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

		public ServerTcpDelegates events
		{
			get
			{
				return _events;
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
					if (_events.onStartTcpListener != null)
						_events.onStartTcpListener.Invoke();

					_listener.Start();

					while (!cancellationToken.IsCancellationRequested)
						DispatchIncomingClient(await _listener.AcceptTcpClientAsync(), cancellationToken);
				}
				finally
				{
					if (_events.onStopTcpListener != null)
						_events.onStopTcpListener.Invoke();

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
				if (_events.onIncomingClient != null)
					_events.onIncomingClient.Invoke(tcpClient);

				var session = new ServerSession(tcpClient, _protocol);
				session.client.SendTimeout = _sessionSendTimeout;
				session.client.ReceiveTimeout = _sessionReceiveTimeout;
				session.OnCompletion(OnCompletionSession);
				session.Start(cancellationToken);

				if (_events.onIncomingClientSession != null)
					_events.onIncomingClientSession.Invoke(session);

				lock (_sessions) _sessions.Add(session);
			}
		}

		private void OnCompletionSession(ServerSession clientSession)
		{
			if (_events.onOutcomingClientSession != null)
				_events.onOutcomingClientSession.Invoke(clientSession);

			lock (_sessions) _sessions.Remove(clientSession);
		}
	}
}