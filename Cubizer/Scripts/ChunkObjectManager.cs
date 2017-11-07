using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Cubizer
{
	using ChunkPos = System.Byte;
	using ChunkPosition = Math.Vector3<System.Byte>;

	public struct ChunkMesh
	{
		public Color32[] colors;
		public Vector3[] vertices;
		public Vector3[] normals;
		public Vector2[] uv;
		public int[] triangles;
	}

	public struct VisiableFaces
	{
		public bool left;
		public bool right;
		public bool bottom;
		public bool top;
		public bool back;
		public bool front;

		public VisiableFaces(bool _left, bool _right, bool _bottom, bool _top, bool _back, bool _front)
		{
			left = _left;
			right = _right;
			bottom = _bottom;
			top = _top;
			back = _back;
			front = _front;
		}
	}

	public class ChunkObjectManager : MonoBehaviour
	{
		private ChunkTree _chunkMap;

		private float _repeatRateUpdate = 2.0f;
		private List<KeyValuePair<ChunkPosition, ChunkEntity>> _chunkEntitiesDynamic;

		public ChunkTree map
		{
			set
			{
				if (_chunkMap != value)
				{
					if (_chunkMap != null)
						_chunkMap._onChunkChange -= UpdateChunk;

					_chunkMap = value;

					if (_chunkMap != null)
					{
						_chunkMap._onChunkChange += UpdateChunk;
						this.UpdateChunk();
					}
				}
			}
			get
			{
				return _chunkMap;
			}
		}

		public bool GetVisiableFaces(VoxelNode<ChunkPosition, ChunkEntity> it, Math.Vector3<int> size, ref VoxelVisiableFaces faces)
		{
			ChunkEntity[] instanceID = new ChunkEntity[6] { null, null, null, null, null, null };

			var x = it.position.x;
			var y = it.position.y;
			var z = it.position.z;

			if (x >= 1) _chunkMap.Get((byte)(x - 1), y, z, ref instanceID[0]);
			if (y >= 1) _chunkMap.Get(x, (byte)(y - 1), z, ref instanceID[2]);
			if (z >= 1) _chunkMap.Get(x, y, (byte)(z - 1), ref instanceID[4]);
			if (x <= size.x) _chunkMap.Get((byte)(x + 1), y, z, ref instanceID[1]);
			if (y <= size.y) _chunkMap.Get(x, (byte)(y + 1), z, ref instanceID[3]);
			if (z <= size.z) _chunkMap.Get(x, y, (byte)(z + 1), ref instanceID[5]);

			if (it.element.is_transparent)
			{
				var name = it.element.name;

				bool f1 = (instanceID[0] == null) ? true : instanceID[0].name != name ? true : false;
				bool f2 = (instanceID[1] == null) ? true : instanceID[1].name != name ? true : false;
				bool f3 = (instanceID[2] == null) ? true : instanceID[2].name != name ? true : false;
				bool f4 = (instanceID[3] == null) ? true : instanceID[3].name != name ? true : false;
				bool f5 = (instanceID[4] == null) ? true : instanceID[4].name != name ? true : false;
				bool f6 = (instanceID[5] == null) ? true : instanceID[5].name != name ? true : false;

				if (!it.element.is_actor)
				{
					if (x == 0) f1 = false;
					if (z == 0) f5 = false;
					if (x + 1 == size.x) f2 = false;
					if (z + 1 == size.z) f6 = false;
				}

				faces.left = f1;
				faces.right = f2;
				faces.bottom = f3;
				faces.top = f4;
				faces.front = f5;
				faces.back = f6;
			}
			else
			{
				bool f1 = (instanceID[0] == null) ? true : instanceID[0].is_transparent ? true : false;
				bool f2 = (instanceID[1] == null) ? true : instanceID[1].is_transparent ? true : false;
				bool f3 = (instanceID[2] == null) ? true : instanceID[2].is_transparent ? true : false;
				bool f4 = (instanceID[3] == null) ? true : instanceID[3].is_transparent ? true : false;
				bool f5 = (instanceID[4] == null) ? true : instanceID[4].is_transparent ? true : false;
				bool f6 = (instanceID[5] == null) ? true : instanceID[5].is_transparent ? true : false;

				faces.left = f1;
				faces.right = f2;
				faces.bottom = f3;
				faces.top = f4;
				faces.front = f5;
				faces.back = f6;
			}

			if (it.element.is_actor)
			{
				bool all = faces.left | faces.right | faces.bottom | faces.top | faces.front | faces.back;

				faces.left = all;
				faces.right = all;
				faces.bottom = all;
				faces.top = all;
				faces.front = all;
				faces.back = all;
			}

			return faces.left | faces.right | faces.bottom | faces.top | faces.front | faces.back;
		}

		public int CalcFaceCountAsAllocate(Math.Vector3<int> chunkSize, ref Dictionary<string, int> entities)
		{
			var enumerator = _chunkMap.GetEnumerator();
			if (enumerator == null)
				return 0;

			var faces = new VoxelVisiableFaces();

			foreach (var it in enumerator)
			{
				if (GetVisiableFaces(it, chunkSize, ref faces))
				{
					bool[] visiable = new bool[] { faces.left, faces.right, faces.top, faces.bottom, faces.front, faces.back };

					int facesCount = 0;

					for (int j = 0; j < 6; j++)
					{
						if (visiable[j])
							facesCount++;
					}

					if (facesCount > 0)
					{
						if (!entities.ContainsKey(it.element.name))
							entities.Add(it.element.name, facesCount);
						else
							entities[it.element.name] += facesCount;
					}
				}
			}

			return entities.Count;
		}

		public void UpdateChunk()
		{
			for (int i = 0; i < transform.childCount; i++)
				Destroy(transform.GetChild(i).gameObject);

			if (_chunkMap == null || _chunkMap.Count == 0)
				return;

			if (_chunkEntitiesDynamic != null)
			{
				if (_chunkEntitiesDynamic.Count > 0)
				{
					StopCoroutine("OnUpdateEntities");
					_chunkEntitiesDynamic.Clear();
				}
			}

			var entities = new Dictionary<string, int>();
			if (this.CalcFaceCountAsAllocate(map.manager.bound, ref entities) == 0)
				return;

			foreach (var entity in entities)
			{
				var index = 0;
				var faces = new VoxelVisiableFaces();
				var allocSize = entity.Value;

				var data = new ChunkMesh();
				data.vertices = new Vector3[allocSize * 4];
				data.normals = new Vector3[allocSize * 4];
				data.uv = new Vector2[allocSize * 4];
				data.triangles = new int[allocSize * 6];

				bool isTransparent = false;

				int material = 0;

				foreach (var it in _chunkMap.GetEnumerator())
				{
					if (it.element.name != entity.Key)
						continue;

					if (this.GetVisiableFaces(it, map.manager.bound, ref faces))
						it.element.OnCreateBlock(ref data, ref index, new Vector3(it.position.x, it.position.y, it.position.z), Vector3.one, faces);

					if (it.element.is_dynamic)
						_chunkEntitiesDynamic.Add(new KeyValuePair<ChunkPosition, ChunkEntity>(it.position, it.element));

					material = it.element.material;

					isTransparent |= it.element.is_transparent;
				}

				if (data.triangles.GetLength(0) > 0)
				{
					Mesh mesh = new Mesh();
					mesh.vertices = data.vertices;
					mesh.normals = data.normals;
					mesh.uv = data.uv;
					mesh.triangles = data.triangles;

					var gameObject = new GameObject("object");
					gameObject.isStatic = true;
					gameObject.name = entity.Key;
					gameObject.AddComponent<MeshFilter>().mesh = mesh;
					gameObject.AddComponent<MeshRenderer>().material = (Material)Resources.Load(material);
					gameObject.transform.parent = this.transform;
					gameObject.transform.position = this.transform.position;

					if (!isTransparent)
						gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
				}

				if (_chunkEntitiesDynamic.Count > 0)
					StartCoroutine("OnUpdateEntities");
			}
		}

		private IEnumerator OnUpdateEntities()
		{
			yield return new WaitForSeconds(_repeatRateUpdate);

			bool needUpdate = false;
			foreach (var it in _chunkEntitiesDynamic)
				needUpdate |= it.Value.OnUpdateChunk(ref _chunkMap, it.Key);

			if (needUpdate)
				this.UpdateChunk();
			else
				StartCoroutine("OnUpdateEntities");
		}

		private void Awake()
		{
			_chunkEntitiesDynamic = new List<KeyValuePair<ChunkPosition, ChunkEntity>>();
		}

		private void OnDestroy()
		{
			_chunkMap.manager.Set(_chunkMap.position, null);
		}

		private void OnDrawGizmos()
		{
			if (map == null)
				return;

			if (map.position.y == 0)
			{
				var bound = map.bound;

				Vector3 pos = transform.position;
				pos.x += (bound.x - 1) * 0.5f;
				pos.y += (bound.y - 1) * 0.5f;
				pos.z += (bound.z - 1) * 0.5f;

				Gizmos.color = Color.black;
				Gizmos.DrawWireCube(pos, new Vector3(bound.x, bound.y, bound.z));
			}
		}
	}
}