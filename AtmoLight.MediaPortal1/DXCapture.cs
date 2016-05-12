using SlimDX.Direct3D9;
using System;

namespace AtmoLight
{
  public class DxScreenCapture
  {
    public Device device;
    public int deviceIndex;
    public int refreshRate;
    private SlimDX.Windows.DisplayMonitor deviceMonitor;
    public DxScreenCapture(int index)
    {
      try
      {
        PresentParameters present_params = new PresentParameters();
        present_params.Windowed = true;
        present_params.SwapEffect = SwapEffect.Discard;
        present_params.PresentationInterval = PresentInterval.Immediate;
        int deviceIndex = getMonitor(index);

        device = new Device(new Direct3D(), deviceIndex, DeviceType.Hardware, IntPtr.Zero,
          CreateFlags.SoftwareVertexProcessing, present_params);
        refreshRate = device.GetDisplayMode(0).RefreshRate;
      }
      catch (Exception ex)
      {
        Log.Error("Error during DxScreenCapture class startup");
        Log.Error(ex.ToString());
      }
    }

    public Surface CaptureScreen()
    {
      try
      {
        Surface s = Surface.CreateOffscreenPlain(device, deviceMonitor.Bounds.Width,
          deviceMonitor.Bounds.Height, Format.A8R8G8B8, Pool.Scratch);
        Surface b = Surface.CreateOffscreenPlain(device, (64), (64), Format.A8R8G8B8, Pool.Scratch);
        device.GetFrontBufferData(0, s);
        Surface.FromSurface(b, s, Filter.Triangle, 0);
        s.Dispose();
        return b;
      }
      catch (Exception)
      {
      }
      return null;
    }

    private int getMonitor(int monitorIndex)
    {
      var monitorArray = SlimDX.Windows.DisplayMonitor.EnumerateMonitors();
      int index = 0;
      foreach (var monitor in monitorArray)
      {
        if (index == monitorIndex)
        {
          Log.Debug("AtmoLight - gonna use this monitor for DirectX capture: " + monitor.DeviceName);
          Log.Debug("Is primary: " + monitor.IsPrimary);
          Log.Debug("Width: " + monitor.Bounds.Width);
          Log.Debug("Height: " + monitor.Bounds.Height);
          Log.Debug("Index: " + index);
          deviceMonitor = monitor;

          return index;
        }

        index++;
      }
      return index;
    }
  }
}
