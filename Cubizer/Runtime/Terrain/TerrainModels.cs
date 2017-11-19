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
			[SerializeField]
			public int seed;

			public static TerrainSettings defaultSettings
			{
				get
				{
					return new TerrainSettings
					{
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