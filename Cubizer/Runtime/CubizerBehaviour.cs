using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace Cubizer
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Cubizer/CubizerBehaviour")]
	public class CubizerBehaviour : MonoBehaviour
	{
		[SerializeField]
		private CubizerProfile _profile;
		private CubizerContext _context;

		private LiveManagerComponent _lives;
		private ChunkManagerComponent _chunkManager;
		private BiomeManagerComponent _biomeManager;
		private DbComponent _database;
		private ServerComponent _server;
		private ClientComponent _client;

		private readonly List<ICubizerComponent> _components = new List<ICubizerComponent>();

		private readonly PlayerManagerModel _players = new PlayerManagerModel();
		private readonly CubizerDelegates _events = new CubizerDelegates();
		private readonly static IVoxelMaterialManager _materialFactory = VoxelMaterialManager.GetInstance();

		public CubizerProfile Profile
		{
			get { return _profile; }
		}

		public CubizerDelegates Events
		{
			get { return _events; }
		}

		public BiomeManagerComponent BiomeManager
		{
			get { return _biomeManager; }
		}

		public ChunkManagerComponent ChunkManager
		{
			get { return _chunkManager; }
		}

		public ServerComponent Server
		{
			get { return _server; }
		}

		public ClientComponent Client
		{
			get { return _client; }
		}

		public void Connection(IPlayer player)
		{
			if (player != null)
			{
				_players.settings.players.Add(player);

				if (this.Events.OnPlayerConnection != null)
					this.Events.OnPlayerConnection(player);
			}
			else
			{
				throw new System.ArgumentNullException("Connection() fail");
			}
		}

		public void Disconnect(IPlayer player)
		{
			if (player != null)
			{
				_players.settings.players.Remove(player);

				if (this.Events.OnPlayerDisconnect != null)
					this.Events.OnPlayerDisconnect(player);
			}
			else
			{
				throw new System.ArgumentNullException("Disconnect() fail");
			}
		}

		private void OnLoadChunkDataBefore(int x, int y, int z, ref ChunkPrimer chunk)
		{
			if (this.Events.OnLoadChunkBefore != null)
				this.Events.OnLoadChunkBefore(x, y, z, ref chunk);
		}

		private void OnLoadChunkDataAfter(ChunkPrimer chunk)
		{
			if (this.Events.OnLoadChunkAfter != null)
				this.Events.OnLoadChunkAfter(chunk);
		}

		private void OnDestroyChunkData(ChunkPrimer chunk)
		{
			if (this.Events.OnDestroyChunk != null)
				this.Events.OnDestroyChunk(chunk);
		}

		private void OnAddBlockBefore(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
			if (this.Events.OnAddBlockBefore != null)
				this.Events.OnAddBlockBefore(chunk, x, y, z, voxel);
		}

		private void OnAddBlockAfter(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
			if (this.Events.OnAddBlockAfter != null)
				this.Events.OnAddBlockAfter(chunk, x, y, z, voxel);
		}

		private void OnRemoveBlockBefore(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
			if (this.Events.OnRemoveBlockBefore != null)
				this.Events.OnRemoveBlockBefore(chunk, x, y, z, voxel);
		}

		private void OnRemoveBlockAfter(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
			if (this.Events.OnRemoveBlockAfter != null)
				this.Events.OnRemoveBlockAfter(chunk, x, y, z, voxel);
		}

		private void EnableComponents()
		{
			foreach (var component in _components)
			{
				var model = component.GetModel();
				if (component.Active && model != null)
					component.OnEnable();
			}
		}

		private void DisableComponents()
		{
			foreach (var component in _components)
			{
				var model = component.GetModel();
				if (component.Active && model != null)
					component.OnDisable();
			}
		}

		private void UpdateComponents()
		{
			foreach (var component in _components)
			{
				var model = component.GetModel();
				if (component.Active && model != null)
					component.Update();
			}
		}

		private T AddComponent<T>(T component) where T : ICubizerComponent
		{
			Debug.Assert(component != null);
			_components.Add(component);
			return component;
		}

		private bool RemoveComponent<T>(T component) where T : ICubizerComponent
		{
			Debug.Assert(component != null);
			return _components.Remove(component);
		}

		private IEnumerator UpdateComponentsWithCoroutine()
		{
			yield return new WaitForSeconds(Profile.terrain.settings.repeatRate);

			UpdateComponents();

			StartCoroutine("UpdateComponentsWithCoroutine");
		}

		public void Start()
		{
			if (_profile == null)
				Debug.LogError("Please drag a CubizerProfile into Inspector.");

			Debug.Assert(_profile.chunk.settings.chunkSize > 0);

			_context = new CubizerContext();
			_context.profile = _profile;
			_context.behaviour = this;
			_context.materialFactory = _materialFactory;
			_context.players = _players;

			_lives = AddComponent(new LiveManagerComponent());
			_lives.Init(_context, _profile.lives);

			_chunkManager = AddComponent(new ChunkManagerComponent());
			_chunkManager.Init(_context, _profile.chunk);
			_chunkManager.Callbacks.OnLoadChunkBefore += this.OnLoadChunkDataBefore;
			_chunkManager.Callbacks.OnLoadChunkAfter += this.OnLoadChunkDataAfter;
			_chunkManager.Callbacks.OnDestroyChunk += this.OnDestroyChunkData;
			_chunkManager.Callbacks.OnAddBlockBefore += this.OnAddBlockBefore;
			_chunkManager.Callbacks.OnAddBlockAfter += this.OnAddBlockAfter;
			_chunkManager.Callbacks.OnRemoveBlockBefore += this.OnRemoveBlockBefore;
			_chunkManager.Callbacks.OnRemoveBlockAfter += this.OnRemoveBlockAfter;

			_biomeManager = AddComponent(new BiomeManagerComponent());
			_biomeManager.Init(_context, _profile.biome);

			_database = AddComponent(new DbComponent());
			_database.Init(_context, _profile.database);

			_server = AddComponent(new ServerComponent());
			_server.Init(_context, _profile.network);

			_client = AddComponent(new ClientComponent());
			_client.Init(_context, _profile.network);

			Math.Noise.simplex_seed(_profile.terrain.settings.seed);

			this.EnableComponents();

			StartCoroutine("UpdateComponentsWithCoroutine");
		}

		public void OnDestroy()
		{
			this.DisableComponents();

			_components.Clear();
			_materialFactory.Dispose();

			StopCoroutine("UpdateComponentsWithCoroutine");
		}
	}
}