using System;
using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public abstract class CubizerModel
	{
		[SerializeField, GetSet("enabled")]
		private bool _enabled = true;

		public bool enabled
		{
			get { return _enabled; }
			set
			{
				if (_enabled != value)
				{
					_enabled = value;

					if (value)
						OnValidate();
				}
			}
		}

		public abstract void Reset();

		public virtual void OnValidate()
		{
		}
	}
}