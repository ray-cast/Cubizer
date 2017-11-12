using System.Collections;

using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Cubizer/TerrainCreator")]
	public class TerrainCreator : MonoBehaviour
	{
		[SerializeField]
		private Camera _player;

		[SerializeField]
		private Terrain _terrain;

		public int _chunkRadiusGC = 6;
		public int _chunkNumLimits = 1024;

		public Vector2Int _chunkRadiusGenX = new Vector2Int(-3, 3);
		public Vector2Int _chunkRadiusGenY = new Vector2Int(-1, 3);
		public Vector2Int _chunkRadiusGenZ = new Vector2Int(-3, 3);

		public int _terrainHeightLimitLow = -10;
		public int _terrainHeightLimitHigh = 20;

		public float _repeatRateUpdate = 0.1f;

		public Camera player
		{
			set { _player = value; }
			get { return _player; }
		}

		public Terrain terrain
		{
			set { _terrain = value; }
			get { return _terrain; }
		}

		private void Start()
		{
			if (_player == null)
				Debug.LogError("Please assign a camera on the inspector.");

			if (_terrain == null)
				_terrain = GetComponent<Terrain>();

			if (_terrain == null)
				Debug.LogError("Please assign a terrain on the inspector.");
		}

		private void Reset()
		{
			StopCoroutine("UpdateChunkWithCoroutine");
		}

		private void OnEnable()
		{
			StartCoroutine("UpdateChunkWithCoroutine");
		}

		private void OnDisable()
		{
			StopCoroutine("UpdateChunkWithCoroutine");
		}

		private IEnumerator UpdateChunkWithCoroutine()
		{
			yield return new WaitForSeconds(_repeatRateUpdate);

			if (_terrain)
			{
				_terrain.UpdateChunkForDestroy(transform, _chunkRadiusGC);

				if (_player)
				{
					_terrain.UpdateChunkForCreate(
						_player,
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