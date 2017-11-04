using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public class ChunkTreeManager : ChunkMapShort3<ChunkTree>
	{
		public ChunkTreeManager(Vector3Int size)
			: base(size)
		{
		}

		public ChunkTreeManager(Vector3Int size, int allocSize)
			: base(size, allocSize)
		{
		}

		public new bool Set(System.Int16 x, System.Int16 y, System.Int16 z, ChunkTree value)
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
				_data[index] = new ChunkNode<Math.Vector3<System.Int16>, ChunkTree>(new Math.Vector3<System.Int16>(x, y, z), value);
				_count++;

				if (_count >= _allocSize)
					this.Grow();

				return true;
			}

			return false;
		}

		public new FileStream Save(string path)
		{
			var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
			var serializer = new BinaryFormatter();

			serializer.Serialize(stream, this);
			stream.Close();

			return stream;
		}

		public new ChunkTreeManager Load(string path)
		{
			var serializer = new BinaryFormatter();
			var loadFile = new FileStream(path, FileMode.Open, FileAccess.Read);

			return serializer.Deserialize(loadFile) as ChunkTreeManager;
		}
	}
}