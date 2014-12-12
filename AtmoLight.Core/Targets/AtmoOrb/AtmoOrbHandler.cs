using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace AtmoLight.Targets
{
  public enum LampType
  {
    UDP,
    TCP
  }

  class AtmoOrbHandler : ITargets
  {
    #region Fields
    public Target Name { get { return Target.AtmoOrb; } }
    public bool AllowDelay { get { return true; } }
    public List<ContentEffect> SupportedEffects
    {
      get
      {
        return new List<ContentEffect> {  ContentEffect.GIFReader,
                                          ContentEffect.LEDsDisabled,
                                          ContentEffect.MediaPortalLiveMode,
                                          ContentEffect.StaticColor,
                                          ContentEffect.VUMeter,
                                          ContentEffect.VUMeterRainbow
        };
      }
    }

    private Core coreObject = Core.GetInstance();
    private volatile bool initLock = false;
    private Thread initThreadHelper;

    private UdpClient udpServer;
    private UdpClient udpClientBroadcast;

    private double[] gammaCurve = new double[256];

    List<ILamp> lamps = new List<ILamp>();
    #endregion

    #region Constructor
    public AtmoOrbHandler()
    {
      CalcGammaCurve();
      Log.Debug("AtmoOrbHandler - AtmoOrb as target added.");
    }
    #endregion

    #region ITargets Methods

    public void Initialise(bool force = false)
    {
      if (!initLock)
      {
        initThreadHelper = new Thread(() => InitThreaded(force));
        initThreadHelper.Name = "AtmoLight AtmoOrb Init";
        initThreadHelper.IsBackground = true;
        initThreadHelper.Start();
      }
    }

    public void ReInitialise(bool force)
    {
      if (coreObject.reInitOnError || force)
      {
        Initialise(force);
      }
    }

    public void Dispose()
    {
      Log.Debug("AtmoOrbHandler - Disposing AtmoOrb handler.");
      foreach (var lamp in lamps)
      {
        lamp.Disconnect();
      }
      lamps = null;
    }

    public bool IsConnected()
    {
      foreach (var lamp in lamps)
      {
        if (lamp.IsConnected())
        {
          return true;
        }
      }
      return false;
    }

    public bool ChangeEffect(ContentEffect effect)
    {
      switch (effect)
      {
        case ContentEffect.MediaPortalLiveMode:
        case ContentEffect.GIFReader:
        case ContentEffect.VUMeter:
        case ContentEffect.VUMeterRainbow:
          return true;
        case ContentEffect.StaticColor:
          System.Threading.Thread.Sleep(250);
          ChangeColor(coreObject.staticColor[0], coreObject.staticColor[1], coreObject.staticColor[2]);
          return true;
        case ContentEffect.LEDsDisabled:
        case ContentEffect.Undefined:
        default:
          System.Threading.Thread.Sleep(250);
          ChangeColor(0, 0, 0);
          return true;
      }
    }

    public void ChangeImage(byte[] pixeldata, byte[] bmiInfoHeader)
    {
      if (!IsConnected())
      {
        return;
      }
      try
      {
        if (coreObject.GetCurrentEffect() == ContentEffect.VUMeter || coreObject.GetCurrentEffect() == ContentEffect.VUMeterRainbow)
        {
          for (int y = 0; y < coreObject.GetCaptureHeight(); y++)
          {
            int row = coreObject.GetCaptureWidth() * y * 4;

            if (pixeldata[row] != 0 || pixeldata[row + 1] != 0 || pixeldata[row + 2] != 0)
            {
              ChangeColor(pixeldata[row + 2], pixeldata[row + 1], pixeldata[row]);
              return;
            }
            else if (pixeldata[row + ((coreObject.GetCaptureWidth() - 1) * 4)] != 0 || pixeldata[row + ((coreObject.GetCaptureWidth() - 1) * 4) + 1] != 0 || pixeldata[row + ((coreObject.GetCaptureWidth() - 1) * 4) + 2] != 0)
            {
              ChangeColor(pixeldata[row + ((coreObject.GetCaptureWidth() - 1) * 4) + 2], pixeldata[row + ((coreObject.GetCaptureWidth() - 1) * 4) + 1], pixeldata[row + ((coreObject.GetCaptureWidth() - 1) * 4)]);
              return;
            }
          }
          ChangeColor(0, 0, 0);
        }
        else
        {
          foreach (var lamp in lamps)
          {
            if (lamp.IsConnected())
            {
              lamp.OverallAverageColor = new int[3];
              lamp.AverageColor = new int[3];
              lamp.PreviousColor = new int[3];
              lamp.PixelCount = 0;
            }
          }
          for (int y = 0; y < coreObject.GetCaptureHeight(); y++)
          {
            int row = coreObject.GetCaptureWidth() * y * 4;
            for (int x = 0; x < coreObject.GetCaptureWidth(); x++)
            {
              foreach (var lamp in lamps)
              {
                if (lamp.IsConnected())
                {
                  if ((lamp.ZoneInverted == false && x >= lamp.HScanStart / 100 * coreObject.GetCaptureWidth() && x <= lamp.HScanEnd / 100 * coreObject.GetCaptureWidth() && y >= lamp.VScanStart / 100 * coreObject.GetCaptureHeight() && y <= lamp.VScanEnd / 100 * coreObject.GetCaptureHeight())
                   || (lamp.ZoneInverted == true && (x <= lamp.HScanStart / 100 * coreObject.GetCaptureWidth() || x >= lamp.HScanEnd / 100 * coreObject.GetCaptureWidth()) && (y <= lamp.VScanStart / 100 * coreObject.GetCaptureHeight() || y >= lamp.VScanEnd / 100 * coreObject.GetCaptureHeight())))
                  {
                    lamp.OverallAverageColor[0] += pixeldata[row + x * 4 + 2];
                    lamp.OverallAverageColor[1] += pixeldata[row + x * 4 + 1];
                    lamp.OverallAverageColor[2] += pixeldata[row + x * 4];
                    if (Math.Abs(pixeldata[row + x * 4 + 2] - pixeldata[row + x * 4 + 1]) > coreObject.atmoOrbMinDiversion || Math.Abs(pixeldata[row + x * 4 + 2] - pixeldata[row + x * 4]) > coreObject.atmoOrbMinDiversion || Math.Abs(pixeldata[row + x * 4 + 1] - pixeldata[row + x * 4]) > coreObject.atmoOrbMinDiversion)
                    {
                      lamp.AverageColor[0] += pixeldata[row + x * 4 + 2];
                      lamp.AverageColor[1] += pixeldata[row + x * 4 + 1];
                      lamp.AverageColor[2] += pixeldata[row + x * 4];
                      lamp.PixelCount++;
                    }
                  }
                }
              }
            }
          }
          foreach (var lamp in lamps)
          {
            if (lamp.IsConnected())
            {
              lamp.OverallAverageColor[0] /= (coreObject.GetCaptureHeight() * coreObject.GetCaptureWidth());
              lamp.OverallAverageColor[1] /= (coreObject.GetCaptureHeight() * coreObject.GetCaptureWidth());
              lamp.OverallAverageColor[2] /= (coreObject.GetCaptureHeight() * coreObject.GetCaptureWidth());
              if (lamp.PixelCount > 0)
              {
                lamp.AverageColor[0] /= lamp.PixelCount;
                lamp.AverageColor[1] /= lamp.PixelCount;
                lamp.AverageColor[2] /= lamp.PixelCount;

                if (Math.Abs(lamp.AverageColor[0] - lamp.PreviousColor[0]) >= coreObject.atmoOrbThreshold || Math.Abs(lamp.AverageColor[1] - lamp.PreviousColor[1]) >= coreObject.atmoOrbThreshold || Math.Abs(lamp.AverageColor[2] - lamp.PreviousColor[2]) >= coreObject.atmoOrbThreshold)
                {
                  double hue;
                  double saturation;
                  double lightness;
                  double hueOverall;
                  double saturationOverall;
                  double lightnessOverall;

                  // Convert to hsl color model
                  RGB2HSL(lamp.AverageColor[0], lamp.AverageColor[1], lamp.AverageColor[2], out hue, out saturation, out lightness);
                  RGB2HSL(lamp.OverallAverageColor[0], lamp.OverallAverageColor[1], lamp.OverallAverageColor[2], out hueOverall, out saturationOverall, out lightnessOverall);

                  // Convert back to rgb with adjusted saturation and lightness
                  HSL2RGB(hue, Math.Min(saturation + coreObject.atmoOrbSaturation, 1), (coreObject.atmoOrbUseOverallLightness ? lightnessOverall : lightness), out lamp.AverageColor[0], out lamp.AverageColor[1], out lamp.AverageColor[2]);

                  // Save new color as previous color
                  lamp.PreviousColor = lamp.AverageColor;

                  // Adjust gamma level and send to lamp
                  ChangeColor((int)gammaCurve[lamp.AverageColor[0]], (int)gammaCurve[lamp.AverageColor[1]], (int)gammaCurve[lamp.AverageColor[2]]);
                }
              }
              else
              {
                if (Math.Abs(lamp.OverallAverageColor[0] - lamp.PreviousColor[0]) >= coreObject.atmoOrbThreshold || Math.Abs(lamp.OverallAverageColor[1] - lamp.PreviousColor[1]) >= coreObject.atmoOrbThreshold || Math.Abs(lamp.OverallAverageColor[2] - lamp.PreviousColor[2]) >= coreObject.atmoOrbThreshold)
                {
                  if (lamp.OverallAverageColor[0] <= coreObject.atmoOrbBlackThreshold && lamp.OverallAverageColor[1] <= coreObject.atmoOrbBlackThreshold && lamp.OverallAverageColor[2] <= coreObject.atmoOrbBlackThreshold)
                  {
                    ChangeColor(0, 0, 0);
                  }
                  else
                  {
                    // Adjust gamma level and send to lamp
                    ChangeColor((int)gammaCurve[lamp.OverallAverageColor[0]], (int)gammaCurve[lamp.OverallAverageColor[1]], (int)gammaCurve[lamp.OverallAverageColor[2]]);
                  }
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error("AtmoOrbHandler - Error in ChangeImage.");
        Log.Error("AtmoOrbHandler - Exception: {0}", ex.Message);
      }
    }

    public void ChangeProfile()
    {
      return;
    }

    public void PowerModeChanged(PowerModes powerMode)
    {
      if (powerMode == PowerModes.Resume)
      {
        Initialise();
      }
      else if (powerMode == PowerModes.Suspend)
      {
        ChangeEffect(ContentEffect.LEDsDisabled);
      }
    }
    #endregion


    private void InitThreaded(bool force = false)
    {
      if (initLock)
      {
        Log.Debug("AtmoOrbHandler - Initialising locked.");
        return;
      }
      initLock = true;
      try
      {
        // Read lamp map and create lamp instances
        if (lamps.Count == 0)
        {
          for (int i = 0; i < coreObject.atmoOrbLamps.Count; i++)
          {
            string[] settings = coreObject.atmoOrbLamps[i].Split(',');
            if (settings[1] == "UDP")
            {
              lamps.Add(new UDPLamp(settings[0], int.Parse(settings[2]), int.Parse(settings[3]), int.Parse(settings[4]), int.Parse(settings[5]), bool.Parse(settings[6])));
            }
            else if (settings[1] == "TCP")
            {
              lamps.Add(new TCPLamp(settings[0], settings[2], int.Parse(settings[3]), int.Parse(settings[4]), int.Parse(settings[5]), int.Parse(settings[6]), int.Parse(settings[7]), bool.Parse(settings[8])));
            }
          }
        }

        // Connect tcp lamps
        foreach (var lamp in lamps)
        {
          if (lamp.Type == LampType.TCP)
          {
            lamp.Connect(lamp.IP, lamp.Port);
          }
        }

        // Start udp server if udp lamps are being used
        if (UDPLampPresent())
        {
          if (udpServer == null)
          {
            udpServer = new UdpClient(coreObject.atmoOrbBroadcastPort);
            UDPServerListen();
          }

          udpClientBroadcast = new UdpClient();
          IPEndPoint udpClientBroadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, coreObject.atmoOrbBroadcastPort);
          byte[] bytes = Encoding.ASCII.GetBytes("M-SEARCH");
          udpClientBroadcast.Send(bytes, bytes.Length, udpClientBroadcastEndpoint);

          udpClientBroadcast.Close();
        }
        initLock = false;
      }
      catch (Exception ex)
      {
        Log.Error("AtmoOrbHandler - Error while initialising.");
        Log.Error("AtmoOrbHandler - Exception: {0}", ex.Message);
      }
    }

    private bool UDPLampPresent()
    {
      foreach (var lamp in lamps)
      {
        if (lamp.Type == LampType.UDP)
        {
          return true;
        }
      }
      return false;
    }

    private void UDPServerListen()
    {
      udpServer.BeginReceive(UDPServerReceive, new object());
    }
    private void UDPServerReceive(IAsyncResult ar)
    {
      try
      {
        IPEndPoint udpServerEndpoint = new IPEndPoint(IPAddress.Any, coreObject.atmoOrbBroadcastPort);
        byte[] bytes = udpServer.EndReceive(ar, ref udpServerEndpoint);
        string message = Encoding.ASCII.GetString(bytes);
        string[] splitMessage = message.Split(':', ',', ';');
        if (splitMessage.Length >= 4)
        {
          if (splitMessage[0] == "AtmoOrb")
          {
            if (splitMessage[2] == "address")
            {
              foreach (var lamp in lamps)
              {
                if (lamp.ID == splitMessage[1])
                {
                  lamp.Connect(splitMessage[3], int.Parse(splitMessage[4]));
                }
              }
            }
          }
        }
        UDPServerListen();
      }
      catch (Exception ex)
      {
        Log.Error("AtmoOrbHandler - Exception in UDPServerReceive.");
        Log.Error("AtmoOrbHandler - Exception: {0}", ex.Message);
      }
    }

    private void ChangeColor(int red, int green, int blue)
    {
      string redHex = red.ToString("X");
      string greenHex = green.ToString("X");
      string blueHex = blue.ToString("X");
      if (redHex.Length == 1)
      {
        redHex = "0" + redHex;
      }
      if (greenHex.Length == 1)
      {
        greenHex = "0" + greenHex;
      }
      if (blueHex.Length == 1)
      {
        blueHex = "0" + blueHex;
      }

      foreach (var lamp in lamps)
      {
        lamp.ChangeColor(redHex + greenHex + blueHex);
      }
    }

    private void CalcGammaCurve()
    {
      for (int i = 0; i < gammaCurve.Length; i++)
      {
        gammaCurve[i] = Math.Pow((double)i / ((double)gammaCurve.Length - 1.0f), coreObject.atmoOrbGamma) * (gammaCurve.Length - 1.0f);
      }
    }

    // RGB2HSL and HSL2RGB
    //
    // http://www.geekymonkey.com/Programming/CSharp/RGB2HSL_HSL2RGB.htm

    // Given H,S,L in range of 0-1
    // Returns a Color (RGB struct) in range of 0-255
    public void HSL2RGB(double h, double sl, double l, out int red, out int green, out int blue)
    {
      double v;
      double r, g, b;

      r = l;   // default to gray
      g = l;
      b = l;
      v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
      if (v > 0)
      {
        double m;
        double sv;
        int sextant;
        double fract, vsf, mid1, mid2;

        m = l + l - v;
        sv = (v - m) / v;
        h *= 6.0;
        sextant = (int)h;
        fract = h - sextant;
        vsf = v * sv * fract;
        mid1 = m + vsf;
        mid2 = v - vsf;
        switch (sextant)
        {
          case 0:
            r = v;
            g = mid1;
            b = m;
            break;
          case 1:
            r = mid2;
            g = v;
            b = m;
            break;
          case 2:
            r = m;
            g = v;
            b = mid1;
            break;
          case 3:
            r = m;
            g = mid2;
            b = v;
            break;
          case 4:
            r = mid1;
            g = m;
            b = v;
            break;
          case 5:
            r = v;
            g = m;
            b = mid2;
            break;
        }
      }
      red = Convert.ToByte(r * 255.0f);
      green = Convert.ToByte(g * 255.0f);
      blue = Convert.ToByte(b * 255.0f);
    }


    // Given a Color (RGB Struct) in range of 0-255
    // Return H,S,L in range of 0-1
    public void RGB2HSL(int red, int green, int blue, out double h, out double s, out double l)
    {
      double r = red / 255.0;
      double g = green / 255.0;
      double b = blue / 255.0;
      double v;
      double m;
      double vm;
      double r2, g2, b2;

      h = 0; // default to black
      s = 0;
      l = 0;
      v = Math.Max(r, g);
      v = Math.Max(v, b);
      m = Math.Min(r, g);
      m = Math.Min(m, b);
      l = (m + v) / 2.0;
      if (l <= 0.0)
      {
        return;
      }
      vm = v - m;
      s = vm;
      if (s > 0.0)
      {
        s /= (l <= 0.5) ? (v + m) : (2.0 - v - m);
      }
      else
      {
        return;
      }
      r2 = (v - r) / vm;
      g2 = (v - g) / vm;
      b2 = (v - b) / vm;
      if (r == v)
      {
        h = (g == m ? 5.0 + b2 : 1.0 - g2);
      }
      else if (g == v)
      {
        h = (b == m ? 1.0 + r2 : 3.0 - b2);
      }
      else
      {
        h = (r == m ? 3.0 + g2 : 5.0 - r2);
      }
      h /= 6.0;
    }
  }
}
