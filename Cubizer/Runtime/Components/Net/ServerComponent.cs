using System;
using System.Threading;
using System.Net.Sockets;

using Cubizer.Chunk;
using Cubizer.Net.Server;
using Cubizer.Net.Protocol;

using UnityEngine;

namespace Cubizer.Net
{
	public sealed class ServerComponent : CubizerComponent<NetworkModels>
	{
		private ServerTcpRouter _tcpListener;
		private ServerPacketRouter _serverRouter = new ServerPacketRouter();
		private CancellationTokenSource _cancellationToken;

		public override bool active
		{
			get
			{
				return model.enabled;
			}
			set
			{
				if (model.enabled != value)
				{
					if (value)
						this.OnEnable();
					else
						this.OnDisable();

					model.enabled = value;
				}
			}
		}

		public bool isCancellationRequested
		{
			get
			{
				return _cancellationToken != null ? _cancellationToken.IsCancellationRequested : true;
			}
		}

		public int count
		{
			get
			{
				return _tcpListener != null ? _tcpListener.count : 0;
			}
		}

		public ServerComponent()
		{
			_serverRouter.onDispatchIncomingPacket = this.OnDispatchIncomingPacket;
			_serverRouter.onDispatchInvalidPacket = this.OnDispatchInvalidPacket;
		}

		public override void OnEnable()
		{
			context.behaviour.events.OnLoadChunkAfter += this.OnLoadChunkDataAfter;
			context.behaviour.events.OnAddBlockAfter += this.OnAddBlockAfter;
			context.behaviour.events.OnRemoveBlockAfter += this.OnRemoveBlockAfter;
		}

		public override void OnDisable()
		{
			context.behaviour.events.OnLoadChunkAfter -= this.OnLoadChunkDataAfter;
			context.behaviour.events.OnAddBlockAfter -= this.OnAddBlockAfter;
			context.behaviour.events.OnRemoveBlockAfter -= this.OnRemoveBlockAfter;

			this.Close();
		}

		public void Open()
		{
			if (isCancellationRequested)
			{
				_cancellationToken = new CancellationTokenSource();

				try
				{
					_tcpListener = new ServerTcpRouter(model.settings.server.address, model.settings.server.port, _serverRouter);
					_tcpListener.listener.Server.SendTimeout = model.settings.server.sendTimeOut;
					_tcpListener.listener.Server.ReceiveTimeout = model.settings.server.receiveTimeout;
					_tcpListener.sessionsSendTimeout = model.settings.server.sessionSendTimeOut;
					_tcpListener.sessionsReceiveTimeout = model.settings.server.sessionReceiveTimeout;
					_tcpListener.onStartTcpListener = OnStartTcpListener;
					_tcpListener.onStopTcpListener = OnStopTcpListener;
					_tcpListener.onIncomingClient = OnIncomingClient;
					_tcpListener.onIncomingClientSession = OnIncomingClientSession;
					_tcpListener.onOutcomingClientSession = OnOutcomingClientSession;
					_tcpListener.Start(_cancellationToken.Token);
				}
				catch (Exception e)
				{
					_cancellationToken.Cancel();
					_cancellationToken = null;
					throw e;
				}
			}
			else
			{
				throw new System.InvalidOperationException("A server has already working now.");
			}
		}

		public void Close()
		{
			if (_cancellationToken != null)
			{
				_cancellationToken.Token.Register(_tcpListener.Dispose);
				_cancellationToken.Cancel();
				_cancellationToken = null;
			}
		}

		private void OnLoadChunkDataAfter(ChunkPrimer chunk)
		{
		}

		private void OnAddBlockAfter(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
		}

		private void OnRemoveBlockAfter(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
		}

		private void OnStartTcpListener()
		{
			Debug.Log("Starting server listener...");
		}

		private void OnStopTcpListener()
		{
			Debug.Log("Stop server listener...");

			if (_cancellationToken != null)
			{
				_cancellationToken.Cancel();
				_cancellationToken = null;
			}
		}

		private void OnIncomingClient(TcpClient client)
		{
			Debug.Log($"Incoming connection of client from {client.Client.RemoteEndPoint}.");
		}

		private void OnIncomingClientSession(ServerSession session)
		{
			Debug.Log($"Incoming connection of client session from {session.client.Client.RemoteEndPoint}.");
		}

		private void OnOutcomingClientSession(ServerSession session)
		{
			Debug.Log($"Outcoming connection of clisent session.");
		}

		private void OnDispatchInvalidPacket(SessionStatus status, UncompressedPacket packet)
		{
			UnityEngine.Debug.Log($"Invalid Packet: Status:{status} Packet：{packet.packetId}.Length:[{packet.data.Count}byte]");
		}

		private void OnDispatchIncomingPacket(SessionStatus status, IPacketSerializable packet)
		{
			UnityEngine.Debug.Log($"Status:{status} Packet：{packet.GetType().Name}");
		}
	}
}