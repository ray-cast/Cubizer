using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Cubizer.Protocol
{
	public class NetworkReader : IDisposable
	{
		private readonly BinaryReader br;

		public NetworkReader(Stream stream)
		{
			br = new BinaryReader(stream);
		}

		public void Dispose()
		{
			br.Dispose();
		}
	}
}