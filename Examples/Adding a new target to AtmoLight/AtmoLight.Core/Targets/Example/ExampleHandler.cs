using ...;

namespace AtmoLight
{
	class ExampleHandler : ITargets
	{
		public void Initialise(bool force = false)
		{
		// Initialise your target
		}
		
		public void ReInitialise(bool force = false)
		{
		// Reinitialise your target
		}
		
		public void Dispose()
		{
		// Close connections, applications (if needed), ...
		}
		
		public bool IsConnected()
		{
		// Return if you are connected to your target
		}
		
		public bool ChangeEffect(ContentEffect effect)
		{
		// Change the effect on your target
		}
		
		public void ChangeImage(byte[] pixeldata, byte[] bmiInfoHeader)
		{
		// Send a new picture/color infos to your target (MediaPortal Liveview Mode)
		}
	}
}