using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEditor;

namespace Cubizer
{
	internal class CubizerModelEditor
	{
		public CubizerModel target { get; internal set; }
		public SerializedProperty serializedProperty { get; internal set; }

		protected SerializedProperty m_SettingsProperty;
		protected SerializedProperty m_EnabledProperty;

		internal bool alwaysEnabled = false;
		internal CubizerInspector inspector;

		internal void OnPreEnable()
		{
			m_SettingsProperty = serializedProperty.FindPropertyRelative("m_Settings");
			m_EnabledProperty = serializedProperty.FindPropertyRelative("m_Enabled");

			OnEnable();
		}

		public virtual void OnEnable()
		{
		}

		public virtual void OnDisable()
		{
		}

		internal void OnGUI()
		{
			GUILayout.Space(5);
		}

		private void Reset()
		{
			var obj = serializedProperty.serializedObject;
			Undo.RecordObject(obj.targetObject, "Reset");
			target.Reset();
			EditorUtility.SetDirty(obj.targetObject);
		}

		public virtual void OnInspectorGUI()
		{
		}

		public void Repaint()
		{
			inspector.Repaint();
		}
	}
}