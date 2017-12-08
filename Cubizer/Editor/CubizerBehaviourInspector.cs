using UnityEngine;
using UnityEditor;

namespace Cubier
{
	[CustomEditor(typeof(Cubizer.CubizerBehaviour))]
	public class CubizerBehaviourInspector : Editor
	{
		private Cubizer.CubizerBehaviour behaviour;

		public override void OnInspectorGUI()
		{
			behaviour = target as Cubizer.CubizerBehaviour;

			base.DrawDefaultInspector();

			if (behaviour.chunkManager != null)
			{
				if (GUILayout.Button("Load Terrain..."))
				{
					var SelectedPath = EditorUtility.OpenFilePanel("Load Chunk", "", "asset");
					if (SelectedPath.Length == 0)
						return;

					if (behaviour.chunkManager.Load(SelectedPath))
						Debug.Log("Your data of terrain was loaded successfully");
					else
						Debug.Log("load Failed");
				}

				if (GUILayout.Button("Save Terrain..."))
				{
					var SelectedPath = EditorUtility.SaveFilePanel("Save Chunk", "", "New Resource", "asset");
					if (SelectedPath.Length == 0)
						return;

					if (behaviour.chunkManager.Save(SelectedPath))
						Debug.Log("Your data of terrain was saved successfully");
					else
						Debug.Log("Save Failed");
				}
			}

			if (behaviour.server != null)
			{
				if (behaviour.server.isCancellationRequested)
				{
					if (GUILayout.Button("Create Server..."))
						behaviour.server.Open();
				}
				else
				{
					if (GUILayout.Button("Close Server..."))
						behaviour.server.Close();
				}
			}

			if (behaviour.client != null)
			{
				if (behaviour.client.isCancellationRequested)
				{
					if (GUILayout.Button("Connect Server..."))
						behaviour.client.Connect();
				}
				else
				{
					if (GUILayout.Button("Disconnect Server..."))
						behaviour.client.Disconnect();
				}
			}
		}
	}
}