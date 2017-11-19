using System.Collections.Generic;

using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Cubizer/Terrain")]
	public class Terrain : MonoBehaviour
	{
		[SerializeField]
		private CubizerProfile _profile;
		private CubizerContext _context;

		private LiveComponent _lives;
		private ChunkManagerComponent _chunkManager;
		private BiomeManagerComponent _biomeManager;

		private List<ICubizerComponent> _components;

		private TerrainDelegates _events;

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
		}

		public void Start()
		{
			Debug.Assert(_profile.terrain.settings.chunkSize > 0);

			Math.Noise.simplex_seed(_profile.terrain.settings.seed);

			_events = new TerrainDelegates();
			_context = new CubizerContext();
			_context.profile = _profile;
			_context.terrain = this;

			_components = new List<ICubizerComponent>();

			_lives = AddComponent(new LiveComponent());
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