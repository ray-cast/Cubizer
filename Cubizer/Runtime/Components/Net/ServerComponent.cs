using System.Threading;

namespace Cubizer
{
	public class ServerComponent : CubizerComponent<ServerModels>
	{
		private bool _active = true;

		private TcpRouter _tcpListener;
		private CancellationTokenSource _cancellationToken;

		public bool IsCancellationRequested
		{
			get
			{
				return _cancellationToken != null ? _cancellationToken.IsCancellationRequested : true;
			}
		}

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

		public override void OnEnable()
		{
			context.behaviour.events.OnLoadChunkAfter += this.OnLoadChunkDataAfter;
			context.behaviour.events.OnAddBlockAfter += this.OnAddBlockAfter;
			context.behaviour.events.OnRemoveBlockAfter += this.OnRemoveBlockAfter;
			context.behaviour.events.OnOpenServer += this.OnOpenServer;
			context.behaviour.events.OnCloseServer += this.OnCloseServer;
			context.behaviour.events.OnPlayerConnection += this.OnConnection;
			context.behaviour.events.OnPlayerDisconnect += this.OnDisconnect;
		}

		public override void OnDisable()
		{
			context.behaviour.events.OnLoadChunkAfter -= this.OnLoadChunkDataAfter;
			context.behaviour.events.OnAddBlockAfter -= this.OnAddBlockAfter;
			context.behaviour.events.OnRemoveBlockAfter -= this.OnRemoveBlockAfter;
			context.behaviour.events.OnOpenServer -= this.OnOpenServer;
			context.behaviour.events.OnCloseServer -= this.OnCloseServer;
			context.behaviour.events.OnPlayerConnection -= this.OnConnection;
			context.behaviour.events.OnPlayerDisconnect -= this.OnDisconnect;
		}

		private void OnOpenServer()
		{
			_cancellationToken = new CancellationTokenSource();

			if (_tcpListener == null)
			{
				_tcpListener = new TcpRouter(model.settings.address, model.settings.port);
				_tcpListener.Start(_cancellationToken.Token);
			}
			else
			{
				throw new System.InvalidOperationException("There is a server already working now.");
			}
		}

		private void OnCloseServer()
		{
			if (_cancellationToken != null)
			{
				_cancellationToken.Cancel();
				_cancellationToken = null;
			}
		}

		private void OnConnection(IPlayer player)
		{
			if (!this.IsCancellationRequested)
			{
			}
		}

		private void OnDisconnect(IPlayer player)
		{
			if (!this.IsCancellationRequested)
			{
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