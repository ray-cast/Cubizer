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

public class CubizerEditor : EditorWindow
{
	public bool _isSelectCreatePrefab = true;
	public bool _isSelectCreateAssetbundle = true;

	public static GameObject LoadVoxelFileAsGameObject(string path)
	{
		var voxel = MagicalVoxelFileLoad.Load(path);
		if (voxel == null)
		{
			UnityEngine.Debug.LogError(path + ": Invalid file");
			return null;
		}

		var gameObject = MagicalVoxelFileLoad.CreateGameObject(voxel);
		if (gameObject == null)
		{
			UnityEngine.Debug.LogError(path + ": Failed to create game object");
			return null;
		}

		gameObject.name = Path.GetFileNameWithoutExtension(path);

		return gameObject;
	}

	public static GameObject LoadVoxelFileAsPrefab(string path)
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

	[MenuItem("Tools/Cubizer/Show Cubizer Inspector")]
	public static void ShowWindow()
	{
		CubizerEditor.CreateInstance<CubizerEditor>().Show();
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

	public static bool CreateVoxelPrefabsFromSelection()
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

	public static bool CreateVoxelGameObjectFromSelection()
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
				var gameObject = LoadVoxelFileAsGameObject(path);
			}
		}

		return true;
	}

	public static void CreateAssetBunldesFromSelectionToStreamingAssets()
	{
		var SelectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

		foreach (var obj in SelectedAsset)
		{
			string sourcePath = AssetDatabase.GetAssetPath(obj);
			string targetPath = Application.dataPath + "/StreamingAssets/" + obj.name + ".assetbundle";

			if (BuildPipeline.BuildAssetBundle(obj, null, targetPath, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, BuildTarget.StandaloneWindows))
				UnityEngine.Debug.Log(obj.name + ": loaded successfully");
			else
				UnityEngine.Debug.Log(obj.name + ": failed to load");
		}

		AssetDatabase.Refresh();
	}

	public static void CreateAssetBunldesWithFolderPanel()
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
				CreateAssetBunldesFromSelectionToStreamingAssets();

			if (GUILayout.Button("Selection To Selected Folder"))
				CreateAssetBunldesWithFolderPanel();
		}
	}
}