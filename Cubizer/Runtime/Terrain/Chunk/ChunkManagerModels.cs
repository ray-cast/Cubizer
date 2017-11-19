using System;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class ChunkManagerModels : CubizerModel
	{
		[Serializable]
		public struct ChunkDataSettings
		{
			[SerializeField, Range(256, 2048)]
			public int chunkNumLimits;

			public IChunkDataManager chunkManager;

			public static ChunkDataSettings defaultSettings
			{
				get
				{
					return new ChunkDataSettings
					{
						chunkManager = new ChunkDataManager(0xFF),
						chunkNumLimits = 1024,
					};
				}
			}
		}

		[SerializeField]
		private ChunkDataSettings _settings = ChunkDataSettings.defaultSettings;

		public ChunkDataSettings settings
		{
			get { return _settings; }
			set { _settings = value; }
		}

		public override void Reset()
		{
			_settings = ChunkDataSettings.defaultSettings;
		}
	}
}