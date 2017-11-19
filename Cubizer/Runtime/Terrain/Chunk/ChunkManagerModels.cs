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
			[SerializeField, Range(256, 2048)]
			public int chunkNumLimits;

			public IChunkDataManager chunkManager;

			public static ChunkManagerSettings defaultSettings
			{
				get
				{
					return new ChunkManagerSettings
					{
						chunkManager = new ChunkDataManager(0xFF),
						chunkNumLimits = 1024,
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