using System;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

using UnityEngine;

using Cubizer.Protocol;
using Cubizer.Protocol.Login;

namespace Cubizer
{
	public sealed class ClientComponent : CubizerComponent<NetworkModels>
	{
		private Task _task;
		private Client _client;
		private IClientProtocol _clientProtocol;
		private CancellationTokenSource _cancellationToken;

		public override bool active
		{
			get
			{
				return model.enabled;
			}
			set
			{
				if (model.enabled != value)
				{
					if (value)
						this.OnEnable();
					else
						this.OnDisable();

					model.enabled = value;
				}
			}
		}

		public bool isCancellationRequested
		{
			get
			{
				return _cancellationToken != null ? _cancellationToken.IsCancellationRequested : true;
			}
		}

		public override void OnEnable()
		{
			var assembly = Assembly.GetAssembly(typeof(IClientProtocol));
			if (assembly == null)
				throw new MissingReferenceException($"Failed to load assembly: {typeof(IClientProtocol).FullName}.");

			_clientProtocol = assembly.CreateInstance(model.settings.client.protocol) as IClientProtocol;
			if (_clientProtocol == null)
				throw new ArgumentException($"Invalid type name of protocol: {model.settings.client.protocol}.");

			context.behaviour.events.OnLoadChunkAfter += this.OnLoadChunkDataAfter;
			context.behaviour.events.OnAddBlockAfter += this.OnAddBlockAfter;
			context.behaviour.events.OnRemoveBlockAfter += this.OnRemoveBlockAfter;
		}

		public override void OnDisable()
		{
			context.behaviour.events.OnLoadChunkAfter -= this.OnLoadChunkDataAfter;
			context.behaviour.events.OnAddBlockAfter -= this.OnAddBlockAfter;
			context.behaviour.events.OnRemoveBlockAfter -= this.OnRemoveBlockAfter;
		}

		public bool Connect()
		{
			if (isCancellationRequested)
			{
				_cancellationToken = new CancellationTokenSource();

				try
				{
					_client = new Client(model.settings.client.address, model.settings.client.port, _clientProtocol);
					_client.sendTimeout = model.settings.client.sendTimeOut;
					_client.receiveTimeout = model.settings.client.receiveTimeout;
					_client.events.onStartClientListener = OnStartClientListener;
					_client.events.onStopClientListener = OnStopClientListener;

					if (!_client.Connect())
					{
						_cancellationToken.Cancel();
						return false;
					}

					_client.Start(_cancellationToken.Token);

					return _client.connected;
				}
				catch (Exception e)
				{
					_cancellationToken.Cancel();
					_cancellationToken = null;
					throw e;
				}
			}
			else
			{
				throw new InvalidOperationException("There is a client already working now.");
			}
		}

		public void Disconnect()
		{
			if (_cancellationToken != null)
			{
				_cancellationToken.Token.Register(_client.Close);
				_cancellationToken.Cancel();
				_cancellationToken = null;
			}
		}

		private void OnLoadChunkDataAfter(ChunkPrimer chunk)
		{
		}

		private void OnAddBlockAfter(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
		}

		private void OnRemoveBlockAfter(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
		}

		private void OnStartClientListener()
		{
			Debug.Log("Starting client listener...");
		}

		private void OnStopClientListener()
		{
			Debug.Log("Stop client listener...");

			_cancellationToken.Cancel();
			_cancellationToken = null;
		}
	}
}