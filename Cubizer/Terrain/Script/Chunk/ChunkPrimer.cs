using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Cubizer.Math;

namespace Cubizer
{
	[Serializable]
	public class ChunkPrimer
	{
		private Vector3<System.Int16> _position;
		private VoxelData<VoxelMaterial> _voxels;

		[NonSerialized]
		private ChunkPrimerDelegates.OnChangeDelegate _onChunkChange;

		[NonSerialized]
		private ChunkPrimerDelegates.OnDestroyDelegate _onChunkDestroy;

		public Vector3<System.Int16> position
		{
			set { _position = value; }
			get { return _position; }
		}

		public VoxelData<VoxelMaterial> voxels
		{
			set { _voxels = value; }
			get { return _voxels; }
		}

		public ChunkPrimerDelegates.OnChangeDelegate onChunkChange
		{
			set { _onChunkChange = value; }
			get { return _onChunkChange; }
		}

		public ChunkPrimerDelegates.OnDestroyDelegate onChunkDestroy
		{
			set { _onChunkDestroy = value; }
			get { return _onChunkDestroy; }
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

		public ChunkPrimer(Vector3<int> bound)
		{
			_voxels = new VoxelData<VoxelMaterial>(bound);
		}

		public ChunkPrimer(Vector3<int> bound, int cout)
		{
			_voxels = new VoxelData<VoxelMaterial>(bound, cout);
		}

		public ChunkPrimer(Vector3<int> bound, Vector3<System.Int16> pos, int cout = 0)
		{
			_position = pos;
			_voxels = new VoxelData<VoxelMaterial>(bound, cout);
		}

		public ChunkPrimer(Vector3<int> bound, System.Int16 x, System.Int16 y, System.Int16 z, int cout = 0)
		{
			_position = new Vector3<System.Int16>(x, y, z);
			_voxels = new VoxelData<VoxelMaterial>(bound, cout);
		}

		public ChunkPrimer(System.Int32 bound_x, System.Int32 bound_y, System.Int32 bound_z, System.Int16 x, System.Int16 y, System.Int16 z, int cout = 0)
		{
			_position = new Vector3<System.Int16>(x, y, z);
			_voxels = new VoxelData<VoxelMaterial>(bound_x, bound_y, bound_z, cout);
		}

		public float GetDistance(int x, int y, int z)
		{
			x = System.Math.Abs(_position.x - x);
			y = System.Math.Abs(_position.y - y);
			z = System.Math.Abs(_position.z - z);
			return System.Math.Max(System.Math.Max(x, y), z);
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
	}
}