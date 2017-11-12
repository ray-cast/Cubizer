using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Cubizer/Terrain")]
	public class Terrain : MonoBehaviour
	{
		[SerializeField]
		private int _chunkSize = 24;

		public TerrainDelegates.OnSaveData onSaveData;
		public TerrainDelegates.OnLoadData onLoadData;

		private ChunkDataManager _chunks;

		public ChunkDataManager chunks { get { return _chunks; } }

		public int chunkSize
		{
			set
			{
				if (_chunks != null)
					_chunks.chunkSize = value;

				_chunkSize = value;
			}
			get
			{
				return _chunkSize;
			}
		}

		public Terrain(int chunkSize = 24)
		{
			Debug.Assert(chunkSize > 0);

			_chunks = new ChunkDataManager(chunkSize);
			_chunkSize = chunkSize;
		}

		public Terrain(int chunkSize, ChunkDataManager chunks)
		{
			Debug.Assert(chunks != null);
			Debug.Assert(chunkSize > 0);

			_chunks = chunks;
			_chunkSize = chunkSize;
		}

		public void Start()
		{
			UnityEngine.Debug.Assert(_chunkSize > 0);

			_chunks = new ChunkDataManager(_chunkSize);
		}

		public bool HitTestByRay(Ray ray, int hitDistance, ref ChunkData chunk, out byte outX, out byte outY, out byte outZ, ref ChunkData lastChunk, out byte lastX, out byte lastY, out byte lastZ)
		{
			var chunkX = CalcChunkPos(ray.origin.x, _chunkSize);
			var chunkY = CalcChunkPos(ray.origin.y, _chunkSize);
			var chunkZ = CalcChunkPos(ray.origin.z, _chunkSize);

			lastChunk = null;
			lastX = lastY = lastZ = outX = outY = outZ = 255;

			if (!this.GetChunk(chunkX, chunkY, chunkZ, ref chunk))
				return false;

			Vector3 origin = ray.origin;
			origin.x -= chunk.position.x * _chunkSize;
			origin.y -= chunk.position.y * _chunkSize;
			origin.z -= chunk.position.z * _chunkSize;

			VoxelMaterial instanceID = null;

			for (int i = 0; i < hitDistance && instanceID == null; i++)
			{
				int ix = Mathf.RoundToInt(origin.x);
				int iy = Mathf.RoundToInt(origin.y);
				int iz = Mathf.RoundToInt(origin.z);

				if (outX == ix && outY == iy && outZ == iz)
					continue;

				bool isOutOfChunk = false;
				if (ix < 0) { ix = ix + _chunkSize; origin.x += _chunkSize; chunkX--; isOutOfChunk = true; }
				if (iy < 0) { iy = iy + _chunkSize; origin.y += _chunkSize; chunkY--; isOutOfChunk = true; }
				if (iz < 0) { iz = iz + _chunkSize; origin.z += _chunkSize; chunkZ--; isOutOfChunk = true; }
				if (ix + 1 > _chunkSize) { ix = ix - _chunkSize; origin.x -= _chunkSize; chunkX++; isOutOfChunk = true; }
				if (iy + 1 > _chunkSize) { iy = iy - _chunkSize; origin.y -= _chunkSize; chunkY++; isOutOfChunk = true; }
				if (iz + 1 > _chunkSize) { iz = iz - _chunkSize; origin.z -= _chunkSize; chunkZ++; isOutOfChunk = true; }

				lastX = outX;
				lastY = outY;
				lastZ = outZ;
				lastChunk = chunk;

				if (isOutOfChunk)
				{
					if (!this.GetChunk(chunkX, chunkY, chunkZ, ref chunk))
						return false;
				}

				chunk.voxels.Get((byte)ix, (byte)iy, (byte)iz, ref instanceID);

				origin += ray.direction;

				outX = (byte)ix;
				outY = (byte)iy;
				outZ = (byte)iz;
			}

			return instanceID != null;
		}

		public bool HitTestByRay(Ray ray, int hitDistance, ref ChunkData chunk, out byte outX, out byte outY, out byte outZ)
		{
			byte lx, ly, lz;
			ChunkData chunkLast = null;

			return this.HitTestByRay(ray, hitDistance, ref chunk, out outX, out outY, out outZ, ref chunkLast, out lx, out ly, out lz);
		}

		public bool HitTestByScreenPos(Vector3 pos, int hitDistance, ref ChunkData chunk, out byte outX, out byte outY, out byte outZ, ref ChunkData lastChunk, out byte lastX, out byte lastY, out byte lastZ)
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ray.origin = Camera.main.transform.position;
			return this.HitTestByRay(ray, hitDistance, ref chunk, out outX, out outY, out outZ, ref lastChunk, out lastX, out lastY, out lastZ);
		}

		public bool HitTestByScreenPos(Vector3 pos, int hitDistance, ref ChunkData chunk, out byte outX, out byte outY, out byte outZ)
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ray.origin = Camera.main.transform.position;
			return this.HitTestByRay(ray, hitDistance, ref chunk, out outX, out outY, out outZ);
		}

		public bool AddBlockByRay(Ray ray, int hitDistance, VoxelMaterial block)
		{
			Debug.Assert(block != null);

			byte x, y, z, lx, ly, lz;
			ChunkData chunkNow = null;
			ChunkData chunkLast = null;

			if (HitTestByRay(ray, hitDistance, ref chunkNow, out x, out y, out z, ref chunkLast, out lx, out ly, out lz))
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
			ChunkData chunk = null;

			if (HitTestByRay(ray, hitDistance, ref chunk, out x, out y, out z))
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

		public bool GetChunk(System.Int16 x, System.Int16 y, System.Int16 z, ref ChunkData chunk)
		{
			return _chunks.Get(x, y, z, ref chunk);
		}

		public bool GetEmptryChunkPos(Vector3 translate, Plane[] planes, Vector2Int[] radius, out Vector3Int position)
		{
			int x = CalcChunkPos(translate.x, _chunkSize);
			int y = CalcChunkPos(translate.y, _chunkSize);
			int z = CalcChunkPos(translate.z, _chunkSize);

			int bestX = 0, bestY = 0, bestZ = 0;
			int bestScore = int.MaxValue;

			int start = bestScore;

			Vector3 _chunkOffset = (Vector3.one * _chunkSize - Vector3.one) * 0.5f;

			for (int ix = radius[0].x; ix <= radius[0].y; ix++)
			{
				for (int iy = radius[1].x; iy <= radius[1].y; iy++)
				{
					for (int iz = radius[2].x; iz <= radius[2].y; iz++)
					{
						int dx = x + ix;
						int dy = y + iy;
						int dz = z + iz;

						var hit = _chunks.Exists((short)dx, (short)dy, (short)dz);
						if (hit)
							continue;

						var p = _chunkOffset + new Vector3(dx, dy, dz) * _chunkSize;

						int invisiable = GeometryUtility.TestPlanesAABB(planes, new Bounds(p, Vector3.one * _chunkSize)) ? 0 : 1;
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

		public void DestroyChunks(Transform transform)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				var transformChild = transform.GetChild(i);
				Destroy(transformChild.gameObject);
			}
		}

		public void UpdateChunkForDestroy(Transform transform, float maxDistance)
		{
			var length = transform.childCount;

			for (int i = 0; i < length; i++)
			{
				var transformChild = transform.GetChild(i);
				var distance = Vector3.Distance(transformChild.position, Camera.main.transform.position) / _chunkSize;
				if (distance > maxDistance)
				{
					if (onSaveData != null)
						onSaveData(transformChild.gameObject);

					Destroy(transformChild.gameObject);
					break;
				}
			}
		}

		public void UpdateChunkForCreate(Camera camera, ChunkGenerator chunkGenerator, Vector2Int[] radius, float maxChunkCount, int _terrainHeightLimitLow, int _terrainHeightLimitHigh)
		{
			if (_chunks.count > maxChunkCount)
				return;

			Vector3Int position;
			if (!GetEmptryChunkPos(camera.transform.position, GeometryUtility.CalculateFrustumPlanes(Camera.main), radius, out position))
				return;

			if (position.y < _terrainHeightLimitLow || position.y > _terrainHeightLimitHigh)
				return;

			ChunkData chunk = null;
			if (onLoadData != null)
				onLoadData(position, out chunk);

			if (chunk == null && chunkGenerator)
			{
				chunk = new ChunkData(_chunkSize, _chunkSize, _chunkSize, (short)position.x, (short)position.y, (short)position.z, 0);
				chunkGenerator.OnCreateChunk(chunk);
			}

			if (chunk != null)
			{
				var gameObject = new GameObject("Chunk");
				gameObject.transform.parent = transform;
				gameObject.transform.position = position * _chunkSize;
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

		public bool Load(string path)
		{
			Debug.Assert(path != null);

			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				if (stream == null)
					return false;

				var serializer = new BinaryFormatter();

				_chunks = serializer.Deserialize(stream) as ChunkDataManager;
				_chunkSize = _chunks.chunkSize;

				while (transform.childCount > 0)
				{
					var transformChild = transform.GetChild(0);
					transformChild.parent = null;
					DestroyImmediate(transformChild.gameObject);
				}

				foreach (var chunk in _chunks.GetEnumerator())
				{
					var gameObject = new GameObject("Chunk");
					gameObject.transform.parent = transform;
					gameObject.transform.position = new Vector3(chunk.position.x, chunk.position.y, chunk.position.z) * _chunkSize;
					gameObject.AddComponent<TerrainData>().chunk = chunk.value;
				}

				return true;
			}
		}

		public static short CalcChunkPos(float x, int size)
		{
			return (short)Mathf.FloorToInt(x / size);
		}
	}
}