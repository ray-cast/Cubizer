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

		public ChunkPrimer OnCreateChunk(Terrain terrain, short x, short y, short z)
		{
			var size = terrain.profile.terrain.settings.chunkSize;
			var map = new ChunkPrimer(size, x, y, z, size * size * _params.floorBase);

			for (byte ix = 0; ix < map.voxels.bound.x; ix++)
			{
				for (byte iz = 0; iz < map.voxels.bound.z; iz++)
				{
					for (byte iy = 0; iy < _params.floorBase; iy++)
						map.voxels.Set(ix, iy, iz, _materials.grass);
				}
			}

			return map;
		}
	}
}
