using System.Collections;

using UnityEngine;
using UnityEditor;

using Cubizer;

[CustomEditor(typeof(ChunkObjectManager))]
public class SaveLoadChunkInspector : Editor
{
	public override void OnInspectorGUI()
	{
		ChunkObjectManager chunk = (ChunkObjectManager)target;

		EditorGUILayout.HelpBox("Load chunk from Asset", MessageType.Info);

		if (GUILayout.Button("Load..."))
		{
			var SelectedPath = EditorUtility.OpenFilePanel("Load Chunk", "", "asset");
			if (SelectedPath.Length == 0)
				return;

			var map = ChunkTree.Load(SelectedPath, chunk.map.manager);
			if (map != null)
			{
				map.position = chunk.map.position;
				chunk.map = map;
				Debug.Log("Your data of chunk was loaded successfully");
			}
			else
			{
				Debug.Log("load Failed");
			}
		}

		if (GUILayout.Button("Save..."))
		{
			var SelectedPath = EditorUtility.SaveFilePanel("Save Chunk", "", "New Resource", "asset");
			if (SelectedPath.Length == 0)
				return;

			var map = chunk.map;
			if (map != null)
			{
				if (ChunkTree.Save(SelectedPath, map))
					Debug.Log("Your data of chunk was saved successfully");
				else
					Debug.Log("Save Failed");
			}
		}
	}
}