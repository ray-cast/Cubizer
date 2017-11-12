using System.Collections;
using System.Collections.Generic;

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
				if (_material == null)
					_material = new VoxelMaterial(name, is_transparent, is_dynamic, is_merge);
				return _material;
			}
		}

		public void RegisterDefaultMaterial()
		{
			GameResources.RegisterMaterial(this.gameObject.name, gameObject);
		}

		public virtual void Start()
		{
			this.RegisterDefaultMaterial();
		}

		public abstract uint GetVerticesCount(uint faceCount);

		public abstract uint GetIndicesCount(uint faceCount);

		public abstract bool OnUpdateChunk(ref ChunkData map, System.Byte x, System.Byte y, System.Byte z);

		public abstract void OnCreateBlock(ref TerrainMesh mesh, ref int index, Vector3 translate, Vector3 scale, VoxelVisiableFaces faces);
	}
}