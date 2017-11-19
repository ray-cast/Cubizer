namespace Cubizer
{
	public abstract class BiomeGenerator : IBiomeGenerator
	{
		private Terrain _terrain;

		public Terrain terrain
		{
			get { return _terrain; }
		}

		public void InvokeDefaultOnEnable()
		{
			if (transform.parent != null)
				_terrain = transform.parent.GetComponent<BiomeManagerComponent>().context.terrain;
		}

		public void OnEnable()
		{
			this.InvokeDefaultOnEnable();
		}
	}
}