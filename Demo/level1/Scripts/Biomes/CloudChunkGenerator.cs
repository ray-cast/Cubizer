using Cubizer.Chunk;

namespace Cubizer
{
	public class CloudChunkGenerator : IChunkGenerator
	{
		private BasicObjectsParams _params;
		private BasicObjectsMaterials _materials;

		public CloudChunkGenerator(BasicObjectsParams parameters, BasicObjectsMaterials materials)
		{
			_params = parameters;
			_materials = materials;
		}

		public ChunkPrimer OnCreateChunk(CubizerBehaviour terrain, int x, int y, int z)
		{
			var map = new ChunkPrimer(terrain.profile.chunk.settings.chunkSize, (short)x, (short)y, (short)z);

			int offsetX = x * map.voxels.Bound.x;
			int offsetY = y * map.voxels.Bound.y;
			int offsetZ = z * map.voxels.Bound.z;

			for (int ix = 0; ix < map.voxels.Bound.x; ix++)
			{
				for (int iz = 0; iz < map.voxels.Bound.y; iz++)
				{
					int dx = offsetX + ix;
					int dz = offsetZ + iz;

					for (int iy = 0; iy < 8; iy++)
					{
						int dy = offsetY + iy;

						if (Math.Noise.simplex3(dx * 0.01f, dy * 0.1f, dz * 0.01f, 8, 0.5f, 2) > _params.thresholdCloud)
							map.voxels.Set((byte)ix, (byte)iy, (byte)iz, _materials.cloud);
					}
				}
			}

			return map;
		}
	}
}