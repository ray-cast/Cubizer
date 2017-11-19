using UnityEngine;
using System.Collections;

namespace Cubizer
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Cubizer/TerrainPickCtrl")]
	public class TerrainPickCtrl : MonoBehaviour
	{
		[SerializeField]
		private Camera _player;

		[SerializeField]
		private CubizerBehaviour _terrain;

		[SerializeField]
		private LiveBehaviour _block;

		[SerializeField]
		private Mesh _drawPickMesh;

		[SerializeField]
		private Material _drawPickMaterial;

		[SerializeField] private bool _isHitTestEnable = true;
		[SerializeField] private bool _isHitTestWireframe = true;
		[SerializeField] private bool _isHitTesting = false;

		[SerializeField] private int _hitTestDistance = 8;

		public CubizerBehaviour terrain
		{
			set { _terrain = value; }
			get { return _terrain; }
		}

		public Mesh drawPickMesh
		{
			set { _drawPickMesh = value; }
			get { return _drawPickMesh; }
		}

		public Material drawPickMaterial
		{
			set { _drawPickMaterial = value; }
			get { return _drawPickMaterial; }
		}

		public LiveBehaviour block
		{
			set { _block = value; }
			get { return _block; }
		}

		public bool isHitTestEnable
		{
			set { _isHitTestEnable = value; }
			get { return _isHitTestEnable; }
		}

		public bool isHitTestWireframe
		{
			set { _isHitTestWireframe = value; }
			get { return _isHitTestWireframe; }
		}

		public int hitTestDistance
		{
			set { _hitTestDistance = value; }
			get { return _hitTestDistance; }
		}

		private void Start()
		{
			if (_terrain == null)
				_terrain = GetComponent<CubizerBehaviour>();

			if (_terrain == null)
				Debug.LogError("Please assign a terrain on the inspector");

			if (_player == null)
				Debug.LogError("Please assign a camera on the inspector.");

			if (_drawPickMesh == null)
				Debug.LogError("Please assign a material on the inspector");

			if (_drawPickMaterial == null)
				Debug.LogError("Please assign a mesh on the inspector");
		}

		private void Reset()
		{
			_isHitTesting = false;

			StopCoroutine("AddEnitiyByScreenPosWithCoroutine");
			StopCoroutine("RemoveEnitiyByScreenPosWithCoroutine");
		}

		private void LateUpdate()
		{
			this.UpdateChunkForHit(_drawPickMesh, _drawPickMaterial);
		}

		private void OnApplicationFocus(bool focus)
		{
			_isHitTesting = false;
		}

		private void OnApplicationPause(bool pause)
		{
			_isHitTesting = false;
		}

		private void UpdateChunkForHit(Mesh mesh, Material material)
		{
			if (_isHitTestEnable && Cursor.lockState == CursorLockMode.Locked)
			{
				if (Input.GetMouseButton(0))
					StartCoroutine("RemoveEnitiyByScreenPosWithCoroutine");
				else if (Input.GetMouseButton(1))
					StartCoroutine("AddEnitiyByScreenPosWithCoroutine");
			}

			if (_isHitTestWireframe)
			{
				byte x, y, z;
				ChunkPrimer chunk;

				if (_terrain != null)
				{
					var ray = _player.ScreenPointToRay(Input.mousePosition);
					ray.origin = _player.transform.position;

					if (_terrain.chunkManager.HitTestByRay(ray, _hitTestDistance, out chunk, out x, out y, out z))
					{
						var position = new Vector3(chunk.position.x, chunk.position.y, chunk.position.z) * _terrain.profile.chunk.settings.chunkSize + new Vector3(x, y, z);
						Graphics.DrawMesh(mesh, position, Quaternion.identity, material, gameObject.layer, _player);
					}
				}
			}
		}

		private IEnumerator AddEnitiyByScreenPosWithCoroutine()
		{
			if (_isHitTesting)
				yield break;

			_isHitTesting = true;

			if (_terrain != null && _block != null)
				_terrain.chunkManager.AddBlockByScreenPos(Input.mousePosition, _hitTestDistance, _block.material);

			yield return new WaitWhile(() => Input.GetMouseButton(1));

			_isHitTesting = false;
		}

		private IEnumerator RemoveEnitiyByScreenPosWithCoroutine()
		{
			if (_isHitTesting)
				yield break;

			_isHitTesting = true;

			if (_terrain != null)
				_terrain.chunkManager.RemoveBlockByScreenPos(Input.mousePosition, _hitTestDistance);

			yield return new WaitWhile(() => Input.GetMouseButton(0));

			_isHitTesting = false;
		}
	}
}