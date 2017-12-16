using UnityEngine;
using System.Collections.Generic;

namespace Cubizer.Biome
{
	public sealed class BiomeManagerComponent : CubizerComponent<BiomeManagerModels>
	{
		private readonly string _name;
		private GameObject _biomeObject;
		private List<IBiomeGenerator> _biomeGenerators;

		public override bool active
		{
			get
			{
				return model.enabled;
			}
			set
			{
				if (model.enabled != value)
				{
					if (value)
						this.OnEnable();
					else
						this.OnDisable();

					model.enabled = value;
				}
			}
		}

		public int Count
		{
			get { return _biomeObject != null ? _biomeObject.transform.childCount : 0; }
		}

		private IBiomeDataManager Biomes
		{
			get { return model.settings.biomeManager; }
		}

		public BiomeManagerComponent(string name = "ServerBiomes")
		{
			_name = name;
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

		public IBiomeData BuildBiomeIfNotExist(int x, int y, int z)
		{
			Debug.Assert(model.settings.biomeNull != null);

			IBiomeData biomeData = null;
			if (this.Biomes.Get(x, y, z, out biomeData))
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
			if (this.Biomes.count > model.settings.biomeNumLimits)
				this.Biomes.GC();
		}

		public override void Update()
		{
			this.AutoGC();
		}
	}
}