using System.Threading;

namespace Cubizer
{
	public enum ThreadTaskState
	{
		QUIT,
		IDLE,
		BUSY,
		DONE,
	};

	public class ThreadTask
	{
		private ThreadTaskState _state;
		private Thread _thread;
		private EventWaitHandle _event;
		private ThreadUpdateDelegate _dispose;

		private bool _isQuitRequest;

		private IBiomeData _data;
		private ChunkPrimer _chunk;

		public int _x;
		public int _y;
		public int _z;

		public delegate void ThreadUpdateDelegate(IBiomeData data, int x, int y, int z, out ChunkPrimer chunk);

		public IBiomeData data
		{
			get
			{
				UnityEngine.Debug.Assert(_state == ThreadTaskState.DONE);
				return _data;
			}
		}

		public ChunkPrimer chunk
		{
			get
			{
				UnityEngine.Debug.Assert(_state == ThreadTaskState.DONE);
				return _chunk;
			}
		}

		public ThreadTaskState state
		{
			set
			{
				_state = value;
			}
			get
			{
				return _state;
			}
		}

		public ThreadTask(ThreadUpdateDelegate callback)
		{
			_state = ThreadTaskState.IDLE;
			_isQuitRequest = true;

			_dispose = callback;
			_event = new AutoResetEvent(false);
			_thread = new Thread(UpdateThread);
		}

		~ThreadTask()
		{
			this.Quit();
		}

		public void Task(IBiomeData data, int xx, int yy, int zz)
		{
			_x = xx;
			_y = yy;
			_z = zz;
			_data = data;
			_state = ThreadTaskState.BUSY;
			_event.Set();
		}

		public void Start()
		{
			if (_isQuitRequest)
			{
				_isQuitRequest = false;
				_thread.Start();
			}
		}

		public void Quit()
		{
			if (!_isQuitRequest)
			{
				_isQuitRequest = true;

				_state = ThreadTaskState.BUSY;
				_data = null;
				_event.Set();
				_thread.Join();
			}
		}

		private void UpdateThread()
		{
			while (!_isQuitRequest)
			{
				try
				{
					while (_state != ThreadTaskState.BUSY)
						_event.WaitOne();

					if (_data != null)
						_dispose.Invoke(_data, _x, _y, _z, out _chunk);
				}
				finally
				{
					this.state = ThreadTaskState.DONE;
				}
			}
		}
	}
}