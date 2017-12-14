namespace Cubizer.Models
{
	public sealed class VoxData
	{
		public int x, y, z;
		public int[,,] voxels;

		public int count
		{
			get
			{
				int _count = 0;

				for (int i = 0; i < x; ++i)
				{
					for (int j = 0; j < y; ++j)
						for (int k = 0; k < z; ++k)
							if (voxels[i, j, k] != int.MaxValue)
								_count++;
				}

				return _count;
			}
		}

		public VoxData()
		{
			x = 0; y = 0; z = 0;
		}

		public VoxData(byte[] _voxels, int xx, int yy, int zz)
		{
			x = xx;
			y = zz;
			z = yy;
			voxels = new int[x, y, z];

			for (int i = 0; i < x; ++i)
			{
				for (int j = 0; j < y; ++j)
					for (int k = 0; k < z; ++k)
						voxels[i, j, k] = int.MaxValue;
			}

			for (int j = 0; j < _voxels.Length; j += 4)
			{
				var x = _voxels[j];
				var y = _voxels[j + 1];
				var z = _voxels[j + 2];
				var c = _voxels[j + 3];

				voxels[x, z, y] = c;
			}
		}

		public int GetMajorityColorIndex(int xx, int yy, int zz, int lodLevel)
		{
			xx = System.Math.Min(xx * lodLevel, x - 2);
			yy = System.Math.Min(yy * lodLevel, y - 2);
			zz = System.Math.Min(zz * lodLevel, z - 2);

			int[] samples = new int[lodLevel * lodLevel * lodLevel];

			for (int i = 0; i < lodLevel; i++)
			{
				for (int j = 0; j < lodLevel; j++)
				{
					for (int k = 0; k < lodLevel; k++)
					{
						if (xx + i > x - 1 || yy + j > y - 1 || zz + k > z - 1)
							samples[i * lodLevel * lodLevel + j * lodLevel + k] = int.MaxValue;
						else
							samples[i * lodLevel * lodLevel + j * lodLevel + k] = voxels[xx + i, yy + j, zz + k];
					}
				}
			}

			int maxNum = 1;
			int maxNumIndex = 0;

			int[] numIndex = new int[samples.Length];

			for (int i = 0; i < samples.Length; i++)
				numIndex[i] = samples[i] == int.MaxValue ? 0 : 1;

			for (int i = 0; i < samples.Length; i++)
			{
				for (int j = 0; j < samples.Length; j++)
				{
					if (i != j && samples[i] != int.MaxValue && samples[i] == samples[j])
					{
						numIndex[i]++;
						if (numIndex[i] > maxNum)
						{
							maxNum = numIndex[i];
							maxNumIndex = i;
						}
					}
				}
			}

			return samples[maxNumIndex];
		}

		public VoxData GetVoxelDataLOD(int level)
		{
			if (x <= 1 || y <= 1 || z <= 1)
				return null;

			level = System.Math.Max(0, System.Math.Min(level, 16));
			if (level <= 1)
				return this;

			if (x <= level && y <= level && z <= level)
				return this;

			VoxData data = new VoxData();
			data.x = (int)System.Math.Ceiling((float)x / level);
			data.y = (int)System.Math.Ceiling((float)y / level);
			data.z = (int)System.Math.Ceiling((float)z / level);

			data.voxels = new int[data.x, data.y, data.z];

			for (int x = 0; x < data.x; x++)
			{
				for (int y = 0; y < data.y; y++)
				{
					for (int z = 0; z < data.z; z++)
					{
						data.voxels[x, y, z] = this.GetMajorityColorIndex(x, y, z, level);
					}
				}
			}

			return data;
		}
	}
}