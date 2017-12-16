namespace Cubizer.Time
{
	public class TimeComponent : CubizerComponent<TimeModels>
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
	}
}