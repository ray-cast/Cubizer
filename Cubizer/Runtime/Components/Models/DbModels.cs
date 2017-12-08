using System;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class DbModels : CubizerModel
	{
		[Serializable]
		public struct DbSettings
		{
			[SerializeField]
			public string url;

			[SerializeField]
			public string name;

			public static DbSettings defaultSettings
			{
				get
				{
					return new DbSettings
					{
						url = "localhost",
						name = "cubizer.db"
					};
				}
			}
		}

		[SerializeField]
		private DbSettings _settings = DbSettings.defaultSettings;

		public DbSettings settings
		{
			get { return _settings; }
			set { _settings = value; }
		}

		public override void Reset()
		{
			_settings = DbSettings.defaultSettings;
		}

		public override void OnValidate()
		{
			if (string.IsNullOrEmpty(_settings.url))
				_settings.url = "localhost";

			if (string.IsNullOrEmpty(_settings.name))
				_settings.name = "cubizer.db";
		}
	}
}