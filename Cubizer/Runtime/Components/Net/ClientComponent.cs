using System.Threading;
using System.Threading.Tasks;

using UnityEngine;

namespace Cubizer
{
	public class ClientComponent : CubizerComponent<NetworkModels>
	{
		private Task _task;
		private Client _client;
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
				try
				{
					_cancellationToken = new CancellationTokenSource();

					_client = new Client(model.settings.client.protocol, model.settings.network.address, model.settings.network.port);
					_client.sendTimeout = model.settings.client.sendTimeOut;
					_client.receiveTimeout = model.settings.client.receiveTimeout;
					_client.events.onStartClientListener = OnStartClientListener;
					_client.events.onStopClientListener = OnStopClientListener;

					if (!_client.Connect())
					{
						_cancellationToken.Cancel();
						return false;
					}

					if (!_client.Login())
					{
						_cancellationToken.Cancel();
						return false;
					}

					_client.Start(_cancellationToken.Token);

					return _client.connected;
				}
				catch (System.Exception e)
				{
					_cancellationToken.Cancel();
					throw e;
				}
			}
			else
			{
				throw new System.InvalidOperationException("There is a client already working now.");
			}
		}

		public void Disconnect()
		{
			if (_cancellationToken != null)
			{
				_cancellationToken.Token.Register(_client.Loginout);
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
		}
	}
}