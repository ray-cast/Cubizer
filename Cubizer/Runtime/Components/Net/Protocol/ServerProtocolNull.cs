using System.Threading.Tasks;

namespace Cubizer.Protocol
{
	public sealed class ServerProtocolNull : IPacketRouter
	{
		Task IPacketRouter.DispatchIncomingPacket(UncompressedPacket packet)
		{
			return Task.CompletedTask;
		}
	}
}