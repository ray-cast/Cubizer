using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;

using Cubizer.Protocol;

namespace Cubizer.Server
{
	public sealed class ServerSession : IDisposable
	{
		private readonly TcpClient _tcpClient;
		private readonly IPacketRouter _packRouter;
		private readonly IPacketCompress _packetCompress;
		private readonly CompressedPacket _compressedPacket = new CompressedPacket();

		private Task _task;
		private ServerTcpDelegates.OnOutcomingClientSession _onCompletion;

		public TcpClient client
		{
			get
			{
				return _tcpClient;
			}
		}

		public ServerSession(TcpClient client, IPacketRouter protocol, IPacketCompress packetCompress = null)
		{
			Debug.Assert(client != null && protocol != null);

			_tcpClient = client;
			_packRouter = protocol;
			_packetCompress = packetCompress ?? new PacketCompress();
		}

		~ServerSession()
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
			catch (Exception e)
			{
				UnityEngine.Debug.LogException(e);
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

		private void SendUncompressedPacket(UncompressedPacket packet)
		{
			if (packet == null)
				_tcpClient.Client.Shutdown(SocketShutdown.Send);
			else
			{
				var newPacket = _packetCompress.Compress(packet);
				newPacket.Serialize(_tcpClient.GetStream());
			}
		}

		public void SendPacket(IPacketSerializable packet)
		{
			if (packet == null)
				this.SendUncompressedPacket(null);
			else
			{
				using (var stream = new MemoryStream())
				{
					using (var bw = new NetworkWrite(stream))
						packet.Serialize(bw);

					var newPacket = new UncompressedPacket();
					newPacket.packetId = packet.packId;
					newPacket.data = new ArraySegment<byte>(stream.ToArray());

					this.SendUncompressedPacket(newPacket);
				}
			}
		}

		private void DispatchIncomingPacket(Stream stream)
		{
			int count = _compressedPacket.Deserialize(stream);
			if (count > 0)
				this.DispatchIncomingPacket(_packetCompress.Decompress(_compressedPacket));
			else
				throw new EndOfStreamException();
		}

		private void DispatchIncomingPacket(UncompressedPacket packet)
		{
			_packRouter.DispatchIncomingPacket(packet);
		}
	}
}