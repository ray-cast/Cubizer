using Cubizer.Net.Protocol;

namespace Cubizer.Net.Server
{
	internal interface IPacketHeader
	{
		void OnDispatchIncomingPacket(IPacketSerializable packet);
	}
}