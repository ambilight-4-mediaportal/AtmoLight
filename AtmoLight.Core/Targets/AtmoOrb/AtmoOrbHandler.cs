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
      get {
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
                  // Convert to HSV colors
                  Color avgColor = Color.FromArgb(lamp.AverageColor[0], lamp.AverageColor[1], lamp.AverageColor[2]);
                  Color avgColorOverall = Color.FromArgb(lamp.OverallAverageColor[0], lamp.OverallAverageColor[1], lamp.OverallAverageColor[2]);

                  // Adjust saturation and use brightness from OverallAverageColor
                  HsvToRgb(avgColor.GetHue(), Math.Min(avgColor.GetSaturation() + coreObject.atmoOrbSaturation, 1), avgColorOverall.GetBrightness(), out lamp.AverageColor[0], out lamp.AverageColor[1], out lamp.AverageColor[2]);

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


    // http://www.splinter.com.au/converting-hsv-to-rgb-colour-using-c/

    /// <summary>
    /// Convert HSV to RGB
    /// h is from 0-360
    /// s,v values are 0-1
    /// r,g,b values are 0-255
    /// Based upon http://ilab.usc.edu/wiki/index.php/HSV_And_H2SV_Color_Space#HSV_Transformation_C_.2F_C.2B.2B_Code_2
    /// </summary>
    void HsvToRgb(double h, double S, double V, out int r, out int g, out int b)
    {
      // ######################################################################
      // T. Nathan Mundhenk
      // mundhenk@usc.edu
      // C/C++ Macro HSV to RGB

      double H = h;
      while (H < 0) { H += 360; };
      while (H >= 360) { H -= 360; };
      double R, G, B;
      if (V <= 0)
      { R = G = B = 0; }
      else if (S <= 0)
      {
        R = G = B = V;
      }
      else
      {
        double hf = H / 60.0;
        int i = (int)Math.Floor(hf);
        double f = hf - i;
        double pv = V * (1 - S);
        double qv = V * (1 - S * f);
        double tv = V * (1 - S * (1 - f));
        switch (i)
        {

          // Red is the dominant color

          case 0:
            R = V;
            G = tv;
            B = pv;
            break;

          // Green is the dominant color

          case 1:
            R = qv;
            G = V;
            B = pv;
            break;
          case 2:
            R = pv;
            G = V;
            B = tv;
            break;

          // Blue is the dominant color

          case 3:
            R = pv;
            G = qv;
            B = V;
            break;
          case 4:
            R = tv;
            G = pv;
            B = V;
            break;

          // Red is the dominant color

          case 5:
            R = V;
            G = pv;
            B = qv;
            break;

          // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

          case 6:
            R = V;
            G = tv;
            B = pv;
            break;
          case -1:
            R = V;
            G = pv;
            B = qv;
            break;

          // The color is not defined, we should throw an error.

          default:
            //LFATAL("i Value error in Pixel conversion, Value is %d", i);
            R = G = B = V; // Just pretend its black/white
            break;
        }
      }
      r = Clamp((int)(R * 255.0));
      g = Clamp((int)(G * 255.0));
      b = Clamp((int)(B * 255.0));
    }

    /// <summary>
    /// Clamp a value to 0-255
    /// </summary>
    int Clamp(int i)
    {
      if (i < 0) return 0;
      if (i > 255) return 255;
      return i;
    }
  }
}
