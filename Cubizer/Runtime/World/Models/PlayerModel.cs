using System;
using System.Collections.Generic;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class PlayerModel : CubizerModel
	{
		[Serializable]
		public struct PlayerSettings
		{
			[NonSerialized]
			public Vector3 position;

			[SerializeField]
			public int chunkRadiusGC;

			[SerializeField]
			public Vector2Int[] chunkRadius;

			public static PlayerSettings defaultSettings
			{
				get
				{
					return new PlayerSettings
					{
						chunkRadiusGC = 14,
						chunkRadius = new Vector2Int[]
						{
							new Vector2Int(-8, 8),
							new Vector2Int(-1, 3),
							new Vector2Int(-8, 8)
						}
					};
				}
			}
		}

		[SerializeField]
		private PlayerSettings _settings = PlayerSettings.defaultSettings;

		public PlayerSettings settings
		{
			get { return _settings; }
			set { _settings = value; }
		}

		public void SetTransform(Transform transform)
		{
			_settings.position = transform.position;
		}

		public override void Reset()
		{
			_settings = PlayerSettings.defaultSettings;
		}
	}
}