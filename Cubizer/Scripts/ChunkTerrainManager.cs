using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class ChunkTerrain
	{
		public int _chunkSize;

		private ChunkTreeManager _chunkFactory;

		public ChunkTerrain(int chunkSize)
		{
			UnityEngine.Debug.Assert(chunkSize > 0);

			_chunkSize = chunkSize;
			_chunkFactory = new ChunkTreeManager(_chunkSize, _chunkSize, _chunkSize);
		}

		~ChunkTerrain()
		{
		}

		public static short CalcChunkPos(float x, int size)
		{
			return (short)Mathf.FloorToInt(x / (float)size);
		}

		public bool HitTestByRay(Ray ray, int hitDistance, ref ChunkTree chunk, out byte outX, out byte outY, out byte outZ, ref ChunkTree lastChunk, out byte lastX, out byte lastY, out byte lastZ)
		{
			var chunkX = CalcChunkPos(ray.origin.x, _chunkSize);
			var chunkY = CalcChunkPos(ray.origin.y, _chunkSize);
			var chunkZ = CalcChunkPos(ray.origin.z, _chunkSize);

			lastChunk = null;
			lastX = lastY = lastZ = outX = outY = outZ = 255;

			if (!_chunkFactory.Get(chunkX, chunkY, chunkZ, ref chunk))
				return false;

			Vector3 origin = ray.origin;
			origin.x -= chunk.position.x * _chunkSize;
			origin.y -= chunk.position.y * _chunkSize;
			origin.z -= chunk.position.z * _chunkSize;

			ChunkEntity instanceID = null;

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
					if (!_chunkFactory.Get(chunkX, chunkY, chunkZ, ref chunk))
						return false;
				}

				chunk.Get((byte)ix, (byte)iy, (byte)iz, ref instanceID);

				origin += ray.direction;

				outX = (byte)ix;
				outY = (byte)iy;
				outZ = (byte)iz;
			}

			return instanceID != null;
		}

		public bool HitTestByRay(Ray ray, int hitDistance, ref ChunkTree chunk, out byte outX, out byte outY, out byte outZ)
		{
			byte lx, ly, lz;
			ChunkTree chunkLast = null;

			return this.HitTestByRay(ray, hitDistance, ref chunk, out outX, out outY, out outZ, ref chunkLast, out lx, out ly, out lz);
		}

		public bool HitTestByScreenPos(Vector3 pos, int hitDistance, ref ChunkTree chunk, out byte outX, out byte outY, out byte outZ, ref ChunkTree lastChunk, out byte lastX, out byte lastY, out byte lastZ)
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ray.origin = Camera.main.transform.position;
			return this.HitTestByRay(ray, hitDistance, ref chunk, out outX, out outY, out outZ, ref lastChunk, out lastX, out lastY, out lastZ);
		}

		public bool HitTestByScreenPos(Vector3 pos, int hitDistance, ref ChunkTree chunk, out byte outX, out byte outY, out byte outZ)
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ray.origin = Camera.main.transform.position;
			return this.HitTestByRay(ray, hitDistance, ref chunk, out outX, out outY, out outZ);
		}

		public bool AddEnitiyByRay(Ray ray, int hitDistance, string className)
		{
			byte x, y, z, lx, ly, lz;
			ChunkTree chunkNow = null;
			ChunkTree chunkLast = null;

			if (HitTestByRay(ray, hitDistance, ref chunkNow, out x, out y, out z, ref chunkLast, out lx, out ly, out lz))
			{
				var grass = GameObject.Find("Grass");
				if (grass != null)
				{
					var chunk = chunkLast != null ? chunkLast : chunkNow;
					chunk.Set(lx, ly, lz, new Cubizer.ChunkEntity(grass.name, grass.GetComponent<MeshRenderer>().material.GetHashCode(), false, false, false));
					chunk.OnChunkChange();
				}

				return true;
			}

			return false;
		}

		public bool AddEnitiyByScreenPos(Vector3 pos, int hitDistance, string className)
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ray.origin = Camera.main.transform.position;
			return this.AddEnitiyByRay(ray, hitDistance, className);
		}

		public bool RemoveEnitiyByRay(Ray ray, int hitDistance)
		{
			byte x, y, z;
			ChunkTree chunk = null;

			if (HitTestByRay(ray, hitDistance, ref chunk, out x, out y, out z))
			{
				chunk.Set(x, y, z, null);
				chunk.OnChunkChange();

				return true;
			}

			return false;
		}

		public bool RemoveEnitiyByScreenPos(Vector3 pos, int hitDistance)
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ray.origin = Camera.main.transform.position;
			return this.RemoveEnitiyByRay(ray, hitDistance);
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

						var hit = _chunkFactory.Exists((short)dx, (short)dy, (short)dz);
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

		public static bool Save(string path, ChunkTerrainManager _self)
		{
			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				var serializer = new BinaryFormatter();
				serializer.Serialize(stream, _self);

				stream.Close();
				return true;
			}
		}

		public static ChunkTerrainManager Load(string path)
		{
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				var serializer = new BinaryFormatter();
				return serializer.Deserialize(stream) as ChunkTerrainManager;
			}
		}

		public void DestroyChunks(Transform transform)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				var transformChild = transform.GetChild(i);
				transformChild.parent = null;
				GameObject.Destroy(transformChild.gameObject);
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
					transformChild.parent = null;
					GameObject.Destroy(transformChild.gameObject);
					break;
				}
			}
		}

		public void UpdateChunkForCreate(Transform transform, GameObject terrainGenerator, Vector2Int[] radius, float maxChunkCount, int _terrainHeightLimitLow, int _terrainHeightLimitHigh)
		{
			if (_chunkFactory.Count > maxChunkCount)
				return;

			var cameraTranslate = Camera.main.transform.position;
			var cameraPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

			Vector3Int position;
			if (!GetEmptryChunkPos(cameraTranslate, cameraPlanes, radius, out position))
				return;

			if (position.y < _terrainHeightLimitLow || position.y > _terrainHeightLimitHigh)
				return;

			if (terrainGenerator)
			{
				var map = new ChunkTree(_chunkSize, _chunkSize, _chunkSize, (short)position.x, (short)position.y, (short)position.z, 0);
				map.manager = _chunkFactory;

				var _terrainTransform = terrainGenerator.transform;
				var length = _terrainTransform.childCount;

				for (int i = 0; i < length; i++)
				{
					var script = _terrainTransform.GetChild(i).gameObject.GetComponent<ChunkTerrainGenerator>();
					if (script)
						script.OnCreateChunk(map);
				}

				var gameObject = new GameObject("Chunk");
				gameObject.transform.parent = transform;
				gameObject.transform.position = position * _chunkSize;
				gameObject.AddComponent<ChunkObjectManager>().map = map;
			}
		}
	}

	public class ChunkTerrainManager : MonoBehaviour
	{
		public int _chunkRadiusGC = 6;
		public int _chunkNumLimits = 512;

		public Vector2Int _chunkRadiusGenX = new Vector2Int(-3, 3);
		public Vector2Int _chunkRadiusGenY = new Vector2Int(-1, 3);
		public Vector2Int _chunkRadiusGenZ = new Vector2Int(-3, 3);

		public bool _isHitTestEnable = true;
		public bool _isHitTestWireframe = true;
		public bool _isHitTesting = false;

		public int _terrainSeed = 255;
		public int _terrainHeightLimitLow = -10;
		public int _terrainHeightLimitHigh = 20;

		public int _hitTestDistance = 8;

		public float _repeatRateUpdate = 0.1f;

		public ChunkTerrain _terrain;
		private GameObject _terrainGenerator;

		public Mesh _drawPickMesh;
		public Material _drawPickMaterial;

		private void OnEnable()
		{
			StartCoroutine("UpdateChunkWithCoroutine");
		}

		private void OnDisable()
		{
			StopCoroutine("UpdateChunkWithCoroutine");
		}

		private void Awake()
		{
			Math.Noise.simplex_seed(_terrainSeed);
		}

		private void Start()
		{
			if (_drawPickMesh == null)
				UnityEngine.Debug.LogError("Please assign a material on the inspector");

			if (_drawPickMaterial == null)
				UnityEngine.Debug.LogError("Please assign a mesh on the inspector");

			_terrain = new ChunkTerrain(ChunkSetting.CHUNK_SIZE);
			_terrainGenerator = GameObject.Find("TerrainGenerator");
		}

		private void Reset()
		{
			_isHitTesting = false;

			StopCoroutine("UpdateChunkWithCoroutine");
			StopCoroutine("AddEnitiyByScreenPosWithCoroutine");
			StopCoroutine("RemoveEnitiyByScreenPosWithCoroutine");
		}

		private void Update()
		{
			this.UpdateChunkForHit(_drawPickMesh, _drawPickMaterial);
		}

		private void UpdateChunkForHit(Mesh mesh, Material material)
		{
			if (_isHitTestEnable)
			{
				if (Input.GetMouseButton(0))
					StartCoroutine("RemoveEnitiyByScreenPosWithCoroutine");
				else if (Input.GetMouseButton(1))
					StartCoroutine("AddEnitiyByScreenPosWithCoroutine");
			}

			if (_isHitTestWireframe)
			{
				byte x, y, z;
				ChunkTree chunk = null;

				if (_terrain.HitTestByScreenPos(Input.mousePosition, _hitTestDistance, ref chunk, out x, out y, out z))
				{
					var position = new Vector3(chunk.position.x, chunk.position.y, chunk.position.z) * ChunkSetting.CHUNK_SIZE + new Vector3(x, y, z);
					Graphics.DrawMesh(mesh, position, Quaternion.identity, material, gameObject.layer, Camera.main);
				}
			}
		}

		private IEnumerator UpdateChunkWithCoroutine()
		{
			yield return new WaitForSeconds(_repeatRateUpdate);

			Vector2Int[] radius = new Vector2Int[] { _chunkRadiusGenX, _chunkRadiusGenY, _chunkRadiusGenZ };

			_terrain.UpdateChunkForDestroy(transform, _chunkRadiusGC);
			_terrain.UpdateChunkForCreate(transform, _terrainGenerator, radius, _chunkNumLimits, _terrainHeightLimitLow, _terrainHeightLimitHigh);

			StartCoroutine("UpdateChunkWithCoroutine");
		}

		private IEnumerator AddEnitiyByScreenPosWithCoroutine()
		{
			if (_isHitTesting)
				yield break;

			_isHitTesting = true;

			_terrain.AddEnitiyByScreenPos(Input.mousePosition, _hitTestDistance, "ChunkGrass");

			yield return new WaitWhile(() => Input.GetMouseButton(1));

			_isHitTesting = false;
		}

		private IEnumerator RemoveEnitiyByScreenPosWithCoroutine()
		{
			if (_isHitTesting)
				yield break;

			_isHitTesting = true;

			_terrain.RemoveEnitiyByScreenPos(Input.mousePosition, _hitTestDistance);

			yield return new WaitWhile(() => Input.GetMouseButton(0));

			_isHitTesting = false;
		}
	}
}