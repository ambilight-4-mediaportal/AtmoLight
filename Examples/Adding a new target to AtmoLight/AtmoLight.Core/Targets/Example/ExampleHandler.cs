using ...;

namespace AtmoLight
{
	class ExampleHandler : ITargets
	{
	    public Target Name { get { return Target.Example; } }
		
		// Define if you want this target to use the delay feature.
		// In most cases this should return true.
		public bool AllowDelay { get { return true; } }

		// List of all the effects supported by your target.
		// Typically targets support at least the setting of a color.
		// So LEDsDisabled as well as StaticColor would be supported then.
		// If your target has an interface to receive image data,
		// then MediaPortalLiveMode, GIFReader, VUMeter and VUMeterRainbow should also be supported.
		// If your target has a method of its own to grab a screenshot, then ExternalLiveMode should be supported.
		public List<ContentEffect> SupportedEffects
		{
			get
			{
				return new List<ContentEffect> {	ContentEffect.LEDsDisabled,
													ContentEffect.MediaPortalLiveMode,
													ContentEffect.StaticColor,
													ContentEffect.GIFReader,
													ContentEffect.VUMeter,
													ContentEffect.VUMeterRainbow
				};
			}
		}
	
		public void Initialise(bool force = false)
		{
			// Initialise your target
			// Best practice is to start a new thread which will try to connect to your software
			// Make sure this thread cant be called more than once at the same time.
		}
		
		public void ReInitialise(bool force = false)
		{
			// Reinitialise your target
			// In general this looks like this:
			/*
			if (coreObject.reInitOnError || force)
			{
				Initialise(force);
			}
			*/
			// Make sure ReInitialise can only happen if the user allows it or if force==true (when user chooses to reconnect via context menu)
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
		
		public void ChangeProfile()
		{
			// Change the profile of your target
		}
		
		public void PowerModeChanged(PowerModes powerMode)
		{
			// Reconnect on resume if needed
			// ChangeEffect to Core.currentEffect
		}
	}
}