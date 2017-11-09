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

			var n = 0;
			var faces = new VoxelVisiableFaces(true, true, true, true, true, true);

			foreach (var it in map.GetEnumerator())
			{
				var x = it.position.x;
				var y = it.position.y;
				var z = it.position.z;
				var c = it.value;

				crunchers[n++] = new VoxelCruncher(x, x, z, z, y, y, faces, c);
			}

			return new VoxelModel(crunchers);
		}
	}

	public class VoxelCruncherCulled : IVoxelCruncherStrategy
	{
		public static bool GetVisiableFaces(VoxelMaterial[,,] map, Vector3Int bound, int x, int y, int z, VoxelMaterial material, out VoxelVisiableFaces faces)
		{
			VoxelMaterial[] instanceID = new VoxelMaterial[6] { null, null, null, null, null, null };

			if (x >= 1) instanceID[0] = map[(byte)(x - 1), y, z];
			if (y >= 1) instanceID[2] = map[x, (byte)(y - 1), z];
			if (z >= 1) instanceID[4] = map[x, y, (byte)(z - 1)];
			if (x < bound.x - 1) instanceID[1] = map[(byte)(x + 1), y, z];
			if (y < bound.y - 1) instanceID[3] = map[x, (byte)(y + 1), z];
			if (z < bound.z - 1) instanceID[5] = map[x, y, (byte)(z + 1)];

			if (material.is_transparent)
			{
				var name = material.material;

				bool f1 = (instanceID[0] == null) ? true : instanceID[0].material != name ? true : false;
				bool f2 = (instanceID[1] == null) ? true : instanceID[1].material != name ? true : false;
				bool f3 = (instanceID[2] == null) ? true : instanceID[2].material != name ? true : false;
				bool f4 = (instanceID[3] == null) ? true : instanceID[3].material != name ? true : false;
				bool f5 = (instanceID[4] == null) ? true : instanceID[4].material != name ? true : false;
				bool f6 = (instanceID[5] == null) ? true : instanceID[5].material != name ? true : false;

				if (material.is_merge)
				{
					if (x == 0) f1 = false;
					if (z == 0) f5 = false;
					if (x + 1 == bound.x) f2 = false;
					if (z + 1 == bound.z) f6 = false;
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

			if (!material.is_merge)
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

		public VoxelModel CalcVoxelCruncher(VoxelData<VoxelMaterial> voxels)
		{
			var map = new VoxelMaterial[voxels.bound.x, voxels.bound.y, voxels.bound.z];

			for (int i = 0; i < voxels.bound.x; ++i)
			{
				for (int j = 0; j < voxels.bound.y; ++j)
					for (int k = 0; k < voxels.bound.z; ++k)
						map[i, j, k] = null;
			}

			foreach (var it in voxels.GetEnumerator())
				map[it.position.x, it.position.y, it.position.z] = it.value;

			var crunchers = new List<VoxelCruncher>();
			var bound = new Vector3Int(voxels.bound.x, voxels.bound.y, voxels.bound.z);

			foreach (var it in voxels.GetEnumerator())
			{
				var x = it.position.x;
				var y = it.position.y;
				var z = it.position.z;
				var c = it.value;

				VoxelVisiableFaces faces;
				if (!GetVisiableFaces(map, bound, x, y, z, c, out faces))
					continue;

				crunchers.Add(new VoxelCruncher(x, x, y, y, z, z, faces, c));
			}

			var array = new VoxelCruncher[crunchers.Count];

			int numbers = 0;
			foreach (var it in crunchers)
				array[numbers++] = it;

			return new VoxelModel(array);
		}
	}

	public class VoxelCruncherGreedy : IVoxelCruncherStrategy
	{
		public void CalcVoxelCruncher(VoxelMaterial[,,] voxels, Vector3Int bound, ref List<VoxelCruncher> crunchers)
		{
			var dims = new int[] { bound.x, bound.y, bound.z };

			var alloc = System.Math.Max(dims[0], System.Math.Max(dims[1], dims[2]));

			var mask = new VoxelMaterial[alloc * alloc];
			var mask2 = new bool[alloc * alloc];

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
							bool edge = x[d] < 0 || x[d] + q[d] >= dims[d];
							var a = x[d] >= 0 ? voxels[x[0], x[1], x[2]] : null;
							var b = x[d] < dims[d] - 1 ? voxels[x[0] + q[0], x[1] + q[1], x[2] + q[2]] : null;
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

			var crunchers = new List<VoxelCruncher>();
			var faces = new VoxelVisiableFaces(true, true, true, true, true, true);

			foreach (var it in voxels.GetEnumerator())
			{
				if (it.value.is_merge)
					map[it.position.x, it.position.y, it.position.z] = it.value;
				else
					crunchers.Add(new VoxelCruncher(it.position.x, it.position.x, it.position.y, it.position.y, it.position.z, it.position.z, faces, it.value));
			}

			CalcVoxelCruncher(map, new Vector3Int(voxels.bound.x, voxels.bound.y, voxels.bound.z), ref crunchers);

			var array = new VoxelCruncher[crunchers.Count];

			int numbers = 0;
			foreach (var it in crunchers)
				array[numbers++] = it;

			return new VoxelModel(array);
		}
	}

	public class VoxelPolygonCruncher
	{
		public static VoxelModel CalcVoxelCruncher(VoxelData<VoxelMaterial> map, VoxelCruncherMode mode)
		{
			switch (mode)
			{
				case VoxelCruncherMode.Stupid:
					return new VoxelCruncherStupid().CalcVoxelCruncher(map);

				case VoxelCruncherMode.Culled:
					return new VoxelCruncherCulled().CalcVoxelCruncher(map);

				case VoxelCruncherMode.Greedy:
					return new VoxelCruncherGreedy().CalcVoxelCruncher(map);

				default:
					return null;
			}
		}
	}
}