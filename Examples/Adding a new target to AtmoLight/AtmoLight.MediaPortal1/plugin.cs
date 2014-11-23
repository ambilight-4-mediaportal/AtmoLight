using ...;

namespace AtmoLight
{
	...

	public class Plugin : ISetupForm, IPlugin
	{
		...
		#region Plugin Ctor/Start/Stop
		...
		plugin void Start()
		{
			...
			AtmoLightObject = Core.GetInstance();
			
			//Example
			if (Settings.exampleTarget)
			{
				AtmoLightObject.AddTarget(Target.Example);
			}
			AtmoLightObject.exampleIP = Settings.exampleIP;
			AtmoLightObject.exmaplePort = Settings.exmaplePort;
			...
		}
		#endregion
		...
	}
}