using UnityEngine;

namespace Cubizer
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	public abstract class LiveBehaviour : MonoBehaviour
	{
		[SerializeField] private bool _dynamic = false;
		[SerializeField] private bool _transparent = false;
		[SerializeField] private bool _merge = true;

		public bool is_dynamic { set { _dynamic = value; } get { return _dynamic; } }
		public bool is_transparent { set { _transparent = value; } get { return _transparent; } }
		public bool is_merge { set { _merge = value; } get { return _merge; } }

		private VoxelMaterial _material;

		public VoxelMaterial material
		{
			get
			{
				return _material;
			}
		}

		public void RegisterDefaultMaterial()
		{
			if (_material == null)
				_material = new VoxelMaterial(name, is_transparent, is_dynamic, is_merge);

			LiveResources.RegisterMaterial(this.gameObject.name, this);
		}

		public void Awake()
		{
			this.RegisterDefaultMaterial();
		}

		public abstract int GetVerticesCount(int faceCount);

		public abstract int GetIndicesCount(int faceCount);

		public abstract bool OnUpdateChunk(ref ChunkPrimer map, System.Byte x, System.Byte y, System.Byte z);

		public abstract void OnBuildBlock(ref TerrainMesh mesh, ref int index, Vector3 translate, Vector3 scale, VoxelVisiableFaces faces);

		public abstract void OnBuildComponents(GameObject gameObject, Mesh mesh);
	}
}