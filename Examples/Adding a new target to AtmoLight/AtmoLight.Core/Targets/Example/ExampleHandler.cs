using ...;

namespace AtmoLight
{
	class ExampleHandler : ITargets
	{
	    public Target Name { get { return Target.Example; } }

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