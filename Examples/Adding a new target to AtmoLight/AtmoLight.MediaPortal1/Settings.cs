using ...;

namespace AtmoLight
{
	public class Settings
	{
		#region Config variables
		...
		// Example
		public static bool exampleTarget;
		public static string exampleIP;
		public static int exmaplePort;
		#endregion
		...
		public static void LoadSettings()
		{
			using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MediaPortal.xml")))
			{
				...
				exampleTarget = reader.GetValueAsBool("atmolight", "exampleTarget", false);
				exampleIP = reader.GetValueAsString("atmolight", "exampleIP", "127.0.0.1");
				exmaplePort = reader.GetValueAsInt("atmolight", "exmaplePort", 1234);
			}
		}
		
		public static void SaveSettings()
		{
			using (MediaPortal.Profile.Settings reader = new MediaPortal.Profile.Settings(MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, "MediaPortal.xml")))
			{
				...
				reader.SetValueAsBool("atmolight", "exampleTarget", exampleTarget);
				reader.SetValue("atmolight", "exampleIP", exampleIP);
				reader.SetValue("atmolight", "exmaplePort", exmaplePort);
			}
		}
	}
}