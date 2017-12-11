using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubizer.Protocol
{
	public interface ISerializablePacket
	{
		void Serialize(BinaryWriter bw);
	}
}