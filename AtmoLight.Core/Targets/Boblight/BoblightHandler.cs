﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;
using MinimalisticTelnet;

namespace AtmoLight
{
  class BoblightHandler : ITargets
  {
    #region Fields
    public Target Name { get { return Target.Boblight; } }
    public bool AllowDelay { get { return true; } }
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
    private List<Light> lights = new List<Light>();
    private int totalLights;
    private double[] gammaCurve = new double[256];

    private volatile bool initLock = false;
    private volatile int reconnectAttempts = 0;

    private bool oldIterpolation;
    private int oldSpeed;
    private double oldGamma;
    #endregion

    #region Constructor
    public BoblightHandler()
    {
      Log.Debug("BoblightHandler - Boblight as target added.");
      oldIterpolation = coreObject.boblightInterpolation;
      oldSpeed = coreObject.boblightSpeed;
      oldGamma = coreObject.boblightGamma;
      CalcGammaCurve();
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
      Disconnect();
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
      try
      {
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
      catch (Exception ex)
      {
        Log.Error("BoblightHandler - Error changing effect to {0}", effect.ToString());
        Log.Error("BoblightHandler - Exception: {0}", ex.Message);
        ReInitialise();
        return false;
      }
    }

    public void ChangeProfile(string profileName)
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

      try
      {
        // Check if settings have changed and need updating
        if (coreObject.boblightInterpolation != oldIterpolation)
        {
          SetOption("interpolation", Convert.ToInt32(coreObject.boblightInterpolation).ToString());
          oldIterpolation = coreObject.boblightInterpolation;
        }
        if (coreObject.boblightSpeed != oldSpeed)
        {
          SetOption("speed", coreObject.boblightSpeed.ToString());
          oldSpeed = coreObject.boblightSpeed;
        }
        if (coreObject.boblightGamma != oldGamma)
        {
          CalcGammaCurve();
          oldGamma = coreObject.boblightGamma;
        }

        // Analyze image
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
      catch (Exception ex)
      {
        Log.Error("BoblightHandler - Error in ChangeImage.");
        Log.Error("BoblightHandler - Exception: {0}", ex.Message);
        ReInitialise();
      }
    }

    public void PowerModeChanged(PowerModes powerMode)
    {
      if (powerMode == PowerModes.Resume)
      {
        Disconnect();
        Initialise();
      }
      else if (powerMode == PowerModes.Suspend)
      {
        ChangeEffect(ContentEffect.LEDsDisabled);
      }
    }
    #endregion

    #region Init Thread
    private bool InitThreaded(bool force = false)
    {
      if (initLock)
      {
        Log.Debug("BoblightHandler - Initialising locked.");
        return false;
      }
      initLock = true;
      Disconnect();
      reconnectAttempts++;

      if (Connect())
      {
        Log.Info("BoblightHandler - Successfully connected to {0}:{1}.", coreObject.boblightIP, coreObject.boblightPort);
        reconnectAttempts = 0;
        initLock = false;

        ChangeEffect(coreObject.GetCurrentEffect());

        return true;
      }
      else
      {
        Log.Error("BoblightHandler - Error connecting to {0}:{1}.", coreObject.boblightIP, coreObject.boblightPort);
        if ((coreObject.reInitOnError || force) && reconnectAttempts < coreObject.boblightMaxReconnectAttempts)
        {
          System.Threading.Thread.Sleep(coreObject.boblightReconnectDelay);
          initLock = false;
          InitThreaded();
        }
        else
        {
          Disconnect();
          reconnectAttempts = 0;
          coreObject.NewConnectionLost(Name);
          initLock = false;
        }
        return false;
      }
    }
    #endregion

    #region Connect
    private bool Connect()
    {
      try
      {
        lights.Clear();
        boblightConnection = new TelnetConnection(coreObject.boblightIP, coreObject.boblightPort);
        if (boblightConnection.IsConnected)
        {
          if (SendCommand("hello\n", true) != "hello")
          {
            return false;
          }

          string lightsMessage = SendCommand("get lights\n", true);
          if (string.IsNullOrEmpty(lightsMessage))
          {
            return false;
          }
          string[] lightsArray = lightsMessage.Split(' ', '\n');
          for (int i = 0; i < lightsArray.Length; i++)
          {
            if (lightsArray[i] == "lights")
            {
              totalLights = Convert.ToInt32(lightsArray[i + 1]);
              i++;
            }
            else if (lightsArray[i] == "light")
            {
              lights.Add(new Light(lightsArray[i + 1],
                (int)Math.Round((Convert.ToDouble(lightsArray[i + 3]) / 100 * (coreObject.GetCaptureHeight() - 1))),
                (int)Math.Round((Convert.ToDouble(lightsArray[i + 4]) / 100 * (coreObject.GetCaptureHeight() - 1))),
                (int)Math.Round((Convert.ToDouble(lightsArray[i + 5]) / 100 * (coreObject.GetCaptureWidth() - 1))),
                (int)Math.Round((Convert.ToDouble(lightsArray[i + 6]) / 100 * (coreObject.GetCaptureWidth() - 1)))));
              i += 6;
            }
          }
          if (lights.Count != totalLights)
          {
            return false;
          }
          SetOption("speed", coreObject.boblightSpeed.ToString());
          SetOption("interpolation", Convert.ToInt32(coreObject.boblightInterpolation).ToString());
          return true;
        }
        else
        {
          return false;
        }
      }
      catch (Exception ex)
      {
        Log.Error("BoblightHandler - Error during connecting.");
        Log.Error("BoblightHandler - Exception: {0}", ex.Message);
        return false;
      }
    }

    private void Disconnect()
    {
      if (boblightConnection != null)
      {
        boblightConnection.Dispose();
        boblightConnection = null;
      }
    }
    #endregion

    #region Boblight
    private void AddPixelXY(int x, int y, int[] rgb)
    {
      for (int i = 0; i < totalLights; i++)
      {
        if (x >= lights[i].hScanStart && x <= lights[i].hScanEnd && y >= lights[i].vScanStart && y <= lights[i].vScanEnd)
        {
          AddPixel(i, rgb);
        }
      }
    }

    private void AddPixel(int index, int[] rgb)
    {
      if (rgb[0] >= coreObject.boblightThreshold || rgb[1] >= coreObject.boblightThreshold || rgb[2] >= coreObject.boblightThreshold)
      {
        if (coreObject.boblightGamma == 1.0)
        {
          lights[index].rgb[0] += Math.Min(Math.Max(rgb[0], 0), 255);
          lights[index].rgb[1] += Math.Min(Math.Max(rgb[1], 0), 255);
          lights[index].rgb[2] += Math.Min(Math.Max(rgb[2], 0), 255);
        }
        else
        {
          lights[index].rgb[0] += (int)gammaCurve[Math.Min(Math.Max(rgb[0], 0), gammaCurve.Length - 1)];
          lights[index].rgb[1] += (int)gammaCurve[Math.Min(Math.Max(rgb[1], 0), gammaCurve.Length - 1)];
          lights[index].rgb[2] += (int)gammaCurve[Math.Min(Math.Max(rgb[2], 0), gammaCurve.Length - 1)];
        }
      }
      lights[index].rgbCount++;
    }

    private void SendRGB()
    {
      string data = null;
      for (int i = 0; i < totalLights; i++)
      {
        double[] rgb = GetRGB(i);
        data += "set light " + lights[i].name + " rgb " + rgb[0].ToString() + " " + rgb[1].ToString() + " " + rgb[2].ToString() + "\n";
        if (coreObject.boblightAutospeed > 0 && lights[i].singleChange > 0.0)
        {
          data += "set light " + lights[i].name + " singlechange " + lights[i].singleChange.ToString() + "\n";
        }
      }
      data += "sync\n";
      SendCommand(data);
    }

    private double[] GetRGB(int index)
    {
      double[] rgb = new double[3];
      if (lights[index].rgbCount == 0)
      {
        for (int i = 0; i < 3; i++)
        {
          lights[index].rgb[i] = 0;
        }
        return new double[] { 0, 0, 0 };
      }

      for (int i = 0; i < 3; i++)
      {
        rgb[i] = Math.Min(Math.Max((double)lights[index].rgb[i] / (double)lights[index].rgbCount / 255.0f, 0.0f), 1.0f);
        lights[index].rgb[i] = 0;
      }
      lights[index].rgbCount = 0;

      if (coreObject.boblightAutospeed > 0)
      {
        double change = Math.Abs(rgb[0] - lights[index].rgbPrev[0]) + Math.Abs(rgb[1] - lights[index].rgbPrev[1]) + Math.Abs(rgb[2] - lights[index].rgbPrev[2]);
        change /= 3.0;

        if (change > 0.001)
        {
          lights[index].singleChange = Math.Min(Math.Max(change * coreObject.boblightAutospeed / 10.0, 0.0), 1.0);
        }
        else
        {
          lights[index].singleChange = 0.0;
        }
      }

      lights[index].rgbPrev[0] = (int)rgb[0];
      lights[index].rgbPrev[1] = (int)rgb[1];
      lights[index].rgbPrev[2] = (int)rgb[2];

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
      SendCommand("set priority " + priority.ToString() + "\n");
    }

    private void SetOption(string option, string value)
    {
      string data = null;
      for (int i = 0; i < totalLights; i++)
      {
        data += "set light " + lights[i].name + " " + option + " " + value + "\n";
      }
      SendCommand(data);
    }

    private string SendCommand(string command, bool noReinit = false)
    {
      try
      {
        boblightConnection.Write(command);
        return CleanupReadString(boblightConnection.Read());
      }
      catch (Exception ex)
      {
        Log.Error("BoblightHandler - Error communicating with server.");
        Log.Error("BoblightHandler - Command: {0}", command);
        Log.Error("BoblightHandler - Exception: {0}", ex.Message);
        if (!noReinit)
        {
          ReInitialise();
        }
        return null;
      }
    }
    #endregion;

    #region Utilities
    private string CleanupReadString(string readString)
    {
      if (readString.Length == 0)
      {
        return null;
      }
      return readString.Remove(readString.Length - 1, 1);
    }

    private void CalcGammaCurve()
    {
      for (int i = 0; i < gammaCurve.Length; i++)
      {
        gammaCurve[i] = Math.Pow((double)i / ((double)gammaCurve.Length - 1.0f), coreObject.boblightGamma) * (gammaCurve.Length - 1.0f);
      }
    }
    #endregion
  }
}
