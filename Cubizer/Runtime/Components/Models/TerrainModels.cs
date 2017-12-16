using System;
using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class TerrainModels : CubizerModel
	{
		[Serializable]
		public struct TerrainSettings
		{
			[SerializeField]
			public int seed;

			[SerializeField]
			public float repeatRate;

			public static TerrainSettings defaultSettings
			{
				get
				{
					return new TerrainSettings
					{
						seed = 255,
						repeatRate = 1.0f / 60.0f
					};
				}
			}
		}

		[SerializeField]
		private TerrainSettings _settings = TerrainSettings.defaultSettings;

		public TerrainSettings settings
		{
			get { return _settings; }
			set { _settings = value; }
		}

		public override void Reset()
		{
			_settings = TerrainSettings.defaultSettings;
		}
	}
}