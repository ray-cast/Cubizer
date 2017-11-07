using System.Collections;

using UnityEngine;
using UnityEngine.Serialization;

namespace Cubizer
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Terrain))]
	public class TerrainPickCtrl : MonoBehaviour
	{
		public Mesh _drawPickMesh;
		public Material _drawPickMaterial;
		public GameObject _block;

		public bool _isHitTestEnable = true;
		public bool _isHitTestWireframe = true;
		public bool _isHitTesting = false;

		public int _hitTestDistance = 8;

		private Terrain _terrain;

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
					var position = new Vector3(chunk.position.x, chunk.position.y, chunk.position.z) * _terrain._chunkSize + new Vector3(x, y, z);
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
				var entity = new ChunkEntity(_block.name, _block.name, false, false, false);
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