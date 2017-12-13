using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;

using Cubizer.Math;

namespace Cubizer.Chunk
{
	using ChunkNode = ChunkDataNode<Vector3<int>, ChunkPrimer>;

	[Serializable]
	public class ChunkDataManager : IChunkDataManager
	{
		private int _count = 0;
		private int _allocSize = 0;
		private ChunkNode[] _data;

		public int count { get { return _count; } }

		public ChunkNode[] data
		{
			get { return _data; }
		}

		public ChunkDataManager()
		{
			_count = 0;
			_allocSize = 0;
		}

		public ChunkDataManager(int count)
		{
			_count = 0;
			_allocSize = 0;
			this.Create(count);
		}

		public void Create(int count)
		{
			int usage = 1;
			while (usage < count) usage = usage << 1 | 1;

			_count = 0;
			_allocSize = usage;
			_data = new ChunkNode[_allocSize + 1];
		}

		public void Set(int x, int y, int z, ChunkPrimer value)
		{
			lock (this)
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
							element.InvokeOnChunkDestroy();

						_data[index].value = value;
					}

					index = (index + 1) & _allocSize;
					entry = _data[index];
				}

				if (value != null)
				{
					_data[index] = new ChunkNode(new Vector3<int>(x, y, z), value);
					_count++;

					if (_count >= _allocSize)
						this.Grow();
				}
			}
		}

		public void Set(Vector3<int> pos, ChunkPrimer value)
		{
			Set(pos.x, pos.y, pos.z, value);
		}

		public bool Get(int x, int y, int z, out ChunkPrimer chunk)
		{
			Debug.Assert(_allocSize > 0);

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

		public bool Get(Vector3<int> pos, out ChunkPrimer instanceID)
		{
			return this.Get(pos.x, pos.y, pos.z, out instanceID);
		}

		public bool Exists(int x, int y, int z)
		{
			ChunkPrimer instanceID;
			return this.Get(x, y, z, out instanceID);
		}

		public bool Empty()
		{
			return _count == 0;
		}

		public void GC()
		{
			lock (this)
			{
				var map = new ChunkDataManager();
				map.Create(this._allocSize);

				foreach (ChunkNode it in GetEnumerator())
					map.Grow(it);

				_count = map._count;
				_allocSize = map._allocSize;
				_data = map._data;
			}
		}

		public IEnumerable GetEnumerator()
		{
			return new ChunkDataNodeEnumerable<Vector3<int>, ChunkPrimer>(_data);
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
				var _new = new BinaryFormatter().Deserialize(stream) as ChunkDataManager;
				_count = _new.count;
				_data = _new._data;
				_allocSize = _new._allocSize;

				return true;
			}
		}

		private bool Grow(ChunkNode data)
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
			var map = new ChunkDataManager();
			map.Create(this._allocSize << 1 | 1);

			foreach (ChunkNode it in GetEnumerator())
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