using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Cubier
{
	[CustomEditor(typeof(Cubizer.Terrain))]
	public class TerrainInspector : Editor
	{
		private Cubizer.Terrain terrain;

		public override void OnInspectorGUI()
		{
			terrain = target as Cubizer.Terrain;

			base.DrawDefaultInspector();

			if (GUILayout.Button("Load..."))
			{
				var SelectedPath = EditorUtility.OpenFilePanel("Load Chunk", "", "asset");
				if (SelectedPath.Length == 0)
					return;

				if (terrain.Load(SelectedPath))
					Debug.Log("Your data of terrain was loaded successfully");
				else
					Debug.Log("load Failed");
			}

			if (GUILayout.Button("Save..."))
			{
				var SelectedPath = EditorUtility.SaveFilePanel("Save Chunk", "", "New Resource", "asset");
				if (SelectedPath.Length == 0)
					return;

				if (terrain.Save(SelectedPath))
					Debug.Log("Your data of terrain was saved successfully");
				else
					Debug.Log("Save Failed");
			}
		}
	}
}