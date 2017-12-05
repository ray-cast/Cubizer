using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cubizer
{
	[Serializable]
	public class VoxelMaterial : ICloneable
	{
		private int _instanceID;

		private string _name;

		private VoxelMaterialModels _model;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public bool is_transparent
		{
			get { return _model.transparent; }
			set { _model.transparent = value; }
		}

		public bool canMerge
		{
			get { return _model.merge; }
			set { _model.merge = value; }
		}

		public object Userdata
		{
			get
			{
				return _model.userdata;
			}
			internal set
			{
				_model.userdata = value;
			}
		}

		public VoxelMaterial(string name, VoxelMaterialModels models, int instanceID)
		{
			_name = name;
			_model = models;
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