using System.Collections;

using UnityEngine;
using UnityEditor;

namespace Cubizer
{
	[CustomEditor(typeof(TerrainData))]
	public class TerrainDataInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			TerrainData data = (TerrainData)target;

			base.DrawDefaultInspector();

			EditorGUILayout.HelpBox("Load & Save chunk of terrain from Asset", MessageType.Info);

			if (GUILayout.Button("Load..."))
			{
				var SelectedPath = EditorUtility.OpenFilePanel("Load Chunk", "", "asset");
				if (SelectedPath.Length == 0)
					return;

				var map = ChunkData.Load(SelectedPath);
				if (map != null)
				{
					if (data.chunk != null)
						map.position = data.chunk.position;

					data.chunk = map;
					data.UpdateChunk();

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

				var map = data.chunk;
				if (map != null)
				{
					if (ChunkData.Save(SelectedPath, map))
						Debug.Log("Your data of chunk was saved successfully");
					else
						Debug.Log("Save Failed");
				}
			}
		}
	}
}