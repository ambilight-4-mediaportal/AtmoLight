using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace AtmoLight.Targets
{
  public enum LampType
  {
    TCP,
    UDPIP,
    UDPBroadcast,
    UDPMultiCast
  }

  internal class AtmoOrbHandler : ITargets
  {
    #region Fields

    public Target Name
    {
      get { return Target.AtmoOrb; }
    }

    public bool AllowDelay
    {
      get { return true; }
    }

    public List<ContentEffect> SupportedEffects
    {
      get
      {
        return new List<ContentEffect>
        {
          ContentEffect.GIFReader,
          ContentEffect.LEDsDisabled,
          ContentEffect.MediaPortalLiveMode,
          ContentEffect.StaticColor,
          ContentEffect.VUMeter,
          ContentEffect.VUMeterRainbow
        };
      }
    }

    private readonly Core coreObject = Core.GetInstance();
    private volatile bool initLock;
    private Thread initThreadHelper;

    private UdpClient udpBroadcastServer;
    private UdpClient udpBroadcastClient;

    private readonly double[] gammaCurve = new double[256];
    private double vuMeterHue;

    private List<ILamp> lamps = new List<ILamp>();
    private Dictionary<String, Socket> multicastGroups = new Dictionary<String, Socket>();
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
      Disconnect();
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
          ChangeColor((byte)gammaCurve[coreObject.staticColor[0]], (byte)gammaCurve[coreObject.staticColor[1]], (byte)gammaCurve[coreObject.staticColor[2]]);
          return true;
        case ContentEffect.Undefined:
        case ContentEffect.LEDsDisabled:
        default:
          ChangeColor(0, 0, 0, true);
          return true;
      }
    }

    public void ChangeProfile()
    {
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
        Disconnect();
      }
    }
    #endregion

    #region Init and Disconnect
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
          for (var i = 0; i < coreObject.atmoOrbLamps.Count; i++)
          {
            var settings = coreObject.atmoOrbLamps[i].Split(',');
            if (settings[1] == "UDP_IP")
            {
              lamps.Add(new UDPIPLamp(settings[0], settings[2], int.Parse(settings[3]), int.Parse(settings[4]),
                int.Parse(settings[5]), int.Parse(settings[6]), int.Parse(settings[7]), bool.Parse(settings[8]), int.Parse(settings[9])));
            }
            else if (settings[1] == "UDP_Broadcast")
            {
              lamps.Add(new UDPBroadcastLamp(settings[0], int.Parse(settings[2]), int.Parse(settings[3]),
                int.Parse(settings[4]), int.Parse(settings[5]), bool.Parse(settings[6])));
            }
            else if (settings[1] == "UDP_Multicast")
            {
              string multiCastIp = settings[2];

              // Join new or excisting multicast group
              if (!multicastGroups.ContainsKey(multiCastIp))
              {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Parse(multiCastIp), 49692);
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                  new MulticastOption(IPAddress.Parse(multiCastIp)));
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
                socket.Connect(clientEndpoint);

                multicastGroups.Add(multiCastIp, socket);

                lamps.Add(new UDPMulticastLamp(settings[0], settings[2], int.Parse(settings[3]), int.Parse(settings[4]),
                  int.Parse(settings[5]), int.Parse(settings[6]), int.Parse(settings[7]), bool.Parse(settings[8]),
                  int.Parse(settings[9]), socket));
              }
              else
              {
                Socket multicastSocket = multicastGroups[multiCastIp];

                lamps.Add(new UDPMulticastLamp(settings[0], settings[2], int.Parse(settings[3]),
                  int.Parse(settings[4]),
                  int.Parse(settings[5]), int.Parse(settings[6]), int.Parse(settings[7]), bool.Parse(settings[8]),
                  int.Parse(settings[9]), multicastSocket));
              }
            }
            else if (settings[1] == "TCP")
            {
              lamps.Add(new TCPLamp(settings[0], settings[2], int.Parse(settings[3]), int.Parse(settings[4]),
                int.Parse(settings[5]), int.Parse(settings[6]), int.Parse(settings[7]), bool.Parse(settings[8]), int.Parse(settings[9])));
            }
          }
        }

        // Connect tcp and/or join udp multicast groups
        foreach (var lamp in lamps)
        {
          if (lamp.Type == LampType.TCP || lamp.Type == LampType.UDPIP || lamp.Type == LampType.UDPMultiCast)
          {
            lamp.Connect(lamp.IP, lamp.Port);
          }
        }

        // Start udp server if udp lamps are being used
        if (UdpBroadcastLampPresent())
        {
          if (udpBroadcastServer == null)
          {
            udpBroadcastServer = new UdpClient(coreObject.atmoOrbBroadcastPort);
            UdpBroadcastServerListen();
          }

          udpBroadcastClient = new UdpClient();
          var udpClientBroadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, coreObject.atmoOrbBroadcastPort);
          var bytes = Encoding.ASCII.GetBytes("M-SEARCH");
          udpBroadcastClient.Send(bytes, bytes.Length, udpClientBroadcastEndpoint);

          udpBroadcastClient.Close();
        }

        initLock = false;
      }
      catch (Exception ex)
      {
        Log.Error("AtmoOrbHandler - Error while initialising.");
        Log.Error("AtmoOrbHandler - Exception: {0}", ex.Message);
      }
    }

    private void Disconnect()
    {
      foreach (var lamp in lamps)
      {
        lamp.Disconnect();
      }
    }

    #endregion

    #region ChangeImage/Color
    public void ChangeImage(byte[] pixeldata, byte[] bmiInfoHeader)
    {
      if (!IsConnected())
      {
        return;
      }

      try
      {
        if (coreObject.GetCurrentEffect() == ContentEffect.VUMeter ||
            coreObject.GetCurrentEffect() == ContentEffect.VUMeterRainbow)
        {
          for (var y = 0; y < coreObject.GetCaptureHeight(); y++)
          {
            var row = coreObject.GetCaptureWidth() * y * 4;

            if ((pixeldata[row] != 0 || pixeldata[row + 1] != 0 || pixeldata[row + 2] != 0) ||
                (pixeldata[row + ((coreObject.GetCaptureWidth() - 1) * 4)] != 0 ||
                 pixeldata[row + ((coreObject.GetCaptureWidth() - 1) * 4) + 1] != 0 ||
                 pixeldata[row + ((coreObject.GetCaptureWidth() - 1) * 4) + 2] != 0))
            {
              int r, g, b;
              double s, l;
              vuMeterHue += 1.0 / 1200.0;
              if (vuMeterHue > 1)
              {
                vuMeterHue -= 1;
              }
              s = 1;
              l = 0.5 - ((double)y / coreObject.GetCaptureHeight() / 2);
              HSL.HSL2RGB(vuMeterHue, s, l, out r, out g, out b);
              ChangeColor((byte)gammaCurve[r], (byte)gammaCurve[g], (byte)gammaCurve[b]);
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
          for (var y = 0; y < coreObject.GetCaptureHeight(); y++)
          {
            var row = coreObject.GetCaptureWidth() * y * 4;
            for (var x = 0; x < coreObject.GetCaptureWidth(); x++)
            {
              foreach (var lamp in lamps.Where(lamp => lamp.IsConnected()))
              {
                if ((lamp.ZoneInverted == false && x >= lamp.HScanStart * coreObject.GetCaptureWidth() / 100 &&
                     x <= lamp.HScanEnd * coreObject.GetCaptureWidth() / 100 &&
                     y >= lamp.VScanStart * coreObject.GetCaptureHeight() / 100 &&
                     y <= lamp.VScanEnd * coreObject.GetCaptureHeight() / 100)
                    ||
                    (lamp.ZoneInverted &&
                     (x <= lamp.HScanStart * coreObject.GetCaptureWidth() / 100 ||
                      x >= lamp.HScanEnd * coreObject.GetCaptureWidth() / 100) &&
                     (y <= lamp.VScanStart * coreObject.GetCaptureHeight() / 100 ||
                      y >= lamp.VScanEnd * coreObject.GetCaptureHeight() / 100)))
                {
                  if (Math.Abs(pixeldata[row + x * 4 + 2] - pixeldata[row + x * 4 + 1]) > coreObject.atmoOrbMinDiversion ||
                      Math.Abs(pixeldata[row + x * 4 + 2] - pixeldata[row + x * 4]) > coreObject.atmoOrbMinDiversion ||
                      Math.Abs(pixeldata[row + x * 4 + 1] - pixeldata[row + x * 4]) > coreObject.atmoOrbMinDiversion)
                  {
                    lamp.AverageColor[0] += pixeldata[row + x * 4 + 2];
                    lamp.AverageColor[1] += pixeldata[row + x * 4 + 1];
                    lamp.AverageColor[2] += pixeldata[row + x * 4];
                    lamp.PixelCount++;
                  }
                }

                lamp.OverallAverageColor[0] += pixeldata[row + x * 4 + 2];
                lamp.OverallAverageColor[1] += pixeldata[row + x * 4 + 1];
                lamp.OverallAverageColor[2] += pixeldata[row + x * 4];
              }
            }
          }
          foreach (var lamp in lamps.Where(lamp => lamp.IsConnected()))
          {
            Boolean skipSmoothing = false;
            lamp.OverallAverageColor[0] /= (coreObject.GetCaptureHeight() * coreObject.GetCaptureWidth());
            lamp.OverallAverageColor[1] /= (coreObject.GetCaptureHeight() * coreObject.GetCaptureWidth());
            lamp.OverallAverageColor[2] /= (coreObject.GetCaptureHeight() * coreObject.GetCaptureWidth());

            if (lamp.PixelCount > 0)
            {
              lamp.AverageColor[0] /= lamp.PixelCount;
              lamp.AverageColor[1] /= lamp.PixelCount;
              lamp.AverageColor[2] /= lamp.PixelCount;

              if (Math.Abs(lamp.AverageColor[0] - lamp.PreviousColor[0]) >= coreObject.atmoOrbThreshold ||
                  Math.Abs(lamp.AverageColor[1] - lamp.PreviousColor[1]) >= coreObject.atmoOrbThreshold ||
                  Math.Abs(lamp.AverageColor[2] - lamp.PreviousColor[2]) >= coreObject.atmoOrbThreshold)
              {
                double hue;
                double saturation;
                double lightness;
                double hueOverall;
                double saturationOverall;
                double lightnessOverall;

                // Save new color as previous color
                lamp.PreviousColor = lamp.AverageColor;

                // Convert to hsl color model
                HSL.RGB2HSL(lamp.AverageColor[0], lamp.AverageColor[1], lamp.AverageColor[2], out hue, out saturation,
                  out lightness);
                HSL.RGB2HSL(lamp.OverallAverageColor[0], lamp.OverallAverageColor[1], lamp.OverallAverageColor[2],
                  out hueOverall, out saturationOverall, out lightnessOverall);

                // Convert back to rgb with adjusted saturation and lightness
                HSL.HSL2RGB(hue, Math.Min(saturation + coreObject.atmoOrbSaturation, 1),
                  (coreObject.atmoOrbUseOverallLightness ? lightnessOverall : lightness), out lamp.AverageColor[0],
                  out lamp.AverageColor[1], out lamp.AverageColor[2]);

                if (Math.Abs(gammaCurve[lamp.PreviousColor[0]] - gammaCurve[lamp.AverageColor[0]]) >= coreObject.atmoOrbSmoothThreshold || Math.Abs(gammaCurve[lamp.PreviousColor[1]] - gammaCurve[lamp.AverageColor[1]]) >= coreObject.atmoOrbSmoothThreshold || Math.Abs(gammaCurve[lamp.PreviousColor[2]] - gammaCurve[lamp.AverageColor[2]]) >= coreObject.atmoOrbSmoothThreshold)
                {
                  skipSmoothing = true;
                }

                // Adjust gamma level and send to lamp
                lamp.ChangeColor((byte)gammaCurve[lamp.AverageColor[0]], (byte)gammaCurve[lamp.AverageColor[1]], (byte)gammaCurve[lamp.AverageColor[2]], skipSmoothing, lamp.ID);
              }
            }
            else
            {
              if (Math.Abs(lamp.OverallAverageColor[0] - lamp.PreviousColor[0]) >= coreObject.atmoOrbThreshold ||
                  Math.Abs(lamp.OverallAverageColor[1] - lamp.PreviousColor[1]) >= coreObject.atmoOrbThreshold ||
                  Math.Abs(lamp.OverallAverageColor[2] - lamp.PreviousColor[2]) >= coreObject.atmoOrbThreshold)
              {
                // Save new color as previous color
                lamp.PreviousColor = lamp.OverallAverageColor;

                if (lamp.OverallAverageColor[0] <= coreObject.atmoOrbBlackThreshold &&
                    lamp.OverallAverageColor[1] <= coreObject.atmoOrbBlackThreshold &&
                    lamp.OverallAverageColor[2] <= coreObject.atmoOrbBlackThreshold)
                {
                  if (gammaCurve[lamp.PreviousColor[0]] >= coreObject.atmoOrbSmoothThreshold || gammaCurve[lamp.PreviousColor[1]] >= coreObject.atmoOrbSmoothThreshold || gammaCurve[lamp.PreviousColor[2]] >= coreObject.atmoOrbSmoothThreshold)
                  {
                    skipSmoothing = true;
                  }

                  lamp.ChangeColor(0, 0, 0, skipSmoothing, lamp.ID);
                }
                else
                {
                  if (Math.Abs(gammaCurve[lamp.PreviousColor[0]] - gammaCurve[lamp.OverallAverageColor[0]]) >= coreObject.atmoOrbSmoothThreshold || Math.Abs(gammaCurve[lamp.PreviousColor[1]] - gammaCurve[lamp.OverallAverageColor[1]]) >= coreObject.atmoOrbSmoothThreshold || Math.Abs(gammaCurve[lamp.PreviousColor[2]] - gammaCurve[lamp.OverallAverageColor[2]]) >= coreObject.atmoOrbSmoothThreshold)
                  {
                    skipSmoothing = true;
                  }

                  // Adjust gamma level and send to lamp
                  lamp.ChangeColor((byte)gammaCurve[lamp.OverallAverageColor[0]], (byte)gammaCurve[lamp.OverallAverageColor[1]], (byte)gammaCurve[lamp.OverallAverageColor[2]], skipSmoothing, lamp.ID);
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

    private void ChangeColor(byte red, byte green, byte blue, bool skipSmoothing = false)
    {
      if (!IsConnected())
      {
        return;
      }

      foreach (var lamp in lamps)
      {
        lamp.ChangeColor((byte)red, (byte)green, (byte)blue, skipSmoothing, lamp.ID);
      }
    }
    #endregion

    #region UDP Server
    private bool UdpBroadcastLampPresent()
    {
      foreach (var lamp in lamps)
      {
        if (lamp.Type == LampType.UDPBroadcast)
        {
          return true;
        }
      }
      return false;
    }

    private void UdpBroadcastServerListen()
    {
      udpBroadcastServer.BeginReceive(UdpBroadcastServerReceive, new object());
    }

    private void UdpBroadcastServerReceive(IAsyncResult ar)
    {
      try
      {
        var udpServerEndpoint = new IPEndPoint(IPAddress.Any, coreObject.atmoOrbBroadcastPort);
        var bytes = udpBroadcastServer.EndReceive(ar, ref udpServerEndpoint);
        var message = Encoding.ASCII.GetString(bytes);

        var splitMessage = message.Split(':', ',', ';');
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
        UdpBroadcastServerListen();
      }
      catch (Exception ex)
      {
        Log.Error("AtmoOrbHandler - Exception in UDPServerReceive.");
        Log.Error("AtmoOrbHandler - Exception: {0}", ex.Message);
      }
    }
    #endregion

    #region Utilities
    private void CalcGammaCurve()
    {
      for (var i = 0; i < gammaCurve.Length; i++)
      {
        gammaCurve[i] = Math.Pow(i / ((double)gammaCurve.Length - 1.0f), coreObject.atmoOrbGamma) *
                        (gammaCurve.Length - 1.0f);
      }
    }
    #endregion
  }
}