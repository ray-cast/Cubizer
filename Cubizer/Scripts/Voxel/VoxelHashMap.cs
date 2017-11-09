using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Cubizer
{
	[Serializable]
	public class VoxelNode<_Tx, _Ty>
		where _Tx : struct
		where _Ty : class
	{
		public _Tx position;
		public _Ty value;

		public VoxelNode()
		{
			value = null;
		}

		public VoxelNode(_Tx x, _Ty value)
		{
			position = x;
			this.value = value;
		}

		public bool is_empty()
		{
			return value == null;
		}
	}

	public class VoxelNodeEnumerable<_Tx, _Ty> : IEnumerable
		where _Tx : struct
		where _Ty : class
	{
		private VoxelNode<_Tx, _Ty>[] _array;

		public VoxelNodeEnumerable(VoxelNode<_Tx, _Ty>[] array)
		{
			_array = array;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}

		public VoxelNodeEnum<_Tx, _Ty> GetEnumerator()
		{
			return new VoxelNodeEnum<_Tx, _Ty>(_array);
		}
	}

	public class VoxelNodeEnum<_Tx, _Ty> : IEnumerator
		where _Tx : struct
		where _Ty : class
	{
		private int position = -1;
		private VoxelNode<_Tx, _Ty>[] _array;

		public VoxelNodeEnum(VoxelNode<_Tx, _Ty>[] list)
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

		public VoxelNode<_Tx, _Ty> Current
		{
			get
			{
				return _array[position];
			}
		}
	}

	[Serializable]
	public class VoxelData<_Element>
		where _Element : class
	{
		protected int _count;
		protected int _allocSize;
		protected Math.Vector3<int> _bound;

		protected VoxelNode<Math.Vector3<System.Byte>, _Element>[] _data;

		public int Count { get { return _count; } }
		public Math.Vector3<int> bound { get { return _bound; } }

		public VoxelData(Math.Vector3<int> bound)
		{
			_count = 0;
			_bound = bound;
			_allocSize = 0;
		}

		public VoxelData(Math.Vector3<int> bound, int allocSize)
		{
			_count = 0;
			_bound = bound;
			_allocSize = 0;
			this.Create(allocSize);
		}

		public VoxelData(int bound_x, int bound_y, int bound_z, int allocSize)
		{
			_count = 0;
			_bound = new Math.Vector3<int>(bound_x, bound_y, bound_z);
			_allocSize = 0;
			this.Create(allocSize);
		}

		public void Create(int allocSize)
		{
			int usage = 1;
			while (usage < allocSize) usage = usage << 1 | 1;

			_count = 0;
			_allocSize = usage;
			_data = new VoxelNode<Math.Vector3<System.Byte>, _Element>[usage + 1];
		}

		public bool Set(System.Byte x, System.Byte y, System.Byte z, _Element value, bool replace = true)
		{
			if (_allocSize == 0)
				this.Create(0xFF);

			var index = HashInt(x, y, z) & _allocSize;
			var entry = _data[index];

			while (entry != null)
			{
				var pos = entry.position;
				if (pos.x == x && pos.y == y && pos.z == z)
				{
					if (replace)
					{
						_data[index].value = value;
						return true;
					}

					return false;
				}

				index = (index + 1) & _allocSize;
				entry = _data[index];
			}

			if (value != null)
			{
				_data[index] = new VoxelNode<Math.Vector3<System.Byte>, _Element>(new Math.Vector3<System.Byte>(x, y, z), value);
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

			var index = HashInt(x, y, z) & _allocSize;
			var entry = _data[index];

			while (entry != null)
			{
				var pos = entry.position;
				if (pos.x == x && pos.y == y && pos.z == z)
				{
					instanceID = entry.value;
					return instanceID != null;
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

		public VoxelNodeEnumerable<Math.Vector3<System.Byte>, _Element> GetEnumerator()
		{
			if (_data == null)
				throw new System.ApplicationException("GetEnumerator: Empty data");

			return new VoxelNodeEnumerable<Math.Vector3<System.Byte>, _Element>(_data);
		}

		public static bool Save(string path, VoxelData<_Element> map)
		{
			UnityEngine.Debug.Assert(map != null);

			var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
			var serializer = new BinaryFormatter();

			serializer.Serialize(stream, map);
			stream.Close();

			return true;
		}

		public static VoxelData<_Element> Load(string path)
		{
			var serializer = new BinaryFormatter();
			var loadFile = new FileStream(path, FileMode.Open, FileAccess.Read);
			return serializer.Deserialize(loadFile) as VoxelData<_Element>;
		}

		private bool Grow(VoxelNode<Math.Vector3<System.Byte>, _Element> data)
		{
			var pos = data.position;
			var index = HashInt(pos.x, pos.y, pos.z) & _allocSize;
			var entry = _data[index];

			while (entry != null)
			{
				index = (index + 1) & _allocSize;
				entry = _data[index];
			}

			if (data.value != null)
			{
				_data[index] = data;
				_count++;

				return true;
			}

			return false;
		}

		private void Grow()
		{
			var map = new VoxelData<_Element>(_bound, _allocSize << 1 | 1);

			foreach (var it in GetEnumerator())
				map.Grow(it);

			_count = map._count;
			_allocSize = map._allocSize;
			_data = map._data;
		}

		private static int _hash_int(int key)
		{
			key = ~key + (key << 15);
			key = key ^ (key >> 12);
			key = key + (key << 2);
			key = key ^ (key >> 4);
			key = key * 2057;
			key = key ^ (key >> 16);
			return key;
		}

		public static int HashInt(int x, int y, int z)
		{
			return _hash_int(x) ^ _hash_int(y) ^ _hash_int(z);
		}
	}

	[Serializable]
	public class VoxelHashMapShort3<_Element>
		where _Element : class
	{
		protected int _count;
		protected int _allocSize;
		protected Math.Vector3<int> _bound;

		protected VoxelNode<Math.Vector3<System.Int16>, _Element>[] _data;

		public int Count { get { return _count; } }
		public Math.Vector3<int> bound { get { return _bound; } }

		public VoxelHashMapShort3(Math.Vector3<int> bound)
		{
			_count = 0;
			_bound = bound;
			_allocSize = 0;
		}

		public VoxelHashMapShort3(Math.Vector3<int> bound, int size)
		{
			_count = 0;
			_bound = bound;
			_allocSize = 0;
			if (size > 0) this.Create(size);
		}

		public VoxelHashMapShort3(int bound_x, int bound_y, int bound_z, int allocSize)
		{
			_count = 0;
			_bound = new Math.Vector3<int>(bound_x, bound_y, bound_z);
			_allocSize = 0;
			this.Create(allocSize);
		}

		public void Create(int allocSize)
		{
			int usage = 1;
			while (usage < allocSize) usage = usage << 1 | 1;

			_count = 0;
			_allocSize = usage;
			_data = new VoxelNode<Math.Vector3<System.Int16>, _Element>[_allocSize + 1];
		}

		public bool Set(System.Int16 x, System.Int16 y, System.Int16 z, _Element value)
		{
			if (_allocSize == 0)
				this.Create(0xFF);

			var index = HashInt(x, y, z) & _allocSize;
			var entry = _data[index];

			while (entry != null)
			{
				var pos = entry.position;
				if (pos.x == x && pos.y == y && pos.z == z)
				{
					_data[index].value = value;

					return true;
				}

				index = (index + 1) & _allocSize;
				entry = _data[index];
			}

			if (value != null)
			{
				_data[index] = new VoxelNode<Math.Vector3<System.Int16>, _Element>(new Math.Vector3<System.Int16>(x, y, z), value);
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

			var index = HashInt(x, y, z) & _allocSize;
			var entry = _data[index];

			while (entry != null)
			{
				var pos = entry.position;
				if (pos.x == x && pos.y == y && pos.z == z)
				{
					instanceID = entry.value;
					return instanceID != null;
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

		public VoxelNodeEnumerable<Math.Vector3<System.Int16>, _Element> GetEnumerator()
		{
			return new VoxelNodeEnumerable<Math.Vector3<System.Int16>, _Element>(_data);
		}

		public static bool Save(string path, VoxelHashMapShort3<_Element> _self)
		{
			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				var serializer = new BinaryFormatter();
				serializer.Serialize(stream, _self);

				stream.Close();

				return true;
			}
		}

		public static VoxelHashMapShort3<_Element> Load(string path)
		{
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				var serializer = new BinaryFormatter();

				return serializer.Deserialize(stream) as VoxelHashMapShort3<_Element>;
			}
		}

		protected bool Grow(VoxelNode<Math.Vector3<System.Int16>, _Element> data)
		{
			var pos = data.position;
			var index = HashInt(pos.x, pos.y, pos.z) & _allocSize;
			var entry = _data[index];

			while (entry != null)
			{
				index = (index + 1) & _allocSize;
				entry = _data[index];
			}

			if (data.value != null)
			{
				_data[index] = data;
				_count++;

				return true;
			}

			return false;
		}

		protected void Grow()
		{
			var map = new VoxelHashMapShort3<_Element>(_bound, _allocSize << 1 | 1);

			foreach (var it in GetEnumerator())
				map.Grow(it);

			_count = map._count;
			_allocSize = map._allocSize;
			_data = map._data;
		}

		private static int _hash_int(int key)
		{
			key = ~key + (key << 15);
			key = key ^ (key >> 12);
			key = key + (key << 2);
			key = key ^ (key >> 4);
			key = key * 2057;
			key = key ^ (key >> 16);
			return key;
		}

		public static int HashInt(int x, int y, int z)
		{
			return _hash_int(x) ^ _hash_int(y) ^ _hash_int(z);
		}
	}
}