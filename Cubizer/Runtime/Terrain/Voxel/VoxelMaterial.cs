using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class VoxelMaterial : ICloneable
	{
		private int _instanceID;

		[SerializeField]
		private bool _transparent = false;

		[SerializeField]
		private bool _merge = false;

		private string _name;

		public object userdata;

		public bool is_transparent { set { _transparent = value; } get { return _transparent; } }
		public bool is_merge { set { _merge = value; } get { return _merge; } }

		public string name { set { _name = value; } get { return _name; } }

		public VoxelMaterial(string name, int instanceID)
		{
			_name = name;
			_instanceID = instanceID;
		}

		public int GetInstanceID()
		{
			return _instanceID;
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