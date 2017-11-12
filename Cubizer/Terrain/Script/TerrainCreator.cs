using System.Collections;

using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Terrain))]
	[AddComponentMenu("Cubizer/TerrainCreator")]
	public class TerrainCreator : MonoBehaviour
	{
		public int _chunkRadiusGC = 6;
		public int _chunkNumLimits = 1024;

		public Vector2Int _chunkRadiusGenX = new Vector2Int(-3, 3);
		public Vector2Int _chunkRadiusGenY = new Vector2Int(-1, 3);
		public Vector2Int _chunkRadiusGenZ = new Vector2Int(-3, 3);

		public int _terrainSeed = 255;
		public int _terrainHeightLimitLow = -10;
		public int _terrainHeightLimitHigh = 20;

		public Camera _camera;
		public GameObject _terrainBiome;
		public GameObject _terrainGenerator;

		public float _repeatRateUpdate = 0.1f;

		private Terrain _terrain;

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
			if (_camera == null)
				UnityEngine.Debug.LogError("Please drag a Camera into Hierarchy View.");

			if (_terrainBiome == null)
				UnityEngine.Debug.LogError("Please drag a TerrainBiome into Hierarchy View.");

			if (_terrainGenerator == null)
				UnityEngine.Debug.LogError("Please drag a TerrainGenerator into Hierarchy View.");

			_terrain = GetComponent<Terrain>();
			_terrainBiome.GetComponent<TerrainBiome>().terrain = _terrain;
			_terrainGenerator.GetComponent<ChunkGeneratorManager>().terrain = _terrain;
		}

		private void Reset()
		{
			StopCoroutine("UpdateChunkWithCoroutine");
		}

		private IEnumerator UpdateChunkWithCoroutine()
		{
			yield return new WaitForSeconds(_repeatRateUpdate);

			if (_terrain)
			{
				_terrain.UpdateChunkForDestroy(transform, _chunkRadiusGC);

				var generator = _terrainGenerator.transform.GetChild(0).gameObject.GetComponent<ChunkGenerator>();
				if (generator)
				{
					_terrain.UpdateChunkForCreate(
						_camera,
						generator,
						new Vector2Int[] { _chunkRadiusGenX, _chunkRadiusGenY, _chunkRadiusGenZ },
						_chunkNumLimits,
						_terrainHeightLimitLow,
						_terrainHeightLimitHigh
						);
				}
			}

			StartCoroutine("UpdateChunkWithCoroutine");
		}
	}
}