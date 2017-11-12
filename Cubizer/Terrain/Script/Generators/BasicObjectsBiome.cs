using System.Collections;

using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Cubizer/BasicObjectsBiome")]
	public class BasicObjectsBiome : BiomeGenerator
	{
		public int maxBiomeCount = 2048;

		public int _heightLimitLow = -10;
		public int _heightLimitHigh = 20;

		public Vector2Int _chunkRadiusGenX = new Vector2Int(-6, 6);
		public Vector2Int _chunkRadiusGenY = new Vector2Int(-2, 6);
		public Vector2Int _chunkRadiusGenZ = new Vector2Int(-6, 6);

		public float _repeatRateUpdate = 0.05f;

		public void Start()
		{
			if (this.chunkGenerator == null)
				this.chunkGenerator = GameObject.Find("BaseObjectsGenerator").GetComponent<ChunkGenerator>();

			if (this.chunkGenerator == null)
				UnityEngine.Debug.LogError("Please drag a BaseObjectsGenerator into Hierarchy View.");
		}

		private void Reset()
		{
			StopCoroutine("UpdateBiomeWithCoroutine");
		}

		private new void OnEnable()
		{
			this.InvokeDefaultOnEnable();

			StartCoroutine("UpdateBiomeWithCoroutine");
		}

		private new void OnDestroy()
		{
			this.InvokeDefaultOnDisable();

			StopCoroutine("UpdateBiomeWithCoroutine");
		}

		public override BiomeGenerator OnBuildBiome(short x, short y, short z)
		{
			return this;
		}

		public bool GetEmptryBiome(Vector3 translate, Plane[] planes, Vector2Int[] radius, out Vector3Int position)
		{
			int x = terrain.CalcChunkPos(translate.x);
			int y = terrain.CalcChunkPos(translate.y);
			int z = terrain.CalcChunkPos(translate.z);

			int bestX = 0, bestY = 0, bestZ = 0;
			int bestScore = int.MaxValue;

			int start = bestScore;

			Vector3 _chunkOffset = (Vector3.one * terrain.chunkSize - Vector3.one) * 0.5f;

			for (int ix = radius[0].x; ix <= radius[0].y; ix++)
			{
				for (int iy = radius[1].x; iy <= radius[1].y; iy++)
				{
					for (int iz = radius[2].x; iz <= radius[2].y; iz++)
					{
						int dx = x + ix;
						int dy = y + iy;
						int dz = z + iz;

						var hit = terrain.chunks.Exists((short)dx, (short)dy, (short)dz);
						if (hit)
							continue;

						var p = _chunkOffset + new Vector3(dx, dy, dz) * terrain.chunkSize;

						int invisiable = GeometryUtility.TestPlanesAABB(planes, new Bounds(p, Vector3.one * terrain.chunkSize)) ? 0 : 1;
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

		public void UpdateBiomeForCreate(Camera camera, Vector2Int[] radius, int heightLimitLow, int heightLimitHigh)
		{
			Vector3Int position;
			if (!GetEmptryBiome(camera.transform.position, GeometryUtility.CalculateFrustumPlanes(Camera.main), radius, out position))
				return;

			if (position.y < heightLimitLow || position.y > heightLimitHigh)
				return;

			terrain.biomeManager.biomes.Set((short)position.x, (short)position.y, (short)position.z, this);
		}

		private IEnumerator UpdateBiomeWithCoroutine()
		{
			yield return new WaitForSeconds(_repeatRateUpdate);

			if (this.terrain)
			{
				if (this.terrain.biomeManager.biomes.count < maxBiomeCount)
				{
					var creators = this.terrain.GetComponents<TerrainCreator>();
					foreach (var creator in creators)
					{
						UpdateBiomeForCreate(
							creator.player,
							new Vector2Int[] { _chunkRadiusGenX, _chunkRadiusGenY, _chunkRadiusGenZ },
							_heightLimitLow,
							_heightLimitHigh
							);
					}
				}
			}

			StartCoroutine("UpdateBiomeWithCoroutine");
		}
	}
}