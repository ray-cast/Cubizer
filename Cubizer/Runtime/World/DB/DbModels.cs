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