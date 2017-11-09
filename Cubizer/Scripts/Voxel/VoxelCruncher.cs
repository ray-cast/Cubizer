using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace Cubizer
{
	public enum VoxelCruncherMode
	{
		Stupid,
		Culled,
		Greedy,
	}

	public struct VoxelVisiableFaces
	{
		public bool left;
		public bool right;
		public bool bottom;
		public bool top;
		public bool back;
		public bool front;

		public VoxelVisiableFaces(bool _left, bool _right, bool _bottom, bool _top, bool _back, bool _front)
		{
			left = _left;
			right = _right;
			bottom = _bottom;
			top = _top;
			back = _back;
			front = _front;
		}
	}

	public class VoxelCruncher
	{
		public byte begin_x;
		public byte begin_y;
		public byte begin_z;

		public byte end_x;
		public byte end_y;
		public byte end_z;

		public VoxelMaterial material;
		public VoxelVisiableFaces faces;

		public VoxelCruncher(byte begin_x, byte end_x, byte begin_y, byte end_y, byte begin_z, byte end_z, VoxelMaterial _material)
		{
			this.begin_x = begin_x;
			this.begin_y = begin_y;
			this.begin_z = begin_z;
			this.end_x = end_x;
			this.end_y = end_y;
			this.end_z = end_z;

			material = _material;

			faces.left = true;
			faces.right = true;
			faces.top = true;
			faces.bottom = true;
			faces.front = true;
			faces.back = true;
		}

		public VoxelCruncher(byte begin_x, byte end_x, byte begin_y, byte end_y, byte begin_z, byte end_z, VoxelVisiableFaces _faces, VoxelMaterial _material)
		{
			this.begin_x = begin_x;
			this.begin_y = begin_y;
			this.begin_z = begin_z;

			this.end_x = end_x;
			this.end_y = end_y;
			this.end_z = end_z;

			material = _material;
			faces = _faces;
		}

		public void GetTranslateScale(out Vector3 pos, out Vector3 scale)
		{
			pos.x = (begin_x + end_x + 1) * 0.5f - 0.5f;
			pos.y = (begin_y + end_y + 1) * 0.5f - 0.5f;
			pos.z = (begin_z + end_z + 1) * 0.5f - 0.5f;

			scale.x = (end_x + 1 - begin_x);
			scale.y = (end_y + 1 - begin_y);
			scale.z = (end_z + 1 - begin_z);
		}
	}

	public interface IVoxelCruncherStrategy
	{
		VoxelModel CalcVoxelCruncher(VoxelData<VoxelMaterial> map);
	}

	public class VoxelCruncherStupid : IVoxelCruncherStrategy
	{
		public VoxelModel CalcVoxelCruncher(VoxelData<VoxelMaterial> map)
		{
			var crunchers = new VoxelCruncher[map.Count];

			var faces = new VoxelVisiableFaces(true, true, true, true, true, true);
			var n = 0;

			foreach (var it in map.GetEnumerator())
			{
				var x = it.position.x;
				var y = it.position.y;
				var z = it.position.z;
				var c = it.element;

				crunchers[n++] = new VoxelCruncher(x, x, z, z, y, y, faces, c);
			}

			return new VoxelModel(crunchers);
		}
	}

	/*public class VoxelCruncherCulled<Material> : IVoxelCruncherStrategy<Material> where Material : class
	{
		public static bool GetVisiableFaces(Material[,,] map, Vector3Int bound, int x, int y, int z, Material material, out VoxelVisiableFaces faces)
		{
			Material[] instanceID = new Material[6] { null, null, null, null, null, null };

			if (x >= 1) instanceID[0] = map[(byte)(x - 1), y, z];
			if (y >= 1) instanceID[2] = map[x, (byte)(y - 1), z];
			if (z >= 1) instanceID[4] = map[x, y, (byte)(z - 1)];
			if (x <= bound.x) instanceID[1] = map[(byte)(x + 1), y, z];
			if (y <= bound.y) instanceID[3] = map[x, (byte)(y + 1), z];
			if (z <= bound.z) instanceID[5] = map[x, y, (byte)(z + 1)];

			if (it.element.is_transparent)
			{
				var name = it.element.name;

				bool f1 = (instanceID[0] == null) ? true : instanceID[0].name != name ? true : false;
				bool f2 = (instanceID[1] == null) ? true : instanceID[1].name != name ? true : false;
				bool f3 = (instanceID[2] == null) ? true : instanceID[2].name != name ? true : false;
				bool f4 = (instanceID[3] == null) ? true : instanceID[3].name != name ? true : false;
				bool f5 = (instanceID[4] == null) ? true : instanceID[4].name != name ? true : false;
				bool f6 = (instanceID[5] == null) ? true : instanceID[5].name != name ? true : false;

				if (!it.element.is_actor)
				{
					if (x == 0) f1 = false;
					if (z == 0) f5 = false;
					if (x + 1 == size.x) f2 = false;
					if (z + 1 == size.z) f6 = false;
				}

				faces.left = f1;
				faces.right = f2;
				faces.bottom = f3;
				faces.top = f4;
				faces.front = f5;
				faces.back = f6;
			}
			else
			{
				bool f1 = (instanceID[0] == null) ? true : instanceID[0].is_transparent ? true : false;
				bool f2 = (instanceID[1] == null) ? true : instanceID[1].is_transparent ? true : false;
				bool f3 = (instanceID[2] == null) ? true : instanceID[2].is_transparent ? true : false;
				bool f4 = (instanceID[3] == null) ? true : instanceID[3].is_transparent ? true : false;
				bool f5 = (instanceID[4] == null) ? true : instanceID[4].is_transparent ? true : false;
				bool f6 = (instanceID[5] == null) ? true : instanceID[5].is_transparent ? true : false;

				faces.left = f1;
				faces.right = f2;
				faces.bottom = f3;
				faces.top = f4;
				faces.front = f5;
				faces.back = f6;
			}

			if (it.element.is_actor)
			{
				bool all = faces.left | faces.right | faces.bottom | faces.top | faces.front | faces.back;

				faces.left = all;
				faces.right = all;
				faces.bottom = all;
				faces.top = all;
				faces.front = all;
				faces.back = all;
			}

			return faces.left | faces.right | faces.bottom | faces.top | faces.front | faces.back;
		}

		public VoxelModel<Material> CalcVoxelCruncher(VoxelData<Material> map)
		{
			var map = new Material[chunk.size.x, chunk.size.z, chunk.size.y];

			for (int i = 0; i < chunk.size.x; ++i)
			{
				for (int j = 0; j < chunk.size.y; ++j)
					for (int k = 0; k < chunk.size.z; ++k)
						map[i, k, j] = null;
			}

			for (int j = 0; j < chunk.xyzi.voxels.Length; j += 4)
			{
				var x = chunk.xyzi.voxels[j];
				var y = chunk.xyzi.voxels[j + 1];
				var z = chunk.xyzi.voxels[j + 2];
				var c = chunk.xyzi.voxels[j + 3];

				map[x, z, y] = c;
			}

			var crunchers = new List<VoxelCruncher>();
			var bound = new Vector3Int(chunk.size.x, chunk.size.z, chunk.size.y);

			for (int j = 0; j < chunk.xyzi.voxels.Length; j += 4)
			{
				var x = chunk.xyzi.voxels[j];
				var y = chunk.xyzi.voxels[j + 1];
				var z = chunk.xyzi.voxels[j + 2];
				var c = chunk.xyzi.voxels[j + 3];

				VoxelVisiableFaces faces;
				if (!GetVisiableFaces(map, bound, x, z, y, c, palette, out faces))
					continue;

				crunchers.Add(new VoxelCruncher(x, x, z, z, y, y, faces, c));
			}

			var array = new VoxelCruncher[crunchers.Count];

			int numbers = 0;
			foreach (var it in crunchers)
				array[numbers++] = it;

			return new VoxelModel<Material>(array);
		}
	}

	public class VoxelCruncherGreedy<Material> : IVoxelCruncherStrategy<Material> where Material : class
	{
		public VoxelModel<Material> CalcVoxelCruncher(VoxelData<Material> map)
		{
			var map = new Material[chunk.size.x, chunk.size.z, chunk.size.y];

			for (int i = 0; i < chunk.size.x; ++i)
			{
				for (int j = 0; j < chunk.size.y; ++j)
					for (int k = 0; k < chunk.size.z; ++k)
						map[i, k, j] = null;
			}

			for (int j = 0; j < chunk.xyzi.voxels.Length; j += 4)
			{
				var x = chunk.xyzi.voxels[j];
				var y = chunk.xyzi.voxels[j + 1];
				var z = chunk.xyzi.voxels[j + 2];
				var c = chunk.xyzi.voxels[j + 3];

				map[x, z, y] = c;
			}

			var crunchers = new List<VoxelCruncher>();
			var dims = new int[] { chunk.size.x, chunk.size.z, chunk.size.y };

			var alloc = System.Math.Max(dims[0], System.Math.Max(dims[1], dims[2]));
			var mask = new Material[alloc * alloc];

			for (var d = 0; d < 3; ++d)
			{
				var u = (d + 1) % 3;
				var v = (d + 2) % 3;

				var x = new int[3] { 0, 0, 0 };
				var q = new int[3] { 0, 0, 0 };

				q[d] = 1;

				var faces = new VoxelVisiableFaces(false, false, false, false, false, false);

				for (x[d] = -1; x[d] < dims[d];)
				{
					var n = 0;

					for (x[v] = 0; x[v] < dims[v]; ++x[v])
					{
						for (x[u] = 0; x[u] < dims[u]; ++x[u])
						{
							var a = x[d] >= 0 ? map[x[0], x[1], x[2]] : null;
							var b = x[d] < dims[d] - 1 ? map[x[0] + q[0], x[1] + q[1], x[2] + q[2]] : null;
							if (a != b)
							{
								if (a == null)
									mask[n++] = b;
								else if (b == null)
									mask[n++] = -a;
								else
									mask[n++] = -b;
							}
							else
							{
								mask[n++] = null;
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
							if (c == null)
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

							crunchers.Add(new VoxelCruncher((byte)v1.x, (byte)(v2.x), (byte)(v1.y), (byte)(v2.y), (byte)(v1.z), (byte)(v2.z), faces, c));

							for (var l = 0; l < h; ++l)
							{
								for (k = 0; k < w; ++k)
									mask[n + k + l * dims[u]] = null;
							}

							i += w; n += w;
						}
					}
				}
			}

			var array = new VoxelCruncher[crunchers.Count];

			int numbers = 0;
			foreach (var it in crunchers)
				array[numbers++] = it;

			return new VoxelModel<Material>(array);
		}
	}

	public class VOXPolygonCruncher
	{
		public static VoxelModel<Material> CalcVoxelCruncher(VoxelData<Material> map, VoxelCruncherMode mode) where Material : class
		{
			switch (mode)
			{
				case VoxelCruncherMode.Stupid:
					return new VoxelCruncherStupid<Material>().CalcVoxelCruncher(map);

				case VoxelCruncherMode.Culled:
					return new VoxelCruncherCulled<Material>().CalcVoxelCruncher(map);

				case VoxelCruncherMode.Greedy:
					return new VoxelCruncherGreedy<Material>().CalcVoxelCruncher(map);

				default:
					return null;
			}
		}
	}*/
}