using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace AtmoLight
{
  unsafe class BoblightHandler : ITargets
  {
    // http://mirrors.xbmc.org/build-deps/addon-deps/binaries/libboblight/win32/libboblight-win32.0.dll.zip
    #region libboblight Import
    [DllImport("libboblight-win32.0.dll")]
    private static extern IntPtr boblight_init();
    [DllImport("libboblight-win32.0.dll")]
    private static extern void boblight_destroy(IntPtr vpboblight);
    [DllImport("libboblight-win32.0.dll")]
    private static extern int boblight_connect(IntPtr vpboblight, string address, int port, int usectimeout);
    [DllImport("libboblight-win32.0.dll")]
    private static extern int boblight_setpriority(IntPtr vpboblight, int priority);
    [DllImport("libboblight-win32.0.dll")]
    private static extern string boblight_geterror(IntPtr vpboblight);
    [DllImport("libboblight-win32.0.dll")]
    private static extern int boblight_getnrlights(IntPtr vpboblight);
    [DllImport("libboblight-win32.0.dll")]
    private static extern string boblight_getlightname(IntPtr vpboblight, int lightnr);
    [DllImport("libboblight-win32.0.dll")]
    private static extern int boblight_getnroptions(IntPtr vpboblight);
    [DllImport("libboblight-win32.0.dll")]
    private static extern string boblight_getoptiondescript(IntPtr vpboblight, int option);
    [DllImport("libboblight-win32.0.dll")]
    private static extern int boblight_setoption(IntPtr vpboblight, int lightnr, string option);
    [DllImport("libboblight-win32.0.dll")]
    private static extern int boblight_getoption(IntPtr vpboblight, int lightnr, string option, string[] output);
    [DllImport("libboblight-win32.0.dll")]
    private static extern void boblight_setscanrange(IntPtr vpboblight, int width, int height);
    [DllImport("libboblight-win32.0.dll")]
    private static extern int boblight_addpixel(IntPtr vpboblight, int lightnr, int[] rgb);
    [DllImport("libboblight-win32.0.dll")]
    private static extern void boblight_addpixelxy(IntPtr vpboblight, int x, int y, int[] rgb);
    [DllImport("libboblight-win32.0.dll")]
    private static extern int boblight_sendrgb(IntPtr vpboblight, int sync, int[] outputused);
    [DllImport("libboblight-win32.0.dll")]
    private static extern int boblight_ping(IntPtr vpboblight, int[] outputused);
    #endregion

    #region Fields
    public Target Name { get { return Target.Boblight; } }

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

    private Core coreObject = Core.GetInstance();
    private IntPtr boblightHandle;
    private bool isConnected = false;
    #endregion

    public BoblightHandler()
    {
      boblightHandle = boblight_init();
    }

    public void Initialise(bool force = false)
    {
      string address = "192.168.1.33";
      int port = 19333;
      int timeout = 1000000;
      if (boblight_connect(boblightHandle, address, port, timeout) != 0)
      {
        isConnected = true;
        Log.Info("BoblightHandler - Successfully connected to {0}:{1}.", address, port);
      }
      else
      {
        isConnected = false;
        Log.Error("BoblightHandler - Error connecting to {0}:{1}.", address, port);
      }
    }

    public void ReInitialise(bool force = false)
    {
      return;
    }

    public void Dispose()
    {
      boblight_destroy(boblightHandle);
      isConnected = false;
    }

    public bool IsConnected()
    {
      return isConnected;
    }

    public bool ChangeEffect(ContentEffect effect)
    {
      return true;
    }

    public void ChangeProfile()
    {
      return;
    }

    Stopwatch stopwatch = new Stopwatch();
    public void ChangeImage(byte[] pixeldata, byte[] bmiInfoHeader)
    {
      if (!stopwatch.IsRunning && IsConnected())
      {
        stopwatch.Start();
      }
      if (stopwatch.ElapsedMilliseconds < 100 || !IsConnected())
      {
        stopwatch.Stop();
        return;
      }
      boblight_setscanrange(boblightHandle, coreObject.GetCaptureWidth(), coreObject.GetCaptureHeight());
      int[] rgb = new int[] { 0, 0, 0 };
      for (int y = 0; y < coreObject.GetCaptureHeight(); y++)
      {
        int row = coreObject.GetCaptureWidth() * y * 4;
        for (int x = 0; x < coreObject.GetCaptureWidth(); x++)
        {
          rgb[0] = pixeldata[row + x * 4 + 2];
          rgb[1] = pixeldata[row + x * 4 + 1];
          rgb[2] = pixeldata[row + x * 4];
          boblight_addpixelxy(boblightHandle, x, y, rgb);
        }
      }
      boblight_setpriority(boblightHandle, 128);
      boblight_sendrgb(boblightHandle, 1, null);
      stopwatch.Restart();
    }
  }
}
