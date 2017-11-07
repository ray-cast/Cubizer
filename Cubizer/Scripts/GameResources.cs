using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace Cubizer
{
	public class GameResources
	{
		private static Dictionary<string, UnityEngine.Object> _objects = new Dictionary<string, UnityEngine.Object>();

		public static bool RegisterMaterial(string name, Material material)
		{
			if (_objects.ContainsKey(name))
				return false;
			_objects[name] = material;
			return true;
		}

		public static UnityEngine.Object Load(string name)
		{
			if (_objects.ContainsKey(name))
				return _objects[name];

			return null;
		}
	}
}