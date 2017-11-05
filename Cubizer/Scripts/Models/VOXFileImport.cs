using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;

using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Cubizer
{
	namespace Model
	{
		public struct VoxFileHeader
		{
			public byte[] header;
			public Int32 version;
		}

		public struct VoxFilePack
		{
			public byte[] name;
			public Int32 chunkContent;
			public Int32 chunkNums;
			public Int32 modelNums;
		}

		public struct VoxFileSize
		{
			public byte[] name;
			public Int32 chunkContent;
			public Int32 chunkNums;
			public Int32 x;
			public Int32 y;
			public Int32 z;
		}

		public struct VoxFileXYZI
		{
			public byte[] name;
			public Int32 chunkContent;
			public Int32 chunkNums;
			public Int32 voxelNums;
			public byte[] voxels;
		}

		public struct VoxFileRGBA
		{
			public byte[] name;
			public Int32 chunkContent;
			public Int32 chunkNums;
			public uint[] values;
		}

		public struct VoxFileChunkChild
		{
			public VoxFileSize size;
			public VoxFileXYZI xyzi;
		}

		public struct VoxFileChunk
		{
			public byte[] name;
			public Int32 chunkContent;
			public Int32 chunkNums;
		}

		public struct VoxFileMaterial
		{
			public int id;
			public int type;
			public float weight;
			public int propertyBits;
			public float[] propertyValue;
		}

		public class VoxFileData
		{
			public VoxFileHeader hdr;
			public VoxFileChunk main;
			public VoxFilePack pack;
			public VoxFileChunkChild[] chunkChild;
			public VoxFileRGBA palette;
		}

		public class VoxFileImport
		{
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

			public static VoxFileData Load(string path)
			{
				using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
				{
					if (stream == null)
						return null;

					using (var reader = new BinaryReader(stream))
					{
						VoxFileData voxel = new VoxFileData();
						voxel.hdr.header = reader.ReadBytes(4);
						voxel.hdr.version = reader.ReadInt32();

						if (voxel.hdr.header[0] != 'V' || voxel.hdr.header[1] != 'O' || voxel.hdr.header[2] != 'X' || voxel.hdr.header[3] != ' ')
							return null;

						if (voxel.hdr.version != 150)
							return null;

						voxel.main.name = reader.ReadBytes(4);
						voxel.main.chunkContent = reader.ReadInt32();
						voxel.main.chunkNums = reader.ReadInt32();

						if (voxel.main.name[0] != 'M' || voxel.main.name[1] != 'A' || voxel.main.name[2] != 'I' || voxel.main.name[3] != 'N')
							return null;

						if (voxel.main.chunkContent != 0)
							return null;

						if (reader.PeekChar() == 'P')
						{
							voxel.pack.name = reader.ReadBytes(4);
							if (voxel.pack.name[0] != 'P' || voxel.pack.name[1] != 'A' || voxel.pack.name[2] != 'C' || voxel.pack.name[3] != 'K')
								return null;

							voxel.pack.chunkContent = reader.ReadInt32();
							voxel.pack.chunkNums = reader.ReadInt32();
							voxel.pack.modelNums = reader.ReadInt32();

							if (voxel.pack.modelNums == 0)
								return null;
						}
						else
						{
							voxel.pack.chunkContent = 0;
							voxel.pack.chunkNums = 0;
							voxel.pack.modelNums = 1;
						}

						voxel.chunkChild = new VoxFileChunkChild[voxel.pack.modelNums];

						for (int i = 0; i < voxel.pack.modelNums; i++)
						{
							var chunk = new VoxFileChunkChild();

							chunk.size.name = reader.ReadBytes(4);
							chunk.size.chunkContent = reader.ReadInt32();
							chunk.size.chunkNums = reader.ReadInt32();
							chunk.size.x = reader.ReadInt32();
							chunk.size.y = reader.ReadInt32();
							chunk.size.z = reader.ReadInt32();

							if (chunk.size.name[0] != 'S' || chunk.size.name[1] != 'I' || chunk.size.name[2] != 'Z' || chunk.size.name[3] != 'E')
								return null;

							if (chunk.size.chunkContent != 12)
								return null;

							chunk.xyzi.name = reader.ReadBytes(4);
							if (chunk.xyzi.name[0] != 'X' || chunk.xyzi.name[1] != 'Y' || chunk.xyzi.name[2] != 'Z' || chunk.xyzi.name[3] != 'I')
								return null;

							chunk.xyzi.chunkContent = reader.ReadInt32();
							chunk.xyzi.chunkNums = reader.ReadInt32();
							chunk.xyzi.voxelNums = reader.ReadInt32();

							if (chunk.xyzi.chunkNums != 0)
								return null;

							chunk.xyzi.voxels = new byte[chunk.xyzi.voxelNums * 4];
							if (reader.Read(chunk.xyzi.voxels, 0, chunk.xyzi.voxels.Length) != chunk.xyzi.voxels.Length)
								return null;

							voxel.chunkChild[i] = chunk;
						}

						if (reader.BaseStream.Position < reader.BaseStream.Length)
						{
							byte[] palette = reader.ReadBytes(4);
							if (palette[0] == 'R' && palette[1] == 'G' && palette[2] == 'B' && palette[3] == 'A')
							{
								voxel.palette.chunkContent = reader.ReadInt32();
								voxel.palette.chunkNums = reader.ReadInt32();

								var bytePalette = new byte[voxel.palette.chunkContent];
								reader.Read(bytePalette, 0, voxel.palette.chunkContent);

								voxel.palette.values = new uint[voxel.palette.chunkContent / 4];

								for (int i = 0; i < bytePalette.Length; i += 4)
								{
									voxel.palette.values[i / 4] = BitConverter.ToUInt32(bytePalette, i);
								}
							}
						}
						else
						{
							voxel.palette.values = new uint[256];
							_paletteDefault.CopyTo(voxel.palette.values, 0);
						}

						return voxel;
					}
				}
			}

			public static Color32[] CreateColor32FromPelatte(uint[] palette)
			{
				Debug.Assert(palette.Length == 256);

				Color32[] colors = new Color32[256];

				for (int j = 0; j < 256; j++)
				{
					uint rgba = palette[j];

					Color32 color;
					color.r = (byte)((rgba >> 0) & 0xFF);
					color.g = (byte)((rgba >> 8) & 0xFF);
					color.b = (byte)((rgba >> 16) & 0xFF);
					color.a = (byte)((rgba >> 24) & 0xFF);

					colors[j] = color;
				}

				return colors;
			}

			public static Texture2D CreateTextureFromColor16x16(Color32[] colors)
			{
				Debug.Assert(colors.Length == 256);

				Texture2D texture = new Texture2D(16, 16, TextureFormat.ARGB32, false, false);
				texture.SetPixels32(colors);
				texture.Apply();

				return texture;
			}

			public static Texture2D CreateTextureFromColor256(Color32[] colors)
			{
				Debug.Assert(colors.Length == 256);

				Texture2D texture = new Texture2D(256, 1, TextureFormat.ARGB32, false, false);
				texture.SetPixels32(colors);
				texture.Apply();

				return texture;
			}

			public static Texture2D CreateTextureFromPelatte16x16(uint[] palette)
			{
				Debug.Assert(palette.Length == 256);
				return CreateTextureFromColor16x16(CreateColor32FromPelatte(palette));
			}

			public static int CalcFaceCountAsAllocate(List<VOXPolygonCruncher.VoxelCruncher> list, Color32[] palette, ref Dictionary<string, int> entities)
			{
				entities.Add("alpha", 0);
				entities.Add("opaque", 0);

				foreach (var it in list)
				{
					bool[] visiable = new bool[] { it.faces.left, it.faces.right, it.faces.top, it.faces.bottom, it.faces.front, it.faces.back };

					int facesCount = 0;

					for (int j = 0; j < 6; j++)
					{
						if (visiable[j])
							facesCount++;
					}

					if (palette[it.material].a < 255)
						entities["alpha"] += 6;
					else
						entities["opaque"] += 6;
				}

				return entities.Count;
			}

			public static GameObject LoadVoxelFileAsGameObject(string path)
			{
				var voxel = VoxFileImport.Load(path);
				if (voxel == null)
				{
					UnityEngine.Debug.LogError(path + ": Invalid file");
					return null;
				}

				GameObject gameObject = new GameObject();
				gameObject.name = Path.GetFileNameWithoutExtension(path);

				var colors = CreateColor32FromPelatte(voxel.palette.values);
				var texture = CreateTextureFromColor256(colors);

				foreach (var chunk in voxel.chunkChild)
				{
					var cruncher = VOXPolygonCruncher.CalcVoxelCruncher(chunk);

					var entities = new Dictionary<string, int>();
					if (CalcFaceCountAsAllocate(cruncher, colors, ref entities) == 0)
					{
						UnityEngine.Debug.LogError(path + ": Empty file");
						return null;
					}

					foreach (var entity in entities)
					{
						if (entity.Value == 0)
							continue;

						var index = 0;
						var data = new ChunkMesh();
						var allocSize = cruncher.Count * 6 * 6;

						data.vertices = new Vector3[allocSize];
						data.normals = new Vector3[allocSize];
						data.uv = new Vector2[allocSize];
						data.triangles = new int[allocSize];

						bool isTransparent = false;

						foreach (var it in cruncher)
						{
							Vector3 pos;
							pos.x = (it.begin_x + it.end_x + 1) * 0.5f;
							pos.y = (it.begin_y + it.end_y + 1) * 0.5f;
							pos.z = (it.begin_z + it.end_z + 1) * 0.5f;

							Vector3 scale;
							scale.x = (it.end_x + 1 - it.begin_x);
							scale.y = (it.end_y + 1 - it.begin_y);
							scale.z = (it.end_z + 1 - it.begin_z);

							ChunkEntity.CreateCubeMesh16x16(ref data.vertices, ref data.normals, ref data.uv, ref data.triangles, ref index, it.faces, pos, scale, (uint)it.material);

							isTransparent |= (colors[it.material].a < 255) ? true : false;
						}

						if (data.triangles.GetLength(0) > 0)
						{
							Mesh mesh = new Mesh();
							mesh.vertices = data.vertices;
							mesh.normals = data.normals;
							mesh.uv = data.uv;
							mesh.triangles = data.triangles;

							gameObject.transform.parent = gameObject.transform;
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

#if UNITY_EDITOR

			public static GameObject LoadVoxelFileAsPrefab(string path)
			{
				GameObject gameObject = null;

				try
				{
					gameObject = VoxFileImport.LoadVoxelFileAsGameObject(path);
					if (gameObject == null)
						return null;

					var name = Path.GetFileNameWithoutExtension(path);

					var meshFilter = gameObject.GetComponent<MeshFilter>();
					if (meshFilter != null)
					{
						var outpath = "Assets/" + name + ".obj";

						ObjFileExport.WriteToFile(outpath, meshFilter, new Vector3(-0.1f, 0.1f, 0.1f));

						AssetDatabase.Refresh();

						meshFilter.mesh = AssetDatabase.LoadAssetAtPath<Mesh>(outpath);

						var collider = gameObject.GetComponent<MeshCollider>();
						if (collider != null)
							collider.sharedMesh = meshFilter.sharedMesh;
					}

					AssetDatabase.Refresh();

					var renderer = gameObject.GetComponent<MeshRenderer>();
					if (renderer != null)
					{
						if (renderer.sharedMaterial != null)
						{
							var material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/" + name + "Mat.mat");
							if (material != null)
							{
								material.mainTexture = renderer.sharedMaterial.mainTexture;

								renderer.sharedMaterial = material;
							}
						}
					}

					UnityEngine.Object prefab = PrefabUtility.CreatePrefab("Assets/" + name + ".prefab", gameObject);
					if (prefab == null)
						UnityEngine.Debug.LogError(Selection.activeObject.name + ": failed to save prefab");
				}
				catch (Exception)
				{
					GameObject.DestroyImmediate(gameObject);
				}

				return gameObject;
			}
		}

#endif
	}
}