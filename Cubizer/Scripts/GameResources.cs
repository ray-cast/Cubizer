using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace Cubizer
{
	public class GameResources
	{
		private static Dictionary<string, GameObject> _objects = new Dictionary<string, GameObject>();

		public static bool RegisterMaterial(string name, GameObject entity)
		{
			if (_objects.ContainsKey(name))
				return false;
			_objects[name] = entity;
			return true;
		}

		public static GameObject Load(string name)
		{
			if (_objects.ContainsKey(name))
				return _objects[name];

			return null;
		}
	}
}