using System.Collections.Generic;

using UnityEngine;

namespace Cubizer
{
	public class VoxelCruncherGreedy : IVoxelCruncher
	{
		private static int[] x = new int[3] { 0, 0, 0 };
		private static int[] q = new int[3] { 0, 0, 0 };

		private static int[] du = new int[3] { 0, 0, 0 };
		private static int[] dv = new int[3] { 0, 0, 0 };

		private static VoxelVisiableFaces faces = new VoxelVisiableFaces(false);

		public void CalcVoxelCruncher(VoxelMaterial[,,] voxels, Bounds bound, ref List<VoxelPrimitive> crunchers)
		{
			var min = new int[] { (int)bound.min.x, (int)bound.min.y, (int)bound.min.z };
			var max = new int[] { (int)bound.max.x, (int)bound.max.y, (int)bound.max.z };

			var alloc = System.Math.Max(max[0], System.Math.Max(max[1], max[2]));

			var mask = new VoxelMaterial[alloc * alloc];
			var mask2 = new bool[alloc * alloc];

			for (var d = 0; d < 3; ++d)
			{
				var u = (d + 1) % 3;
				var v = (d + 2) % 3;

				x[0] = 0; x[1] = 0; x[2] = 0;
				q[0] = 0; q[1] = 0; q[2] = 0;
				q[d] = 1;

				for (x[d] = min[d] - 1; x[d] < max[d];)
				{
					var n = 0;

					for (x[v] = min[v]; x[v] < max[v]; ++x[v])
					{
						for (x[u] = min[u]; x[u] < max[u]; ++x[u])
						{
							bool edge = x[d] < 0 || x[d] + q[d] >= max[d];
							var a = x[d] >= 0 ? voxels[x[0], x[1], x[2]] : null;
							var b = x[d] < max[d] - 1 ? voxels[x[0] + q[0], x[1] + q[1], x[2] + q[2]] : null;
							if (a != b)
							{
								if (a == null)
								{
									if (!b.is_transparent)
									{
										mask2[n] = true;
										mask[n++] = b;
									}
									else
									{
										mask[n++] = null;
									}
								}
								else if (b == null)
								{
									if (!edge || !a.is_transparent)
									{
										mask2[n] = false;
										mask[n++] = a;
									}
									else
									{
										mask[n++] = null;
									}
								}
								else
								{
									mask2[n] = b.is_transparent ? false : true;
									mask[n++] = b.is_transparent ? a : b;
								}
							}
							else
							{
								mask[n++] = null;
							}
						}
					}

					++x[d];

					n = 0;

					for (var j = min[v]; j < max[v]; ++j)
					{
						for (var i = min[u]; i < max[u];)
						{
							var c = mask[n];
							if (c == null)
							{
								++i; ++n;
								continue;
							}

							var w = 1;
							var h = 1;
							var k = 0;

							for (; (i + w) < max[u] && c == mask[n + w]; ++w) { }

							var done = false;
							for (; (j + h) < max[v]; ++h)
							{
								for (k = 0; k < w; ++k)
								{
									if (c != mask[n + k + h * max[u]])
									{
										done = true;
										break;
									}
								}

								if (done)
									break;
							}

							x[u] = i; x[v] = j;

							du[0] = 0; du[1] = 0; du[2] = 0;
							dv[0] = 0; dv[1] = 0; dv[2] = 0;

							du[u] = w;
							dv[v] = h;

							var v1 = new Vector3(x[0], x[1], x[2]);
							var v2 = new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]);

							v2.x = System.Math.Max(v2.x - 1, 0);
							v2.y = System.Math.Max(v2.y - 1, 0);
							v2.z = System.Math.Max(v2.z - 1, 0);

							if (mask2[n])
							{
								faces.front = d == 2;
								faces.back = false;
								faces.left = d == 0;
								faces.right = false;
								faces.top = false;
								faces.bottom = d == 1;
							}
							else
							{
								faces.front = false;
								faces.back = d == 2;
								faces.left = false;
								faces.right = d == 0;
								faces.top = d == 1;
								faces.bottom = false;
							}

							crunchers.Add(new VoxelPrimitive((byte)v1.x, (byte)(v2.x), (byte)(v1.y), (byte)(v2.y), (byte)(v1.z), (byte)(v2.z), faces, c));

							for (var l = 0; l < h; ++l)
							{
								for (k = 0; k < w; ++k)
									mask[n + k + l * max[u]] = null;
							}

							i += w; n += w;
						}
					}
				}
			}
		}

		public VoxelModel CalcVoxelCruncher(VoxelData<VoxelMaterial> voxels)
		{
			var map = new VoxelMaterial[voxels.bound.x, voxels.bound.y, voxels.bound.z];

			for (int i = 0; i < voxels.bound.x; ++i)
			{
				for (int j = 0; j < voxels.bound.y; ++j)
					for (int k = 0; k < voxels.bound.z; ++k)
						map[i, j, k] = null;
			}

			var crunchers = new List<VoxelPrimitive>();
			var faces = new VoxelVisiableFaces();

			Bounds bound = new Bounds();
			foreach (var it in voxels.GetEnumerator())
			{
				if (it.value.is_merge)
				{
					bound.Encapsulate(new Vector3(it.position.x, it.position.y, it.position.z));
					map[it.position.x, it.position.y, it.position.z] = it.value;
				}
				else
					crunchers.Add(new VoxelPrimitive(it.position.x, it.position.x, it.position.y, it.position.y, it.position.z, it.position.z, faces, it.value));
			}

			var max = bound.max;
			max.x = System.Math.Min(max.x + 1, voxels.bound.x);
			max.y = System.Math.Min(max.y + 2, voxels.bound.y);
			max.z = System.Math.Min(max.z + 1, voxels.bound.z);
			bound.max = max;

			CalcVoxelCruncher(map, bound, ref crunchers);

			return new VoxelModel(crunchers);
		}
	}
}