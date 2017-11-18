using System;
using System.Collections.Generic;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class ChunkDataModels : CubizerModel
	{
		[Serializable]
		public struct ChunkDataSettings
		{
			public static ChunkDataSettings defaultSettings
			{
				get
				{
					return new ChunkDataSettings
					{

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