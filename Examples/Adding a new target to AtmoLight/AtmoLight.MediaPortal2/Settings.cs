using ...;

namespace AtmoLight
{
	public class Settings
	{
		...
		[Setting(SettingScope.User, false)]
		public bool ExampleTarget { get; set; }
		
		[Setting(SettingScope.User, "127.0.0.1")]
		public string ExampleIP { get; set; }
		
		[Setting(SettingScope.User, 1234)]
		public int ExamplePort { get; set; }
		
		...
		
		public bool LoadAll()
		{
			settings = settingsManager.Load<Settings>();
			...
			ExampleTarget = settings.ExampleTarget;
			ExampleIP = settings.ExampleIP;
			ExamplePort = settings.ExamplePort;
			...
			return true;
		}
		
		public bool SaveAll()
		{
			...
			settings.ExampleTarget = ExampleTarget;
			settings.ExampleIP = ExampleIP;
			settings.ExamplePort = ExamplePort:
			...
			settingsManager.Save(settings);
			return true;
		}
	}
}