using UnityEngine;
using System.Collections.Generic;

namespace Cubizer
{
	public class BiomeManagerComponent : CubizerComponent<BiomeManagerModels>
	{
		private readonly string _name;
		private GameObject _biomeObject;
		private List<IBiomeGenerator> _biomeGenerators;
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

		public int count
		{
			get { return _biomeObject != null ? _biomeObject.transform.childCount : 0; }
		}

		private IBiomeDataManager biomes
		{
			get { return model.settings.biomeManager; }
		}

		public BiomeManagerComponent(string name = "ServerBiomes")
		{
			_name = name;
			_active = true;
		}

		public override void OnEnable()
		{
			_biomeObject = new GameObject(_name);
			_biomeGenerators = new List<IBiomeGenerator>();

			foreach (var it in model.settings.biomeGenerators)
			{
				if (it != null)
				{
					var generator = GameObject.Instantiate(it.gameObject).GetComponent<IBiomeGenerator>();
					generator.gameObject.name = it.name;
					generator.gameObject.transform.parent = _biomeObject.transform;
					generator.Init(this.context);

					_biomeGenerators.Add(generator);
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

			if (_biomeGenerators != null)
			{
				_biomeGenerators.Clear();
				_biomeGenerators = null;
			}
		}

		public IBiomeData buildBiomeIfNotExist(int x, int y, int z)
		{
			Debug.Assert(model.settings.biomeNull != null);

			IBiomeData biomeData = null;
			if (this.biomes.Get(x, y, z, out biomeData))
				return biomeData;

			foreach (var it in _biomeGenerators)
			{
				biomeData = it.OnBuildBiome(x, y, z);
				if (biomeData != null)
					break;
			}

			if (biomeData == null)
				biomeData = model.settings.biomeNull;

			model.settings.biomeManager.Set(x, y, z, biomeData);
			return biomeData;
		}

		private void AutoGC()
		{
			if (this.biomes.Count() > model.settings.biomeNumLimits)
				this.biomes.GC();
		}

		public override void Update()
		{
			this.AutoGC();
		}
	}
}