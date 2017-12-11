using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;

using Cubizer.Protocol;

namespace Cubizer
{
	public sealed class Client : IDisposable
	{
		private readonly int _port;
		private readonly string _hostname;
		private readonly IClientProtocol _tcpProtocol;
		private readonly PacketCompress _packetCompress = new PacketCompress();
		private readonly ClientDelegates _events = new ClientDelegates();
		private readonly CompressedPacket _compressedPacket = new CompressedPacket();

		private int _sendTimeout = 0;
		private int _receiveTimeout = 0;

		private Task _tcpTask;
		private TcpClient _tcpClient;

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

		public Client(string hostname, int port, IClientProtocol protocal)
		{
			Debug.Assert(protocal != null);

			_port = port;
			_hostname = hostname;
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
				_tcpClient.Connect(_hostname, _port);

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
			int count = _compressedPacket.Deserialize(stream);
			if (count > 0)
				return DispatchIncomingPacket(_packetCompress.Decompress(_compressedPacket));
			else
				throw new EndOfStreamException();
		}

		private bool DispatchIncomingPacket(UncompressedPacket packet)
		{
			return _tcpProtocol.DispatchIncomingPacket(packet);
		}
	}
}