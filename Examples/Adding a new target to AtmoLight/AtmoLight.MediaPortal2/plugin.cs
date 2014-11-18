using ...;

namespace AtmoLight
{
	public class Plugin : IPluginStateTracker
	{
		...
		private void Initialise()
		{
			...
			// Example
			if (settings.ExampleTarget)
			{
				AtmoLightObject.AddTarget(Target.Example);
			}
			AtmoLightObject.exampleIP = settings.ExampleIP;
			AtmoLightObject.examplePort = settings.ExamplePort;
			...
		}
		...
	}
}