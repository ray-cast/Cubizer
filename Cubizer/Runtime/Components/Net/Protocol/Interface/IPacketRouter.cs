using System.Threading.Tasks;

namespace Cubizer.Net.Protocol
{
	public interface IPacketRouter
	{
		Task DispatchIncomingPacket(UncompressedPacket packet);
	}
}