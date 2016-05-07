using SlimDX.Direct3D9;
using System;

namespace AtmoLight
{
  public class DxScreenCapture
  {
    public Device device;
    public int deviceIndex;
    public int refreshRate = 60;
    public int resolutionHeight;
    public int resolutionWidth;
    public DxScreenCapture(int index, string monitorName)
    {
      try
      {
        PresentParameters present_params = new PresentParameters();
        present_params.Windowed = true;
        present_params.SwapEffect = SwapEffect.Discard;
        present_params.PresentationInterval = PresentInterval.Immediate;

        int deviceIndex = getMonitor(monitorName);
        device = new Device(new Direct3D(), deviceIndex, DeviceType.Hardware, IntPtr.Zero,
          CreateFlags.SoftwareVertexProcessing, present_params);

        refreshRate = device.GetDisplayMode(deviceIndex).RefreshRate;
        resolutionWidth = device.GetDisplayMode(deviceIndex).Width;
        resolutionHeight = device.GetDisplayMode(deviceIndex).Height;
      }
      catch (Exception ex){
        Log.Error("Error during DxScreenCapture class startup");
        Log.Error(ex.ToString());
      }
    }

    public Surface CaptureScreen()
    {
      try
      {
        Surface s = Surface.CreateOffscreenPlain(device, resolutionWidth,
          resolutionHeight, Format.A8R8G8B8, Pool.Scratch);
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

    private int getMonitor(string monitorName)
    {
      var monitorArray = SlimDX.Windows.DisplayMonitor.EnumerateMonitors();
      int index = 0;

      foreach (var monitor in monitorArray)
      {
        Log.Error("Found monitor: " + monitor.DeviceName);
        Log.Error("Is primary: " + monitor.IsPrimary);
        Log.Error("Width: " + monitor.Bounds.Width);
        Log.Error("Height: " + monitor.Bounds.Height);
        Log.Error("Index: " + index);

        if (Equals(monitor.DeviceName.ToLower(), monitorName.ToLower()))
        {
          Log.Error("Gonna use monitor: " + monitor.DeviceName);
          Log.Error("Is primary: " + monitor.IsPrimary);
          Log.Error("Width: " + monitor.Bounds.Width);
          Log.Error("Height: " + monitor.Bounds.Height);
          Log.Error("Index: " + index);

          return index;
        }

        index++;
      }

      // No named match, fallback to default
      if (monitorArray.Length == 1)
      {
        index = 0;
      }

      return index;
    }
  }
}
