using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;

using Cubizer.Net.Protocol;
using Cubizer.Net.Protocol.Serialization;

namespace Cubizer.Net.Server
{
	public sealed class ServerSession : IDisposable
	{
		private readonly TcpClient _tcpClient;
		private readonly IPacketRouter _packRouter;
		private readonly IPacketCompress _packetCompress;

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

		public Task Start(CancellationToken cancellationToken)
		{
			if (!_tcpClient.Connected)
				return null;

			_task = Task.Run(async () =>
			{
				using (var stream = _tcpClient.GetStream())
				{
					try
					{
						while (!cancellationToken.IsCancellationRequested)
							await DispatchIncomingPacket(stream);
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

		public async Task SendUncompressedPacket(UncompressedPacket packet)
		{
			if (packet == null)
				_tcpClient.Client.Shutdown(SocketShutdown.Send);
			else
			{
				var newPacket = _packetCompress.Compress(packet);
				await newPacket.SerializeAsync(_tcpClient.GetStream());
			}
		}

		public async Task SendPacket(IPacketSerializable packet)
		{
			if (packet == null)
				await SendUncompressedPacket(null);
			else
			{
				using (var stream = new MemoryStream())
				{
					using (var bw = new NetworkWrite(stream))
						packet.Serialize(bw);

					await SendUncompressedPacket(new UncompressedPacket(packet.packId, new ArraySegment<byte>(stream.ToArray())));
				}
			}
		}

		private async Task DispatchIncomingPacket(Stream stream)
		{
			var compressedPacket = new CompressedPacket();

			int count = await compressedPacket.DeserializeAsync(stream);
			if (count > 0)
				await this.DispatchIncomingPacket(_packetCompress.Decompress(compressedPacket));
			else
				throw new EndOfStreamException();
		}

		private async Task DispatchIncomingPacket(UncompressedPacket packet)
		{
			await _packRouter.DispatchIncomingPacket(packet);
		}
	}
}