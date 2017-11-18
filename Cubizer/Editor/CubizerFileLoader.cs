using System;
using System.IO;

using UnityEngine;
using UnityEditor;

using Cubizer.Model;

public class VOXFileLoader : EditorWindow
{
	public bool _isSelectCreatePrefab = true;
	public bool _isSelectCreateAssetbundle = true;

	[MenuItem("Tools/Cubizer/Show VOXFileLoader Inspector", priority = 301)]
	public static void ShowWindow()
	{
		VOXFileLoader.CreateInstance<VOXFileLoader>().Show();
	}

	[MenuItem("Tools/Cubizer/Load .vox file as Prefab", priority = 201)]
	public static void LoadVoxelFileAsPrefab()
	{
		var filepath = EditorUtility.OpenFilePanel("Load .vox file", "", "vox");
		if (!String.IsNullOrEmpty(filepath))
		{
			if (!filepath.Contains(".vox"))
			{
				EditorUtility.DisplayDialog("Invalid File", "The end of the path wasn't \".vox\"", "Ok");
				return;
			}

			VoxFileImport.LoadVoxelFileAsPrefab(filepath);
		}
	}

	[MenuItem("Tools/Cubizer/Load .vox file as Prefab LOD", priority = 202)]
	public static void LoadVoxelFileAsPrefabLOD()
	{
		var filepath = EditorUtility.OpenFilePanel("Load .vox file", "", "vox");
		if (!String.IsNullOrEmpty(filepath))
		{
			if (!filepath.Contains(".vox"))
			{
				EditorUtility.DisplayDialog("Invalid File", "The end of the path wasn't \".vox\"", "Ok");
				return;
			}

			VoxFileImport.LoadVoxelFileAsPrefab(filepath, "Assets/", 3);
		}
	}

	[MenuItem("Tools/Cubizer/Load .vox file as GameObject", priority = 203)]
	public static void LoadVoxelFileAsGameObject()
	{
		var filepath = EditorUtility.OpenFilePanel("Load .vox file", "", "vox");
		if (!String.IsNullOrEmpty(filepath))
		{
			if (!filepath.Contains(".vox"))
			{
				EditorUtility.DisplayDialog("Invalid File", "The end of the path wasn't \".vox\"", "Ok");
				return;
			}

			VoxFileImport.LoadVoxelFileAsGameObject(filepath);
		}
	}

	[MenuItem("Tools/Cubizer/Load .vox file as GameObject LOD", priority = 204)]
	public static void LoadVoxelFileAsGameObjectLOD()
	{
		var filepath = EditorUtility.OpenFilePanel("Load .vox file", "", "vox");
		if (!String.IsNullOrEmpty(filepath))
		{
			if (!filepath.Contains(".vox"))
			{
				EditorUtility.DisplayDialog("Invalid File", "The end of the path wasn't \".vox\"", "Ok");
				return;
			}

			VoxFileImport.LoadVoxelFileAsGameObjectLOD(filepath, 3);
		}
	}

	public void OnGUI()
	{
		GUILayout.Label("Selected Object:", EditorStyles.boldLabel);

		this._isSelectCreatePrefab = EditorGUILayout.Foldout(this._isSelectCreatePrefab, "Create Model from .vox file");
		if (this._isSelectCreatePrefab)
		{
			if (GUILayout.Button("Create Prefab from .vox file"))
				CreateVoxelPrefabsFromSelection();

			if (GUILayout.Button("Create Prefab LOD from .vox file"))
				CreateVoxelPrefabsFromSelection(3);

			if (GUILayout.Button("Create GameObject from .vox file"))
				CreateVoxelGameObjectFromSelection();

			if (GUILayout.Button("Create GameObject LOD from .vox file"))
				CreateVoxelGameObjectFromSelection(3);
		}

		this._isSelectCreateAssetbundle = EditorGUILayout.Foldout(this._isSelectCreateAssetbundle, "Create AssetBundle");
		if (this._isSelectCreateAssetbundle)
		{
			if (GUILayout.Button("Selection To StreamingAssets folder"))
				CreateAssetBundlesFromSelectionToStreamingAssets();

			if (GUILayout.Button("Selection To Selected Folder"))
				CreateAssetBundlesWithFolderPanel();

			if (GUILayout.Button("Selection To .bytes Files"))
				RenameFilesFromSelection();
		}
	}

	private static bool CreateVoxelPrefabsFromSelection(int lodLevel = 0)
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
			if (!path.Contains(".vox"))
			{
				EditorUtility.DisplayDialog("Invalid File", "The end of the path wasn't \".vox\"", "Ok");
				return false;
			}

			if (path.Remove(0, path.LastIndexOf('.')) == ".vox")
			{
				if (lodLevel == 0)
					VoxFileImport.LoadVoxelFileAsPrefab(path);
				else
					VoxFileImport.LoadVoxelFileAsPrefab(path, "Assets/", lodLevel);
			}
		}

		return true;
	}

	private static bool CreateVoxelGameObjectFromSelection(int lodLevel = 0)
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
			if (!path.Contains(".vox"))
			{
				EditorUtility.DisplayDialog("Invalid File", "The end of the path wasn't \".vox\"", "Ok");
				return false;
			}

			if (lodLevel == 0)
				VoxFileImport.LoadVoxelFileAsGameObject(path);
			else
				VoxFileImport.LoadVoxelFileAsGameObjectLOD(path, lodLevel);
		}

		return true;
	}

	private static void CreateAssetBundlesFromSelection(string targetPath, string bundleName = "Resource", string ext = "")
	{
		var SelectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

		if (SelectedAsset.Length > 0)
		{
			AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
			buildMap[0].assetBundleName = bundleName + ext;
			buildMap[0].assetNames = new string[SelectedAsset.Length];

			for (int i = 0; i < SelectedAsset.Length; i++)
				buildMap[0].assetNames[i] = AssetDatabase.GetAssetPath(SelectedAsset[i]);

			if (!BuildPipeline.BuildAssetBundles(targetPath, buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows))
				UnityEngine.Debug.Log(targetPath + ": Failed to create");

			AssetDatabase.Refresh();
		}
	}

	private static void RenameFilesFromSelection(string ext = ".bytes")
	{
		if (String.IsNullOrEmpty(ext))
			return;

		var SelectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

		if (SelectedAsset.Length > 0)
		{
			for (int i = 0; i < SelectedAsset.Length; i++)
			{
				var path = AssetDatabase.GetAssetPath(SelectedAsset[i]);
				var extName = Path.GetExtension(path).ToLower();

				if (extName != ".jpg" && extName != ".png" && extName != ".psd" && extName != ".tiff" && extName != ".tga" && extName != ".gif" &&
					extName != ".mov" && extName != ".mpg" && extName != ".mpeg" && extName != ".mp4" && extName != ".avi" && extName != ".asf" &&
					extName != ".mp3" && extName != ".wav" && extName != ".ogg" && extName != ".aiff" &&
					extName != ".fbx" && extName != ".obj" && extName != ".dae" && extName != ".3ds" && extName != ".dxf" &&
					extName != ".txt" && extName != ".bytes" &&
					extName != ".prefab" && extName != ".asset" &&
					extName != ext)
				{
					var outpath = Path.GetFullPath(path);
					File.Move(outpath, outpath + ext);
				}
			}
		}

		AssetDatabase.Refresh();
	}

	private static void CreateAssetBundlesWithFolderPanel(string bundleName = "Resource", string ext = "")
	{
		var SelectedPath = EditorUtility.SaveFolderPanel("Save Resource", "", "New Resource");
		if (SelectedPath.Length == 0)
			return;

		CreateAssetBundlesFromSelection(SelectedPath + "/", bundleName, ext);
	}

	private static void CreateAssetBundlesFromSelectionToStreamingAssets(string bundleName = "Resource", string ext = "")
	{
		CreateAssetBundlesFromSelection(Application.dataPath + "/StreamingAssets/", bundleName, "Assets");
	}
}