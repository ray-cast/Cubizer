using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;

namespace Cubizer
{
	public sealed class Client : IDisposable
	{
		private readonly int _port;
		private readonly IPAddress _address;
		private readonly IClientProtocol _tcpProtocol;
		private readonly byte[] buffer = new byte[8192];

		private Task _tcpTask;
		private TcpClient _tcpClient;

		private int _sendTimeout = 0;
		private int _receiveTimeout = 0;

		public ClientDelegates _events = new ClientDelegates();

		public int sendTimeout
		{
			set
			{
				if (_sendTimeout != value)
				{
					if (_tcpClient != null)
						_tcpClient.SendTimeout = value;

					_sendTimeout = value;
				}
			}
			get
			{
				return _sendTimeout;
			}
		}

		public int receiveTimeout
		{
			set
			{
				if (_receiveTimeout != value)
				{
					if (_tcpClient != null)
						_tcpClient.ReceiveTimeout = value;

					_receiveTimeout = value;
				}
			}
			get
			{
				return _receiveTimeout;
			}
		}

		public bool connected
		{
			get
			{
				return _tcpClient != null ? _tcpClient.Connected : false;
			}
		}

		public ClientDelegates events
		{
			get
			{
				return _events;
			}
		}

		public Client(IClientProtocol protocal, string ip, int port)
		{
			Debug.Assert(protocal != null);

			_port = port;
			_address = IPAddress.Parse(ip);
			_tcpProtocol = protocal;
		}

		~Client()
		{
			this.Loginout();
		}

		public bool Connect()
		{
			try
			{
				_tcpClient = new TcpClient();
				_tcpClient.SendTimeout = _sendTimeout;
				_tcpClient.ReceiveTimeout = _receiveTimeout;
				_tcpClient.Connect(_address, _port);

				return _tcpClient.Connected;
			}
			catch (Exception)
			{
				_tcpClient.Close();
				_tcpClient = null;
				return false;
			}
		}

		public bool Login()
		{
			if (!_tcpClient.Connected)
				throw new InvalidOperationException("Please connect the server before login");

			try
			{
				var stream = _tcpClient.GetStream();
				if (!_tcpProtocol.ConnectRequire(stream))
					return false;

				return DispatchIncomingPacket(stream) && _tcpClient.Connected;
			}
			catch (Exception e)
			{
				_tcpClient.Close();
				_tcpClient = null;
				throw e;
			}
		}

		public void Loginout()
		{
			try
			{
				if (_tcpTask != null)
					_tcpTask.Wait();
			}
			catch (Exception)
			{
			}
			finally
			{
				_tcpTask = null;

				if (_tcpClient != null)
				{
					_tcpClient.Close();
					_tcpClient = null;
				}
			}
		}

		public Task Start(CancellationToken cancellationToken)
		{
			if (!_tcpClient.Connected)
				throw new InvalidOperationException("Please connect the server before login");

			_tcpTask = Task.Run(() =>
			{
				if (!_tcpClient.Connected)
					throw new InvalidOperationException("Please connect the server before login");

				using (var stream = _tcpClient.GetStream())
				{
					try
					{
						if (_events.onStartClientListener != null)
							_events.onStartClientListener.Invoke();

						while (!cancellationToken.IsCancellationRequested)
							DispatchIncomingPacket(stream);
					}
					finally
					{
						if (_events.onStopClientListener != null)
							_events.onStopClientListener.Invoke();
					}
				}
			});

			return _tcpTask;
		}

		public void Dispose()
		{
			this.Loginout();
		}

		private bool DispatchIncomingPacket(NetworkStream stream)
		{
			int count = stream.Read(buffer, 0, buffer.Length);
			if (count > 0)
				return DispatchIncomingPacket(buffer, count);
			else
				throw new EndOfStreamException();
		}

		private bool DispatchIncomingPacket(byte[] buffer, int length)
		{
			return _tcpProtocol.DispatchIncomingPacket(buffer, length);
		}
	}
}