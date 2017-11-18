using System;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class ChunkDataModels : CubizerModel
	{
		[Serializable]
		public struct ChunkDataSettings
		{
			public IChunkDataManager chunkManager;

			public static ChunkDataSettings defaultSettings
			{
				get
				{
					return new ChunkDataSettings
					{
						chunkManager = new ChunkDataManager()
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