using System;
using System.Collections.Generic;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class BiomeGeneratorModels : CubizerModel
	{
		[Serializable]
		public struct BiomeGeneratorSettings
		{
			[SerializeField]
			public IBiomeData biomeNull;

			[SerializeField]
			public IBiomeDataManager biomes;

			[SerializeField]
			public List<BiomeGenerator> biomeGenerators;

			public static BiomeGeneratorSettings defaultSettings
			{
				get
				{
					return new BiomeGeneratorSettings
					{
						biomeNull = new BiomeDataNull(),
						biomes = new BiomeDataManager()
					};
				}
			}

			
		}

		[SerializeField]
		private BiomeGeneratorSettings _settings = BiomeGeneratorSettings.defaultSettings;

		public BiomeGeneratorSettings settings
		{
			get { return _settings; }
			set { _settings = value; }
		}

		public override void Reset()
		{
			_settings = BiomeGeneratorSettings.defaultSettings;
		}
	}
}