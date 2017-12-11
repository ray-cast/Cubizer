using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using UnityEngine;
using Cubizer.Protocol;

namespace Cubizer
{
	public sealed class ServerTcpRouter : IDisposable
	{
		private readonly int _port;
		private readonly IPAddress _address;
		private readonly TcpListener _listener;
		private readonly IServerProtocol _protocol;
		private readonly List<ServerSession> _sessions = new List<ServerSession>();
		private readonly ServerTcpDelegates _events = new ServerTcpDelegates();

		private Task _task;

		private int _sendTimeout = 0;
		private int _receiveTimeout = 0;

		public int count
		{
			get
			{
				return _sessions.Count;
			}
		}

		public int sendTimeout
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

		public int receiveTimeout
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

		public ServerTcpDelegates events
		{
			get
			{
				return _events;
			}
		}

		public ServerTcpRouter(string ip, int port, IServerProtocol protocol)
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
			if (cancellationToken.IsCancellationRequested)
				return;

			if (tcpClient.Connected)
			{
				if (_events.onIncomingClient != null)
					_events.onIncomingClient.Invoke(tcpClient);

				var session = new ServerSession(tcpClient, _protocol);
				session.client.SendTimeout = _sendTimeout;
				session.client.ReceiveTimeout = _receiveTimeout;
				session.OnCompletion(OnCompletionSession);
				session.StartAsync(cancellationToken);

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