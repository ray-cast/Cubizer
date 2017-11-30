using System;
using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class ServerModels : CubizerModel
	{
		[Serializable]
		public struct ServerSettings
		{
			[SerializeField]
			public string address;

			[SerializeField]
			public int port;

			public static ServerSettings defaultSettings
			{
				get
				{
					return new ServerSettings
					{
						address = "127.0.0.1",
						port = 10000
					};
				}
			}
		}

		[SerializeField]
		private ServerSettings _settings = ServerSettings.defaultSettings;

		public ServerSettings settings
		{
			get { return _settings; }
			set { _settings = value; }
		}

		public override void Reset()
		{
			_settings = ServerSettings.defaultSettings;
		}
	}
}