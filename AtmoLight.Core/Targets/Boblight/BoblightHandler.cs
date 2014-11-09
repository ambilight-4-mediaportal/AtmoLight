using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using MinimalisticTelnet;

namespace AtmoLight
{
  class BoblightHandler : ITargets
  {
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

    Stopwatch maxFPSStopwatch = new Stopwatch();
    private Thread initThreadHelper;

    private TelnetConnection boblightConnection;
    private List<string> lightNames = new List<string>();
    private List<int> vscanStart = new List<int>();
    private List<int> vscanEnd = new List<int>();
    private List<int> hscanStart = new List<int>();
    private List<int> hscanEnd = new List<int>();
    private List<int[]> rgbValues = new List<int[]>();
    private List<int[]> rgbValuesPrev = new List<int[]>();
    private List<double> singleChange = new List<double>();
    private List<int> rgbCount = new List<int>();
    private int totalLights;
    private double gamma = 1.0;

    private volatile bool initLock = false;
    private volatile int reconnectAttempts = 0;
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
      boblightConnection.Dispose();
    }

    public bool IsConnected()
    {
      if (boblightConnection == null)
      {
        return false;
      }
      return boblightConnection.IsConnected && !initLock;
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
          ChangeStaticColor(coreObject.staticColor);
          return true;
        case ContentEffect.LEDsDisabled:
        case ContentEffect.Undefined:
        default:
          ChangeStaticColor(new int[] { 0, 0, 0 });
          return true;
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
      int[] rgb = new int[] { 0, 0, 0 };
      for (int y = 0; y < coreObject.GetCaptureHeight(); y++)
      {
        int row = coreObject.GetCaptureWidth() * y * 4;
        for (int x = 0; x < coreObject.GetCaptureWidth(); x++)
        {
          rgb[0] = pixeldata[row + x * 4 + 2];
          rgb[1] = pixeldata[row + x * 4 + 1];
          rgb[2] = pixeldata[row + x * 4];
          AddPixelXY(x, y, rgb);
        }
      }
      SetPriority(128);
      SendRGB();
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

      reconnectAttempts++;

      if (Connect())
      {
        reconnectAttempts = 0;
        initLock = false;

        ChangeEffect(coreObject.GetCurrentEffect());
        coreObject.SetAtmoLightOn(coreObject.GetCurrentEffect() == ContentEffect.LEDsDisabled || coreObject.GetCurrentEffect() == ContentEffect.LEDsDisabled ? false : true);

        return true;
      }
      else
      {
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

    #region Boblight
    private bool Connect()
    {
      try
      {
        lightNames.Clear();
        vscanStart.Clear();
        vscanEnd.Clear();
        hscanStart.Clear();
        hscanEnd.Clear();
        rgbValues.Clear();
        rgbValuesPrev.Clear();
        singleChange.Clear();
        rgbCount.Clear();
        boblightConnection = new TelnetConnection(coreObject.boblightIP, coreObject.boblightPort);
        if (boblightConnection.IsConnected)
        {
          boblightConnection.WriteLine("hello");
          if (CleanupReadString(boblightConnection.Read()) != "hello")
          {
            Log.Error("BoblightHandler - Error connecting to {0}:{1}.", coreObject.boblightIP, coreObject.boblightPort);
            return false;
          }

          boblightConnection.WriteLine("get lights");
          string lights = CleanupReadString(boblightConnection.Read());
          if (string.IsNullOrEmpty(lights))
          {
            Log.Error("BoblightHandler - Error connecting to {0}:{1}.", coreObject.boblightIP, coreObject.boblightPort);
            return false;
          }
          string[] lightsArray = lights.Split(' ', '\n');
          for (int i = 0; i < lightsArray.Length; i++)
          {
            if (lightsArray[i] == "lights")
            {
              totalLights = Convert.ToInt32(lightsArray[i + 1]);
              i++;
            }
            else if (lightsArray[i] == "light")
            {
              lightNames.Add(lightsArray[i + 1]);
              vscanStart.Add((int)Math.Round((Convert.ToDouble(lightsArray[i + 3]) / 100 * (coreObject.GetCaptureHeight() - 1))));
              vscanEnd.Add((int)Math.Round((Convert.ToDouble(lightsArray[i + 4]) / 100 * (coreObject.GetCaptureHeight() - 1))));
              hscanStart.Add((int)Math.Round((Convert.ToDouble(lightsArray[i + 5]) / 100 * (coreObject.GetCaptureWidth() - 1))));
              hscanEnd.Add((int)Math.Round((Convert.ToDouble(lightsArray[i + 6]) / 100 * (coreObject.GetCaptureWidth() - 1))));
              rgbValues.Add(new int[] { 0, 0, 0 });
              rgbValuesPrev.Add(new int[] { 0, 0, 0 });
              singleChange.Add(0.0f);
              rgbCount.Add(0);
              i += 6;
            }
          }
          Log.Info("BoblightHandler - Successfully connected to {0}:{1}.", coreObject.boblightIP, coreObject.boblightPort);
          return true;
        }
        else
        {
          Log.Error("BoblightHandler - Error connecting to {0}:{1}.", coreObject.boblightIP, coreObject.boblightPort);
          return false;
        }
      }
      catch (Exception ex)
      {
        Log.Error("BoblightHandler - Error connecting to {0}:{1}.", coreObject.boblightIP, coreObject.boblightPort);
        Log.Error("BoblightHandler - Exception: {0}", ex.Message);
        return false;
      }
    }

    private void AddPixelXY(int x, int y, int[] rgb)
    {
      for (int i = 0; i < totalLights; i++)
      {
        if (x >= hscanStart[i] && x <= hscanEnd[i] && y >= vscanStart[i] && y <= vscanEnd[i])
        {
          AddPixel(i, rgb);
        }
      }
    }

    private void AddPixel(int index, int[] rgb)
    {
      if (rgb[0] >= coreObject.boblightThreshold || rgb[1] >= coreObject.boblightThreshold || rgb[2] >= coreObject.boblightThreshold)
      {
        if (gamma == 1.0)
        {
          rgbValues[index][0] += Math.Min(Math.Max(rgb[0], 0), 255);
          rgbValues[index][1] += Math.Min(Math.Max(rgb[1], 0), 255);
          rgbValues[index][2] += Math.Min(Math.Max(rgb[2], 0), 255);
        }
        else
        {
          Log.Error("BoblightHandler - You should never see this error message.");
          /*
          m_rgb[0] += m_gammacurve[Clamp(rgb[0], 0, GAMMASIZE - 1)];
          m_rgb[1] += m_gammacurve[Clamp(rgb[1], 0, GAMMASIZE - 1)];
          m_rgb[2] += m_gammacurve[Clamp(rgb[2], 0, GAMMASIZE - 1)];
           * */
        }
      }
      rgbCount[index]++;
    }

    private void SendRGB()
    {
      string data = "";
      for (int i = 0; i < totalLights; i++)
      {
        double[] rgb = GetRGB(i);
        data += "set light " + lightNames[i] + " rgb " + rgb[0].ToString() + " " + rgb[1].ToString() + " " + rgb[2].ToString() + "\n";
        if (coreObject.boblightAutospeed > 0 && singleChange[i] > 0.0)
        {
          data += "set light " + lightNames[i] + " singlechange " + singleChange[i].ToString() + "\n";
        }
      }
      data += "sync\n";
      boblightConnection.Write(data);
    }

    private double[] GetRGB(int index)
    {
      double[] rgb = new double[3];
      if (rgbCount[index] == 0)
      {
        for (int i = 0; i < 3; i++)
        {
          rgbValues[index][i] = 0;
        }
        return new double[] { 0, 0, 0 };
      }

      for (int i = 0; i < 3; i++)
      {
        rgb[i] = Math.Min(Math.Max((double)rgbValues[index][i] / (double)rgbCount[index] / 255.0f, 0.0f), 1.0f);
        rgbValues[index][i] = 0;
      }
      rgbCount[index] = 0;

      if (coreObject.boblightAutospeed > 0)
      {
        double change = Math.Abs(rgb[0] - rgbValuesPrev[index][0]) + Math.Abs(rgb[1] - rgbValuesPrev[index][1]) + Math.Abs(rgb[2] - rgbValuesPrev[index][2]);
        change /= 3.0;

        if (change > 0.001)
        {
          singleChange[index] = Math.Min(Math.Max(change * coreObject.boblightAutospeed / 10.0, 0.0), 1.0);
        }
        else
        {
          singleChange[index] = 0.0;
        }
      }

      rgbValuesPrev[index][0] = (int)rgb[0];
      rgbValuesPrev[index][1] = (int)rgb[1];
      rgbValuesPrev[index][2] = (int)rgb[2];

      if (coreObject.boblightValue != 1.0 || coreObject.boblightSaturation != 1.0)
      {
        double[] hsv = new double[3];
        double max = Math.Max(Math.Max(rgb[0], rgb[1]), rgb[2]);
        double min = Math.Min(Math.Min(rgb[0], rgb[1]), rgb[2]);

        if (min == max)
        {
          hsv[0] = -1.0f;
          hsv[1] = 0.0;
          hsv[2] = min;
        }
        else
        {
          if (max == rgb[0])
          {
            hsv[0] = (60.0f * ((rgb[1] - rgb[2]) / (max - min)) + 360.0f);
            while (hsv[0] >= 360.0f)
            {
              hsv[0] -= 360.0f;
            }
          }
          else if (max == rgb[1])
          {
            hsv[0] = 60.0f * ((rgb[2] - rgb[0]) / (max - min)) + 120.0f;
          }
          else if (max == rgb[2])
          {
            hsv[0] = 60.0f * ((rgb[0] - rgb[1]) / (max - min)) + 240.0f;
          }

          hsv[1] = (max - min) / max;
          hsv[2] = max;
        }

        hsv[1] = Math.Min(Math.Max(hsv[1] * coreObject.boblightSaturation, 0.0f), 1.0f);
        hsv[2] = Math.Min(Math.Max(hsv[2] * coreObject.boblightValue, 0.0f), 1.0f);

        if (hsv[0] == -1.0f)
        {
          for (int i = 0; i < 3; i++)
          {
            rgb[i] = hsv[2];
          }
        }
        else
        {
          int hi = (int)(hsv[0] / 60.0f) % 6;
          double f = (hsv[0] / 60.0f) - (float)(int)(hsv[0] / 60.0f);

          double s = hsv[1];
          double v = hsv[2];
          double p = v * (1.0f - s);
          double q = v * (1.0f - f * s);
          double t = v * (1.0f - (1.0f - f) * s);

          if (hi == 0)
          { rgb[0] = v; rgb[1] = t; rgb[2] = p; }
          else if (hi == 1)
          { rgb[0] = q; rgb[1] = v; rgb[2] = p; }
          else if (hi == 2)
          { rgb[0] = p; rgb[1] = v; rgb[2] = t; }
          else if (hi == 3)
          { rgb[0] = p; rgb[1] = q; rgb[2] = v; }
          else if (hi == 4)
          { rgb[0] = t; rgb[1] = p; rgb[2] = v; }
          else if (hi == 5)
          { rgb[0] = v; rgb[1] = p; rgb[2] = q; }
        }

        for (int i = 0; i < 3; i++)
        {
          rgb[i] = Math.Min(Math.Max(rgb[i], 0.0f), 1.0f);
        }
      }
      return rgb;
    }

    private void ChangeStaticColor(int[] rgb)
    {
      for (int i = 0; i < totalLights; i++)
      {
        AddPixel(i, rgb);
      }
      SendRGB();
    }

    private void SetPriority(int priority)
    {
      boblightConnection.WriteLine("set priority " + priority.ToString());
    }
    #endregion;

    #region Utilities
    private string CleanupReadString(string readString)
    {
      return readString.Remove(readString.Length - 1, 1);
    }
    #endregion
  }
}
