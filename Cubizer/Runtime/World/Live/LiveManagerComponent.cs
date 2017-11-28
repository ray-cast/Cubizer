using System;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public sealed class LiveManagerComponent : CubizerComponent<LiveManagerModels>
	{
		private string _name;
		private GameObject _biomeObject;
		private bool _active;

		public override bool active
		{
			get
			{
				return _active;
			}
			set
			{
				if (_active != value)
				{
					if (value)
						this.OnEnable();
					else
						this.OnDisable();

					_active = value;
				}
			}
		}

		public LiveManagerComponent(string name = "ServerEntities")
		{
			_name = name;
			_active = true;
		}

		public override void OnEnable()
		{
			_biomeObject = new GameObject(_name);

			foreach (var it in model.settings.lives)
			{
				if (it != null)
				{
					var gameObject = GameObject.Instantiate(it.gameObject);
					gameObject.name = it.name;
					gameObject.transform.parent = _biomeObject.transform;
					gameObject.GetComponent<LiveBehaviour>().material = context.materialFactory.CreateMaterial(it.name, it.settings);
				}
			}
		}

		public override void OnDisable()
		{
			if (_biomeObject != null)
			{
				GameObject.Destroy(_biomeObject);
				_biomeObject = null;
			}
		}
	}
}