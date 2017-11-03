using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace Cubizer
{
	using ChunkPos = System.Int16;
	using ChunkPosition = Cubizer.Math.Vector3<System.Int16>;

	[Serializable]
	public class ChunkTreeManager
	{
		private int _count;
		private byte _size;
		private int _allocSize;
		private ChunkNode<ChunkPosition, ChunkTree>[] _data;

		public int Count { get { return _count; } }
		public byte Size { get { return _size; } }

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
				if (_array == null)
					return false;

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

		public ChunkTreeManager(byte chunkSize)
		{
			_count = 0;
			_size = chunkSize;
			_allocSize = 0;
		}

		public ChunkTreeManager(byte chunkSize, int size)
		{
			_count = 0;
			_size = chunkSize;
			_allocSize = 0;
			if (size > 0) this.Create(size);
		}

		public void Create(int size)
		{
			_count = 0;
			_allocSize = size;
			_data = new ChunkNode<ChunkPosition, ChunkTree>[_allocSize + 1];
		}

		public bool Set(ChunkPos x, ChunkPos y, ChunkPos z, ChunkTree value)
		{
			if (_allocSize == 0)
				this.Create(0xFF);

			var index = ChunkUtility.HashInt(x, y, z) & _allocSize;
			var entry = _data[index];

			while (entry != null)
			{
				var pos = entry.position;
				if (pos.x == x && pos.y == y && pos.z == z)
				{
					var element = _data[index].element;
					if (element != value && element != null)
						element.OnChunkDestroy();

					_data[index].element = value;

					return true;
				}

				index = (index + 1) & _allocSize;
				entry = _data[index];
			}

			if (value != null)
			{
				_data[index] = new ChunkNode<ChunkPosition, ChunkTree>(new ChunkPosition(x, y, z), value);
				_count++;

				if (_count >= _allocSize)
					this.Grow();

				return true;
			}

			return false;
		}

		public bool Set(ChunkPosition pos, ChunkTree value)
		{
			return Set(pos.x, pos.y, pos.z, value);
		}

		public bool Get(ChunkPos x, ChunkPos y, ChunkPos z, ref ChunkTree instanceID)
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

		public bool Get(ChunkPosition pos, ref ChunkTree instanceID)
		{
			return this.Get(pos.x, pos.y, pos.z, ref instanceID);
		}

		public bool Exists(ChunkPos x, ChunkPos y, ChunkPos z)
		{
			ChunkTree instanceID = null;
			return this.Get(x, y, z, ref instanceID);
		}

		public bool Empty()
		{
			return _count == 0;
		}

		public ChunkNodeEnumerable<ChunkPosition, ChunkTree> GetEnumerator()
		{
			return new ChunkNodeEnumerable<ChunkPosition, ChunkTree>(_data);
		}

		public FileStream Save(string path)
		{
			var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
			var serializer = new BinaryFormatter();

			serializer.Serialize(stream, this);
			stream.Close();

			return stream;
		}

		public ChunkTreeManager Load(string path)
		{
			var serializer = new BinaryFormatter();
			var loadFile = new FileStream(path, FileMode.Open, FileAccess.Read);

			return serializer.Deserialize(loadFile) as ChunkTreeManager;
		}

		private bool Grow(ChunkNode<ChunkPosition, ChunkTree> data)
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
			var map = new ChunkTreeManager(_size, _allocSize << 1 | 1);

			foreach (var it in GetEnumerator())
				map.Grow(it);

			_count = map._count;
			_allocSize = map._allocSize;
			_data = map._data;
		}
	}
}