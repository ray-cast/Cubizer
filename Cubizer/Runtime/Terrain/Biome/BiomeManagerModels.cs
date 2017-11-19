using System;
using System.Collections.Generic;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class BiomeManagerModels : CubizerModel
	{
		[Serializable]
		public struct BiomeManagerSettings
		{
			[SerializeField]
			public IBiomeData biomeNull;

			[SerializeField]
			public IBiomeDataManager biomeManager;

			[SerializeField]
			public List<BiomeGenerator> biomeGenerators;

			public static BiomeManagerSettings defaultSettings
			{
				get
				{
					return new BiomeManagerSettings
					{
						biomeNull = new BiomeDataNull(),
						biomeManager = new BiomeDataManager()
					};
				}
			}
		}

		[SerializeField]
		private BiomeManagerSettings _settings = BiomeManagerSettings.defaultSettings;

		public BiomeManagerSettings settings
		{
			get { return _settings; }
			set { _settings = value; }
		}

		public override void Reset()
		{
			_settings = BiomeManagerSettings.defaultSettings;
		}
	}
}