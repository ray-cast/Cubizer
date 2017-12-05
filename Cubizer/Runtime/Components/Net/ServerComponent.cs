using System.Threading;

namespace Cubizer
{
	public class ServerComponent : CubizerComponent<NetworkModels>
	{
		private bool _active = true;

		private ServerTcpRouter _tcpListener;
		private CancellationTokenSource _cancellationToken;

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

		public int Count
		{
			get
			{
				return _tcpListener != null ? _tcpListener.Count : 0;
			}
		}

		public override void OnEnable()
		{
			Context.behaviour.Events.OnLoadChunkAfter += this.OnLoadChunkDataAfter;
			Context.behaviour.Events.OnAddBlockAfter += this.OnAddBlockAfter;
			Context.behaviour.Events.OnRemoveBlockAfter += this.OnRemoveBlockAfter;
		}

		public override void OnDisable()
		{
			Context.behaviour.Events.OnLoadChunkAfter -= this.OnLoadChunkDataAfter;
			Context.behaviour.Events.OnAddBlockAfter -= this.OnAddBlockAfter;
			Context.behaviour.Events.OnRemoveBlockAfter -= this.OnRemoveBlockAfter;

			this.Close();
		}

		public void Open()
		{
			if (IsCancellationRequested)
			{
				_cancellationToken = new CancellationTokenSource();

				_tcpListener = new ServerTcpRouter(Model.settings.server.protocol, Model.settings.network.address, Model.settings.network.port);
				_tcpListener.SendTimeout = Model.settings.server.sendTimeOut;
				_tcpListener.ReceiveTimeout = Model.settings.server.receiveTimeout;
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