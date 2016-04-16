using SlimDX.Direct3D9;
using System;
using System.Windows.Forms;

namespace AtmoLight
{
  public class DxScreenCapture
  {
    public Device device;

    public DxScreenCapture()
    {
      try
      {
        PresentParameters present_params = new PresentParameters();
        present_params.Windowed = true;
        present_params.SwapEffect = SwapEffect.Discard;

        device = new Device(new Direct3D(), getMonitor(0), DeviceType.Hardware, IntPtr.Zero,
          CreateFlags.SoftwareVertexProcessing, present_params);
      }
      catch (Exception ex)
      {
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
      catch (Exception ex)
      {
      }
      return null;
    }

    private int getMonitor(int monitorIndex)
    {
      var monitorArray = SlimDX.Windows.DisplayMonitor.EnumerateMonitors();
      if ((monitorArray.Length - 1) >= monitorIndex)
      {
        return (monitorArray[monitorIndex] != null) ? monitorIndex : 0;
      }
      return 0;
    }
  }
}
