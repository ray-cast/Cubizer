using System;

using UnityEngine;

namespace Cubizer
{
	[Serializable]
	public sealed class LiveManagerComponent : CubizerComponent<LiveManagerModels>
	{
		private GameObject _biomeObject;

		public override bool active
		{
			get { return true; }
		}

		public override void OnEnable()
		{
			_biomeObject = new GameObject("TerrainEntities");

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
	}
}