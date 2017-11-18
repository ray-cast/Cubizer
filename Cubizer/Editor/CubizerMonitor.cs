using System;

using UnityEngine;

namespace Cubizer
{
	public abstract class CubizerMonitor : IDisposable
	{
		protected CubizerInspector m_BaseEditor;

		public void Init(CubizerInspector baseEditor)
		{
			m_BaseEditor = baseEditor;
		}

		public abstract bool IsSupported();

		public abstract GUIContent GetMonitorTitle();

		public virtual void OnMonitorSettings() {}

		public abstract void OnMonitorGUI(Rect r);

		public virtual void OnFrameData(RenderTexture source) {}

		public virtual void Dispose()
		{
		}
	}
}
