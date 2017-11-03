using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEditor;

namespace Cubizer
{
	namespace Model
	{
		public struct MagicalVoxelFileHeader
		{
			public byte[] header;
			public Int32 version;
		}

		public struct MagicalVoxelPack
		{
			public byte[] name;
			public Int32 chunkContent;
			public Int32 chunkNums;
			public Int32 ModelNums;
		}

		public struct MagicalVoxelSize
		{
			public byte[] name;
			public Int32 chunkContent;
			public Int32 chunkNums;
			public Int32 x;
			public Int32 y;
			public Int32 z;
		}

		public struct MagicalVoxelXYZI
		{
			public byte[] name;
			public Int32 chunkContent;
			public Int32 chunkNums;
			public Int32 VoxelNums;
			public byte[] voxels;
		}

		public struct MagicalVoxelChunkChild
		{
			public MagicalVoxelSize size;
			public MagicalVoxelXYZI xyzi;
		}

		public struct MagicalVoxelChunk
		{
			public byte[] name;
			public Int32 chunkContent;
			public Int32 chunkNums;
		}

		public struct MagicalVoxelMaterial
		{
			public int id;
			public int type;
			public float weight;
			public int propertyBits;
			public float[] propertyValue;
		}

		public class MagicalVoxelFile
		{
			public MagicalVoxelFileHeader hdr;
			public MagicalVoxelChunk main;
			public MagicalVoxelPack pack;
			public MagicalVoxelChunkChild[] chunkChild;
			public uint[] palette;
		}

		public class MagicalVoxelFileLoad : ScriptableObject
		{
			private class ChunkColor : ChunkEntity
			{
				public uint palette;

				public ChunkColor(uint _palette)
				{
					name = "MagicalVoxel";
					palette = _palette;
				}
			}

			private static uint[] _paletteDefault = new uint[256]
			{
			0x00000000, 0xffffffff, 0xffccffff, 0xff99ffff, 0xff66ffff, 0xff33ffff, 0xff00ffff, 0xffffccff, 0xffccccff, 0xff99ccff, 0xff66ccff, 0xff33ccff, 0xff00ccff, 0xffff99ff, 0xffcc99ff, 0xff9999ff,
			0xff6699ff, 0xff3399ff, 0xff0099ff, 0xffff66ff, 0xffcc66ff, 0xff9966ff, 0xff6666ff, 0xff3366ff, 0xff0066ff, 0xffff33ff, 0xffcc33ff, 0xff9933ff, 0xff6633ff, 0xff3333ff, 0xff0033ff, 0xffff00ff,
			0xffcc00ff, 0xff9900ff, 0xff6600ff, 0xff3300ff, 0xff0000ff, 0xffffffcc, 0xffccffcc, 0xff99ffcc, 0xff66ffcc, 0xff33ffcc, 0xff00ffcc, 0xffffcccc, 0xffcccccc, 0xff99cccc, 0xff66cccc, 0xff33cccc,
			0xff00cccc, 0xffff99cc, 0xffcc99cc, 0xff9999cc, 0xff6699cc, 0xff3399cc, 0xff0099cc, 0xffff66cc, 0xffcc66cc, 0xff9966cc, 0xff6666cc, 0xff3366cc, 0xff0066cc, 0xffff33cc, 0xffcc33cc, 0xff9933cc,
			0xff6633cc, 0xff3333cc, 0xff0033cc, 0xffff00cc, 0xffcc00cc, 0xff9900cc, 0xff6600cc, 0xff3300cc, 0xff0000cc, 0xffffff99, 0xffccff99, 0xff99ff99, 0xff66ff99, 0xff33ff99, 0xff00ff99, 0xffffcc99,
			0xffcccc99, 0xff99cc99, 0xff66cc99, 0xff33cc99, 0xff00cc99, 0xffff9999, 0xffcc9999, 0xff999999, 0xff669999, 0xff339999, 0xff009999, 0xffff6699, 0xffcc6699, 0xff996699, 0xff666699, 0xff336699,
			0xff006699, 0xffff3399, 0xffcc3399, 0xff993399, 0xff663399, 0xff333399, 0xff003399, 0xffff0099, 0xffcc0099, 0xff990099, 0xff660099, 0xff330099, 0xff000099, 0xffffff66, 0xffccff66, 0xff99ff66,
			0xff66ff66, 0xff33ff66, 0xff00ff66, 0xffffcc66, 0xffcccc66, 0xff99cc66, 0xff66cc66, 0xff33cc66, 0xff00cc66, 0xffff9966, 0xffcc9966, 0xff999966, 0xff669966, 0xff339966, 0xff009966, 0xffff6666,
			0xffcc6666, 0xff996666, 0xff666666, 0xff336666, 0xff006666, 0xffff3366, 0xffcc3366, 0xff993366, 0xff663366, 0xff333366, 0xff003366, 0xffff0066, 0xffcc0066, 0xff990066, 0xff660066, 0xff330066,
			0xff000066, 0xffffff33, 0xffccff33, 0xff99ff33, 0xff66ff33, 0xff33ff33, 0xff00ff33, 0xffffcc33, 0xffcccc33, 0xff99cc33, 0xff66cc33, 0xff33cc33, 0xff00cc33, 0xffff9933, 0xffcc9933, 0xff999933,
			0xff669933, 0xff339933, 0xff009933, 0xffff6633, 0xffcc6633, 0xff996633, 0xff666633, 0xff336633, 0xff006633, 0xffff3333, 0xffcc3333, 0xff993333, 0xff663333, 0xff333333, 0xff003333, 0xffff0033,
			0xffcc0033, 0xff990033, 0xff660033, 0xff330033, 0xff000033, 0xffffff00, 0xffccff00, 0xff99ff00, 0xff66ff00, 0xff33ff00, 0xff00ff00, 0xffffcc00, 0xffcccc00, 0xff99cc00, 0xff66cc00, 0xff33cc00,
			0xff00cc00, 0xffff9900, 0xffcc9900, 0xff999900, 0xff669900, 0xff339900, 0xff009900, 0xffff6600, 0xffcc6600, 0xff996600, 0xff666600, 0xff336600, 0xff006600, 0xffff3300, 0xffcc3300, 0xff993300,
			0xff663300, 0xff333300, 0xff003300, 0xffff0000, 0xffcc0000, 0xff990000, 0xff660000, 0xff330000, 0xff0000ee, 0xff0000dd, 0xff0000bb, 0xff0000aa, 0xff000088, 0xff000077, 0xff000055, 0xff000044,
			0xff000022, 0xff000011, 0xff00ee00, 0xff00dd00, 0xff00bb00, 0xff00aa00, 0xff008800, 0xff007700, 0xff005500, 0xff004400, 0xff002200, 0xff001100, 0xffee0000, 0xffdd0000, 0xffbb0000, 0xffaa0000,
			0xff880000, 0xff770000, 0xff550000, 0xff440000, 0xff220000, 0xff110000, 0xffeeeeee, 0xffdddddd, 0xffbbbbbb, 0xffaaaaaa, 0xff888888, 0xff777777, 0xff555555, 0xff444444, 0xff222222, 0xff111111
			};

			public static MagicalVoxelFile Load(string path)
			{
				var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
				if (stream == null)
					return null;

				BinaryReader reader = new BinaryReader(stream);

				MagicalVoxelFile _voxel = new MagicalVoxelFile();
				_voxel.hdr.header = reader.ReadBytes(4);
				_voxel.hdr.version = reader.ReadInt32();

				if (_voxel.hdr.header[0] != 'V' && _voxel.hdr.header[1] != 'O' && _voxel.hdr.header[2] != 'X' && _voxel.hdr.header[3] != ' ')
					return null;

				if (_voxel.hdr.version != 150)
					return null;

				_voxel.main.name = reader.ReadBytes(4);
				_voxel.main.chunkContent = reader.ReadInt32();
				_voxel.main.chunkNums = reader.ReadInt32();

				if (_voxel.main.name[0] != 'M' && _voxel.main.name[1] != 'A' && _voxel.main.name[2] != 'I' && _voxel.main.name[3] != 'N')
					return null;

				if (_voxel.main.chunkContent != 0)
					return null;

				_voxel.pack.name = reader.ReadBytes(4);
				if (_voxel.pack.name[0] != 'P' && _voxel.pack.name[1] != 'A' && _voxel.pack.name[2] != 'C' && _voxel.pack.name[3] != 'K')
					return null;

				_voxel.pack.chunkContent = reader.ReadInt32();
				_voxel.pack.chunkNums = reader.ReadInt32();
				_voxel.pack.ModelNums = reader.ReadInt32();

				if (_voxel.pack.ModelNums == 0)
					return null;

				_voxel.chunkChild = new MagicalVoxelChunkChild[_voxel.pack.ModelNums];

				for (int i = 0; i < _voxel.pack.ModelNums; i++)
				{
					var chunk = _voxel.chunkChild[i];

					chunk.size.name = reader.ReadBytes(4);
					chunk.size.chunkContent = reader.ReadInt32();
					chunk.size.chunkNums = reader.ReadInt32();
					chunk.size.x = reader.ReadInt32();
					chunk.size.y = reader.ReadInt32();
					chunk.size.z = reader.ReadInt32();

					if (chunk.size.name[0] != 'S' && chunk.size.name[1] != 'I' && chunk.size.name[2] != 'Z' && chunk.size.name[3] != 'E')
						return null;

					if (chunk.size.chunkContent != 12)
						return null;

					chunk.xyzi.name = reader.ReadBytes(4);
					if (chunk.xyzi.name[0] != 'X' && chunk.xyzi.name[1] != 'Y' && chunk.xyzi.name[2] != 'Z' && chunk.xyzi.name[3] != 'I')
						return null;

					chunk.xyzi.chunkContent = reader.ReadInt32();
					chunk.xyzi.chunkNums = reader.ReadInt32();
					chunk.xyzi.VoxelNums = reader.ReadInt32();

					if (chunk.xyzi.chunkNums != 0)
						return null;

					chunk.xyzi.voxels = new byte[chunk.xyzi.VoxelNums * 4];
					if (reader.Read(chunk.xyzi.voxels, 0, chunk.xyzi.voxels.Length) != chunk.xyzi.voxels.Length)
						return null;

					_voxel.chunkChild[i] = chunk;
				}

				_voxel.palette = new uint[256];
				_paletteDefault.CopyTo(_voxel.palette, 0);

				return _voxel;
			}

			public static bool GetVisiableFaces(ChunkTree map, ChunkTree.ChunkNode<Math.Vector3<System.Byte>, ChunkEntity> it, int chunkSize, ref VisiableFaces faces)
			{
				ChunkEntity[] instanceID = new ChunkEntity[6] { null, null, null, null, null, null };

				var x = it.position.x;
				var y = it.position.y;
				var z = it.position.z;

				if (x >= 1) map.Get((byte)(x - 1), y, z, ref instanceID[0]);
				if (y >= 1) map.Get(x, (byte)(y - 1), z, ref instanceID[2]);
				if (z >= 1) map.Get(x, y, (byte)(z - 1), ref instanceID[4]);
				if (x <= chunkSize) map.Get((byte)(x + 1), y, z, ref instanceID[1]);
				if (y <= chunkSize) map.Get(x, (byte)(y + 1), z, ref instanceID[3]);
				if (z <= chunkSize) map.Get(x, y, (byte)(z + 1), ref instanceID[5]);

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
						if (x + 1 == chunkSize) f2 = false;
						if (z + 1 == chunkSize) f6 = false;
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

			public static int CalcFaceCountAsAllocate(ChunkTree map, int chunkSize, ref Dictionary<string, int> entities)
			{
				var enumerator = map.GetEnumerator();
				if (enumerator == null)
					return 0;

				var faces = new VisiableFaces();

				foreach (var it in enumerator)
				{
					if (GetVisiableFaces(map, it, chunkSize, ref faces))
					{
						bool[] visiable = new bool[] { faces.left, faces.right, faces.top, faces.bottom, faces.front, faces.back };

						int facesCount = 0;

						for (int j = 0; j < 6; j++)
						{
							if (visiable[j])
								facesCount++;
						}

						if (facesCount > 0)
						{
							if (!entities.ContainsKey(it.element.name))
								entities.Add(it.element.name, facesCount);
							else
								entities[it.element.name] += facesCount;
						}
					}
				}

				return entities.Count;
			}

			public static GameObject CreateGameObject(MagicalVoxelFile voxel)
			{
				GameObject gameObject = new GameObject();

				for (int i = 0; i < voxel.pack.ModelNums; i++)
				{
					var chunk = voxel.chunkChild[i];

					ChunkTree map = new ChunkTree();

					for (int j = 0; j < chunk.xyzi.VoxelNums * 4; j += 4)
					{
						var x = chunk.xyzi.voxels[j];
						var y = chunk.xyzi.voxels[j + 1];
						var z = chunk.xyzi.voxels[j + 2];
						var c = chunk.xyzi.voxels[j + 3];

						map.Set(x, y, z, new ChunkColor(c));
					}

					Color32[] colors = new Color32[256];
					for (int j = 0; j < voxel.palette.Length; j++)
					{
						uint palette = voxel.palette[j];

						Color32 color;
						color.r = (byte)((palette >> 0) & 0xFF);
						color.g = (byte)((palette >> 8) & 0xFF);
						color.b = (byte)((palette >> 16) & 0xFF);
						color.a = (byte)((palette >> 24) & 0xFF);

						colors[j] = color;
					}

					Texture2D texture = new Texture2D(16, 16, TextureFormat.ARGB32, false, false);
					texture.SetPixels32(colors);
					texture.Apply();

					var entities = new Dictionary<string, int>();
					if (CalcFaceCountAsAllocate(map, 32, ref entities) == 0)
						return null;

					foreach (var entity in entities)
					{
						var index = 0;
						var data = new ChunkMesh();
						var faces = new VisiableFaces();
						var allocSize = entity.Value * 6;

						data.vertices = new Vector3[allocSize];
						data.normals = new Vector3[allocSize];
						data.uv = new Vector2[allocSize];
						data.triangles = new int[allocSize];

						bool isTransparent = false;

						foreach (var it in map.GetEnumerator())
						{
							if (it.element.name != entity.Key)
								continue;

							if (GetVisiableFaces(map, it, 32, ref faces))
								ChunkEntity.CreateCubeMesh(ref data, ref index, faces, it.position, ((ChunkColor)it.element).palette);

							isTransparent |= it.element.is_transparent;
						}

						if (data.triangles.GetLength(0) > 0)
						{
							Mesh mesh = new Mesh();
							mesh.vertices = data.vertices;
							mesh.normals = data.normals;
							mesh.uv = data.uv;
							mesh.triangles = data.triangles;

							gameObject.isStatic = true;
							gameObject.name = entity.Key;
							gameObject.AddComponent<MeshFilter>().mesh = mesh;

							var renderer = gameObject.AddComponent<MeshRenderer>();
#if UNITY_EDITOR
							renderer.sharedMaterial = new Material(Shader.Find("Mobile/Diffuse"));
							renderer.sharedMaterial.mainTexture = texture;
#else
							renderer.material = new Material(Shader.Find("Mobile/Diffuse"));
							renderer.material.mainTexture = texture;
#endif

							if (!isTransparent)
								gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
						}
					}
				}

				return gameObject;
			}
		}
	}
}