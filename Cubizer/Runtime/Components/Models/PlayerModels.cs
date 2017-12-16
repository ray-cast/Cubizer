using System;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class PlayerModels : CubizerModel
	{
		[Serializable]
		public struct PlayerSettings
		{
			[SerializeField]
			public string username;

			[SerializeField]
			public bool hitTestEnable;

			[SerializeField]
			public bool hitTestWireframe;

			[SerializeField]
			public int hitTestDistance;

			[SerializeField]
			public int chunkRadiusGC;

			[SerializeField]
			public Vector2Int[] chunkRadius;

			[NonSerialized]
			public Vector3 position;

			public static PlayerSettings defaultSettings
			{
				get
				{
					return new PlayerSettings
					{
						username = "cubizer",
						hitTestEnable = true,
						hitTestWireframe = true,
						hitTestDistance = 8,
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