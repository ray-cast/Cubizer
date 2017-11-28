using System;
using System.Threading;

namespace Cubizer
{
	public class ThreadTask<ThreadData> : IDisposable
		where ThreadData : class
	{
		private bool _isQuitRequest;

		private readonly EventWaitHandle _event;

		private readonly Thread _thread;
		private readonly ThreadUpdateDelegate _dispose;

		private ThreadTaskState _state;
		private ThreadData _context;
		private Exception _except;

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

		public Exception except
		{
			get { return _except; }
		}

		public ThreadTask(ThreadUpdateDelegate callback)
		{
			_state = ThreadTaskState.Idle;
			_isQuitRequest = true;

			_dispose = callback;
			_event = new AutoResetEvent(false);
			_thread = new Thread(OnUpdate);
		}

		~ThreadTask()
		{
			this.Dispose();
		}

		public void Task(ThreadData data)
		{
			_context = data;
			_state = ThreadTaskState.Busy;
			_event.Set();
		}

		public void Start()
		{
			if (_isQuitRequest)
			{
				_except = null;
				_isQuitRequest = false;
				_state = ThreadTaskState.Idle;
				_thread.Start();
			}
		}

		public void Dispose()
		{
			if (!_isQuitRequest)
			{
				_isQuitRequest = true;

				_state = ThreadTaskState.Busy;
				_context = null;
				_event.Set();
				_thread.Join();
				_state = ThreadTaskState.Quit;
			}
		}

		private void OnUpdate()
		{
			try
			{
				while (!_isQuitRequest)
				{
					while (_state != ThreadTaskState.Busy)
						_event.WaitOne();

					if (_context != null)
						_dispose.Invoke(ref _context);

					_state = ThreadTaskState.Done;
				}
			}
			catch (Exception e)
			{
				_except = e;
			}
			finally
			{
				_state = ThreadTaskState.Quit;
			}
		}
	}
}