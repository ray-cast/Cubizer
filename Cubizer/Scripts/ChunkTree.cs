using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cubizer
{
	using Vector3Int = Math.Vector3<int>;

	[Serializable]
	public class ChunkTree : ChunkMapByte3<ChunkEntity>
	{
		public delegate void OnChunkChangeDelegate();

		public delegate void OnDestroyDelegate();

		[NonSerialized]
		private ChunkTreeManager _manager;

		[NonSerialized]
		public OnChunkChangeDelegate _onChunkChange;

		[NonSerialized]
		public OnDestroyDelegate _onChunkDestroy;

		private Math.Vector3<System.Int16> _position;

		public Math.Vector3<System.Int16> position { set { _position = value; } get { return _position; } }

		public ChunkTreeManager manager
		{
			set
			{
				if (_manager != value)
				{
					if (_manager != null)
						_manager.Set(_position, null);

					_manager = value;

					if (_manager != null)
						_manager.Set(_position, this);
				}
			}
			get
			{
				return _manager;
			}
		}

		public ChunkTree(Vector3Int bound)
			: base(bound)
		{
		}

		public ChunkTree(Vector3Int bound, int allocSize)
			: base(bound, allocSize)
		{
		}

		public ChunkTree(Vector3Int bound, Math.Vector3<System.Int16> pos, int allocSize = 0xFF)
			: base(bound, allocSize)
		{
			_position = pos;
		}

		public ChunkTree(Vector3Int bound, System.Int16 x, System.Int16 y, System.Int16 z, int allocSize = 0xFF)
			: base(bound, allocSize)
		{
			_position = new Math.Vector3<System.Int16>(x, y, z);
		}

		public ChunkTree(System.Int32 bound_x, System.Int32 bound_y, System.Int32 bound_z, System.Int16 x, System.Int16 y, System.Int16 z, int allocSize = 0xFF)
			: base(bound_x, bound_y, bound_z, allocSize)
		{
			_position = new Math.Vector3<System.Int16>(x, y, z);
		}

		public bool GetForWrap(int x, int y, int z, ref ChunkEntity instanceID)
		{
			var pos = _position;

			var ix = x;
			var iy = y;
			var iz = z;

			while (ix < 0) { pos.x--; ix += bound.x; };
			while (iy < 0) { pos.y--; iy += bound.y; };
			while (iz < 0) { pos.z--; iz += bound.z; };
			while (ix > (bound.x - 1)) { pos.x++; ix -= bound.x; }
			while (iy > (bound.y - 1)) { pos.y++; iy -= bound.y; }
			while (iz > (bound.z - 1)) { pos.z++; iz -= bound.z; }

			var chunk = this;
			if (pos.x != _position.x || pos.y != _position.y || pos.z != _position.z)
				manager.Get(pos, ref chunk);

			if (chunk != null)
				return chunk.Get((byte)ix, (byte)iy, (byte)iz, ref instanceID);

			return false;
		}

		public float GetDistance(int x, int y, int z)
		{
			x = System.Math.Abs(this._position.x - x);
			y = System.Math.Abs(this._position.y - y);
			z = System.Math.Abs(this._position.z - z);
			return System.Math.Max(System.Math.Max(x, y), z);
		}

		public void OnChunkChange()
		{
			if (_onChunkChange != null)
				_onChunkChange.Invoke();
		}

		public void OnChunkDestroy()
		{
			if (_onChunkDestroy != null)
				_onChunkDestroy.Invoke();
		}

		public static bool Save(string path, ChunkTree map)
		{
			UnityEngine.Debug.Assert(map != null);

			var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
			var serializer = new BinaryFormatter();

			serializer.Serialize(stream, map);
			stream.Close();

			return true;
		}

		public static new ChunkTree Load(string path)
		{
			var serializer = new BinaryFormatter();
			var loadFile = new FileStream(path, FileMode.Open, FileAccess.Read);
			return serializer.Deserialize(loadFile) as ChunkTree;
		}

		public static ChunkTree Load(string path, ChunkTreeManager manager)
		{
			UnityEngine.Debug.Assert(manager != null);

			var serializer = new BinaryFormatter();
			var loadFile = new FileStream(path, FileMode.Open, FileAccess.Read);
			var map = serializer.Deserialize(loadFile) as ChunkTree;
			map.manager = manager;
			return map;
		}
	}
}