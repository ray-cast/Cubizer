using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Cubizer.Math;

namespace Cubizer
{
	[Serializable]
	public class ChunkDataManager
	{
		private int _count;
		private int _allocSize;
		private int _chunkSize;
		private ChunkDataNode<Vector3<System.Int16>, ChunkPrimer>[] _data;

		public int count { get { return _count; } }
		public int chunkSize { get { return _chunkSize; } }

		public ChunkDataNode<Vector3<System.Int16>, ChunkPrimer>[] data
		{
			get { return _data; }
		}

		public ChunkDataManager(int chunkSize)
		{
			_count = 0;
			_chunkSize = chunkSize;
			_allocSize = 0;
		}

		public ChunkDataManager(int chunkSize, int count)
		{
			_count = 0;
			_chunkSize = chunkSize;
			_allocSize = 0;
			if (count > 0) this.Create(count);
		}

		public void Create(int count)
		{
			int usage = 1;
			while (usage < count) usage = usage << 1 | 1;

			_count = 0;
			_allocSize = usage;
			_data = new ChunkDataNode<Vector3<System.Int16>, ChunkPrimer>[_allocSize + 1];
		}

		public bool Set(System.Int16 x, System.Int16 y, System.Int16 z, ChunkPrimer value)
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
					var element = _data[index].value;
					if (element != value && element != null)
						element.OnChunkDestroy();

					_data[index].value = value;

					return true;
				}

				index = (index + 1) & _allocSize;
				entry = _data[index];
			}

			if (value != null)
			{
				_data[index] = new ChunkDataNode<Vector3<System.Int16>, ChunkPrimer>(new Vector3<System.Int16>(x, y, z), value);
				_count++;

				if (_count >= _allocSize)
					this.Grow();

				return true;
			}

			return false;
		}

		public bool Set(Vector3<System.Int16> pos, ChunkPrimer value)
		{
			return Set(pos.x, pos.y, pos.z, value);
		}

		public bool Get(int x, int y, int z, ref ChunkPrimer chunk)
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

		public bool Get(Vector3<System.Int16> pos, ref ChunkPrimer instanceID)
		{
			return this.Get(pos.x, pos.y, pos.z, ref instanceID);
		}

		public bool Exists(int x, int y, int z)
		{
			ChunkPrimer instanceID = null;
			return this.Get(x, y, z, ref instanceID);
		}

		public bool Empty()
		{
			return _count == 0;
		}

		public int Count()
		{
			return _count;
		}

		public ChunkDataNodeEnumerable<Vector3<System.Int16>, ChunkPrimer> GetEnumerator()
		{
			return new ChunkDataNodeEnumerable<Vector3<System.Int16>, ChunkPrimer>(_data);
		}

		public static bool Save(string path, ChunkDataManager _self)
		{
			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				var serializer = new BinaryFormatter();
				serializer.Serialize(stream, _self);

				return true;
			}
		}

		public static ChunkDataManager Load(string path)
		{
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				var serializer = new BinaryFormatter();
				return serializer.Deserialize(stream) as ChunkDataManager;
			}
		}

		private bool Grow(ChunkDataNode<Vector3<System.Int16>, ChunkPrimer> data)
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
			var map = new ChunkDataManager(_chunkSize, _allocSize << 1 | 1);

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