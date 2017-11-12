using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Cubizer.Math;

namespace Cubizer
{
	using Vector3Int = Vector3<int>;

	[Serializable]
	public class ChunkData
	{
		[NonSerialized]
		private ChunkDataDelegates.OnDestroyDelegate _onChunkDestroy;

		[NonSerialized]
		private ChunkDataDelegates.OnChunkChangeDelegate _onChunkChange;

		private Vector3<System.Int16> _position;
		private VoxelData<VoxelMaterial> _voxels;

		public Vector3<System.Int16> position { set { _position = value; } get { return _position; } }
		public VoxelData<VoxelMaterial> voxels { set { _voxels = value; } get { return _voxels; } }

		public ChunkDataDelegates.OnChunkChangeDelegate onChunkChange { set { _onChunkChange = value; } get { return _onChunkChange; } }
		public ChunkDataDelegates.OnDestroyDelegate onChunkDestroy { set { _onChunkDestroy = value; } get { return _onChunkDestroy; } }

		public ChunkData(Vector3Int bound)
		{
			_voxels = new VoxelData<VoxelMaterial>(bound);
		}

		public ChunkData(Vector3Int bound, int allocSize)
		{
			_voxels = new VoxelData<VoxelMaterial>(bound, allocSize);
		}

		public ChunkData(Vector3Int bound, Vector3<System.Int16> pos, int allocSize = 0xFF)
		{
			_position = pos;
			_voxels = new VoxelData<VoxelMaterial>(bound, allocSize);
		}

		public ChunkData(Vector3Int bound, System.Int16 x, System.Int16 y, System.Int16 z, int allocSize = 0xFF)
		{
			_position = new Vector3<System.Int16>(x, y, z);
			_voxels = new VoxelData<VoxelMaterial>(bound, allocSize);
		}

		public ChunkData(System.Int32 bound_x, System.Int32 bound_y, System.Int32 bound_z, System.Int16 x, System.Int16 y, System.Int16 z, int allocSize = 0xFF)
		{
			_position = new Vector3<System.Int16>(x, y, z);
			_voxels = new VoxelData<VoxelMaterial>(bound_x, bound_y, bound_z, allocSize);
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

		public static ChunkData Load(string path)
		{
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				return new BinaryFormatter().Deserialize(stream) as ChunkData;
			}
		}

		public static bool Save(string path, ChunkData map)
		{
			UnityEngine.Debug.Assert(map != null);

			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				var serializer = new BinaryFormatter();
				serializer.Serialize(stream, map);
			}

			return true;
		}
	}
}