using System;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class DatabaseModels : CubizerModel
	{
		[Serializable]
		public struct DatabaseSettings
		{
			[SerializeField]
			public string url;

			[SerializeField]
			public string username;

			public static DatabaseSettings defaultSettings
			{
				get
				{
					return new DatabaseSettings
					{
						username = "cubizer"
					};
				}
			}
		}

		[SerializeField]
		private DatabaseSettings _settings = DatabaseSettings.defaultSettings;

		public DatabaseSettings settings
		{
			get { return _settings; }
			set { _settings = value; }
		}

		public override void Reset()
		{
			_settings = DatabaseSettings.defaultSettings;
		}
	}
}