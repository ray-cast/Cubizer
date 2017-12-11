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
		private readonly IPacketCompress _packetCompress;
		private readonly ClientDelegates _events = new ClientDelegates();
		private readonly CompressedPacket _compressedPacket = new CompressedPacket();

		private int _sendTimeout = 0;
		private int _receiveTimeout = 0;

		private Task _tcpTask;
		private Stream _stream;
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

		public Client(string hostname, int port, IClientProtocol protocal, IPacketCompress packetCompress = null)
		{
			Debug.Assert(protocal != null);

			_port = port;
			_hostname = hostname;
			_tcpProtocol = protocal;
			_packetCompress = packetCompress ?? new PacketCompress();
		}

		~Client()
		{
			this.Close();
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

		public Task Start(CancellationToken cancellationToken)
		{
			if (!_tcpClient.Connected)
				throw new InvalidOperationException("Please connect the server before Start()");

			_tcpTask = Task.Run(() =>
			{
				if (!_tcpClient.Connected)
					throw new InvalidOperationException("Please connect the server before Start()");

				using (_stream = _tcpClient.GetStream())
				{
					try
					{
						if (_events.onStartClientListener != null)
							_events.onStartClientListener.Invoke();

						while (!cancellationToken.IsCancellationRequested)
							DispatchIncomingPacket(_stream);
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

		public void Close()
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

		public async Task SendPacket(UncompressedPacket packet)
		{
			if (packet == null)
				_tcpClient.Client.Shutdown(SocketShutdown.Send);
			else
			{
				var newPacket = _packetCompress.Compress(packet);
				await newPacket.SerializeAsync(_stream);
			}
		}

		public void Dispose()
		{
			this.Close();
		}

		private bool DispatchIncomingPacket(Stream stream)
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