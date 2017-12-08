using System;
using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class NetworkModels : CubizerModel
	{
		[Serializable]
		public struct NetworkSettings
		{
			[SerializeField]
			public string address;

			[SerializeField]
			public int port;

			public static NetworkSettings defaultSettings
			{
				get
				{
					return new NetworkSettings
					{
						address = "127.0.0.1",
						port = 10000,
					};
				}
			}
		}

		[Serializable]
		public struct ServerSettings
		{
			[SerializeField]
			public int sendTimeOut;

			[SerializeField]
			public int receiveTimeout;

			[SerializeField]
			public IServerProtocol protocol;

			[SerializeField]
			public IClientProtocol clientProtocol;

			public static ServerSettings defaultSettings
			{
				get
				{
					return new ServerSettings
					{
						sendTimeOut = 5000,
						receiveTimeout = 10000,
						protocol = new ServerProtocolNull(),
						clientProtocol = new ClientProtocolNull()
					};
				}
			}
		}

		[Serializable]
		public struct ClientSettings
		{
			[SerializeField]
			public int sendTimeOut;

			[SerializeField]
			public int receiveTimeout;

			[SerializeField]
			public IClientProtocol protocol;

			public static ClientSettings defaultSettings
			{
				get
				{
					return new ClientSettings
					{
						sendTimeOut = 5000,
						receiveTimeout = 10000,
						protocol = new ClientProtocolNull()
					};
				}
			}
		}

		[Serializable]
		public struct Settings
		{
			public NetworkSettings network;
			public ClientSettings client;
			public ServerSettings server;

			public static Settings defaultSettings
			{
				get
				{
					return new Settings
					{
						network = NetworkSettings.defaultSettings,
						client = ClientSettings.defaultSettings,
						server = ServerSettings.defaultSettings
					};
				}
			}
		}

		[SerializeField]
		private Settings _settings = Settings.defaultSettings;

		public Settings settings
		{
			get { return _settings; }
			set { _settings = value; }
		}

		public override void Reset()
		{
			_settings = Settings.defaultSettings;
		}

		public override void OnValidate()
		{
			if (string.IsNullOrEmpty(_settings.network.address))
				_settings.network.address = "127.0.0.1";

			if (_settings.network.port == 0)
				_settings.network.port = 10000;
		}
	}
}