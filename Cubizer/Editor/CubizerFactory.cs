using System.IO;

using UnityEngine;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;

namespace Cubizer
{
	public class CubizerFactory
	{
		class DoCreateCubizerProfile : EndNameEditAction
		{
			CubizerProfile CreateCubizerProfileAtPath(string path)
			{
				var profile = CreateInstance<CubizerProfile>();
				profile.name = Path.GetFileName(path);
				AssetDatabase.CreateAsset(profile, path);

				return profile;
			}

			public override void Action(int instanceId, string pathName, string resourceFile)
			{
				ProjectWindowUtil.ShowCreatedAsset(CreateCubizerProfileAtPath(pathName));
			}
		}

		[MenuItem("Tools/Cubizer/Create Cubizer Profile", priority = 101)]
		static void MenuCreateCubizerProfile()
		{
			var icon = EditorGUIUtility.FindTexture("ScriptableObject Icon");
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateCubizerProfile>(), "New Cubizer Profile.asset", icon, null);
		}
	}
}