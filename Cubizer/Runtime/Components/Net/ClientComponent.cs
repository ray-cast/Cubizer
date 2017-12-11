using System;
using System.Threading;
using System.Threading.Tasks;

using Cubizer.Client;
using Cubizer.Protocol;
using Cubizer.Protocol.Login;
using Cubizer.Protocol.Handshake;

using UnityEngine;

namespace Cubizer
{
	public sealed class ClientComponent : CubizerComponent<NetworkModels>
	{
		private Task _task;
		private ClientSession _client;
		private IPacketRouter _clientProtocol = new ClientProtocol();
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
		}

		public bool Connect()
		{
			if (isCancellationRequested)
			{
				_cancellationToken = new CancellationTokenSource();

				try
				{
					_client = new ClientSession(model.settings.client.address, model.settings.client.port, _clientProtocol);
					_client.sendTimeout = model.settings.client.sendTimeOut;
					_client.receiveTimeout = model.settings.client.receiveTimeout;
					_client.events.onStartClientListener = OnStartClientListener;
					_client.events.onStopClientListener = OnStopClientListener;

					if (!_client.Connect())
					{
						_cancellationToken.Cancel();
						return false;
					}

					_client.Start(_cancellationToken.Token);

					_client.SendPacket(new Handshake(model.settings.network.version, model.settings.client.address, model.settings.client.port, SessionStatus.Login));
					_client.SendPacket(new LoginStart { name = "test" });

					return _client.connected;
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
				throw new InvalidOperationException("A client has already working now.");
			}
		}

		public void Disconnect()
		{
			if (_cancellationToken != null)
			{
				_cancellationToken.Token.Register(_client.Close);
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

		private void OnStartClientListener()
		{
			Debug.Log("Starting client listener...");
		}

		private void OnStopClientListener()
		{
			Debug.Log("Stop client listener...");

			_cancellationToken.Cancel();
			_cancellationToken = null;
		}
	}
}