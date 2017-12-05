using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

using Cubizer.Math;

namespace Cubizer
{
	public class ChunkManagerComponent : CubizerComponent<ChunkManagerModels>
	{
		private readonly string _name;
		private GameObject _chunkObject;
		private readonly ChunkDelegates _callbacks;
		private Task[] _tasks;

		private readonly ConcurrentQueue<ChunkPrimer> _deferredUpdater = new ConcurrentQueue<ChunkPrimer>();

		private bool _active;

		public override bool Active
		{
			get
			{
				return _active;
			}
			set
			{
				if (_active != value)
				{
					if (value)
						this.OnEnable();
					else
						this.OnDisable();

					_active = value;
				}
			}
		}

		public int Count
		{
			get { return _chunkObject != null ? _chunkObject.transform.childCount : 0; }
		}

		public ChunkDelegates Callbacks
		{
			get { return _callbacks; }
		}

		private IChunkDataManager Manager
		{
			get { return Model.settings.chunkManager; }
		}

		public ChunkManagerComponent(string name = "ServerChunks")
		{
			_name = name;
			_active = true;
			_callbacks = new ChunkDelegates();
		}

		public override void OnEnable()
		{
			_chunkObject = new GameObject(_name);
			_tasks = new Task[Model.settings.chunkThreadNums];
		}

		public override void OnDisable()
		{
			if (_chunkObject != null)
			{
				this.DestroyChunks();
				GameObject.Destroy(_chunkObject);
				_chunkObject = null;
			}
		}

		public void CreateChunk(IPlayer player, int x, int y, int z)
		{
			ChunkPrimer chunk = null;
			if (_callbacks.OnLoadChunkBefore != null)
				_callbacks.OnLoadChunkBefore(x, y, z, ref chunk);

			if (chunk == null)
			{
				IBiomeData biomeData = Context.behaviour.BiomeManager.BuildBiomeIfNotExist(x, y, z);
				if (biomeData != null)
				{
					if (chunk == null)
						chunk = biomeData.OnBuildChunk(this.Context.behaviour, x, y, z);
				}
			}

			if (chunk == null)
				chunk = new ChunkPrimer(Model.settings.chunkSize, x, y, z);

			if (_callbacks.OnLoadChunkAfter != null)
				_callbacks.OnLoadChunkAfter(chunk);

			this.Manager.Set(x, y, z, chunk);

			if (chunk.Voxels.Count > 0)
				_deferredUpdater.Enqueue(chunk);
		}

		public void DestroyChunks(bool noticeAll = true)
		{
			Debug.Assert(_chunkObject != null);

			var transform = _chunkObject.transform;

			for (int i = 0; i < transform.childCount; i++)
			{
				var child = transform.GetChild(i).gameObject;

				if (noticeAll)
				{
					var chunk = child.GetComponent<ChunkData>();

					if (_callbacks.OnDestroyChunk != null)
						_callbacks.OnDestroyChunk(chunk.Chunk);
				}

				GameObject.Destroy(child);
			}
		}

		public void DestroyChunk(Vector3 point, float radius)
		{
			Debug.Assert(_chunkObject != null);

			int x = CalculateChunkPosByWorld(point.x);
			int y = CalculateChunkPosByWorld(point.y);
			int z = CalculateChunkPosByWorld(point.z);
			var chunkPos = new Vector3(x, y, z) * Model.settings.chunkSize;

			var transform = _chunkObject.transform;
			var maxRadius = radius * Model.settings.chunkSize;

			for (int i = 0; i < transform.childCount; i++)
			{
				var transformChild = transform.GetChild(i);

				var distance = Vector3.Distance(transformChild.position, chunkPos);
				if (distance > maxRadius)
				{
					var chunk = transformChild.GetComponent<ChunkData>();

					if (_callbacks.OnDestroyChunk != null)
						_callbacks.OnDestroyChunk(chunk.Chunk);

					this.Manager.Set(chunk.Chunk.Position.x, chunk.Chunk.Position.y, chunk.Chunk.Position.z, null);

					GameObject.Destroy(transformChild.gameObject);
					break;
				}
			}
		}

		public void DestroyChunk(int x, int y, int z)
		{
			Debug.Assert(_chunkObject != null);

			var transform = _chunkObject.transform;

			for (int i = 0; i < transform.childCount; i++)
			{
				var transformChild = transform.GetChild(i);
				if (transformChild.position.x == x &&
					transformChild.position.y == y &&
					transformChild.position.z == z)
				{
					var chunk = transformChild.GetComponent<ChunkData>();

					if (_callbacks.OnDestroyChunk != null)
						_callbacks.OnDestroyChunk(chunk.Chunk);

					GameObject.Destroy(transformChild.gameObject);

					this.Manager.Set(x, y, z, null);

					break;
				}
			}
		}

		public ChunkPrimer FindChunk(int x, int y, int z)
		{
			ChunkPrimer chunk;
			if (Manager.Get(x, y, z, out chunk))
				return chunk;
			return null;
		}

		public short CalculateChunkPosByWorld(float x)
		{
			return (short)Mathf.FloorToInt(x / Model.settings.chunkSize);
		}

		private bool HitTestByRay(Ray ray, int hitDistance, out ChunkPrimer chunk, out byte outX, out byte outY, out byte outZ, out ChunkPrimer lastChunk, out byte lastX, out byte lastY, out byte lastZ)
		{
			var chunkX = CalculateChunkPosByWorld(ray.origin.x);
			var chunkY = CalculateChunkPosByWorld(ray.origin.y);
			var chunkZ = CalculateChunkPosByWorld(ray.origin.z);

			lastChunk = null;
			lastX = lastY = lastZ = outX = outY = outZ = 255;

			if (!this.Manager.Get(chunkX, chunkY, chunkZ, out chunk))
				return false;

			Vector3 origin = ray.origin;
			origin.x -= chunk.Position.x * Model.settings.chunkSize;
			origin.y -= chunk.Position.y * Model.settings.chunkSize;
			origin.z -= chunk.Position.z * Model.settings.chunkSize;

			VoxelMaterial block = null;

			for (int i = 0; i < hitDistance && block == null; i++)
			{
				int ix = Mathf.RoundToInt(origin.x);
				int iy = Mathf.RoundToInt(origin.y);
				int iz = Mathf.RoundToInt(origin.z);

				if (outX == ix && outY == iy && outZ == iz)
					continue;

				bool isOutOfChunk = false;
				if (ix < 0) { ix = ix + Model.settings.chunkSize; origin.x += Model.settings.chunkSize; chunkX--; isOutOfChunk = true; }
				if (iy < 0) { iy = iy + Model.settings.chunkSize; origin.y += Model.settings.chunkSize; chunkY--; isOutOfChunk = true; }
				if (iz < 0) { iz = iz + Model.settings.chunkSize; origin.z += Model.settings.chunkSize; chunkZ--; isOutOfChunk = true; }
				if (ix + 1 > Model.settings.chunkSize) { ix = ix - Model.settings.chunkSize; origin.x -= Model.settings.chunkSize; chunkX++; isOutOfChunk = true; }
				if (iy + 1 > Model.settings.chunkSize) { iy = iy - Model.settings.chunkSize; origin.y -= Model.settings.chunkSize; chunkY++; isOutOfChunk = true; }
				if (iz + 1 > Model.settings.chunkSize) { iz = iz - Model.settings.chunkSize; origin.z -= Model.settings.chunkSize; chunkZ++; isOutOfChunk = true; }

				lastX = outX;
				lastY = outY;
				lastZ = outZ;
				lastChunk = chunk;

				if (isOutOfChunk)
				{
					chunk = FindChunk(chunkX, chunkY, chunkZ);
					if (chunk == null)
						return false;
				}

				chunk.Voxels.Get(ix, iy, iz, ref block);

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

				if (_callbacks.OnAddBlockBefore != null)
					_callbacks.OnAddBlockBefore(chunk, lx, ly, lz, block);

				chunk.Voxels.Set(lx, ly, lz, block);
				chunk.Dirty = true;

				if (_callbacks.OnAddBlockAfter != null)
					_callbacks.OnAddBlockAfter(chunk, lx, ly, lz, block);

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
				if (_callbacks.OnRemoveBlockBefore != null)
					_callbacks.OnRemoveBlockBefore(chunk, x, y, z, null);

				chunk.Voxels.Set(x, y, z, null);
				chunk.Dirty = true;

				if (_callbacks.OnRemoveBlockAfter != null)
					_callbacks.OnRemoveBlockAfter(chunk, x, y, z, null);

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

		public bool Load(string path, bool is_save = true)
		{
			Debug.Assert(path != null);

			if (this.Manager.Load(path))
			{
				DestroyChunks(is_save);

				foreach (ChunkPrimer chunk in this.Manager.GetEnumerator())
				{
					var gameObject = new GameObject("Chunk");
					gameObject.transform.parent = _chunkObject.transform;
					gameObject.transform.position = new Vector3(chunk.Position.x, chunk.Position.y, chunk.Position.z) * Model.settings.chunkSize;
					gameObject.AddComponent<ChunkData>().Init(chunk);
				}

				return true;
			}

			return false;
		}

		public bool Save(string path)
		{
			Debug.Assert(path != null);

			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				var serializer = new BinaryFormatter();
				serializer.Serialize(stream, this.Manager);

				return true;
			}
		}

		private void UpdatePlayer(IPlayer player)
		{
			var chunkX = CalculateChunkPosByWorld(player.player.transform.position.x);
			var chunkY = CalculateChunkPosByWorld(player.player.transform.position.y);
			var chunkZ = CalculateChunkPosByWorld(player.player.transform.position.z);

			var radius = Model.settings.chunkUpdateRadius;
			for (int dx = -radius; dx <= radius; dx++)
			{
				for (int dz = -radius; dz <= radius; dz++)
				{
					var x = chunkX + dx;
					var y = chunkY;
					var z = chunkZ + dz;

					var chunk = FindChunk(x, y, z);
					if (chunk != null)
					{
						if (!chunk.Dirty)
							chunk.InvokeOnUpdate();
						else
						{
							chunk.InvokeOnChunkChange();
							chunk.Dirty = false;
						}
					}
					else
					{
						CreateChunk(player, x, y, z);
					}
				}
			}
		}

		private Task UpdateCamera(IPlayer player, Vector3 translate, Plane[] planes, Vector2Int[] radius)
		{
			int x = CalculateChunkPosByWorld(translate.x);
			int y = CalculateChunkPosByWorld(translate.y);
			int z = CalculateChunkPosByWorld(translate.z);

			int bestX = 0, bestY = 0, bestZ = 0;
			int bestScore = int.MaxValue;

			int start = bestScore;

			var chunkOffset = (Vector3.one * (Model.settings.chunkSize - 1)) * 0.5f;

			for (int iy = radius[1].x; iy <= radius[1].y; iy++)
			{
				int dy = y + iy;

				if (dy < Model.settings.chunkHeightLimitLow || dy > Model.settings.chunkHeightLimitHigh)
					continue;

				for (int ix = radius[0].x; ix <= radius[0].y; ix++)
				{
					int dx = x + ix;

					for (int iz = radius[2].x; iz <= radius[2].y; iz++)
					{
						int dz = z + iz;

						var hit = FindChunk(dx, dy, dz);
						if (hit != null)
							continue;

						var p = chunkOffset + new Vector3(dx, dy, dz) * Model.settings.chunkSize;

						int invisiable = GeometryUtility.TestPlanesAABB(planes, new Bounds(p, Vector3.one * Model.settings.chunkSize)) ? 0 : 1;
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

			if (start == bestScore)
				return null;

			return Task.Run(() => { this.CreateChunk(player, bestX, bestY, bestZ); });
		}

		private void UpdateResults()
		{
			ChunkPrimer chunk;
			if (_deferredUpdater.TryDequeue(out chunk))
			{
				var gameObject = new GameObject("Chunk");
				gameObject.transform.parent = _chunkObject.transform;
				gameObject.transform.position = chunk.Position.ConvertToVector3() * Context.profile.chunk.settings.chunkSize;
				gameObject.AddComponent<ChunkData>().Init(chunk);
			}
		}

		private void AutoGC()
		{
			if (this.Manager.Count > Model.settings.chunkNumLimits)
				this.Manager.GC();
		}

		public override void Update()
		{
			if (this.Count > Model.settings.chunkNumLimits)
				return;

			this.AutoGC();

			var players = Context.players.settings.players;
			foreach (var it in players)
				DestroyChunk(it.player.transform.position, it.model.settings.chunkRadiusGC);

			foreach (var it in players)
				UpdatePlayer(it);

			this.UpdateResults();

			foreach (var it in players)
			{
				var translate = it.player.transform.position;
				var planes = GeometryUtility.CalculateFrustumPlanes(it.player);

				for (int i = 0; i < _tasks.Length; i++)
				{
					if (!(_tasks[i] == null || _tasks[i].IsCompleted))
						continue;

					_tasks[i] = UpdateCamera(it, translate, planes, it.model.settings.chunkRadius);
				}
			}
		}
	}
}