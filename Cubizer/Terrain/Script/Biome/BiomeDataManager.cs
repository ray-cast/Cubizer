using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Cubizer.Math;

namespace Cubizer
{
	[Serializable]
	public class BiomeDataManager : IBiomeDataManager
	{
		private int _count;
		private int _allocSize;

		private BiomeDataNode<Vector3<int>, IBiomeData>[] _data;

		public int count
		{
			get { return _count; }
		}

		public BiomeDataManager()
		{
			_count = 0;
			_allocSize = 0;
		}

		public BiomeDataManager(int count)
		{
			_count = 0;
			_allocSize = 0;
			if (count > 0) this.Create(count);
		}

		public void Create(int count)
		{
			int usage = 1;
			while (usage < count) usage = usage << 1 | 1;

			_count = 0;
			_allocSize = usage;
			_data = new BiomeDataNode<Vector3<int>, IBiomeData>[_allocSize + 1];
		}

		public bool Set(int x, int y, int z, IBiomeData value)
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
				_data[index] = new BiomeDataNode<Vector3<int>, IBiomeData>(new Vector3<int>(x, y, z), value);
				_count++;

				if (_count >= _allocSize)
					this.Grow();

				return true;
			}

			return false;
		}

		public bool Set(Vector3<int> pos, BiomeData value)
		{
			return Set(pos.x, pos.y, pos.z, value);
		}

		public bool Get(int x, int y, int z, ref IBiomeData chunk)
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
					chunk = entry.value;
					return chunk != null;
				}

				index = (index + 1) & _allocSize;
				entry = _data[index];
			}

			chunk = null;
			return false;
		}

		public bool Get(Vector3<int> pos, ref IBiomeData instanceID)
		{
			return this.Get(pos.x, pos.y, pos.z, ref instanceID);
		}

		public bool Exists(int x, int y, int z)
		{
			IBiomeData instanceID = null;
			return this.Get(x, y, z, ref instanceID);
		}

		public bool Empty()
		{
			return _count == 0;
		}

		public void GC()
		{
			var map = new BiomeDataManager(_allocSize);

			foreach (var it in GetEnumerator())
				map.Grow(it);

			_count = map._count;
			_allocSize = map._allocSize;
			_data = map._data;
		}

		public BiomeDataNodeEnumerable<Vector3<int>, IBiomeData> GetEnumerator()
		{
			return new BiomeDataNodeEnumerable<Vector3<int>, IBiomeData>(_data);
		}

		public bool Save(string path)
		{
			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				var serializer = new BinaryFormatter();
				serializer.Serialize(stream, this);

				return true;
			}
		}

		public bool Load(string path)
		{
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				var _new = new BinaryFormatter().Deserialize(stream) as BiomeDataManager;

				this._count = _new._count;
				this._data = _new._data;
				this._allocSize = _new._allocSize;

				return true;
			}
		}

		private bool Grow(BiomeDataNode<Vector3<int>, IBiomeData> data)
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
			var map = new BiomeDataManager(_allocSize << 1 | 1);

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