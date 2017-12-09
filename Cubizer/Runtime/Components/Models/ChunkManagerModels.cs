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

			[SerializeField, Range(256, 8192)]
			public int chunkNumLimits;

			[SerializeField, Range(1, 16)]
			public int chunkUpdateRadius;

			[SerializeField, Range(1, 4)]
			public int chunkThreadNums;

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
						chunkNumLimits = 4096,
						chunkUpdateRadius = 1,
						chunkThreadNums = 2,
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

		public override void OnValidate()
		{
			if (_settings.chunkSize == 0)
				_settings.chunkSize = 24;
		}
	}
}