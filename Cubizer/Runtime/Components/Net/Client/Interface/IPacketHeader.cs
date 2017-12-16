using Cubizer.Net.Protocol;

namespace Cubizer.Net.Client
{
	internal interface IPacketHeader
	{
		void OnDispatchIncomingPacket(IPacketSerializable packet);
	}
}