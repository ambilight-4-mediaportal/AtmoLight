using ...;

namespace AtmoLight.Configuration
{
	...
	public class ExampleTarget : YesNo
	{
		public override void Load()
		{
			_yes = SettingsManager.Load<Settings>().ExampleTarget;
		}

		public override void Save()
		{
			base.Save();
			Settings settings = SettingsManager.Load<Settings>();
			settings.ExampleTarget = _yes;
			SettingsManager.Save(settings);

			if (_yes)
			{
				Core.GetInstance().AddTarget(Target.Example);
				Core.GetInstance().Initialise();
			}
			else
			{
				Core.GetInstance().RemoveTarget(Target.Example);
			}
		}
	}

	public class ExampleIP : Entry
	{
		public override void Load()
		{
			_value = SettingsManager.Load<Settings>().ExampleIP;
		}

		public override void Save()
		{
			base.Save();
			Settings settings = SettingsManager.Load<Settings>();
			IPAddress ip;
			if (IPAddress.TryParse(_value, out ip))
			{
				settings.ExampleIP = _value;
				SettingsManager.Save(settings);

				Core.GetInstance().exampleIP = _value;
			}
		}

		public override int DisplayLength
		{
			get { return 15; }
		}
	}

	public class ExamplePort : LimitedNumberSelect
	{
		public override void Load()
		{
			_type = NumberType.Integer;
			_step = 1;
			_lowerLimit = 1;
			_upperLimit = 65535;
			_value = SettingsManager.Load<Settings>().ExamplePort;
		}

		public override void Save()
		{
			base.Save();
			Settings settings = SettingsManager.Load<Settings>();
			settings.ExamplePort = (int)_value;
			SettingsManager.Save(settings);

			Core.GetInstance().examplePort = (int)_value;
		}
	}
}