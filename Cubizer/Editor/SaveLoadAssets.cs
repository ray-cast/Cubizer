using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using UnityEditor;
using UnityEngine;

using Cubizer;
using Cubizer.Model;

public class ChunkEditor : EditorWindow
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
		var gameObject = LoadVoxelFileAsGameObject(path);
		if (gameObject)
		{
			var meshFilter = gameObject.GetComponent<MeshFilter>();
			if (meshFilter == null)
			{
				DestroyImmediate(gameObject);
				return gameObject;
			}

			var outpath = "Assets/" + Selection.activeObject.name + ".obj";
			ChunkUtility.SaveMeshAsObjFile(outpath, meshFilter, new Vector3(-1f, 1f, 1f));

			AssetDatabase.Refresh();

			meshFilter.mesh = AssetDatabase.LoadAssetAtPath<Mesh>(outpath);

			UnityEngine.Object prefab = PrefabUtility.CreatePrefab("Assets/" + Selection.activeObject.name + ".prefab", gameObject);
			if (prefab == null)
				UnityEngine.Debug.LogError(Selection.activeObject.name + ": failed to save prefab");
		}

		return gameObject;
	}

	[MenuItem("Tools/Chunk/Show Inspector")]
	public static void ShowWindow()
	{
		ChunkEditor.CreateInstance<ChunkEditor>().Show();
	}

	[MenuItem("Tools/Chunk/Load .vox file as Prefab")]
	public static void LoadVoxelFileAsPrefab()
	{
		var filepath = EditorUtility.OpenFilePanel("Load .vox file", "", "vox");
		if (filepath != null)
		{
			var gameObject = LoadVoxelFileAsPrefab(filepath);
			DestroyImmediate(gameObject);
		}
	}

	[MenuItem("Tools/Chunk/Load .vox file as GameObject")]
	public static void LoadVoxelFileAsGameObject()
	{
		var filepath = EditorUtility.OpenFilePanel("Load .vox file", "", "vox");
		if (filepath != null)
		{
			LoadVoxelFileAsGameObject(filepath);
		}
	}

	public static void CreateVoxelPrefabsFromSelection()
	{
		var SelectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

		foreach (var asset in SelectedAsset)
		{
			var path = AssetDatabase.GetAssetPath(asset);
			if (path.Contains(".vox"))
			{
				var gameObject = LoadVoxelFileAsPrefab(path);
				DestroyImmediate(gameObject);
			}
		}
	}

	public static void CreateVoxelGameObjectFromSelection()
	{
		var SelectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

		foreach (var asset in SelectedAsset)
		{
			var path = AssetDatabase.GetAssetPath(asset);
			if (path.Contains(".vox"))
			{
				var gameObject = LoadVoxelFileAsGameObject(path);
			}
		}
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