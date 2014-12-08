using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

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
    int ledCount;
    String ledArrangement;
    int[] prevColor = new int[3];
    int threshold = 1;
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
      if (coreObject.GetCurrentEffect() == ContentEffect.MediaPortalLiveMode)
      {
        int[] rgb = new int[] { 0, 0, 0 };
        int pixel = 0;
        for (int y = 0; y < coreObject.GetCaptureHeight(); y++)
        {
          int row = coreObject.GetCaptureWidth() * y * 4;
          for (int x = 0; x < coreObject.GetCaptureWidth(); x++)
          {
            rgb[0] += pixeldata[row + x * 4 + 2];
            rgb[1] += pixeldata[row + x * 4 + 1];
            rgb[2] += pixeldata[row + x * 4];
            pixel++;
          }
        }
        rgb[0] = rgb[0] / pixel;
        rgb[1] = rgb[1] / pixel;
        rgb[2] = rgb[2] / pixel;

        if (Math.Abs(rgb[0] - prevColor[0]) >= threshold || Math.Abs(rgb[1] - prevColor[1]) >= threshold || Math.Abs(rgb[2] - prevColor[2]) >= threshold)
        {
          prevColor = rgb;
          ChangeColor(rgb[0], rgb[1], rgb[2]);
        }
      }
      else if (coreObject.GetCurrentEffect() == ContentEffect.VUMeter || coreObject.GetCurrentEffect() == ContentEffect.VUMeterRainbow)
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
      else if (coreObject.GetCurrentEffect() == ContentEffect.GIFReader)
      {
        return;
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
            else if (splitMessage[2] == "ledcount")
            {
              ledCount = int.Parse(splitMessage[3]);
            }
            else if (splitMessage[2] == "arrangement")
            {
              ledArrangement = splitMessage[3];
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
  }
}
