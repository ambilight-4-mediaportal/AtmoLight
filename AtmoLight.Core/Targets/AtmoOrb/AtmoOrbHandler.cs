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

namespace AtmoLight.Targets
{
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
    private UdpClient udpClient;
    private UdpClient udpClientBroadcast;

    IPEndPoint udpClientEndpoint;

    int broadcastPort = 30003;
    String atmoOrbIP;
    int atmoOrbPort;
    String atmoOrbID;
    bool isConnected;
    int[] prevColor = new int[3];
    int threshold = 0;
    int minDiversion = 10;
    double saturation = 0.5;
    #endregion

    #region Constructor
    public AtmoOrbHandler()
    {
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
      Disconnect();
    }

    public bool IsConnected()
    {
      return isConnected && !initLock;
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
          System.Threading.Thread.Sleep(100);
          ChangeColor(coreObject.staticColor[0], coreObject.staticColor[1], coreObject.staticColor[2]);
          return true;
        case ContentEffect.LEDsDisabled:
        case ContentEffect.Undefined:
        default:
          System.Threading.Thread.Sleep(100);
          ChangeColor(0, 0, 0);
          return true;
      }
    }

    public void ChangeImage(byte[] pixeldata, byte[] bmiInfoHeader)
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
        int[] rgb = new int[] { 0, 0, 0 };
        int pixel = 0;
        for (int y = 0; y < coreObject.GetCaptureHeight(); y++)
        {
          int row = coreObject.GetCaptureWidth() * y * 4;
          for (int x = 0; x < coreObject.GetCaptureWidth(); x++)
          {
            if (Math.Abs(pixeldata[row + x * 4 + 2] - pixeldata[row + x * 4 + 1]) > minDiversion || Math.Abs(pixeldata[row + x * 4 + 2] - pixeldata[row + x * 4]) > minDiversion || Math.Abs(pixeldata[row + x * 4 + 1] - pixeldata[row + x * 4]) > minDiversion)
            {
              rgb[0] += pixeldata[row + x * 4 + 2];
              rgb[1] += pixeldata[row + x * 4 + 1];
              rgb[2] += pixeldata[row + x * 4];
              pixel++;
            }
          }
        }
        rgb[0] = rgb[0] / pixel;
        rgb[1] = rgb[1] / pixel;
        rgb[2] = rgb[2] / pixel;

        if (Math.Abs(rgb[0] - prevColor[0]) >= threshold || Math.Abs(rgb[1] - prevColor[1]) >= threshold || Math.Abs(rgb[2] - prevColor[2]) >= threshold)
        {
          Color avgColor = Color.FromArgb(rgb[0], rgb[1], rgb[2]);
          HsvToRgb(avgColor.GetHue(), Math.Min(avgColor.GetSaturation() + saturation, 1), avgColor.GetBrightness(), out rgb[0], out rgb[1], out rgb[2]);
          prevColor = rgb;
          ChangeColor(rgb[0], rgb[1], rgb[2]);
        }
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
        Disconnect();
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
        if (udpServer == null)
        {
          udpServer = new UdpClient(broadcastPort);
          UDPServerListen();
        }

        udpClientBroadcast = new UdpClient();
        IPEndPoint udpClientBroadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, broadcastPort);
        byte[] bytes = Encoding.ASCII.GetBytes("M-SEARCH");

        while (!isConnected)
        {
          udpClientBroadcast.Send(bytes, bytes.Length, udpClientBroadcastEndpoint);
          System.Threading.Thread.Sleep(10000);
        }
        
        udpClientBroadcast.Close();
        initLock = false;
      }
      catch (Exception ex)
      {
        Log.Error("AtmoOrbHandler - Error while initialising.");
        Log.Error("AtmoOrbHandler - Exception: {0}", ex.Message);
      }
    }

    private void UDPServerListen()
    {
      udpServer.BeginReceive(UDPServerReceive, new object());
    }
    private void UDPServerReceive(IAsyncResult ar)
    {
      try
      {
        IPEndPoint udpServerEndpoint = new IPEndPoint(IPAddress.Any, broadcastPort);
        byte[] bytes = udpServer.EndReceive(ar, ref udpServerEndpoint);
        string message = Encoding.ASCII.GetString(bytes);
        string[] splitMessage = message.Split(':', ',', ';');
        if (splitMessage.Length >= 4)
        {
          if (splitMessage[0] == "AtmoOrb")
          {
            atmoOrbID = splitMessage[1];
            if (splitMessage[2] == "address")
            {
              atmoOrbIP = splitMessage[3];
              atmoOrbPort = int.Parse(splitMessage[4]);
              Connect();
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

    private void Connect()
    {
      try
      {
        udpClient = new UdpClient();
        udpClientEndpoint = new IPEndPoint(IPAddress.Parse(atmoOrbIP), atmoOrbPort);
        isConnected = true;
        Log.Debug("AtmoOrbHandler - Secussfully connected to {0}:{1}", atmoOrbIP, atmoOrbPort);
        ChangeEffect(coreObject.GetCurrentEffect());
      }
      catch (Exception ex)
      {
        Log.Error("AtmoOrbHandler - Exception in Connect.");
        Log.Error("AtmoOrbHandler - Exception: {0}", ex.Message);
      }
    }

    private void Disconnect()
    {
      try
      {
        udpClient.Close();
        isConnected = false;
      }
      catch (Exception ex)
      {
        Log.Error("AtmoOrbHandler - Exception in Disconnect.");
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

      byte[] bytes = Encoding.ASCII.GetBytes("setcolor:" + redHex + greenHex + blueHex + ";");
      udpClient.Send(bytes, bytes.Length, udpClientEndpoint);
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
