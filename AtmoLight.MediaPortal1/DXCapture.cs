using SlimDX.Direct3D9;
using System;
using System.Windows.Forms;

namespace AtmoLight
{
  public class DxScreenCapture
  {
    public Device device;
    public int refreshRate = 60;

    public DxScreenCapture(int monitorIndex)
    {
      try
      {
        PresentParameters present_params = new PresentParameters();
        present_params.Windowed = true;
        present_params.SwapEffect = SwapEffect.Discard;
        present_params.PresentationInterval = PresentInterval.Immediate;

        device = new Device(new Direct3D(), monitorIndex, DeviceType.Hardware, IntPtr.Zero,
          CreateFlags.SoftwareVertexProcessing, present_params);
        refreshRate = device.GetDisplayMode(monitorIndex).RefreshRate;
      }
      catch (Exception){
      }

    }

    public Surface CaptureScreen()
    {
      try
      {
        Surface s = Surface.CreateOffscreenPlain(device, Screen.PrimaryScreen.Bounds.Width,
          Screen.PrimaryScreen.Bounds.Height, Format.A8R8G8B8, Pool.Scratch);
        Surface b = Surface.CreateOffscreenPlain(device, (64), (64), Format.A8R8G8B8, Pool.Scratch);
        device.GetFrontBufferData(0, s);
        Surface.FromSurface(b, s, Filter.Triangle, 0);
        s.Dispose();
        return b;
      }
      catch (Exception){
      }
      return null;
    }
  }
}
