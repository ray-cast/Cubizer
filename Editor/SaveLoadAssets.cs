using System.IO;

using UnityEditor;
using UnityEngine;

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
				var loadFile = new FileStream(path, FileMode.Open, FileAccess.Read);

				var voxel = AssetDatabase.LoadAssetAtPath(path, typeof(MagicalVoxelFile)) as MagicalVoxelFile;
				if (voxel)
				{
					var gameObject = new GameObject();

					UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(Application.dataPath + "/StreamingAssets/" + Selection.activeObject.name + ".prefab");
					PrefabUtility.ReplacePrefab(gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
				}
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