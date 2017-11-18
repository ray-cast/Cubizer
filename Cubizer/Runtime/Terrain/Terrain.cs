using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

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

		private List<ICubizerComponent> _components;

		private LiveComponent _lives;
		private ChunkDataComponent _chunks;
		private BiomeGeneratorComponent _biomeManager;

		private TerrainDelegates _events;

		public CubizerProfile profile
		{
			get { return _profile; }
		}

		public BiomeGeneratorComponent biomeManager
		{
			get { return _biomeManager; }
		}

		public TerrainDelegates events
		{
			get { return _events; }
		}

		public ChunkDataComponent chunks
		{
			get { return _chunks; }
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

			_chunks = AddComponent(new ChunkDataComponent());
			_chunks.Init(_context, _profile.chunk);

			_biomeManager = AddComponent(new BiomeGeneratorComponent());
			_biomeManager.Init(_context, _profile.biome);

			this.EnableComponents();
		}

		public void OnDestroy()
		{
			this.DisableComponents();
			this.DestroyChunks();
		}

		public short CalculateChunkPosByWorld(float x)
		{
			return (short)Mathf.FloorToInt(x / _profile.terrain.settings.chunkSize);
		}

		public bool HitTestByRay(Ray ray, int hitDistance, out ChunkPrimer chunk, out byte outX, out byte outY, out byte outZ, out ChunkPrimer lastChunk, out byte lastX, out byte lastY, out byte lastZ)
		{
			var chunkX = CalculateChunkPosByWorld(ray.origin.x);
			var chunkY = CalculateChunkPosByWorld(ray.origin.y);
			var chunkZ = CalculateChunkPosByWorld(ray.origin.z);

			lastChunk = null;
			lastX = lastY = lastZ = outX = outY = outZ = 255;

			if (!_chunks.Get(chunkX, chunkY, chunkZ, out chunk))
				return false;

			Vector3 origin = ray.origin;
			origin.x -= chunk.position.x * _profile.terrain.settings.chunkSize;
			origin.y -= chunk.position.y * _profile.terrain.settings.chunkSize;
			origin.z -= chunk.position.z * _profile.terrain.settings.chunkSize;

			VoxelMaterial block = null;

			for (int i = 0; i < hitDistance && block == null; i++)
			{
				int ix = Mathf.RoundToInt(origin.x);
				int iy = Mathf.RoundToInt(origin.y);
				int iz = Mathf.RoundToInt(origin.z);

				if (outX == ix && outY == iy && outZ == iz)
					continue;

				bool isOutOfChunk = false;
				if (ix < 0) { ix = ix + _profile.terrain.settings.chunkSize; origin.x += _profile.terrain.settings.chunkSize; chunkX--; isOutOfChunk = true; }
				if (iy < 0) { iy = iy + _profile.terrain.settings.chunkSize; origin.y += _profile.terrain.settings.chunkSize; chunkY--; isOutOfChunk = true; }
				if (iz < 0) { iz = iz + _profile.terrain.settings.chunkSize; origin.z += _profile.terrain.settings.chunkSize; chunkZ--; isOutOfChunk = true; }
				if (ix + 1 > _profile.terrain.settings.chunkSize) { ix = ix - _profile.terrain.settings.chunkSize; origin.x -= _profile.terrain.settings.chunkSize; chunkX++; isOutOfChunk = true; }
				if (iy + 1 > _profile.terrain.settings.chunkSize) { iy = iy - _profile.terrain.settings.chunkSize; origin.y -= _profile.terrain.settings.chunkSize; chunkY++; isOutOfChunk = true; }
				if (iz + 1 > _profile.terrain.settings.chunkSize) { iz = iz - _profile.terrain.settings.chunkSize; origin.z -= _profile.terrain.settings.chunkSize; chunkZ++; isOutOfChunk = true; }

				lastX = outX;
				lastY = outY;
				lastZ = outZ;
				lastChunk = chunk;

				if (isOutOfChunk)
				{
					if (!_chunks.Get(chunkX, chunkY, chunkZ, out chunk))
						return false;
				}

				chunk.voxels.Get((byte)ix, (byte)iy, (byte)iz, ref block);

				origin += ray.direction;

				outX = (byte)ix;
				outY = (byte)iy;
				outZ = (byte)iz;
			}

			return block != null;
		}

		public bool HitTestByRay(Ray ray, int hitDistance, out ChunkPrimer chunk, out byte outX, out byte outY, out byte outZ)
		{
			byte lx, ly, lz;
			ChunkPrimer chunkLast = null;

			return this.HitTestByRay(ray, hitDistance, out chunk, out outX, out outY, out outZ, out chunkLast, out lx, out ly, out lz);
		}

		public bool HitTestByScreenPos(Vector3 pos, int hitDistance, out ChunkPrimer chunk, out byte outX, out byte outY, out byte outZ, ref ChunkPrimer lastChunk, out byte lastX, out byte lastY, out byte lastZ)
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ray.origin = Camera.main.transform.position;
			return this.HitTestByRay(ray, hitDistance, out chunk, out outX, out outY, out outZ, out lastChunk, out lastX, out lastY, out lastZ);
		}

		public bool HitTestByScreenPos(Vector3 pos, int hitDistance, out ChunkPrimer chunk, out byte outX, out byte outY, out byte outZ)
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ray.origin = Camera.main.transform.position;
			return this.HitTestByRay(ray, hitDistance, out chunk, out outX, out outY, out outZ);
		}

		public bool AddBlockByRay(Ray ray, int hitDistance, VoxelMaterial block)
		{
			Debug.Assert(block != null);

			byte x, y, z, lx, ly, lz;
			ChunkPrimer chunkNow = null;
			ChunkPrimer chunkLast = null;

			if (HitTestByRay(ray, hitDistance, out chunkNow, out x, out y, out z, out chunkLast, out lx, out ly, out lz))
			{
				var chunk = chunkLast != null ? chunkLast : chunkNow;
				chunk.voxels.Set(lx, ly, lz, block);
				chunk.OnChunkChange();

				return true;
			}

			return false;
		}

		public bool AddBlockByScreenPos(Vector3 pos, int hitDistance, VoxelMaterial block)
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ray.origin = Camera.main.transform.position;
			return this.AddBlockByRay(ray, hitDistance, block);
		}

		public bool RemoveBlockByRay(Ray ray, int hitDistance)
		{
			byte x, y, z;
			ChunkPrimer chunk;

			if (HitTestByRay(ray, hitDistance, out chunk, out x, out y, out z))
			{
				chunk.voxels.Set(x, y, z, null);
				chunk.OnChunkChange();

				return true;
			}

			return false;
		}

		public bool RemoveBlockByScreenPos(Vector3 pos, int hitDistance)
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ray.origin = Camera.main.transform.position;
			return this.RemoveBlockByRay(ray, hitDistance);
		}

		public bool GetEmptryChunkPos(Vector3 translate, Plane[] planes, Vector2Int[] radius, out Vector3Int position)
		{
			int x = CalculateChunkPosByWorld(translate.x);
			int y = CalculateChunkPosByWorld(translate.y);
			int z = CalculateChunkPosByWorld(translate.z);

			int bestX = 0, bestY = 0, bestZ = 0;
			int bestScore = int.MaxValue;

			int start = bestScore;

			Vector3 _chunkOffset = (Vector3.one * _profile.terrain.settings.chunkSize - Vector3.one) * 0.5f;

			for (int ix = radius[0].x; ix <= radius[0].y; ix++)
			{
				for (int iy = radius[1].x; iy <= radius[1].y; iy++)
				{
					for (int iz = radius[2].x; iz <= radius[2].y; iz++)
					{
						int dx = x + ix;
						int dy = y + iy;
						int dz = z + iz;

						if (dy < _profile.terrain.settings.chunkHeightLimitLow || dy > _profile.terrain.settings.chunkHeightLimitHigh)
							continue;

						ChunkPrimer chunk;
						var hit = _chunks.Get((short)dx, (short)dy, (short)dz, out chunk);
						if (hit)
							continue;

						var p = _chunkOffset + new Vector3(dx, dy, dz) * _profile.terrain.settings.chunkSize;

						int invisiable = GeometryUtility.TestPlanesAABB(planes, new Bounds(p, Vector3.one * _profile.terrain.settings.chunkSize)) ? 0 : 1;
						int distance = Mathf.Max(Mathf.Max(Mathf.Abs(ix), Mathf.Abs(iy)), Mathf.Abs(iz));
						int score = (invisiable << 24) | distance;

						if (score < bestScore)
						{
							bestScore = score;
							bestX = dx;
							bestY = dy;
							bestZ = dz;
						}
					}
				}
			}

			position = new Vector3Int(bestX, bestY, bestZ);
			return start != bestScore;
		}

		public void DestroyChunks(bool is_save = true)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				var transformChild = transform.GetChild(i);
				var child = transformChild.gameObject;

				if (is_save)
				{
					if (_events.onSaveChunkData != null)
						_events.onSaveChunkData(child);
				}

				Destroy(child);
			}
		}

		public void DestroyChunksImmediate(bool is_save = true)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				var transformChild = transform.GetChild(i);
				var child = transformChild.gameObject;

				if (is_save)
				{
					if (_events.onSaveChunkData != null)
						_events.onSaveChunkData(child);
				}

				DestroyImmediate(child);
			}
		}

		public void UpdateChunkForDestroy(Camera player, float maxDistance)
		{
			var length = transform.childCount;

			for (int i = 0; i < length; i++)
			{
				var transformChild = transform.GetChild(i);

				var distance = Vector3.Distance(transformChild.position, player.transform.position);
				if (distance > maxDistance * _profile.terrain.settings.chunkSize)
				{
					if (_events.onSaveChunkData != null)
						_events.onSaveChunkData(transformChild.gameObject);

					Destroy(transformChild.gameObject);
					break;
				}
			}
		}

		public void UpdateChunkForCreate(Camera camera, Vector2Int[] radius)
		{
			if (_chunks.Count() > _profile.terrain.settings.chunkNumLimits)
			{
				_chunks.GC();
				_biomeManager.biomes.GC();
				return;
			}

			Vector3Int position;
			if (!GetEmptryChunkPos(camera.transform.position, GeometryUtility.CalculateFrustumPlanes(camera), radius, out position))
				return;

			ChunkPrimer chunk = null;
			if (_events.onLoadChunkData != null)
				_events.onLoadChunkData(position, out chunk);

			if (chunk == null)
				chunk = _biomeManager.buildChunk((short)position.x, (short)position.y, (short)position.z);

			if (chunk != null)
			{
				var gameObject = new GameObject("Chunk");
				gameObject.transform.parent = transform;
				gameObject.transform.position = position * _profile.terrain.settings.chunkSize;
				gameObject.AddComponent<TerrainData>().chunk = chunk;
			}
		}

		public bool Save(string path)
		{
			Debug.Assert(path != null);

			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				var serializer = new BinaryFormatter();
				serializer.Serialize(stream, _chunks);

				return true;
			}
		}

		public bool Load(string path, bool is_save = true)
		{
			Debug.Assert(path != null);

			if (_chunks.Load(path))
			{
				DestroyChunksImmediate(is_save);

				foreach (ChunkPrimer chunk in _chunks.GetEnumerator())
				{
					var gameObject = new GameObject("Chunk");
					gameObject.transform.parent = transform;
					gameObject.transform.position = new Vector3(chunk.position.x, chunk.position.y, chunk.position.z) * _profile.terrain.settings.chunkSize;
					gameObject.AddComponent<TerrainData>().chunk = chunk;
				}

				return true;
			}

			return false;
		}

		void EnableComponents()
		{
			foreach (var component in _components)
			{
				var model = component.GetModel();
				if (model != null)
					component.OnEnable();
			}
		}

		void DisableComponents()
		{
			foreach (var component in _components)
			{
				var model = component.GetModel();
				if (model != null)
					component.OnDisable();
			}
		}

		T AddComponent<T>(T component)	where T : ICubizerComponent
		{
			_components.Add(component);
			return component;
		}
	}
}