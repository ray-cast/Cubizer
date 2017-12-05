using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Cubizer.Math;

namespace Cubizer
{
	public class VoxelData<_Element>
		where _Element : class
	{
		private VoxelData<_Element> _parent;

		private int _count;
		private int _allocSize;
		private Vector3<int> _bound;
		private VoxelDataNode<Vector3<System.Byte>, _Element>[] _data;

		public int Count { get { return _count; } }
		public Vector3<int> Bound { get { return _bound; } }
		public VoxelData<_Element> Parent { internal set { _parent = value; } get { return _parent; } }

		public VoxelData(Vector3<int> bound)
		{
			_count = 0;
			_bound = bound;
			_allocSize = 0;
		}

		public VoxelData(Vector3<int> bound, int count)
		{
			_count = 0;
			_bound = bound;
			_allocSize = 0;
			this.Create(count);
		}

		public VoxelData(int bound_x, int bound_y, int bound_z, int count)
		{
			_count = 0;
			_bound = new Vector3<int>(bound_x, bound_y, bound_z);
			_allocSize = 0;
			this.Create(count);
		}

		public void Create(int count)
		{
			int usage = 1;
			while (usage < count) usage = usage << 1 | 1;

			_count = 0;
			_allocSize = usage;
			_data = new VoxelDataNode<Vector3<System.Byte>, _Element>[usage + 1];
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
				_data[index] = new VoxelDataNode<Vector3<System.Byte>, _Element>(new Vector3<System.Byte>(x, y, z), value);
				_count++;

				if (_count >= _allocSize)
					this.Grow();

				return true;
			}

			return false;
		}

		public bool Set(Vector3<System.Byte> pos, _Element instanceID, bool replace = true)
		{
			return this.Set(pos.x, pos.y, pos.z, instanceID, replace);
		}

		public bool Get(int x, int y, int z, ref _Element instanceID)
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

		public bool Get(Vector3<System.Byte> pos, ref _Element instanceID)
		{
			return this.Get(pos.x, pos.y, pos.z, ref instanceID);
		}

		public bool Exists(System.Byte x, System.Byte y, System.Byte z)
		{
			_Element instanceID = null;
			return this.Get(x, y, z, ref instanceID);
		}

		public bool Exists(Vector3<System.Byte> pos)
		{
			_Element instanceID = null;
			return this.Get(pos, ref instanceID);
		}

		public bool Empty()
		{
			return _count == 0;
		}

		public VoxelDataNodeEnumerable<Vector3<System.Byte>, _Element> GetEnumerator()
		{
			if (_data == null)
				throw new System.ApplicationException("GetEnumerator: Empty data");

			return new VoxelDataNodeEnumerable<Vector3<System.Byte>, _Element>(_data);
		}

		private bool Grow(VoxelDataNode<Vector3<System.Byte>, _Element> data)
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

		private static int HashInt(int x, int y, int z)
		{
			return _hash_int(x) ^ _hash_int(y) ^ _hash_int(z);
		}
	}
}