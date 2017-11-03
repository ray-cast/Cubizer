using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

using UnityEditor;
using UnityEngine;

using Chunk;

public class ChunkEditor : EditorWindow
{
	public bool _isSelectCreatePrefab = true;
	public bool _isSelectCreateAssetbundle = true;

	[MenuItem("Tools/Chunk/Show Inspector")]
	public static void showWindow()
	{
		ChunkEditor.CreateInstance<ChunkEditor>().Show();
	}

	public void CreateVoxelPrefabsFromSelection()
	{
		var SelectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

		foreach (var asset in SelectedAsset)
		{
			var path = AssetDatabase.GetAssetPath(asset);
			if (path.Contains(".vox"))
			{
				var voxel = MagicalVoxelFileLoad.Load(path);
				if (voxel == null)
					continue;

				var gameObject = MagicalVoxelFileLoad.CreateGameObject(voxel);
				if (gameObject == null)
					continue;

				var meshFilter = gameObject.GetComponent<MeshFilter>();
				if (meshFilter == null)
				{
					DestroyImmediate(gameObject);
					continue;
				}

				var outpath = "Assets/" + Selection.activeObject.name + ".obj";
				ChunkUtility.SaveMeshAsObjFile(outpath, meshFilter, new Vector3(-1f, 1f, 1f));

				AssetDatabase.Refresh();

				meshFilter.mesh = AssetDatabase.LoadAssetAtPath<Mesh>(outpath);

				UnityEngine.Object prefab = PrefabUtility.CreatePrefab("Assets/" + Selection.activeObject.name + ".prefab", gameObject);
				if (prefab == null)
					UnityEngine.Debug.Log(Selection.activeObject.name + ": failed to save prefab");

				DestroyImmediate(gameObject);
			}
		}
	}

	public void CreateAssetBunldesFromSelectionToStreamingAssets()
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

	public void CreateAssetBunldesWithFolderPanel()
	{
		var SelectedPath = EditorUtility.SaveFolderPanel("Save Resource", "", "New Resource");
		if (SelectedPath.Length == 0)
			return;

		var SelectedAsset = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);

		foreach (var obj in SelectedAsset)
		{
			string sourcePath = AssetDatabase.GetAssetPath(obj);
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

		this._isSelectCreatePrefab = EditorGUILayout.Foldout(this._isSelectCreatePrefab, "Create Prefab");
		if (this._isSelectCreatePrefab)
		{
			EditorGUILayout.HelpBox("Create Prefab from MagicaVoxel", MessageType.None);

			if (GUILayout.Button("Create Prefab from .vox file"))
				this.CreateVoxelPrefabsFromSelection();

			if (GUILayout.Button("Create Prefab LOD from .vox file"))
				this.CreateVoxelPrefabsFromSelection();
		}

		this._isSelectCreateAssetbundle = EditorGUILayout.Foldout(this._isSelectCreateAssetbundle, "Create AssetBundle");
		if (this._isSelectCreateAssetbundle)
		{
			if (GUILayout.Button("Selection To StreamingAssets folder"))
				this.CreateAssetBunldesFromSelectionToStreamingAssets();

			if (GUILayout.Button("Selection To Selected Folder"))
				this.CreateAssetBunldesWithFolderPanel();
		}
	}
}