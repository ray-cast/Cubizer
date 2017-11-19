namespace Cubizer
{
	public abstract class BiomeGenerator : IBiomeGenerator
	{
		private CubizerBehaviour _terrain;

		public CubizerBehaviour terrain
		{
			get { return _terrain; }
		}

		public void InvokeDefaultOnEnable()
		{
			if (transform.parent != null)
				_terrain = transform.parent.GetComponent<BiomeManagerComponent>().context.behaviour;
		}

		public void OnEnable()
		{
			this.InvokeDefaultOnEnable();
		}
	}
}