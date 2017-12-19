using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Cubizer.Db;
using Cubizer.Net;
using Cubizer.Chunk;
using Cubizer.Biome;
using Cubizer.Live;
using Cubizer.Time;
using Cubizer.Players;

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
		private PlayerComponent _playerComponent;
		private TimeComponent _timeComponent;
		private DbComponent _dbComponent;
		private ServerComponent _serverComponent;
		private ClientComponent _clientComponent;

		private readonly List<ICubizerComponent> _components = new List<ICubizerComponent>();

		private readonly CubizerDelegates _events = new CubizerDelegates();
		private readonly static IVoxelMaterialManager _materialFactory = VoxelMaterialManager.GetInstance();

		public CubizerProfile profile
		{
			get { return _profile; }
		}

		public CubizerDelegates events
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

		public PlayerComponent players
		{
			get { return _playerComponent; }
		}

		public TimeComponent time
		{
			get { return _timeComponent; }
		}

		public ServerComponent server
		{
			get { return _serverComponent; }
		}

		public ClientComponent client
		{
			get { return _clientComponent; }
		}

		private void OnLoadChunkDataBefore(int x, int y, int z, ref ChunkPrimer chunk)
		{
			if (_events.OnLoadChunkBefore != null)
				_events.OnLoadChunkBefore.Invoke(x, y, z, ref chunk);
		}

		private void OnLoadChunkDataAfter(ChunkPrimer chunk)
		{
			if (_events.OnLoadChunkAfter != null)
				_events.OnLoadChunkAfter.Invoke(chunk);
		}

		private void OnDestroyChunkData(ChunkPrimer chunk)
		{
			if (_events.OnDestroyChunk != null)
				_events.OnDestroyChunk.Invoke(chunk);
		}

		private void OnAddBlockBefore(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
			if (_events.OnAddBlockBefore != null)
				_events.OnAddBlockBefore.Invoke(chunk, x, y, z, voxel);
		}

		private void OnAddBlockAfter(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
			if (_events.OnAddBlockAfter != null)
				_events.OnAddBlockAfter.Invoke(chunk, x, y, z, voxel);
		}

		private void OnRemoveBlockBefore(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
			if (_events.OnRemoveBlockBefore != null)
				_events.OnRemoveBlockBefore.Invoke(chunk, x, y, z, voxel);
		}

		private void OnRemoveBlockAfter(ChunkPrimer chunk, int x, int y, int z, VoxelMaterial voxel)
		{
			if (_events.OnRemoveBlockAfter != null)
				_events.OnRemoveBlockAfter.Invoke(chunk, x, y, z, voxel);
		}

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
			Debug.Assert(component != null);
			_components.Add(component);
			return component;
		}

		private bool RemoveComponent<T>(T component) where T : ICubizerComponent
		{
			Debug.Assert(component != null);
			return _components.Remove(component);
		}

		internal ICubizerComponent GetCubizerComponent(string name)
		{
			foreach (var component in _components)
			{
				if (component.GetType().Name == name)
					return component;
			}

			return null;
		}

		internal ICubizerComponent GetCubizerComponent(System.Type type)
		{
			foreach (var component in _components)
			{
				if (component.GetType() == type)
					return component;
			}

			return null;
		}

		internal ICubizerComponent GetCubizerComponent<T>() where T : ICubizerComponent
		{
			return GetCubizerComponent(typeof(T));
		}

		private IEnumerator UpdateComponentsWithCoroutine()
		{
			yield return new WaitForSeconds(profile.terrain.settings.repeatRate);

			UpdateComponents();

			StartCoroutine("UpdateComponentsWithCoroutine");
		}

		public void OnEnable()
		{
			if (_profile == null)
				Debug.LogError("Please drag a CubizerProfile into Inspector.");

			_context = new CubizerContext();
			_context.profile = _profile;
			_context.behaviour = this;
			_context.materialFactory = _materialFactory;

			_lives = AddComponent(new LiveManagerComponent());
			_chunkManager = AddComponent(new ChunkManagerComponent());
			_biomeManager = AddComponent(new BiomeManagerComponent());
			_playerComponent = AddComponent(new PlayerComponent());
			_timeComponent = AddComponent(new TimeComponent());
			_dbComponent = AddComponent(new DbComponent());
			_serverComponent = AddComponent(new ServerComponent());
			_clientComponent = AddComponent(new ClientComponent());

			_lives.Init(_context, _profile.lives);

			_chunkManager.Init(_context, _profile.chunk);
			_chunkManager.callbacks.OnLoadChunkBefore += this.OnLoadChunkDataBefore;
			_chunkManager.callbacks.OnLoadChunkAfter += this.OnLoadChunkDataAfter;
			_chunkManager.callbacks.OnDestroyChunk += this.OnDestroyChunkData;
			_chunkManager.callbacks.OnAddBlockBefore += this.OnAddBlockBefore;
			_chunkManager.callbacks.OnAddBlockAfter += this.OnAddBlockAfter;
			_chunkManager.callbacks.OnRemoveBlockBefore += this.OnRemoveBlockBefore;
			_chunkManager.callbacks.OnRemoveBlockAfter += this.OnRemoveBlockAfter;

			_biomeManager.Init(_context, _profile.biome);
			_playerComponent.Init(_context, _profile.players);
			_timeComponent.Init(_context, _profile.time);
			_dbComponent.Init(_context, _profile.database);
			_serverComponent.Init(_context, _profile.network);
			_clientComponent.Init(_context, _profile.network);

			this.EnableComponents();

			StartCoroutine("UpdateComponentsWithCoroutine");
		}

		public void OnDisable()
		{
			this.DisableComponents();

			_components.Clear();
			_materialFactory.Dispose();

			StopCoroutine("UpdateComponentsWithCoroutine");
		}
	}
}