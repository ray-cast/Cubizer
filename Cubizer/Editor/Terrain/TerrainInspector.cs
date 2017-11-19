using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Cubier
{
	[CustomEditor(typeof(Cubizer.CubizerBehaviour))]
	public class TerrainChunkInspector : Editor
	{
		private Cubizer.CubizerBehaviour terrain;

		public override void OnInspectorGUI()
		{
			terrain = target as Cubizer.CubizerBehaviour;

			base.DrawDefaultInspector();

			if (GUILayout.Button("Load..."))
			{
				var SelectedPath = EditorUtility.OpenFilePanel("Load Chunk", "", "asset");
				if (SelectedPath.Length == 0)
					return;

				if (terrain.chunkManager.Load(SelectedPath))
					Debug.Log("Your data of terrain was loaded successfully");
				else
					Debug.Log("load Failed");
			}

			if (GUILayout.Button("Save..."))
			{
				var SelectedPath = EditorUtility.SaveFilePanel("Save Chunk", "", "New Resource", "asset");
				if (SelectedPath.Length == 0)
					return;

				if (terrain.chunkManager.Save(SelectedPath))
					Debug.Log("Your data of terrain was saved successfully");
				else
					Debug.Log("Save Failed");
			}
		}
	}
}