using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

using Cubizer.Math;

namespace Cubizer
{
	public class Resources
	{
		private static Dictionary<int, UnityEngine.Object> _objects = new Dictionary<int, UnityEngine.Object>();

		public static int Add(Material material)
		{
			int hash = material.GetHashCode();
			if (_objects.ContainsKey(hash))
				return material.GetHashCode();
			_objects[hash] = material;
			return hash;
		}

		public static UnityEngine.Object Load(int key)
		{
			if (_objects.ContainsKey(key))
				return _objects[key];

			return null;
		}
	}
}