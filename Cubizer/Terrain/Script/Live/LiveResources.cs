using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace Cubizer
{
	public class LiveResources : MonoBehaviour
	{
		[SerializeField]
		private Terrain _terrain;

		private static List<LiveBehaviour> _lives = new List<LiveBehaviour>();
		private static Dictionary<string, int> _liveIndex = new Dictionary<string, int>();

		public Terrain terrain
		{
			get
			{
				return _terrain;
			}
		}

		public static Dictionary<string, int> materials
		{
			get { return _liveIndex; }
		}

		public void Awake()
		{
			if (_terrain == null)
				Debug.LogError("Please assign a Terrain in Inspector.");
		}

		public static int RegisterMaterial(string name, LiveBehaviour entity)
		{
			if (_liveIndex.ContainsKey(name))
				return _liveIndex[name];

			_liveIndex.Add(name, _lives.Count);
			_lives.Add(entity);

			return _lives.Count - 1;
		}

		public static LiveBehaviour Load(string name)
		{
			if (_liveIndex.ContainsKey(name))
				return _lives[_liveIndex[name]];
			return null;
		}

		public static LiveBehaviour Load(int id)
		{
			Debug.Assert(_lives.Count > id);
			return _lives[id];
		}
	}
}