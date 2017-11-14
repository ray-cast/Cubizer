using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cubizer
{
	[Serializable]
	public class VoxelMaterial : ICloneable
	{
		private bool _dynamic;
		private bool _transparent;
		private bool _merge;

		private string _name;

		public bool is_dynamic { get { return _dynamic; } }
		public bool is_transparent { get { return _transparent; } }
		public bool is_merge { get { return _merge; } }

		public string name { set { _name = value; } get { return _name; } }

		public VoxelMaterial()
		{
			_dynamic = false;
			_transparent = false;
		}

		public VoxelMaterial(string name, bool transparent = false, bool dynamic = false, bool merge = true)
		{
			_name = name;
			_dynamic = dynamic;
			_merge = merge;
			_transparent = transparent;
		}

		public object Clone()
		{
			object obj = null;

			BinaryFormatter inputFormatter = new BinaryFormatter();
			MemoryStream inputStream;

			using (inputStream = new MemoryStream())
			{
				inputFormatter.Serialize(inputStream, this);
			}

			using (MemoryStream outputStream = new MemoryStream(inputStream.ToArray()))
			{
				BinaryFormatter outputFormatter = new BinaryFormatter();
				obj = outputFormatter.Deserialize(outputStream);
			}

			return obj;
		}

		public object Instance()
		{
			return this.MemberwiseClone();
		}
	}
}