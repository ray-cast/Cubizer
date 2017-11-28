using System.Collections;
using System.Collections.Generic;

using UnityEngine;

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

		private List<ICubizerComponent> _components;

		private PlayerManagerModel _players;
		private IVoxelMaterialManager _materialFactory;

		private ChunkDelegates _events;

		public CubizerProfile profile
		{
			get { return _profile; }
		}

		public ChunkDelegates events
		{
			get { return _events; }
		}

		public BiomeManagerComponent biomeManager
		{
			get { return _biomeManager; }
		}

		public ChunkManagerComponent chunkManager
		{
			get { return _chunkManager; }
		}

		public void AddPlayerListener(IPlayerListener player)
		{
			_players.settings.players.Add(player);
		}

		public List<IPlayerListener> GetPlayerListeners()
		{
			return _players.settings.players;
		}

		#region delegate

		private void OnLoadChunkDataBefore(int x, int y, int z, ref ChunkPrimer chunk)
		{
			if (this.events.OnLoadChunkBefore != null)
				this.events.OnLoadChunkBefore(x, y, z, ref chunk);
		}

		private void OnLoadChunkDataAfter(ChunkPrimer chunk)
		{
			if (this.events.OnLoadChunkAfter != null)
				this.events.OnLoadChunkAfter(chunk);
		}

		private void OnDestroyChunkData(ChunkPrimer chunk)
		{
			if (this.events.OnDestroyChunk != null)
				this.events.OnDestroyChunk(chunk);
		}

		private void OnAddBlockBefore(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
			if (this.events.OnAddBlockBefore != null)
				this.events.OnAddBlockBefore(chunk, x, y, z, voxel);
		}

		private void OnAddBlockAfter(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
			if (this.events.OnAddBlockAfter != null)
				this.events.OnAddBlockAfter(chunk, x, y, z, voxel);
		}

		private void OnRemoveBlockBefore(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
			if (this.events.OnRemoveBlockBefore != null)
				this.events.OnRemoveBlockBefore(chunk, x, y, z, voxel);
		}

		private void OnRemoveBlockAfter(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
			if (this.events.OnRemoveBlockAfter != null)
				this.events.OnRemoveBlockAfter(chunk, x, y, z, voxel);
		}

		#endregion delegate

		#region component

		private void EnableComponents()
		{
			foreach (var component in _components)
			{
				var model = component.GetModel();
				if (component.active && model != null)
					component.OnEnable();
			}
		}

		private void DisableComponents()
		{
			foreach (var component in _components)
			{
				var model = component.GetModel();
				if (component.active && model != null)
					component.OnDisable();
			}
		}

		private void UpdateComponents()
		{
			foreach (var component in _components)
			{
				var model = component.GetModel();
				if (component.active && model != null)
					component.Update();
			}
		}

		private T AddComponent<T>(T component) where T : ICubizerComponent
		{
			_components.Add(component);
			return component;
		}

		public IEnumerator UpdateComponentsWithCoroutine()
		{
			yield return new WaitForSeconds(profile.terrain.settings.repeatRate);

			UpdateComponents();

			StartCoroutine("UpdateComponentsWithCoroutine");
		}

		#endregion component

		#region events

		public void Awake()
		{
			if (_profile == null)
				Debug.LogError("Please drag a CubizerProfile into Inspector.");

			_events = new ChunkDelegates();
			_players = new PlayerManagerModel();
		}

		public void Start()
		{
			Debug.Assert(_profile.chunk.settings.chunkSize > 0);

			_context = new CubizerContext();
			_context.profile = _profile;
			_context.behaviour = this;
			_context.materialFactory = _materialFactory = VoxelMaterialManager.GetInstance();
			_context.players = _players;

			_components = new List<ICubizerComponent>();

			_lives = AddComponent(new LiveManagerComponent());
			_lives.Init(_context, _profile.lives);

			_chunkManager = AddComponent(new ChunkManagerComponent());
			_chunkManager.Init(_context, _profile.chunk);
			_chunkManager.active = true;
			_chunkManager.callbacks.OnLoadChunkBefore += this.OnLoadChunkDataBefore;
			_chunkManager.callbacks.OnLoadChunkAfter += this.OnLoadChunkDataAfter;
			_chunkManager.callbacks.OnDestroyChunk += this.OnDestroyChunkData;
			_chunkManager.callbacks.OnAddBlockBefore += this.OnAddBlockBefore;
			_chunkManager.callbacks.OnAddBlockAfter += this.OnAddBlockAfter;
			_chunkManager.callbacks.OnRemoveBlockBefore += this.OnRemoveBlockBefore;
			_chunkManager.callbacks.OnRemoveBlockAfter += this.OnRemoveBlockAfter;

			_biomeManager = AddComponent(new BiomeManagerComponent());
			_biomeManager.Init(_context, _profile.biome);

			_database = AddComponent(new DbComponent());
			_database.Init(_context, _profile.database);

			Math.Noise.simplex_seed(_profile.terrain.settings.seed);

			this.EnableComponents();

			StartCoroutine("UpdateComponentsWithCoroutine");
		}

		public void OnDestroy()
		{
			this.DisableComponents();

			_components.Clear();
			_materialFactory.Dispose();
		}

		#endregion events
	}
}