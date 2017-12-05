using UnityEngine;

using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Cubizer
{
	public class ClientSession : IDisposable
	{
		private readonly TcpClient _tcpClient;
		private readonly IServerProtocol _tcpProtocol;
		private readonly byte[] buffer = new byte[8192];

		private Task _task;

		public TcpClient Client
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

		public Task StartAsync(CancellationToken cancellationToken)
		{
			if (!_tcpClient.Connected)
				return null;

			_task = Task.Run(() =>
			{
				try
				{
					using (var stream = _tcpClient.GetStream())
					{
						while (!cancellationToken.IsCancellationRequested)
							DispatchIncomingPacket(stream);
					}
				}
				catch (Exception e)
				{
					_tcpClient.Close();
					throw e;
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

		private void DispatchIncomingPacket(NetworkStream stream)
		{
			int count = stream.Read(buffer, 0, buffer.Length);
			if (count > 0)
				DispatchIncomingPacket(stream, buffer, count);
		}

		private void DispatchIncomingPacket(NetworkStream stream, byte[] buffer, int length)
		{
			_tcpProtocol.DispatchIncomingPacket(stream, buffer, length);
		}
	}
}