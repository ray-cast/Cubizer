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

		public override bool Active
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

		public bool IsCancellationRequested
		{
			get
			{
				return _cancellationToken != null ? _cancellationToken.IsCancellationRequested : true;
			}
		}

		public override void OnEnable()
		{
			Context.behaviour.events.OnLoadChunkAfter += this.OnLoadChunkDataAfter;
			Context.behaviour.events.OnAddBlockAfter += this.OnAddBlockAfter;
			Context.behaviour.events.OnRemoveBlockAfter += this.OnRemoveBlockAfter;
		}

		public override void OnDisable()
		{
			Context.behaviour.events.OnLoadChunkAfter -= this.OnLoadChunkDataAfter;
			Context.behaviour.events.OnAddBlockAfter -= this.OnAddBlockAfter;
			Context.behaviour.events.OnRemoveBlockAfter -= this.OnRemoveBlockAfter;
		}

		public bool Connect()
		{
			if (IsCancellationRequested)
			{
				_cancellationToken = new CancellationTokenSource();

				_client = new Client(Model.settings.client.protocol, Model.settings.network.address, Model.settings.network.port);
				_client.SendTimeout = Model.settings.client.sendTimeOut;
				_client.ReceiveTimeout = Model.settings.client.receiveTimeout;

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

					_client.Start(_cancellationToken.Token);
				}
				catch (System.Exception e)
				{
					_cancellationToken.Cancel();
					throw e;
				}

				return _client.Connected;
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