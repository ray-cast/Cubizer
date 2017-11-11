using System.Collections;

using UnityEngine;
using UnityEngine.Serialization;

namespace Cubizer
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Terrain))]
	public class TerrainPickCtrl : MonoBehaviour
	{
		[SerializeField] private Mesh _drawPickMesh;
		[SerializeField] private Material _drawPickMaterial;
		[SerializeField] private GameObject _block;

		[SerializeField] private bool _isHitTestEnable = true;
		[SerializeField] private bool _isHitTestWireframe = true;
		[SerializeField] private bool _isHitTesting = false;

		[SerializeField] private int _hitTestDistance = 8;

		private Terrain _terrain;

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

		public GameObject block
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
			if (_drawPickMesh == null)
				UnityEngine.Debug.LogError("Please assign a material on the inspector");

			if (_drawPickMaterial == null)
				UnityEngine.Debug.LogError("Please assign a mesh on the inspector");

			_terrain = this.GetComponent<Terrain>();
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
				ChunkData chunk = null;

				if (_terrain.HitTestByScreenPos(Input.mousePosition, _hitTestDistance, ref chunk, out x, out y, out z))
				{
					var position = new Vector3(chunk.position.x, chunk.position.y, chunk.position.z) * _terrain.chunkSize + new Vector3(x, y, z);
					Graphics.DrawMesh(mesh, position, Quaternion.identity, material, gameObject.layer, Camera.main);
				}
			}
		}

		private IEnumerator AddEnitiyByScreenPosWithCoroutine()
		{
			if (_isHitTesting)
				yield break;

			_isHitTesting = true;

			if (_block != null)
			{
				var entity = new VoxelMaterial(_block.name, _block.name, false, false, false);
				_terrain.AddEnitiyByScreenPos(Input.mousePosition, _hitTestDistance, entity);
			}

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