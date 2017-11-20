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

		private List<ICubizerComponent> _components;

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

			Math.Noise.simplex_seed(_profile.terrain.settings.seed);
			
			_context = new CubizerContext();
			_context.profile = _profile;
			_context.behaviour = this;

			_components = new List<ICubizerComponent>();

			_lives = AddComponent(new LiveManagerComponent());
			_lives.Init(_context, _profile.lives);

			_chunkManager = AddComponent(new ChunkManagerComponent());
			_chunkManager.Init(_context, _profile.chunk);

			_biomeManager = AddComponent(new BiomeManagerComponent());
			_biomeManager.Init(_context, _profile.biome);

			this.EnableComponents();
		}

		public void OnDestroy()
		{
			this.DisableComponents();
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

		private T AddComponent<T>(T component)	where T : ICubizerComponent
		{
			_components.Add(component);
			return component;
		}
	}
}