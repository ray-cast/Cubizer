using System.Threading;
using System.Threading.Tasks;

namespace Cubizer
{
	public class ClientComponent : CubizerComponent<NetworkModels>
	{
		private Client _client;

		private bool _active = true;
		private CancellationTokenSource _cancellationToken;
		private Task _task;

		public override bool active
		{
			get
			{
				return _active;
			}
			set
			{
				if (_active != value)
				{
					if (value)
						this.OnEnable();
					else
						this.OnDisable();

					_active = value;
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

				_client = new Client(model.settings.client.protocol, model.settings.network.address, model.settings.network.port);
				_client.sendTimeout = model.settings.client.sendTimeOut;
				_client.receiveTimeout = model.settings.client.receiveTimeout;

				try
				{
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

					_client.Start(_cancellationToken.Token).GetAwaiter().OnCompleted(() => { _client = null; }); ;
				}
				catch (System.Exception e)
				{
					_cancellationToken.Cancel();
					throw e;
				}

				return _client.connected;
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
	}
}