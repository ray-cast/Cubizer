using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

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
		private OnOutcomingClientSession _onCompletion;

		public TcpClient client
		{
			get
			{
				return _tcpClient;
			}
		}

		public ServerSession(TcpClient client, IPacketRouter packRouter = null, IPacketCompress packetCompress = null)
		{
			System.Diagnostics.Debug.Assert(client != null && packRouter != null);

			_tcpClient = client;
			_packRouter = packRouter ?? new ServerPacketRouter();
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

		public void OnCompletion(OnOutcomingClientSession continuation)
		{
			_onCompletion = continuation;
		}

		public async Task SendOutcomingPacket(IPacketSerializable packet)
		{
			if (packet == null)
				await SendOutcomingUncompressedPacket(null);
			else
			{
				using (var stream = new MemoryStream())
				{
					using (var bw = new NetworkWrite(stream))
						packet.Serialize(bw);

					await SendOutcomingUncompressedPacket(new UncompressedPacket(packet.packetId, new ArraySegment<byte>(stream.ToArray())));
				}
			}
		}

		public async Task SendOutcomingUncompressedPacket(UncompressedPacket packet)
		{
			if (packet == null)
				_tcpClient.Client.Shutdown(SocketShutdown.Send);
			else
			{
				var newPacket = _packetCompress.Compress(packet);
				await newPacket.SerializeAsync(_tcpClient.GetStream());
			}
		}

		public async Task SendIncomingPacket(IPacketSerializable packet)
		{
			if (packet != null)
			{
				using (var stream = new MemoryStream())
				{
					using (var bw = new NetworkWrite(stream))
						packet.Serialize(bw);

					await SendIncomingUncompressedPacket(new UncompressedPacket(packet.packetId, new ArraySegment<byte>(stream.ToArray())));
				}
			}
		}

		public async Task SendIncomingUncompressedPacket(UncompressedPacket packet)
		{
			await _packRouter.DispatchIncomingPacket(packet);
		}

		private async Task DispatchIncomingPacket(Stream stream)
		{
			var compressedPacket = new CompressedPacket();

			int count = await compressedPacket.DeserializeAsync(stream);
			if (count > 0)
				await SendIncomingUncompressedPacket(_packetCompress.Decompress(compressedPacket));
			else
				throw new EndOfStreamException();
		}
	}
}