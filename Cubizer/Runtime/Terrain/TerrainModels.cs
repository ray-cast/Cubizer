using System;
using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class TerrainModel : CubizerModel
	{
		[Serializable]
		public struct TerrainSettings
		{
			[SerializeField, Range(16, 32)]
			public int chunkSize;

			[SerializeField, Range(256, 2048)]
			public int chunkNumLimits;

			[SerializeField]
			public int chunkHeightLimitLow;

			[SerializeField]
			public int chunkHeightLimitHigh;

			[SerializeField]
			public int seed;

			public static TerrainSettings defaultSettings
			{
				get
				{
					return new TerrainSettings
					{
						chunkSize = 24,
						chunkNumLimits = 1024,
						chunkHeightLimitLow = -10,
						chunkHeightLimitHigh = 20,
						seed = 255
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