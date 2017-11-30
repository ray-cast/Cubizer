using UnityEngine;
using UnityEditor;

namespace Cubier
{
	[CustomEditor(typeof(Cubizer.CubizerBehaviour))]
	public class CubizerBehaviourInspector : Editor
	{
		private Cubizer.CubizerBehaviour terrain;

		public override void OnInspectorGUI()
		{
			terrain = target as Cubizer.CubizerBehaviour;

			base.DrawDefaultInspector();

			if (terrain.chunkManager != null)
			{
				if (GUILayout.Button("Load Terrain..."))
				{
					var SelectedPath = EditorUtility.OpenFilePanel("Load Chunk", "", "asset");
					if (SelectedPath.Length == 0)
						return;

					if (terrain.chunkManager.Load(SelectedPath))
						Debug.Log("Your data of terrain was loaded successfully");
					else
						Debug.Log("load Failed");
				}

				if (GUILayout.Button("Save Terrain..."))
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

			if (terrain.server != null)
			{
				if (!terrain.server.opened)
				{
					if (GUILayout.Button("Create Server..."))
						terrain.OpenServer();
				}
				else
				{
					if (GUILayout.Button("Close Server..."))
						terrain.CloseServer();
				}
			}
		}
	}
}