using UnityEngine;
using UnityEditor;

namespace Cubizer
{
	[CustomEditor(typeof(ChunkData))]
	public class ChunkDataInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			ChunkData data = (ChunkData)target;

			base.DrawDefaultInspector();

			EditorGUILayout.HelpBox("Load & Save chunk of terrain from Asset", MessageType.Info);

			if (GUILayout.Button("Load..."))
			{
				var SelectedPath = EditorUtility.OpenFilePanel("Load Chunk", "", "asset");
				if (SelectedPath.Length == 0)
					return;

				var map = ChunkPrimer.Load(SelectedPath);
				if (map != null)
				{
					data.Chunk = map;
					data.OnBuildChunkAsync();

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

				var map = data.Chunk;
				if (map != null)
				{
					if (ChunkPrimer.Save(SelectedPath, map))
						Debug.Log("Your data of chunk was saved successfully");
					else
						Debug.Log("Save Failed");
				}
			}
		}
	}
}