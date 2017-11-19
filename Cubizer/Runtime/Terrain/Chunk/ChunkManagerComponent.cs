using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace Cubizer
{
	public class ChunkManagerComponent : CubizerComponent<ChunkManagerModels>
	{
		private GameObject _chunkObject;

		private ChunkDelegates _events;

		public class ChunkDelegates
		{
			public delegate void OnSaveData(GameObject chunk);

			public delegate bool OnLoadData(int x, int y, int z, out ChunkPrimer chunk);

			public OnSaveData onSaveChunkData;
			public OnLoadData onLoadChunkData;
		}

		public int count
		{
			get { return _chunkObject != null ? _chunkObject.transform.childCount : 0; }
		}

		public override bool active
		{
			get { return true; }
		}

		public ChunkDelegates events
		{
			get { return _events; }
		}

		public IChunkDataManager data
		{
			get { return model.settings.chunkManager; }
		}

		public override void OnEnable()
		{
			Debug.Assert(model.settings.chunkManager != null);

			_events = new ChunkDelegates();
			_chunkObject = new GameObject("TerrainChunks");
		}

		public override void OnDisable()
		{
			if (_chunkObject != null)
				this.DestroyChunks();
		}

		public bool CreateChunk(int x, int y, int z)
		{
			ChunkPrimer chunk;
			if (this.data.Get(x, y, z, out chunk))
				return false;

			if (this.data.Count() > model.settings.chunkNumLimits)
			{
				this.data.GC();
				context.behaviour.biomeManager.biomes.GC();
			}

			if (_events.onLoadChunkData != null)
				_events.onLoadChunkData(x, y, z, out chunk);

			if (chunk == null)
			{
				IBiomeData biomeData = context.behaviour.biomeManager.buildBiomeIfNotExist(x, y, z);
				if (biomeData != null)
				{
					chunk = biomeData.OnBuildChunk(context.behaviour, (short)x, (short)y, (short)z);
					if (chunk == null)
						chunk = new ChunkPrimer(model.settings.chunkSize, (short)x, (short)y, (short)z);
				}
			}

			if (chunk != null)
			{
				var gameObject = new GameObject("Chunk");
				gameObject.transform.parent = _chunkObject.transform;
				gameObject.transform.position = new Vector3(x, y, z) * model.settings.chunkSize;

				var chunkData = gameObject.AddComponent<ChunkData>();
				chunkData.chunk = chunk;
				chunkData.chunkManager = data;

				this.data.Set(x, y, z, chunk);
			}

			return chunk != null ? true : false;
		}

		public bool CreateChunk(Camera camera, Vector2Int[] radius)
		{
			Vector3Int position;
			if (!this.GetEmptryChunkPos(camera.transform.position, GeometryUtility.CalculateFrustumPlanes(camera), radius, out position))
				return false;

			return CreateChunk(position.x, position.y, position.z);
		}

		public void DestroyChunks(bool is_save = true)
		{
			Debug.Assert(_chunkObject != null);

			var transform = _chunkObject.transform;

			for (int i = 0; i < transform.childCount; i++)
			{
				var transformChild = transform.GetChild(i);
				var child = transformChild.gameObject;

				if (is_save)
				{
					if (_events.onSaveChunkData != null)
						_events.onSaveChunkData(child);
				}

				GameObject.Destroy(child);
			}
		}

		public void DestroyChunksImmediate(bool is_save = true)
		{
			Debug.Assert(_chunkObject != null);

			var transform = _chunkObject.transform;

			for (int i = 0; i < transform.childCount; i++)
			{
				var transformChild = transform.GetChild(i);
				var child = transformChild.gameObject;

				if (is_save)
				{
					if (_events.onSaveChunkData != null)
						_events.onSaveChunkData(child);
				}

				GameObject.DestroyImmediate(child);
			}
		}

		public void DestroyChunk(Vector3 point, float radius)
		{
			Debug.Assert(_chunkObject != null);

			var transform = _chunkObject.transform;
			var maxRadius = radius * model.settings.chunkSize;

			for (int i = 0; i < transform.childCount; i++)
			{
				var transformChild = transform.GetChild(i);

				var distance = Vector3.Distance(transformChild.position, point);
				if (distance > maxRadius)
				{
					if (_events.onSaveChunkData != null)
						_events.onSaveChunkData(transformChild.gameObject);

					GameObject.Destroy(transformChild.gameObject);
					break;
				}
			}
		}

		public short CalculateChunkPosByWorld(float x)
		{
			return (short)Mathf.FloorToInt(x / model.settings.chunkSize);
		}

		public bool HitTestByRay(Ray ray, int hitDistance, out ChunkPrimer chunk, out byte outX, out byte outY, out byte outZ, out ChunkPrimer lastChunk, out byte lastX, out byte lastY, out byte lastZ)
		{
			var chunkX = CalculateChunkPosByWorld(ray.origin.x);
			var chunkY = CalculateChunkPosByWorld(ray.origin.y);
			var chunkZ = CalculateChunkPosByWorld(ray.origin.z);

			lastChunk = null;
			lastX = lastY = lastZ = outX = outY = outZ = 255;

			if (!this.data.Get(chunkX, chunkY, chunkZ, out chunk))
				return false;

			Vector3 origin = ray.origin;
			origin.x -= chunk.position.x * model.settings.chunkSize;
			origin.y -= chunk.position.y * model.settings.chunkSize;
			origin.z -= chunk.position.z * model.settings.chunkSize;

			VoxelMaterial block = null;

			for (int i = 0; i < hitDistance && block == null; i++)
			{
				int ix = Mathf.RoundToInt(origin.x);
				int iy = Mathf.RoundToInt(origin.y);
				int iz = Mathf.RoundToInt(origin.z);

				if (outX == ix && outY == iy && outZ == iz)
					continue;

				bool isOutOfChunk = false;
				if (ix < 0) { ix = ix + model.settings.chunkSize; origin.x += model.settings.chunkSize; chunkX--; isOutOfChunk = true; }
				if (iy < 0) { iy = iy + model.settings.chunkSize; origin.y += model.settings.chunkSize; chunkY--; isOutOfChunk = true; }
				if (iz < 0) { iz = iz + model.settings.chunkSize; origin.z += model.settings.chunkSize; chunkZ--; isOutOfChunk = true; }
				if (ix + 1 > model.settings.chunkSize) { ix = ix - model.settings.chunkSize; origin.x -= model.settings.chunkSize; chunkX++; isOutOfChunk = true; }
				if (iy + 1 > model.settings.chunkSize) { iy = iy - model.settings.chunkSize; origin.y -= model.settings.chunkSize; chunkY++; isOutOfChunk = true; }
				if (iz + 1 > model.settings.chunkSize) { iz = iz - model.settings.chunkSize; origin.z -= model.settings.chunkSize; chunkZ++; isOutOfChunk = true; }

				lastX = outX;
				lastY = outY;
				lastZ = outZ;
				lastChunk = chunk;

				if (isOutOfChunk)
				{
					if (!this.data.Get(chunkX, chunkY, chunkZ, out chunk))
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

			Vector3 _chunkOffset = (Vector3.one * model.settings.chunkSize - Vector3.one) * 0.5f;

			for (int ix = radius[0].x; ix <= radius[0].y; ix++)
			{
				for (int iy = radius[1].x; iy <= radius[1].y; iy++)
				{
					for (int iz = radius[2].x; iz <= radius[2].y; iz++)
					{
						int dx = x + ix;
						int dy = y + iy;
						int dz = z + iz;

						if (dy < model.settings.chunkHeightLimitLow || dy > model.settings.chunkHeightLimitHigh)
							continue;

						ChunkPrimer chunk;
						var hit = this.data.Get((short)dx, (short)dy, (short)dz, out chunk);
						if (hit)
							continue;

						var p = _chunkOffset + new Vector3(dx, dy, dz) * model.settings.chunkSize;

						int invisiable = GeometryUtility.TestPlanesAABB(planes, new Bounds(p, Vector3.one * model.settings.chunkSize)) ? 0 : 1;
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

		public bool Save(string path)
		{
			Debug.Assert(path != null);

			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				var serializer = new BinaryFormatter();
				serializer.Serialize(stream, this.data);

				return true;
			}
		}

		public bool Load(string path, bool is_save = true)
		{
			Debug.Assert(path != null);

			if (this.data.Load(path))
			{
				DestroyChunksImmediate(is_save);

				foreach (ChunkPrimer chunk in this.data.GetEnumerator())
				{
					var gameObject = new GameObject("Chunk");
					gameObject.transform.parent = _chunkObject.transform;
					gameObject.transform.position = new Vector3(chunk.position.x, chunk.position.y, chunk.position.z) * model.settings.chunkSize;
					gameObject.AddComponent<ChunkData>().chunk = chunk;
				}

				return true;
			}

			return false;
		}
	}
}