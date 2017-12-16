using System;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class TimeModels : CubizerModel
	{
		[Serializable]
		public struct TimeSettings
		{
			[SerializeField]
			public long worldAge;

			[SerializeField]
			public long timeOfDay;

			public static TimeSettings defaultSettings
			{
				get
				{
					return new TimeSettings
					{
						worldAge = 0,
						timeOfDay = 0
					};
				}
			}
		}

		[SerializeField]
		private TimeSettings _settings = TimeSettings.defaultSettings;

		public TimeSettings settings
		{
			get { return _settings; }
			set { _settings = value; }
		}

		public override void Reset()
		{
			_settings = TimeSettings.defaultSettings;
		}
	}
}