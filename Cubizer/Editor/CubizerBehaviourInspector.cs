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

			if (behaviour.ChunkManager != null)
			{
				if (GUILayout.Button("Load Terrain..."))
				{
					var SelectedPath = EditorUtility.OpenFilePanel("Load Chunk", "", "asset");
					if (SelectedPath.Length == 0)
						return;

					if (behaviour.ChunkManager.Load(SelectedPath))
						Debug.Log("Your data of terrain was loaded successfully");
					else
						Debug.Log("load Failed");
				}

				if (GUILayout.Button("Save Terrain..."))
				{
					var SelectedPath = EditorUtility.SaveFilePanel("Save Chunk", "", "New Resource", "asset");
					if (SelectedPath.Length == 0)
						return;

					if (behaviour.ChunkManager.Save(SelectedPath))
						Debug.Log("Your data of terrain was saved successfully");
					else
						Debug.Log("Save Failed");
				}
			}

			if (behaviour.Server != null)
			{
				if (behaviour.Server.IsCancellationRequested)
				{
					if (GUILayout.Button("Create Server..."))
						behaviour.Server.Open();
				}
				else
				{
					if (GUILayout.Button("Close Server..."))
						behaviour.Server.Close();

					if (behaviour.Client.IsCancellationRequested)
					{
						if (GUILayout.Button("Connect Server..."))
							behaviour.Client.Connect();
					}
					else
					{
						if (GUILayout.Button("Disconnect Server..."))
							behaviour.Client.Disconnect();
					}
				}
			}
		}
	}
}