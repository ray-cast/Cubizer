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

	[Serializable]
	public class ChunkMapByte3<_Element>
		where _Element : class
	{
		protected int _count;
		protected int _allocSize;
		protected Vector3Int _bound;

		protected ChunkNode<Math.Vector3<System.Byte>, _Element>[] _data;

		public int Count { get { return _count; } }
		public Vector3Int bound { get { return _bound; } }

		public ChunkMapByte3(Vector3Int bound)
		{
			_count = 0;
			_bound = bound;
			_allocSize = 0;
		}

		public ChunkMapByte3(Vector3Int bound, int allocSize)
		{
			_count = 0;
			_bound = bound;
			_allocSize = 0;
			this.Create(allocSize);
		}

		public ChunkMapByte3(int bound_x, int bound_y, int bound_z, int allocSize)
		{
			_count = 0;
			_bound = new Vector3Int(bound_x, bound_y, bound_z);
			_allocSize = 0;
			this.Create(allocSize);
		}

		public void Create(int allocSize)
		{
			int usage = 1;
			while (usage < allocSize) usage = usage << 1 | 1;

			_count = 0;
			_allocSize = usage;
			_data = new ChunkNode<Math.Vector3<System.Byte>, _Element>[usage + 1];
		}

		public bool Set(System.Byte x, System.Byte y, System.Byte z, _Element value, bool replace = true)
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
				_data[index] = new ChunkNode<Math.Vector3<System.Byte>, _Element>(new Math.Vector3<System.Byte>(x, y, z), value);
				_count++;

				if (_count >= _allocSize)
					this.Grow();

				return true;
			}

			return false;
		}

		public bool Set(Math.Vector3<System.Byte> pos, _Element instanceID, bool replace = true)
		{
			return this.Set(pos.x, pos.y, pos.z, instanceID, replace);
		}

		public bool Get(System.Byte x, System.Byte y, System.Byte z, ref _Element instanceID)
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

		public bool Get(Math.Vector3<System.Byte> pos, ref _Element instanceID)
		{
			return this.Get(pos.x, pos.y, pos.z, ref instanceID);
		}

		public bool Exists(System.Byte x, System.Byte y, System.Byte z)
		{
			_Element instanceID = null;
			return this.Get(x, y, z, ref instanceID);
		}

		public bool Exists(Math.Vector3<System.Byte> pos)
		{
			_Element instanceID = null;
			return this.Get(pos, ref instanceID);
		}

		public bool Empty()
		{
			return _count == 0;
		}

		public ChunkNodeEnumerable<Math.Vector3<System.Byte>, _Element> GetEnumerator()
		{
			if (_data == null)
				throw new System.ApplicationException("GetEnumerator: Empty data");

			return new ChunkNodeEnumerable<Math.Vector3<System.Byte>, _Element>(_data);
		}

		public static bool Save(string path, ChunkMapByte3<_Element> map)
		{
			UnityEngine.Debug.Assert(map != null);

			var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
			var serializer = new BinaryFormatter();

			serializer.Serialize(stream, map);
			stream.Close();

			return true;
		}

		public static ChunkMapByte3<_Element> Load(string path)
		{
			var serializer = new BinaryFormatter();
			var loadFile = new FileStream(path, FileMode.Open, FileAccess.Read);
			return serializer.Deserialize(loadFile) as ChunkMapByte3<_Element>;
		}

		private bool Grow(ChunkNode<Math.Vector3<System.Byte>, _Element> data)
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
			var map = new ChunkMapByte3<_Element>(_bound, _allocSize << 1 | 1);

			foreach (var it in GetEnumerator())
				map.Grow(it);

			_count = map._count;
			_allocSize = map._allocSize;
			_data = map._data;
		}
	}

	[Serializable]
	public class ChunkMapShort3<_Element>
		where _Element : class
	{
		protected int _count;
		protected int _allocSize;
		protected Vector3Int _bound;

		protected ChunkNode<Math.Vector3<System.Int16>, _Element>[] _data;

		public int Count { get { return _count; } }
		public Vector3Int bound { get { return _bound; } }

		public ChunkMapShort3(Vector3Int bound)
		{
			_count = 0;
			_bound = bound;
			_allocSize = 0;
		}

		public ChunkMapShort3(Vector3Int bound, int size)
		{
			_count = 0;
			_bound = bound;
			_allocSize = 0;
			if (size > 0) this.Create(size);
		}

		public void Create(int size)
		{
			_count = 0;
			_allocSize = size;
			_data = new ChunkNode<Math.Vector3<System.Int16>, _Element>[_allocSize + 1];
		}

		public bool Set(System.Int16 x, System.Int16 y, System.Int16 z, _Element value)
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
					_data[index].element = value;

					return true;
				}

				index = (index + 1) & _allocSize;
				entry = _data[index];
			}

			if (value != null)
			{
				_data[index] = new ChunkNode<Math.Vector3<System.Int16>, _Element>(new Math.Vector3<System.Int16>(x, y, z), value);
				_count++;

				if (_count >= _allocSize)
					this.Grow();

				return true;
			}

			return false;
		}

		public bool Set(Math.Vector3<System.Int16> pos, _Element value)
		{
			return Set(pos.x, pos.y, pos.z, value);
		}

		public bool Get(System.Int16 x, System.Int16 y, System.Int16 z, ref _Element instanceID)
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

		public bool Get(Math.Vector3<System.Int16> pos, ref _Element instanceID)
		{
			return this.Get(pos.x, pos.y, pos.z, ref instanceID);
		}

		public bool Exists(System.Int16 x, System.Int16 y, System.Int16 z)
		{
			_Element instanceID = null;
			return this.Get(x, y, z, ref instanceID);
		}

		public bool Empty()
		{
			return _count == 0;
		}

		public ChunkNodeEnumerable<Math.Vector3<System.Int16>, _Element> GetEnumerator()
		{
			return new ChunkNodeEnumerable<Math.Vector3<System.Int16>, _Element>(_data);
		}

		public static bool Save(string path, ChunkMapShort3<_Element> _self)
		{
			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				var serializer = new BinaryFormatter();
				serializer.Serialize(stream, _self);

				stream.Close();

				return true;
			}
		}

		public static ChunkMapShort3<_Element> Load(string path)
		{
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				var serializer = new BinaryFormatter();

				return serializer.Deserialize(stream) as ChunkMapShort3<_Element>;
			}
		}

		protected bool Grow(ChunkNode<Math.Vector3<System.Int16>, _Element> data)
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

		protected void Grow()
		{
			var map = new ChunkMapShort3<_Element>(_bound, _allocSize << 1 | 1);

			foreach (var it in GetEnumerator())
				map.Grow(it);

			_count = map._count;
			_allocSize = map._allocSize;
			_data = map._data;
		}
	}
}