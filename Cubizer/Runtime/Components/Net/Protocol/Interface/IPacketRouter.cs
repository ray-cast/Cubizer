using System.Threading.Tasks;

namespace Cubizer.Protocol
{
	public interface IPacketRouter
	{
		Task DispatchIncomingPacket(UncompressedPacket packet);
	}
}