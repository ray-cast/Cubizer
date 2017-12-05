using System;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public sealed class LiveManagerComponent : CubizerComponent<LiveManagerModels>
	{
		private bool _active;
		private readonly string _name;
		private GameObject _liveObject;

		public override bool Active
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
			_liveObject = new GameObject(_name);
			foreach (var it in Model.settings.lives)
			{
				if (it == null)
					throw new NullReferenceException("Missing of index:" + Model.settings.lives.IndexOf(null));

				var gameObject = GameObject.Instantiate(it.gameObject);
				gameObject.name = it.name;
				gameObject.transform.parent = _liveObject.transform;
				gameObject.GetComponent<LiveBehaviour>().material = Context.materialFactory.CreateMaterial(it.name, it.settings);
			}
		}

		public override void OnDisable()
		{
			if (_liveObject != null)
			{
				GameObject.Destroy(_liveObject);
				_liveObject = null;
			}
		}
	}
}