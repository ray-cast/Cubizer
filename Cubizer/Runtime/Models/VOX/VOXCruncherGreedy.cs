using System.Collections.Generic;

using UnityEngine;

namespace Cubizer.Models
{
	using VOXMaterial = System.Int32;

	public class VOXCruncherGreedy : IVOXCruncherStrategy
	{
		public VOXModel CalcVoxelCruncher(VoxData chunk, Color32[] palette)
		{
			var crunchers = new List<VOXCruncher>();
			var dims = new int[] { chunk.x, chunk.y, chunk.z };

			var alloc = System.Math.Max(dims[0], System.Math.Max(dims[1], dims[2]));
			var mask = new int[alloc * alloc];
			var map = chunk.voxels;

			for (var d = 0; d < 3; ++d)
			{
				var u = (d + 1) % 3;
				var v = (d + 2) % 3;

				var x = new int[3] { 0, 0, 0 };
				var q = new int[3] { 0, 0, 0 };

				q[d] = 1;

				var faces = new VOXVisiableFaces(false, false, false, false, false, false);

				for (x[d] = -1; x[d] < dims[d];)
				{
					var n = 0;

					for (x[v] = 0; x[v] < dims[v]; ++x[v])
					{
						for (x[u] = 0; x[u] < dims[u]; ++x[u])
						{
							var a = x[d] >= 0 ? map[x[0], x[1], x[2]] : VOXMaterial.MaxValue;
							var b = x[d] < dims[d] - 1 ? map[x[0] + q[0], x[1] + q[1], x[2] + q[2]] : VOXMaterial.MaxValue;
							if (a != b)
							{
								if (a == VOXMaterial.MaxValue)
									mask[n++] = b;
								else if (b == VOXMaterial.MaxValue)
									mask[n++] = -a;
								else
									mask[n++] = -b;
							}
							else
							{
								mask[n++] = VOXMaterial.MaxValue;
							}
						}
					}

					++x[d];

					n = 0;

					for (var j = 0; j < dims[v]; ++j)
					{
						for (var i = 0; i < dims[u];)
						{
							var c = mask[n];
							if (c == VOXMaterial.MaxValue)
							{
								++i; ++n;
								continue;
							}

							var w = 1;
							var h = 1;
							var k = 0;

							for (; (i + w) < dims[u] && c == mask[n + w]; ++w) { }

							var done = false;
							for (; (j + h) < dims[v]; ++h)
							{
								for (k = 0; k < w; ++k)
								{
									if (c != mask[n + k + h * dims[u]])
									{
										done = true;
										break;
									}
								}

								if (done)
									break;
							}

							x[u] = i; x[v] = j;

							var du = new int[3] { 0, 0, 0 };
							var dv = new int[3] { 0, 0, 0 };

							du[u] = w;
							dv[v] = h;

							var v1 = new Vector3(x[0], x[1], x[2]);
							var v2 = new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2]);

							v2.x = System.Math.Max(v2.x - 1, 0);
							v2.y = System.Math.Max(v2.y - 1, 0);
							v2.z = System.Math.Max(v2.z - 1, 0);

							if (c > 0)
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
								c = -c;
								faces.front = false;
								faces.back = d == 2;
								faces.left = false;
								faces.right = d == 0;
								faces.top = d == 1;
								faces.bottom = false;
							}

							crunchers.Add(new VOXCruncher((byte)v1.x, (byte)(v2.x), (byte)(v1.y), (byte)(v2.y), (byte)(v1.z), (byte)(v2.z), faces, c));

							for (var l = 0; l < h; ++l)
							{
								for (k = 0; k < w; ++k)
									mask[n + k + l * dims[u]] = VOXMaterial.MaxValue;
							}

							i += w; n += w;
						}
					}
				}
			}

			return new VOXModel(crunchers);
		}
	}
}