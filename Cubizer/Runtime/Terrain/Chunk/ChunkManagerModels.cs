using System;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class ChunkManagerModels : CubizerModel
	{
		[Serializable]
		public struct ChunkManagerSettings
		{
			[SerializeField, Range(16, 32)]
			public int chunkSize;

			[SerializeField]
			public int chunkHeightLimitLow;

			[SerializeField]
			public int chunkHeightLimitHigh;

			[SerializeField, Range(256, 2048)]
			public int chunkNumLimits;

			public IChunkDataManager chunkManager;

			public static ChunkManagerSettings defaultSettings
			{
				get
				{
					return new ChunkManagerSettings
					{
						chunkSize = 24,
						chunkHeightLimitLow = -10,
						chunkHeightLimitHigh = 20,
						chunkNumLimits = 1024,
						chunkManager = new ChunkDataManager(0xFF),
					};
				}
			}
		}

		[SerializeField]
		private ChunkManagerSettings _settings = ChunkManagerSettings.defaultSettings;

		public ChunkManagerSettings settings
		{
			get { return _settings; }
			set { _settings = value; }
		}

		public override void Reset()
		{
			_settings = ChunkManagerSettings.defaultSettings;
		}
	}
}