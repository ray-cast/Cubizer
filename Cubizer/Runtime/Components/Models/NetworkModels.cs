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
			public uint version;

			public static NetworkSettings defaultSettings
			{
				get
				{
					return new NetworkSettings
					{
						version = 335
					};
				}
			}
		}

		[Serializable]
		public struct ServerSettings
		{
			[SerializeField]
			public string address;

			[SerializeField]
			public ushort port;

			[SerializeField]
			public int sendTimeOut;

			[SerializeField]
			public int receiveTimeout;

			public static ServerSettings defaultSettings
			{
				get
				{
					return new ServerSettings
					{
						address = "127.0.0.1",
						port = 25565,
						sendTimeOut = 5000,
						receiveTimeout = 10000,
					};
				}
			}
		}

		[Serializable]
		public struct ClientSettings
		{
			[SerializeField]
			public string address;

			[SerializeField]
			public ushort port;

			[SerializeField]
			public int sendTimeOut;

			[SerializeField]
			public int receiveTimeout;

			public static ClientSettings defaultSettings
			{
				get
				{
					return new ClientSettings
					{
						address = "127.0.0.1",
						port = 25565,
						sendTimeOut = 5000,
						receiveTimeout = 10000,
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
			if (string.IsNullOrEmpty(_settings.client.address))
				_settings.client.address = "127.0.0.1";

			if (_settings.client.port == 0)
				_settings.client.port = 25565;

			if (string.IsNullOrEmpty(_settings.server.address))
				_settings.server.address = "127.0.0.1";

			if (_settings.server.port == 0)
				_settings.server.port = 25565;
		}
	}
}