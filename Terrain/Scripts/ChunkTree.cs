using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace Chunk
{
	using ChunkPos = System.Byte;
	using ChunkPosition = ChunkPosition<System.Byte>;
	using ChunkVector3 = ChunkPosition<System.Int16>;

	[Serializable]
	public class ChunkTree
	{
		public delegate void OnChunkChangeDelegate();

		public delegate void OnDestroyDelegate();

		private int _count;
		private int _allocSize;

		private ChunkVector3 _position;
		private ChunkNode<ChunkPosition, ChunkEntity>[] _data;

		[NonSerialized]
		private ChunkTreeManager _manager;

		[NonSerialized]
		public OnChunkChangeDelegate _onChunkChange;

		[NonSerialized]
		public OnDestroyDelegate _onChunkDestroy;

		public ChunkVector3 position { set { _position = value; } get { return _position; } }

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

		public int Count { get { return _count; } }

		[Serializable]
		public class ChunkNode<_Tx, _Ty>
			where _Tx : struct
			where _Ty : class
		{
			public _Tx position;
			public _Ty element;

			public ChunkNode()
			{
				element = null;
			}

			public ChunkNode(_Tx x, _Ty value)
			{
				position = x;
				element = value;
			}

			public bool is_empty()
			{
				return element == null;
			}
		}

		public class ChunkNodeEnumerable<_Tx, _Ty> : IEnumerable
			where _Tx : struct
			where _Ty : class
		{
			private ChunkNode<_Tx, _Ty>[] _array;

			public ChunkNodeEnumerable(ChunkNode<_Tx, _Ty>[] array)
			{
				_array = array;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return (IEnumerator)GetEnumerator();
			}

			public ChunkNodeEnum<_Tx, _Ty> GetEnumerator()
			{
				return new ChunkNodeEnum<_Tx, _Ty>(_array);
			}
		}

		public class ChunkNodeEnum<_Tx, _Ty> : IEnumerator
			where _Tx : struct
			where _Ty : class
		{
			private int position = -1;
			private ChunkNode<_Tx, _Ty>[] _array;

			public ChunkNodeEnum(ChunkNode<_Tx, _Ty>[] list)
			{
				_array = list;
			}

			public bool MoveNext()
			{
				var length = _array.Length;
				for (position++; position < length; position++)
				{
					if (_array[position] == null)
						continue;
					if (_array[position].is_empty())
						continue;
					break;
				}

				return position < _array.Length;
			}

			public void Reset()
			{
				position = -1;
			}

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public ChunkNode<_Tx, _Ty> Current
			{
				get
				{
					return _array[position];
				}
			}
		}

		public ChunkTree()
		{
			_count = 0;
			_allocSize = 0;
			_position = new ChunkVector3(0, 0, 0);
		}

		public ChunkTree(int size)
		{
			_count = 0;
			_allocSize = 0;
			_position = new ChunkVector3(0, 0, 0);
			this.Create(size);
		}

		public ChunkTree(ChunkVector3 pos, int size = 0xFF)
		{
			_count = 0;
			_allocSize = 0;
			_position = pos;
			if (size > 0) this.Create(size);
		}

		public ChunkTree(System.Int16 x, System.Int16 y, System.Int16 z, int size = 0xFF)
		{
			_count = 0;
			_allocSize = 0;
			_position = new ChunkVector3(x, y, z);
			if (size > 0) this.Create(size);
		}

		public void Create(int size)
		{
			_count = 0;
			_allocSize = size;
			_data = new ChunkNode<ChunkPosition, ChunkEntity>[_allocSize + 1];
		}

		public bool Set(ChunkPos x, ChunkPos y, ChunkPos z, ChunkEntity value, bool replace = true)
		{
			UnityEngine.Debug.Assert(x < manager.Size && y < manager.Size && z < manager.Size);

			if (_allocSize == 0)
				this.Create(0xFF);

			var index = ChunkUtility.HashInt(x, y, z) & _allocSize;
			var entry = _data[index];

			while (entry != null)
			{
				var pos = entry.position;
				if (pos.x == x && pos.y == y && pos.z == z)
				{
					if (replace)
					{
						_data[index].element = value;
						return true;
					}

					return false;
				}

				index = (index + 1) & _allocSize;
				entry = _data[index];
			}

			if (value != null)
			{
				_data[index] = new ChunkNode<ChunkPosition, ChunkEntity>(new ChunkPosition(x, y, z), value);
				_count++;

				if (_count >= _allocSize)
					this.Grow();

				return true;
			}

			return false;
		}

		public bool Set(ChunkPosition pos, ChunkEntity instanceID, bool replace = true)
		{
			return this.Set(pos.x, pos.y, pos.z, instanceID, replace);
		}

		public bool Get(ChunkPos x, ChunkPos y, ChunkPos z, ref ChunkEntity instanceID)
		{
			if (_allocSize == 0)
				return false;

			var index = ChunkUtility.HashInt(x, y, z) & _allocSize;
			var entry = _data[index];

			while (entry != null)
			{
				var pos = entry.position;
				if (pos.x == x && pos.y == y && pos.z == z)
				{
					instanceID = entry.element;
					return true;
				}

				index = (index + 1) & _allocSize;
				entry = _data[index];
			}

			instanceID = null;
			return false;
		}

		public bool GetForWrap(int x, int y, int z, ref ChunkEntity instanceID)
		{
			var pos = _position;
			var size = manager.Size;
			var sizeDecOne = manager.Size - 1;

			var ix = x;
			var iy = y;
			var iz = z;

			while (ix < 0) { pos.x--; ix += size; };
			while (iy < 0) { pos.y--; iy += size; };
			while (iz < 0) { pos.z--; iz += size; };
			while (ix > sizeDecOne) { pos.x++; ix -= size; }
			while (iy > sizeDecOne) { pos.y++; iy -= size; }
			while (iz > sizeDecOne) { pos.z++; iz -= size; }

			var chunk = this;
			if (pos.x != _position.x || pos.y != _position.y || pos.z != _position.z)
				manager.Get(pos, ref chunk);

			if (chunk != null)
				return chunk.Get((byte)ix, (byte)iy, (byte)iz, ref instanceID);

			return false;
		}

		public bool Get(ChunkPosition pos, ref ChunkEntity instanceID)
		{
			return this.Get(pos.x, pos.y, pos.z, ref instanceID);
		}

		public bool Exists(ChunkPos x, ChunkPos y, ChunkPos z)
		{
			ChunkEntity instanceID = null;
			return this.Get(x, y, z, ref instanceID);
		}

		public bool Exists(ChunkPosition pos)
		{
			ChunkEntity instanceID = null;
			return this.Get(pos, ref instanceID);
		}

		public bool Empty()
		{
			return _count == 0;
		}

		public float GetDistance(int x, int y, int z)
		{
			x = Mathf.Abs(_position.x - x);
			y = Mathf.Abs(_position.y - y);
			z = Mathf.Abs(_position.z - z);
			return Mathf.Max(Mathf.Max(x, y), z);
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

		public ChunkNodeEnumerable<ChunkPosition, ChunkEntity> GetEnumerator()
		{
			if (_data == null)
				throw new System.ApplicationException("GetEnumerator: Empty data");

			return new ChunkNodeEnumerable<ChunkPosition, ChunkEntity>(_data);
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

		public static ChunkTree Load(string path)
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

		private bool Grow(ChunkNode<ChunkPosition, ChunkEntity> data)
		{
			var pos = data.position;
			var index = ChunkUtility.HashInt(pos.x, pos.y, pos.z) & _allocSize;
			var entry = _data[index];

			while (entry != null)
			{
				index = (index + 1) & _allocSize;
				entry = _data[index];
			}

			if (data.element != null)
			{
				_data[index] = data;
				_count++;

				return true;
			}

			return false;
		}

		private void Grow()
		{
			var map = new ChunkTree(_allocSize << 1 | 1);

			foreach (var it in GetEnumerator())
				map.Grow(it);

			_count = map._count;
			_allocSize = map._allocSize;
			_data = map._data;
		}
	}
}