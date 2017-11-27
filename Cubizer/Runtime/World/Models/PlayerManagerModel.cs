using System;
using System.Collections.Generic;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class PlayerManagerModel : CubizerModel
	{
		[Serializable]
		public struct PlayerSettings
		{
			[SerializeField]
			public List<IPlayerListener> players;

			public static PlayerSettings defaultSettings
			{
				get
				{
					return new PlayerSettings
					{
						players = new List<IPlayerListener>()
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

		public override void Reset()
		{
			_settings = PlayerSettings.defaultSettings;
		}
	}
}