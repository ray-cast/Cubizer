using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Cubizer
{
	public class ChunkTerrainManager : MonoBehaviour
	{
		public int _chunkSize = ChunkSetting.CHUNK_SIZE;
		public int _chunkNumLimits = 512;
		public int _chunkRadiusGC = 6;

		public Vector2Int _chunkRadiusGenX = new Vector2Int(-3, 3);
		public Vector2Int _chunkRadiusGenY = new Vector2Int(-1, 3);
		public Vector2Int _chunkRadiusGenZ = new Vector2Int(-3, 3);

		public int _terrainSeed = 255;
		public int _terrainHeightLimitLow = -10;
		public int _terrainHeightLimitHigh = 20;

		public bool _isHitTestEnable = true;
		public bool _isHitTestWireframe = true;
		public bool _isHitTesting = false;

		public int _hitTestDistance = 8;

		public float _repeatRateUpdate = 0.1f;

		public Mesh _drawPickMesh;
		public Material _drawPickMaterial;

		private ChunkTreeManager _chunkFactory;
		private GameObject _terrainGenerator;
		private Vector3 _chunkOffset = (Vector3.one * ChunkSetting.CHUNK_SIZE - Vector3.one) * 0.5f;

		public bool HitTestByRay(Ray ray, ref ChunkTree chunk, out byte outX, out byte outY, out byte outZ, ref ChunkTree lastChunk, out byte lastX, out byte lastY, out byte lastZ)
		{
			var chunkX = ChunkUtility.CalcChunkPos(ray.origin.x, _chunkSize);
			var chunkY = ChunkUtility.CalcChunkPos(ray.origin.y, _chunkSize);
			var chunkZ = ChunkUtility.CalcChunkPos(ray.origin.z, _chunkSize);

			lastChunk = null;
			lastX = lastY = lastZ = outX = outY = outZ = 255;

			if (!_chunkFactory.Get(chunkX, chunkY, chunkZ, ref chunk))
				return false;

			Vector3 origin = ray.origin;
			origin.x -= chunk.position.x * _chunkSize;
			origin.y -= chunk.position.y * _chunkSize;
			origin.z -= chunk.position.z * _chunkSize;

			ChunkEntity instanceID = null;

			for (int i = 0; i < _hitTestDistance && instanceID == null; i++)
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

		public bool HitTestByRay(Ray ray, ref ChunkTree chunk, out byte outX, out byte outY, out byte outZ)
		{
			byte lx, ly, lz;
			ChunkTree chunkLast = null;

			return this.HitTestByRay(ray, ref chunk, out outX, out outY, out outZ, ref chunkLast, out lx, out ly, out lz);
		}

		public bool HitTestByScreenPos(Vector3 pos, ref ChunkTree chunk, out byte outX, out byte outY, out byte outZ, ref ChunkTree lastChunk, out byte lastX, out byte lastY, out byte lastZ)
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ray.origin = Camera.main.transform.position;
			return this.HitTestByRay(ray, ref chunk, out outX, out outY, out outZ, ref lastChunk, out lastX, out lastY, out lastZ);
		}

		public bool HitTestByScreenPos(Vector3 pos, ref ChunkTree chunk, out byte outX, out byte outY, out byte outZ)
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ray.origin = Camera.main.transform.position;
			return this.HitTestByRay(ray, ref chunk, out outX, out outY, out outZ);
		}

		public bool AddEnitiyByRay(Ray ray, string className)
		{
			byte x, y, z, lx, ly, lz;
			ChunkTree chunkNow = null;
			ChunkTree chunkLast = null;

			if (HitTestByRay(ray, ref chunkNow, out x, out y, out z, ref chunkLast, out lx, out ly, out lz))
			{
				var grass = GameObject.Find("Grass");
				if (grass != null)
				{
					var chunk = chunkLast != null ? chunkLast : chunkNow;
					chunk.Set(lx, ly, lz, new Cubizer.ChunkEntity(grass.name, grass.GetComponent<MeshRenderer>().material, false, false, false));
					chunk.OnChunkChange();
				}

				return true;
			}

			return false;
		}

		public bool AddEnitiyByScreenPos(Vector3 pos, string className)
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ray.origin = Camera.main.transform.position;
			return this.AddEnitiyByRay(ray, className);
		}

		public bool RemoveEnitiyByRay(Ray ray)
		{
			byte x, y, z;
			ChunkTree chunk = null;

			if (HitTestByRay(ray, ref chunk, out x, out y, out z))
			{
				chunk.Set(x, y, z, null);
				chunk.OnChunkChange();

				return true;
			}

			return false;
		}

		public bool RemoveEnitiyByScreenPos(Vector3 pos)
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			ray.origin = Camera.main.transform.position;
			return this.RemoveEnitiyByRay(ray);
		}

		public bool GetEmptryChunkPos(Vector3 translate, Plane[] planes, out Vector3Int position)
		{
			int x = ChunkUtility.CalcChunkPos(translate.x, _chunkSize);
			int y = ChunkUtility.CalcChunkPos(translate.y, _chunkSize);
			int z = ChunkUtility.CalcChunkPos(translate.z, _chunkSize);

			int bestX = 0, bestY = 0, bestZ = 0;
			int bestScore = int.MaxValue;

			int start = bestScore;

			for (int ix = _chunkRadiusGenX.x; ix <= _chunkRadiusGenX.y; ix++)
			{
				for (int iy = _chunkRadiusGenY.x; iy <= _chunkRadiusGenY.y; iy++)
				{
					for (int iz = _chunkRadiusGenZ.x; iz <= _chunkRadiusGenZ.y; iz++)
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

		private void UpdateChunkForDestroy()
		{
			var length = transform.childCount;

			for (int i = 0; i < length; i++)
			{
				var transformChild = transform.GetChild(i);
				var distance = Vector3.Distance(transformChild.position, Camera.main.transform.position) / _chunkSize;
				if (distance > _chunkRadiusGC)
				{
					transformChild.parent = null;
					Destroy(transformChild.gameObject);
					break;
				}
			}
		}

		private void UpdateChunkForCreate()
		{
			if (_chunkFactory.Count > _chunkNumLimits)
				return;

			var cameraTranslate = Camera.main.transform.position;
			var cameraPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

			Vector3Int position;
			if (!GetEmptryChunkPos(cameraTranslate, cameraPlanes, out position))
				return;

			if (position.y < _terrainHeightLimitLow || position.y > _terrainHeightLimitHigh)
				return;

			if (_terrainGenerator)
			{
				var map = new ChunkTree((short)position.x, (short)position.y, (short)position.z, 0);
				map.manager = _chunkFactory;

				var _terrainTransform = _terrainGenerator.transform;
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

		private IEnumerator UpdateChunkWithCoroutine()
		{
			yield return new WaitForSeconds(_repeatRateUpdate);

			this.UpdateChunkForDestroy();
			this.UpdateChunkForCreate();

			StartCoroutine("UpdateChunkWithCoroutine");
		}

		private IEnumerator AddEnitiyByScreenPosWithCoroutine()
		{
			if (_isHitTesting)
				yield break;

			_isHitTesting = true;

			this.AddEnitiyByScreenPos(Input.mousePosition, "ChunkGrass");

			yield return new WaitWhile(() => Input.GetMouseButton(1));

			_isHitTesting = false;
		}

		private IEnumerator RemoveEnitiyByScreenPosWithCoroutine()
		{
			if (_isHitTesting)
				yield break;

			_isHitTesting = true;

			this.RemoveEnitiyByScreenPos(Input.mousePosition);

			yield return new WaitWhile(() => Input.GetMouseButton(0));

			_isHitTesting = false;
		}

		private void UpdateChunkForHit()
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

				if (HitTestByScreenPos(Input.mousePosition, ref chunk, out x, out y, out z))
				{
					var position = new Vector3(chunk.position.x, chunk.position.y, chunk.position.z) * _chunkSize + new Vector3(x, y, z);
					Graphics.DrawMesh(_drawPickMesh, position, Quaternion.identity, _drawPickMaterial, gameObject.layer, Camera.main);
				}
			}
		}

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
				Debug.LogError("Please assign a material on the inspector");

			if (_drawPickMaterial == null)
				Debug.LogError("Please assign a mesh on the inspector");

			_chunkFactory = new ChunkTreeManager((byte)_chunkSize);
			_terrainGenerator = GameObject.Find("TerrainGenerator");
		}

		private void Reset()
		{
			StopCoroutine("UpdateChunkWithCoroutine");
			StopCoroutine("AddEnitiyByScreenPosWithCoroutine");
			StopCoroutine("RemoveEnitiyByScreenPosWithCoroutine");
		}

		private void Update()
		{
			this.UpdateChunkForHit();
		}
	}
}