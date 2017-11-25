using System;
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

	public class ThreadData
	{
		public int x;
		public int y;
		public int z;

		public IBiomeData biome;
		public ChunkPrimer chunk;

		public ThreadData(IBiomeData biomeData, int xx, int yy, int zz)
		{
			x = xx;
			y = yy;
			z = zz;

			biome = biomeData;
		}
	}

	public class ThreadTask : IDisposable
	{
		private bool _isQuitRequest;

		private EventWaitHandle _event;

		private Thread _thread;
		private ThreadTaskState _state;
		private ThreadUpdateDelegate _dispose;
		private ThreadData _context;

		public delegate void ThreadUpdateDelegate(ref ThreadData context);

		public ThreadData context
		{
			get
			{
				return _context;
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
			this.Dispose();
		}

		public void Task(ThreadData data)
		{
			_context = data;
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

		public void Dispose()
		{
			if (!_isQuitRequest)
			{
				_isQuitRequest = true;

				_state = ThreadTaskState.BUSY;
				_context = null;
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

					if (_context != null)
						_dispose.Invoke(ref _context);
				}
				finally
				{
					this.state = ThreadTaskState.DONE;
				}
			}
		}
	}
}