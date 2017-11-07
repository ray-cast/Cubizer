using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class ChunkEntity : ICloneable
	{
		private bool _actor;
		private bool _dynamic;
		private bool _transparent;
		private bool _merge;

		private string _name;
		private int _material;

		public bool is_actor { get { return _actor; } }
		public bool is_dynamic { get { return _dynamic; } }
		public bool is_transparent { get { return _transparent; } }
		public bool is_merge { get { return _merge; } }

		public string name { set { _name = value; } get { return _name; } }
		public int material { set { _material = value; } get { return _material; } }

		public ChunkEntity()
		{
			_actor = false;
			_dynamic = false;
			_transparent = false;
		}

		public ChunkEntity(string name, int material, bool transparent = false, bool dynamic = false, bool actor = false, bool merge = true)
		{
			_name = name;
			_material = material;
			_actor = actor;
			_dynamic = dynamic;
			_merge = merge;
			_transparent = transparent;
		}

		public object Clone()
		{
			object obj = null;

			BinaryFormatter inputFormatter = new BinaryFormatter();
			MemoryStream inputStream;

			using (inputStream = new MemoryStream())
			{
				inputFormatter.Serialize(inputStream, this);
			}

			using (MemoryStream outputStream = new MemoryStream(inputStream.ToArray()))
			{
				BinaryFormatter outputFormatter = new BinaryFormatter();
				obj = outputFormatter.Deserialize(outputStream);
			}

			return obj;
		}

		public object Instance()
		{
			return this.MemberwiseClone();
		}

		public virtual int GetVerticesCount(VoxelCruncher<ChunkEntity> it)
		{
			bool[] visiable = new bool[] { it.faces.left, it.faces.right, it.faces.top, it.faces.bottom, it.faces.front, it.faces.back };

			int facesCount = 0;

			for (int j = 0; j < 6; j++)
			{
				if (visiable[j])
					facesCount++;
			}

			return facesCount;
		}

		public virtual void OnCreateBlock(ref ChunkMesh mesh, ref int index, Vector3 pos, Vector3 scale, VoxelVisiableFaces faces)
		{
			VoxelModel<ChunkEntity>.CreateCubeMesh(ref mesh.vertices, ref mesh.normals, ref mesh.uv, ref mesh.triangles, ref index, faces, pos, scale);
		}

		public virtual bool OnUpdateChunk(ref ChunkTree map, Math.Vector3<System.Byte> translate)
		{
			return false;
		}
	}
}