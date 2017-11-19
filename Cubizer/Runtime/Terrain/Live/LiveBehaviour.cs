using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	public abstract class LiveBehaviour : LiveBehaviourBase
	{
		[SerializeField]
		private VoxelMaterialParams _settings;

		private VoxelMaterial _material;

		public VoxelMaterialParams settings
		{
			set
			{
				_settings = value;

				if (_material != null)
				{
					_material.is_merge = value.merge;
					_material.is_transparent = value.transparent;
				}
			}
			get
			{
				return _settings;
			}
		}

		public VoxelMaterial material
		{
			set
			{
				if (value != null)
					value.userdata = this;
				_material = value;
			}
			get
			{
				return _material;
			}
		}

		public void Reset()
		{
			_settings.Reset();
		}
	}
}