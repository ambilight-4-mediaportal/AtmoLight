using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace AtmoLight
{
  class BoblightHandler : ITargets
  {
    #region libboblight Import
    // Boblight lib needed.
    // Source: https://code.google.com/p/boblight/source/browse/#svn%2Ftrunk%2Fsrc%2Flib
    // Binary: http://mirrors.xbmc.org/build-deps/addon-deps/binaries/libboblight/win32/libboblight-win32.0.dll.zip

    [DllImport("libboblight-win32.0.dll")]
    private static extern IntPtr boblight_init();
    [DllImport("libboblight-win32.0.dll")]
    private static extern void boblight_destroy(IntPtr vpboblight);
    [DllImport("libboblight-win32.0.dll")]
    private static extern int boblight_connect(IntPtr vpboblight, string address, int port, int usectimeout);
    [DllImport("libboblight-win32.0.dll")]
    private static extern int boblight_setpriority(IntPtr vpboblight, int priority);
    [DllImport("libboblight-win32.0.dll")]
    private static extern IntPtr boblight_geterror(IntPtr vpboblight);
    [DllImport("libboblight-win32.0.dll")]
    private static extern int boblight_getnrlights(IntPtr vpboblight);
    [DllImport("libboblight-win32.0.dll")]
    private static extern IntPtr boblight_getlightname(IntPtr vpboblight, int lightnr);
    [DllImport("libboblight-win32.0.dll")]
    private static extern int boblight_getnroptions(IntPtr vpboblight);
    [DllImport("libboblight-win32.0.dll")]
    private static extern IntPtr boblight_getoptiondescript(IntPtr vpboblight, int option);
    [DllImport("libboblight-win32.0.dll")]
    private static extern int boblight_setoption(IntPtr vpboblight, int lightnr, string option);
    [DllImport("libboblight-win32.0.dll")]
    private static extern int boblight_getoption(IntPtr vpboblight, int lightnr, string option, [In][MarshalAsAttribute(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] output);
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

    private IntPtr boblightHandle = IntPtr.Zero;
    private volatile bool isConnected = false;

    Stopwatch maxFPSStopwatch = new Stopwatch();
    private Thread initThreadHelper;
    private volatile bool initLock = false;
    private volatile int reconnectAttempts = 0;
    private int timeout = 1000000;
    #endregion

    #region Constructor
    public BoblightHandler()
    {
      Log.Debug("BoblightHandler - Boblight as target added.");
    }
    #endregion

    #region ITargets methods
    public void Initialise(bool force = false)
    {
      if (!initLock)
      {
        initThreadHelper = new Thread(() => InitThreaded(force));
        initThreadHelper.Name = "AtmoLight Boblight Init";
        initThreadHelper.IsBackground = true;
        initThreadHelper.Start();
      }
    }

    public void ReInitialise(bool force = false)
    {
      if (coreObject.reInitOnError || force)
      {
        Initialise(force);
      }
    }

    public void Dispose()
    {
      Log.Debug("BoblightHandler - Disposing Boblight handler.");
      boblight_destroy(boblightHandle);
      boblightHandle = IntPtr.Zero;
      isConnected = false;
    }

    public bool IsConnected()
    {
      return isConnected;
    }

    public bool ChangeEffect(ContentEffect effect)
    {
      if (!IsConnected())
      {
        return false;
      }
      switch (effect)
      {
        case ContentEffect.MediaPortalLiveMode:
        case ContentEffect.GIFReader:
        case ContentEffect.VUMeter:
        case ContentEffect.VUMeterRainbow:
          return true;
        case ContentEffect.StaticColor:
          boblight_addpixel(boblightHandle, -1, coreObject.staticColor);
          if (boblight_sendrgb(boblightHandle, 1, null) != 0)
          {
            Log.Info("BoblightHandler - Successfully set static color to R:{0} G:{1} B:{2}.", coreObject.staticColor[0], coreObject.staticColor[1], coreObject.staticColor[2]);
            return true;
          }
          else
          {
            ReInitialise();
            return false;
          }
        case ContentEffect.LEDsDisabled:
        case ContentEffect.Undefined:
        default:
          boblight_addpixel(boblightHandle, -1, new int[] { 0, 0, 0 });
          if (boblight_sendrgb(boblightHandle, 1, null) != 0)
          {
            Log.Info("BoblightHandler - Successfully disabled LEDs.");
            return true;
          }
          else
          {
            ReInitialise();
            return false;
          }
      }
    }

    public void ChangeProfile()
    {
      return;
    }

    public void ChangeImage(byte[] pixeldata, byte[] bmiInfoHeader)
    {
      if (!maxFPSStopwatch.IsRunning && IsConnected())
      {
        maxFPSStopwatch.Start();
      }
      if (maxFPSStopwatch.ElapsedMilliseconds < (1000 / coreObject.boblightMaxFPS) || !IsConnected())
      {
        if (!IsConnected() && maxFPSStopwatch.IsRunning)
        {
          maxFPSStopwatch.Stop();
        }
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
      if (boblight_sendrgb(boblightHandle, 1, null) == 0)
      {
        maxFPSStopwatch.Stop();
        ReInitialise();
        return;
      }
      maxFPSStopwatch.Restart();
    }
    #endregion

    #region Threads
    private bool InitThreaded(bool force = false)
    {
      if (initLock)
      {
        Log.Debug("BoblightHandler - Initialising locked.");
        return false;
      }
      initLock = true;
      isConnected = false;
      reconnectAttempts++;
      if (boblightHandle == IntPtr.Zero)
      {
        boblightHandle = boblight_init();
      }
      if (boblight_connect(boblightHandle, coreObject.boblightIP, coreObject.boblightPort, timeout) != 0)
      {
        Log.Info("BoblightHandler - Successfully connected to {0}:{1}.", coreObject.boblightIP, coreObject.boblightPort);

        SetOption("speed", coreObject.boblightSpeed.ToString());
        SetOption("autospeed", coreObject.boblightAutospeed.ToString());
        SetOption("interpolation", Convert.ToInt32(coreObject.boblightInterpolation).ToString());
        SetOption("saturation", coreObject.boblightSaturation.ToString());
        SetOption("value", coreObject.boblightValue.ToString());
        SetOption("threshold", coreObject.boblightThreshold.ToString());

        isConnected = true;
        reconnectAttempts = 0;
        initLock = false;

        ChangeEffect(coreObject.GetCurrentEffect());
        coreObject.SetAtmoLightOn(coreObject.GetCurrentEffect() == ContentEffect.LEDsDisabled || coreObject.GetCurrentEffect() == ContentEffect.LEDsDisabled ? false : true);

        return true;
      }
      else
      {
        Log.Error("BoblightHandler - Error connecting to {0}:{1}.", coreObject.boblightIP, coreObject.boblightPort);
        string err = Marshal.PtrToStringAnsi(boblight_geterror(boblightHandle));
        if (string.IsNullOrEmpty(err))
        {
          Log.Error("BoblightHandler - {0}", err);
        }
        isConnected = false;
        if ((coreObject.reInitOnError || force) && reconnectAttempts < coreObject.boblightMaxReconnectAttempts)
        {
          System.Threading.Thread.Sleep(coreObject.boblightReconnectDelay);
          initLock = false;
          InitThreaded();
        }
        else
        {
          reconnectAttempts = 0;
          coreObject.NewConnectionLost(Name);
          initLock = false;
        }
        return false;
      }
    }
    #endregion

    #region Utilities
    private void SetOption(string option, string value)
    {
      if (boblight_setoption(boblightHandle, -1, option + " " + value.ToString()) == 0)
      {
        Log.Error("BoblightHandler - Error setting {0} to {1}", option, value);
        string err = Marshal.PtrToStringAnsi(boblight_geterror(boblightHandle));
        if (string.IsNullOrEmpty(err))
        {
          Log.Error("BoblightHandler - {0}", err);
        }
      }
    }
    #endregion
  }
}
