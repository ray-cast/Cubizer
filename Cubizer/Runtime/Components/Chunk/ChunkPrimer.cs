using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Cubizer.Math;

namespace Cubizer.Chunk
{
	[Serializable]
	public class ChunkPrimer
	{
		private readonly Vector3<int> _position;
		private readonly VoxelData<VoxelMaterial> _voxels;

		[NonSerialized]
		private OnChangeDelegate _onChunkChange;

		[NonSerialized]
		private OnDestroyDelegate _onChunkDestroy;

		[NonSerialized]
		private OnUpdate _onUpdate;

		private bool _dirty;

		public bool dirty
		{
			get
			{
				return _dirty;
			}
			set
			{
				_dirty = value;
			}
		}

		public Vector3<int> position
		{
			get { return _position; }
		}

		public VoxelData<VoxelMaterial> voxels
		{
			get { return _voxels; }
		}

		public OnChangeDelegate OnChunkChange
		{
			set { _onChunkChange = value; }
			get { return _onChunkChange; }
		}

		public OnDestroyDelegate OnChunkDestroy
		{
			set { _onChunkDestroy = value; }
			get { return _onChunkDestroy; }
		}

		public ChunkPrimer(Vector3<int> bound)
		{
			_voxels = new VoxelData<VoxelMaterial>(bound);
		}

		public ChunkPrimer(Vector3<int> bound, int cout)
		{
			_dirty = true;
			_voxels = new VoxelData<VoxelMaterial>(bound, cout);
		}

		public ChunkPrimer(Vector3<int> bound, Vector3<int> pos, int cout = 0)
		{
			_dirty = true;
			_position = pos;
			_voxels = new VoxelData<VoxelMaterial>(bound, cout);
		}

		public ChunkPrimer(Vector3<int> bound, int x, int y, int z, int cout = 0)
		{
			_dirty = true;
			_position = new Vector3<int>(x, y, z);
			_voxels = new VoxelData<VoxelMaterial>(bound, cout);
		}

		public ChunkPrimer(System.Int32 bound, int x, int y, int z, int cout = 0)
		{
			_dirty = true;
			_position = new Vector3<int>(x, y, z);
			_voxels = new VoxelData<VoxelMaterial>(bound, bound, bound, cout);
		}

		public ChunkPrimer(System.Int32 bound_x, System.Int32 bound_y, System.Int32 bound_z, int x, int y, int z, int cout = 0)
		{
			_dirty = true;
			_position = new Vector3<int>(x, y, z);
			_voxels = new VoxelData<VoxelMaterial>(bound_x, bound_y, bound_z, cout);
		}

		public IVoxelModel CreateVoxelModel(VoxelCullMode mode)
		{
			switch (mode)
			{
				case VoxelCullMode.Stupid:
					return new VoxelCruncherStupid().CalcVoxelCruncher(this.voxels);

				case VoxelCullMode.Culled:
					return new VoxelCruncherCulled().CalcVoxelCruncher(this.voxels);

				case VoxelCullMode.Greedy:
					return new VoxelCruncherGreedy().CalcVoxelCruncher(this.voxels);

				default:
					throw new System.Exception("Bad VoxelCullMode");
			}
		}

		public static ChunkPrimer Load(string path)
		{
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				return new BinaryFormatter().Deserialize(stream) as ChunkPrimer;
			}
		}

		public static bool Save(string path, ChunkPrimer map)
		{
			UnityEngine.Debug.Assert(map != null);

			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				var serializer = new BinaryFormatter();
				serializer.Serialize(stream, map);
			}

			return true;
		}

		public void InvokeOnChunkChange()
		{
			if (_onChunkChange != null)
				_onChunkChange.Invoke();
		}

		public void InvokeOnChunkDestroy()
		{
			if (_onChunkDestroy != null)
				_onChunkDestroy.Invoke();
		}

		public void InvokeOnUpdate()
		{
			if (_onUpdate != null)
				_onUpdate.Invoke();
		}
	}
}