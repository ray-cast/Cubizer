using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	public abstract class LiveBehaviour : MonoBehaviour
	{
		public bool _dynamic = false;
		public bool _transparent = false;
		public bool _merge = true;

		public bool is_dynamic { get { return _dynamic; } }
		public bool is_transparent { get { return _transparent; } }
		public bool is_merge { get { return _merge; } }

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

		public abstract bool OnUpdateChunk(ref ChunkData map, List<Math.Vector3<System.Byte>> translate);

		public abstract void OnCreateBlock(ref TerrainMesh mesh, ref int index, Vector3 translate, Vector3 scale, VoxelVisiableFaces faces);
	}
}