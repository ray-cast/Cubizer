using System.Threading;

namespace Cubizer
{
	public class ServerComponent : CubizerComponent<NetworkModels>
	{
		private bool _active = true;

		private ServerTcpRouter _tcpListener;
		private CancellationTokenSource _cancellationToken;

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

		public int count
		{
			get
			{
				return _tcpListener != null ? _tcpListener.count : 0;
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

			this.Close();
		}

		public void Open()
		{
			if (isCancellationRequested)
			{
				_cancellationToken = new CancellationTokenSource();

				_tcpListener = new ServerTcpRouter(model.settings.server.protocol, model.settings.network.address, model.settings.network.port);
				_tcpListener.sendTimeout = model.settings.server.sendTimeOut;
				_tcpListener.receiveTimeout = model.settings.server.receiveTimeout;
				_tcpListener.Start(_cancellationToken.Token).GetAwaiter().OnCompleted(() => { _tcpListener = null; });
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

		public override void Update()
		{
			if (_tcpListener != null)
				_tcpListener.Update();
		}
	}
}