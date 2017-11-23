using UnityEngine;

namespace Cubizer
{
	[DisallowMultipleComponent]
	public abstract class LiveBehaviour : ILiveBehaviour
	{
		[SerializeField]
		private VoxelMaterialModels _settings;

		private VoxelMaterial _material;

		public override VoxelMaterialModels settings
		{
			get
			{
				return _settings;
			}
			internal set
			{
				_settings = value;

				if (_material != null)
				{
					_material.is_merge = value.merge;
					_material.is_transparent = value.transparent;
				}
			}
		}

		public override VoxelMaterial material
		{
			get
			{
				return _material;
			}
			internal set
			{
				if (value != null)
					value.userdata = this;
				_material = value;
			}
		}

		public void Reset()
		{
			_settings.Reset();
		}
	}
}