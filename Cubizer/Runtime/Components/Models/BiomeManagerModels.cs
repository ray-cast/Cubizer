using System;
using System.Collections.Generic;

using UnityEngine;

using Cubizer.Biome;

namespace Cubizer
{
	[Serializable]
	public class BiomeManagerModels : CubizerModel
	{
		[Serializable]
		public struct BiomeGeneratorSettings
		{
			[SerializeField]
			public IBiomeData biomeNull;

			[SerializeField]
			public IBiomeDataManager biomeManager;

			[SerializeField]
			public List<IBiomeGenerator> biomeGenerators;

			[SerializeField, Range(256, 8192)]
			public int biomeNumLimits;

			public static BiomeGeneratorSettings defaultSettings
			{
				get
				{
					return new BiomeGeneratorSettings
					{
						biomeNull = new BiomeDataNull(),
						biomeManager = new BiomeDataManager(),
						biomeGenerators = new List<IBiomeGenerator>(),
						biomeNumLimits = 4096
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