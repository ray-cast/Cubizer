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
		private DatabaseComponent _database;

		private List<ICubizerComponent> _components;

		private PlayerManagerModel _players = new PlayerManagerModel();
		private IVoxelMaterialManager _materialFactory;

		private TerrainDelegates _events;

		public class TerrainDelegates
		{
			public delegate void OnSaveData(GameObject chunk);
			public delegate bool OnLoadData(int x, int y, int z, out ChunkPrimer chunk);

			public OnSaveData onSaveChunkData;
			public OnLoadData onLoadChunkData;
		}

		public CubizerProfile profile
		{
			get { return _profile; }
		}

		public TerrainDelegates events
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

		public void Awake()
		{
			if (_profile == null)
				Debug.LogError("Please drag a CubizerProfile into Inspector.");

			_events = new TerrainDelegates();
		}

		public void Start()
		{
			Debug.Assert(_profile.chunk.settings.chunkSize > 0);

			_context = new CubizerContext();
			_context.profile = _profile;
			_context.behaviour = this;
			_context.materialFactory = _materialFactory = new VoxelMaterialManager();
			_context.players = _players;

			_components = new List<ICubizerComponent>();

			_lives = AddComponent(new LiveManagerComponent());
			_lives.Init(_context, _profile.lives);

			_chunkManager = AddComponent(new ChunkManagerComponent());
			_chunkManager.Init(_context, _profile.chunk);
			_chunkManager.active = true;
			_chunkManager.events.onLoadChunkData = this.OnLoadData;
			_chunkManager.events.onSaveChunkData = this.OnSaveData;

			_biomeManager = AddComponent(new BiomeManagerComponent());
			_biomeManager.Init(_context, _profile.biome);

			_database = AddComponent(new DatabaseComponent());
			_database.Init(_context, _profile.database);

			Math.Noise.simplex_seed(_profile.terrain.settings.seed);

			this.EnableComponents();
		}

		public void AddPlayer(IPlayer player)
		{
			_players.settings.players.Add(player);
		}

		public void OnDestroy()
		{
			this.DisableComponents();

			_components.Clear();
			_materialFactory.Dispose();
		}

		public void OnSaveData(GameObject chunk)
		{
			this.events.onSaveChunkData(chunk);
		}

		public bool OnLoadData(int x, int y, int z, out ChunkPrimer chunk)
		{
			return this.events.onLoadChunkData(x, y, z, out chunk);
		}

		public void Update()
		{
			UpdateComponents();
		}

		private void EnableComponents()
		{
			foreach (var component in _components)
			{
				var model = component.GetModel();
				if (model != null)
					component.OnEnable();
			}
		}

		private void DisableComponents()
		{
			foreach (var component in _components)
			{
				var model = component.GetModel();
				if (model != null)
					component.OnDisable();
			}
		}

		private void UpdateComponents()
		{
			foreach (var component in _components)
			{
				var model = component.GetModel();
				if (model != null && component.active)
					component.Update();
			}
		}

		private T AddComponent<T>(T component) where T : ICubizerComponent
		{
			_components.Add(component);
			return component;
		}
	}
}