using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using UnityEngine;
using UnityEditor;

using Cubizer;
using Cubizer.Model;

namespace Cubizer
{
	public class VOXFileLoader : EditorWindow
	{
		public bool _isSelectCreatePrefab = true;
		public bool _isSelectCreateAssetbundle = true;

		[MenuItem("Tools/Cubizer/Show Cubizer Inspector")]
		public static void ShowWindow()
		{
			VOXFileLoader.CreateInstance<VOXFileLoader>().Show();
		}

		[MenuItem("Tools/Cubizer/Load .vox file as Prefab")]
		public static void LoadVoxelFileAsPrefab()
		{
			var filepath = EditorUtility.OpenFilePanel("Load .vox file", "", "vox");
			if (filepath != null)
			{
				if (filepath.Remove(0, filepath.LastIndexOf('.')) != ".vox")
				{
					UnityEngine.Debug.LogError("The end of the path wasn't \".vox\"");
					return;
				}

				var gameObject = LoadVoxelFileAsPrefab(filepath);
				DestroyImmediate(gameObject);
			}
		}

		[MenuItem("Tools/Cubizer/Load .vox file as GameObject")]
		public static void LoadVoxelFileAsGameObject()
		{
			var filepath = EditorUtility.OpenFilePanel("Load .vox file", "", "vox");
			if (filepath != null)
			{
				if (filepath.Remove(0, filepath.LastIndexOf('.')) != ".vox")
				{
					UnityEngine.Debug.LogError("The end of the path wasn't \".vox\"");
					return;
				}

				LoadVoxelFileAsGameObject(filepath);
			}
		}

		public void OnGUI()
		{
			GUILayout.Label("Selected Object:", EditorStyles.boldLabel);

			this._isSelectCreatePrefab = EditorGUILayout.Foldout(this._isSelectCreatePrefab, "Create Prefab from MagicaVoxel");
			if (this._isSelectCreatePrefab)
			{
				if (GUILayout.Button("Create Prefab from .vox file"))
					CreateVoxelPrefabsFromSelection();

				if (GUILayout.Button("Create Prefab LOD from .vox file"))
					CreateVoxelPrefabsFromSelection();

				if (GUILayout.Button("Create GameObject from .vox file"))
					CreateVoxelGameObjectFromSelection();

				if (GUILayout.Button("Create GameObject LOD from .vox file"))
					CreateVoxelGameObjectFromSelection();
			}

			this._isSelectCreateAssetbundle = EditorGUILayout.Foldout(this._isSelectCreateAssetbundle, "Create AssetBundle");
			if (this._isSelectCreateAssetbundle)
			{
				if (GUILayout.Button("Selection To StreamingAssets folder"))
					CreateAssetBundlesFromSelectionToStreamingAssets();

				if (GUILayout.Button("Selection To Selected Folder"))
					CreateAssetBundlesWithFolderPanel();
			}
		}

		private static bool CreateVoxelPrefabsFromSelection()
		{
			var SelectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
			if (SelectedAsset.Length == 0)
			{
				EditorUtility.DisplayDialog("No Object Selected", "Please select any .vox file to create to prefab", "Ok");
				return false;
			}

			foreach (var asset in SelectedAsset)
			{
				var path = AssetDatabase.GetAssetPath(asset);
				if (path.Remove(0, path.LastIndexOf('.')) == ".vox")
				{
					var gameObject = LoadVoxelFileAsPrefab(path);
					DestroyImmediate(gameObject);
				}
			}

			return true;
		}

		private static bool CreateVoxelGameObjectFromSelection()
		{
			var SelectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
			if (SelectedAsset.Length == 0)
			{
				EditorUtility.DisplayDialog("No Object Selected", "Please select any .vox file to create to prefab", "Ok");
				return false;
			}

			foreach (var asset in SelectedAsset)
			{
				var path = AssetDatabase.GetAssetPath(asset);
				if (path.Remove(0, path.LastIndexOf('.')) == ".vox")
				{
					LoadVoxelFileAsGameObject(path);
				}
			}

			return true;
		}

		private static void CreateAssetBundlesFromSelectionToStreamingAssets()
		{
			var SelectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

			foreach (var obj in SelectedAsset)
			{
				string targetPath = Application.dataPath + "/StreamingAssets/" + obj.name + ".assetbundle";

				if (BuildPipeline.BuildAssetBundle(obj, null, targetPath, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, BuildTarget.StandaloneWindows))
					UnityEngine.Debug.Log(obj.name + ": loaded successfully");
				else
					UnityEngine.Debug.Log(obj.name + ": failed to load");
			}

			AssetDatabase.Refresh();
		}

		private static void CreateAssetBundlesWithFolderPanel()
		{
			var SelectedPath = EditorUtility.SaveFolderPanel("Save Resource", "", "New Resource");
			if (SelectedPath.Length == 0)
				return;

			var SelectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

			foreach (var obj in SelectedAsset)
			{
				string targetPath = SelectedPath + obj.name + ".assetbundle";

				if (BuildPipeline.BuildAssetBundle(obj, null, targetPath, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, BuildTarget.StandaloneWindows))
					UnityEngine.Debug.Log(obj.name + ": loaded successfully");
				else
					UnityEngine.Debug.Log(obj.name + ": failed to load");
			}

			AssetDatabase.Refresh();
		}

		private static GameObject LoadVoxelFileAsGameObject(string path)
		{
			var voxel = VoxFileImport.Load(path);
			if (voxel == null)
			{
				UnityEngine.Debug.LogError(path + ": Invalid file");
				return null;
			}

			GameObject gameObject = new GameObject();
			gameObject.name = Path.GetFileNameWithoutExtension(path);

			for (int i = 0; i < voxel.pack.modelNums; i++)
			{
				var chunk = voxel.chunkChild[i];

				Color32[] colors = new Color32[256];
				for (int j = 0; j < voxel.palette.values.Length; j++)
				{
					uint palette = voxel.palette.values[j];

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

				var map = new ChunkMapByte3<ChunkEntity>(new Cubizer.Math.Vector3<int>(chunk.size.x, chunk.size.y, chunk.size.z), chunk.xyzi.voxelNums);

				for (int j = 0; j < chunk.xyzi.voxelNums * 4; j += 4)
				{
					var x = chunk.xyzi.voxels[j];
					var y = chunk.xyzi.voxels[j + 1];
					var z = chunk.xyzi.voxels[j + 2];
					var c = chunk.xyzi.voxels[j + 3];

					map.Set(x, y, z, new ChunkEntity("voxel", c));
				}

				var cruncher = VOXPolygonCruncher.CalcVoxelCruncher(map);

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

						Cubizer.Math.Vector3<byte> scale;
						scale.x = (byte)((it.end_x + 1 - it.begin_x));
						scale.y = (byte)((it.end_y + 1 - it.begin_y));
						scale.z = (byte)((it.end_z + 1 - it.begin_z));

						ChunkEntity.CreateCubeMesh(ref data, ref index, it.faces, pos, (uint)it.material, scale);

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

		private static GameObject LoadVoxelFileAsPrefab(string path)
		{
			GameObject gameObject = null;

			try
			{
				gameObject = LoadVoxelFileAsGameObject(path);
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
				DestroyImmediate(gameObject);
			}

			return gameObject;
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
	}
}