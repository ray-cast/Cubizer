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
					};
				}
			}
		}

		[SerializeField]
		private DbSettings _settings = DbSettings.defaultSettings;

		public void SetDefaultURL(string url)
		{
			_settings.url = url;
		}

		public void SetDefaultName(string name)
		{
			_settings.name = name;
		}

		public DbSettings settings
		{
			get { return _settings; }
			set { _settings = value; }
		}

		public override void Reset()
		{
			_settings = DbSettings.defaultSettings;
		}
	}
}