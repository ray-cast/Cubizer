using Cubizer.Chunk;

namespace Cubizer
{
	public class PlaneChunkGenerator : IChunkGenerator
	{
		private BasicObjectsParams _params;
		private BasicObjectsMaterials _materials;

		public PlaneChunkGenerator(BasicObjectsParams parameters, BasicObjectsMaterials materials)
		{
			_params = parameters;
			_materials = materials;
		}

		public ChunkPrimer OnCreateChunk(CubizerBehaviour terrain, int x, int y, int z)
		{
			var size = terrain.profile.chunk.settings.chunkSize;
			var map = new ChunkPrimer(size, x, y, z, size * size * _params.floorBase);

			for (byte ix = 0; ix < map.voxels.Bound.x; ix++)
			{
				for (byte iz = 0; iz < map.voxels.Bound.z; iz++)
				{
					for (byte iy = 0; iy < _params.floorBase; iy++)
						map.voxels.Set(ix, iy, iz, _materials.grass);
				}
			}

			return map;
		}
	}
}