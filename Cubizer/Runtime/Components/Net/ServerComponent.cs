using System;
using System.Threading;
using System.Net.Sockets;
using System.Reflection;

using UnityEngine;
using Cubizer.Protocol;

namespace Cubizer
{
	public sealed class ServerComponent : CubizerComponent<NetworkModels>
	{
		private ServerTcpRouter _tcpListener;
		private IServerProtocol _serverProtocol;
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

		public override void OnEnable()
		{
			var assembly = Assembly.GetAssembly(typeof(IServerProtocol));
			if (assembly == null)
				throw new MissingReferenceException($"Failed to load assembly: {typeof(IServerProtocol).FullName}.");

			_serverProtocol = assembly.CreateInstance(model.settings.server.protocol) as IServerProtocol;
			if (_serverProtocol == null)
				throw new ArgumentException($"Invalid type name of protocol: {model.settings.client.protocol}.");

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
					_tcpListener = new ServerTcpRouter(model.settings.server.address, model.settings.server.port, _serverProtocol);
					_tcpListener.sendTimeout = model.settings.server.sendTimeOut;
					_tcpListener.receiveTimeout = model.settings.server.receiveTimeout;
					_tcpListener.events.onStartTcpListener += OnStartTcpListener;
					_tcpListener.events.onStopTcpListener += OnStopTcpListener;
					_tcpListener.events.onIncomingClient += OnIncomingClient;
					_tcpListener.events.onIncomingClientSession += OnIncomingClientSession;
					_tcpListener.events.onOutcomingClientSession += OnOutcomingClientSession;
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
				throw new System.InvalidOperationException("There is a server already working now.");
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
	}
}