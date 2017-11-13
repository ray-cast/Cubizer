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

		[SerializeField]
		private int _chunkRadiusGC = 8;

		[SerializeField] private Vector2Int _chunkRadiusGenX = new Vector2Int(-4, 4);
		[SerializeField] private Vector2Int _chunkRadiusGenY = new Vector2Int(-1, 3);
		[SerializeField] private Vector2Int _chunkRadiusGenZ = new Vector2Int(-4, 4);

		[SerializeField] private float _repeatRateUpdate = 0.1f;

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

		public int chunkRadiusGC
		{
			set { chunkRadiusGC = value; }
			get { return chunkRadiusGC; }
		}

		public Vector2Int chunkRadiusGenX
		{
			set { _chunkRadiusGenX = value; }
			get { return _chunkRadiusGenX; }
		}

		public Vector2Int chunkRadiusGenY
		{
			set { _chunkRadiusGenY = value; }
			get { return _chunkRadiusGenY; }
		}

		public Vector2Int chunkRadiusGenZ
		{
			set { _chunkRadiusGenZ = value; }
			get { return _chunkRadiusGenZ; }
		}

		public float repeatRateUpdate
		{
			set { _repeatRateUpdate = value; }
			get { return _repeatRateUpdate; }
		}

		public void Start()
		{
			if (_player == null)
				Debug.LogError("Please assign a camera on the inspector.");

			if (_terrain == null)
				_terrain = GetComponent<Terrain>();

			if (_terrain == null)
				Debug.LogError("Please assign a terrain on the inspector.");
		}

		public void Reset()
		{
			StopCoroutine("UpdateChunkWithCoroutine");
		}

		public void OnEnable()
		{
			StartCoroutine("UpdateChunkWithCoroutine");
		}

		public void OnDisable()
		{
			StopCoroutine("UpdateChunkWithCoroutine");
		}

		public IEnumerator UpdateChunkWithCoroutine()
		{
			yield return new WaitForSeconds(_repeatRateUpdate);

			if (_terrain)
			{
				if (_player)
				{
					_terrain.UpdateChunkForDestroy(_player, _chunkRadiusGC);
					_terrain.UpdateChunkForCreate(_player, new Vector2Int[] { _chunkRadiusGenX, _chunkRadiusGenY, _chunkRadiusGenZ });
				}
			}

			StartCoroutine("UpdateChunkWithCoroutine");
		}
	}
}