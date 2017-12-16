namespace Cubizer.Time
{
	public sealed class TimeComponent : CubizerComponent<TimeModels>
	{
		public long worldAge;
		public long timeOfDay;

		public override bool active
		{
			get
			{
				return model.enabled;
			}
			set
			{
				model.enabled = value;
			}
		}

		public override void OnEnable()
		{
			worldAge = model.settings.worldAge;
			timeOfDay = model.settings.timeOfDay;
		}
	}
}