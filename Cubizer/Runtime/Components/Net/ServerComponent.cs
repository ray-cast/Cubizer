using System.Threading.Tasks;

namespace Cubizer
{
	public class ServerComponent : CubizerComponent<ServerModels>
	{
		private bool _active = true;

		private TcpRouter _tcpListener;
		private Task _task;

		public bool opened
		{
			get
			{
				return _tcpListener != null;
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
		}

		public override void OnDisable()
		{
			context.behaviour.events.OnLoadChunkAfter -= this.OnLoadChunkDataAfter;
			context.behaviour.events.OnAddBlockAfter -= this.OnAddBlockAfter;
			context.behaviour.events.OnRemoveBlockAfter -= this.OnRemoveBlockAfter;
			context.behaviour.events.OnOpenServer -= this.OnOpenServer;
			context.behaviour.events.OnCloseServer += this.OnCloseServer;
		}

		private void OnOpenServer()
		{
			if (_tcpListener == null)
			{
				_tcpListener = new TcpRouter(model.settings.address, model.settings.port);
				_task = Task.Run(async () =>
				{
					await _tcpListener.Start();
				});
			}
			else
			{
				throw new System.InvalidOperationException("There is a server already working now.");
			}
		}

		private void OnCloseServer()
		{
			if (_tcpListener != null)
			{
				_tcpListener.Dispose();
				_tcpListener = null;
			}

			if (!_task.IsCompleted)
			{
				_task.Wait();
				_task = null;
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