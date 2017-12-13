namespace Cubizer.Biome
{
	public abstract class BiomeGenerator : IBiomeGenerator
	{
		private CubizerContext _context;

		public override CubizerContext context
		{
			get { return _context; }
		}

		public override void Init(CubizerContext context)
		{
			_context = context;
		}
	}
}