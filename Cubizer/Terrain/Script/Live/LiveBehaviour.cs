using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	public abstract class LiveBehaviour : MonoBehaviour, ILiveBehaviour
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
			_material = VoxelMaterialManager.GetInstance().CreateMaterial(this.name);
			if (_material != null)
			{
				_material.is_transparent = is_transparent;
				_material.is_dynamic = is_dynamic;
				_material.is_merge = is_merge;
				_material.userdata = this;
			}
		}

		public void Awake()
		{
			this.RegisterDefaultMaterial();
		}

		public abstract void OnBuildChunkObject(GameObject parent, IVoxelModel model, int faceCount);
	}
}