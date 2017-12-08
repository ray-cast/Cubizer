namespace Cubizer
{
	public sealed class ClientDelegates
	{
		public delegate void OnStartTcpListener();
		public delegate void OnStopTcpListener();

		public OnStartTcpListener onStartClientListener;
		public OnStopTcpListener onStopClientListener;
	}
}