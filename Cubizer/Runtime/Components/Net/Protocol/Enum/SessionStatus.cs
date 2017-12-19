namespace Cubizer.Net.Protocol
{
	public enum SessionStatus : uint
	{
		Handshaking = 0,
		Status = 1,
		Login = 2,
		Play = 3,
		MaxEnum
	}
}