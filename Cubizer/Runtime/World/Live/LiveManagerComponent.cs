using System;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public sealed class LiveManagerComponent : CubizerComponent<LiveManagerModels>
	{
		private bool _active;
		private string _name;
		private GameObject _biomeObject;

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
				if (it == null)
					throw new NullReferenceException("Missing of index:" + model.settings.lives.IndexOf(null));

				var gameObject = GameObject.Instantiate(it.gameObject);
				gameObject.name = it.name;
				gameObject.transform.parent = _biomeObject.transform;
				gameObject.GetComponent<LiveBehaviour>().material = context.materialFactory.CreateMaterial(it.name, it.settings);
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