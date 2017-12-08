using UnityEngine;

using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Cubizer
{
	public sealed class ClientSession : IDisposable
	{
		private readonly TcpClient _tcpClient;
		private readonly IServerProtocol _tcpProtocol;
		private readonly byte[] buffer = new byte[8192];

		private Task _task;
		private ServerTcpDelegates.OnOutcomingClientSession _onCompletion;

		public TcpClient client
		{
			get
			{
				return _tcpClient;
			}
		}

		public ClientSession(TcpClient client, IServerProtocol protocol)
		{
			Debug.Assert(client != null && protocol != null);
			_tcpClient = client;
			_tcpProtocol = protocol;
		}

		~ClientSession()
		{
			this.Close();
		}

		public void Start(CancellationToken cancellationToken)
		{
			if (!_tcpClient.Connected)
				return;

			using (var stream = _tcpClient.GetStream())
			{
				try
				{
					while (!cancellationToken.IsCancellationRequested)
						DispatchIncomingPacket(stream);
				}
				finally
				{
					if (_onCompletion != null)
						_onCompletion.Invoke(this);
				}
			}
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			if (!_tcpClient.Connected)
				return null;

			_task = Task.Run(() =>
			{
				using (var stream = _tcpClient.GetStream())
				{
					try
					{
						while (!cancellationToken.IsCancellationRequested)
							DispatchIncomingPacket(stream);
					}
					finally
					{
						if (_onCompletion != null)
							_onCompletion.Invoke(this);
					}
				}
			});

			return _task;
		}

		public void Close()
		{
			try
			{
				if (_task != null)
					_task.Wait();
			}
			catch (Exception)
			{
			}
			finally
			{
				_task = null;
				_tcpClient.Dispose();
			}
		}

		public void Dispose()
		{
			this.Close();
		}

		public void OnCompletion(ServerTcpDelegates.OnOutcomingClientSession continuation)
		{
			_onCompletion = continuation;
		}

		private void DispatchIncomingPacket(NetworkStream stream)
		{
			int count = stream.Read(buffer, 0, buffer.Length);
			if (count > 0)
				DispatchIncomingPacket(stream, buffer, count);
			else
				throw new EndOfStreamException();
		}

		private void DispatchIncomingPacket(NetworkStream stream, byte[] buffer, int length)
		{
			_tcpProtocol.DispatchIncomingPacket(stream, buffer, length);
		}
	}
}