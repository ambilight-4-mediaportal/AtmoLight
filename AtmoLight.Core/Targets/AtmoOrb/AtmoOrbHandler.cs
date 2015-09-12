using System;
using System.Collections.Generic;
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

    private readonly Core _coreObject = Core.GetInstance();
    private volatile bool _initLock;
    private Thread _initThreadHelper;

    private UdpClient _udpServer;
    private UdpClient _udpClientBroadcast;

    private readonly double[] _gammaCurve = new double[256];
    private double _vuMeterHue;

    private List<ILamp> lamps = new List<ILamp>();

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
      if (!_initLock)
      {
        _initThreadHelper = new Thread(() => InitThreaded(force));
        _initThreadHelper.Name = "AtmoLight AtmoOrb Init";
        _initThreadHelper.IsBackground = true;
        _initThreadHelper.Start();
      }
    }

    public void ReInitialise(bool force)
    {
      if (_coreObject.reInitOnError || force)
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
          ChangeColor(0, 0, 0, true);
          return true;
        case ContentEffect.GIFReader:
        case ContentEffect.VUMeter:
        case ContentEffect.VUMeterRainbow:
          return true;
        case ContentEffect.StaticColor:
          // Send command 3 times to make sure it arrives
          ChangeColor((byte) _gammaCurve[_coreObject.staticColor[0]], (byte) _gammaCurve[_coreObject.staticColor[1]],
            (byte) _gammaCurve[_coreObject.staticColor[2]]);
          Thread.Sleep(50);
          ChangeColor((byte) _gammaCurve[_coreObject.staticColor[0]], (byte) _gammaCurve[_coreObject.staticColor[1]],
            (byte) _gammaCurve[_coreObject.staticColor[2]]);
          Thread.Sleep(50);
          ChangeColor((byte) _gammaCurve[_coreObject.staticColor[0]], (byte) _gammaCurve[_coreObject.staticColor[1]],
            (byte) _gammaCurve[_coreObject.staticColor[2]]);
          return true;
        case ContentEffect.LEDsDisabled:
          // Send command 3 times to make sure it arrives
          ChangeColor(0, 0, 0, true);
          Thread.Sleep(50);
          ChangeColor(0, 0, 0, true);
          Thread.Sleep(50);
          ChangeColor(0, 0, 0, true);

          return true;
        case ContentEffect.Undefined:
        default:
          // Send command 3 times to make sure it arrives
          ChangeColor(0, 0, 0, true);
          Thread.Sleep(50);
          ChangeColor(0, 0, 0, true);
          Thread.Sleep(50);
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
      if (_initLock)
      {
        Log.Debug("AtmoOrbHandler - Initialising locked.");
        return;
      }
      _initLock = true;
      try
      {
        // Read lamp map and create lamp instances
        if (lamps.Count == 0)
        {
          for (var i = 0; i < _coreObject.atmoOrbLamps.Count; i++)
          {
            var settings = _coreObject.atmoOrbLamps[i].Split(',');

            if (settings[1] == "UDP_IP")
            {
              lamps.Add(new UDPIPLamp(settings[0], settings[2], int.Parse(settings[3]), int.Parse(settings[4]),
                int.Parse(settings[5]), int.Parse(settings[6]), int.Parse(settings[7]), bool.Parse(settings[8])));
            }
            else if (settings[1] == "UDP_Broadcast")
            {
              lamps.Add(new UDPBroadcastLamp(settings[0], int.Parse(settings[4]),int.Parse(settings[5]),
                int.Parse(settings[6]), int.Parse(settings[7]), bool.Parse(settings[8])));
            }
            else if (settings[1] == "UDP_Multicast")
            {
              lamps.Add(new UDPMulticastLamp(settings[0], settings[2], int.Parse(settings[3]), int.Parse(settings[4]),
                int.Parse(settings[5]), int.Parse(settings[6]), int.Parse(settings[7]), bool.Parse(settings[8])));
            }
            else if (settings[1] == "TCP")
            {
              lamps.Add(new TCPLamp(settings[0], settings[2], int.Parse(settings[3]), int.Parse(settings[4]),
                int.Parse(settings[5]), int.Parse(settings[6]), int.Parse(settings[7]), bool.Parse(settings[8])));
            }
          }
        }

        // Connect tcp and/or join udp multicast groups
        foreach (var lamp in lamps)
        {
          if (lamp.Type == LampType.TCP)
          {
            lamp.Connect(lamp.IP, lamp.Port);
          }
          else if (lamp.Type == LampType.UDPMultiCast)
          {
            lamp.Connect(lamp.IP, lamp.Port);
          }
        }

        // Start udp server if udp lamps are being used
        if (UdpBroadcastLampPresent())
        {
          if (_udpServer == null)
          {
            _udpServer = new UdpClient(_coreObject.atmoOrbBroadcastPort);
            UdpBroadcastServerListen();
          }

          _udpClientBroadcast = new UdpClient();
          var udpClientBroadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, _coreObject.atmoOrbBroadcastPort);
          var bytes = Encoding.ASCII.GetBytes("M-SEARCH");
          _udpClientBroadcast.Send(bytes, bytes.Length, udpClientBroadcastEndpoint);

          _udpClientBroadcast.Close();
        }

        _initLock = false;
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
        if (_coreObject.GetCurrentEffect() == ContentEffect.VUMeter ||
            _coreObject.GetCurrentEffect() == ContentEffect.VUMeterRainbow)
        {
          for (var y = 0; y < _coreObject.GetCaptureHeight(); y++)
          {
            var row = _coreObject.GetCaptureWidth()*y*4;

            if ((pixeldata[row] != 0 || pixeldata[row + 1] != 0 || pixeldata[row + 2] != 0) ||
                (pixeldata[row + ((_coreObject.GetCaptureWidth() - 1)*4)] != 0 ||
                 pixeldata[row + ((_coreObject.GetCaptureWidth() - 1)*4) + 1] != 0 ||
                 pixeldata[row + ((_coreObject.GetCaptureWidth() - 1)*4) + 2] != 0))
            {
              int r, g, b;
              double s, l;
              _vuMeterHue += 1.0/1200.0;
              if (_vuMeterHue > 1)
              {
                _vuMeterHue -= 1;
              }
              s = 1;
              l = 0.5 - ((double) y/_coreObject.GetCaptureHeight()/2);
              HSL.HSL2RGB(_vuMeterHue, s, l, out r, out g, out b);
              ChangeColor((byte) _gammaCurve[r], (byte) _gammaCurve[g], (byte) _gammaCurve[b]);
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
          for (var y = 0; y < _coreObject.GetCaptureHeight(); y++)
          {
            var row = _coreObject.GetCaptureWidth()*y*4;
            for (var x = 0; x < _coreObject.GetCaptureWidth(); x++)
            {
              foreach (var lamp in lamps)
              {
                if (lamp.IsConnected())
                {
                  if ((lamp.ZoneInverted == false && x >= lamp.HScanStart*_coreObject.GetCaptureWidth()/100 &&
                       x <= lamp.HScanEnd*_coreObject.GetCaptureWidth()/100 &&
                       y >= lamp.VScanStart*_coreObject.GetCaptureHeight()/100 &&
                       y <= lamp.VScanEnd*_coreObject.GetCaptureHeight()/100)
                      ||
                      (lamp.ZoneInverted &&
                       (x <= lamp.HScanStart*_coreObject.GetCaptureWidth()/100 ||
                        x >= lamp.HScanEnd*_coreObject.GetCaptureWidth()/100) &&
                       (y <= lamp.VScanStart*_coreObject.GetCaptureHeight()/100 ||
                        y >= lamp.VScanEnd*_coreObject.GetCaptureHeight()/100)))
                  {
                    lamp.OverallAverageColor[0] += pixeldata[row + x*4 + 2];
                    lamp.OverallAverageColor[1] += pixeldata[row + x*4 + 1];
                    lamp.OverallAverageColor[2] += pixeldata[row + x*4];
                    if (Math.Abs(pixeldata[row + x*4 + 2] - pixeldata[row + x*4 + 1]) > _coreObject.atmoOrbMinDiversion ||
                        Math.Abs(pixeldata[row + x*4 + 2] - pixeldata[row + x*4]) > _coreObject.atmoOrbMinDiversion ||
                        Math.Abs(pixeldata[row + x*4 + 1] - pixeldata[row + x*4]) > _coreObject.atmoOrbMinDiversion)
                    {
                      lamp.AverageColor[0] += pixeldata[row + x*4 + 2];
                      lamp.AverageColor[1] += pixeldata[row + x*4 + 1];
                      lamp.AverageColor[2] += pixeldata[row + x*4];
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
              lamp.OverallAverageColor[0] /= (_coreObject.GetCaptureHeight()*_coreObject.GetCaptureWidth());
              lamp.OverallAverageColor[1] /= (_coreObject.GetCaptureHeight()*_coreObject.GetCaptureWidth());
              lamp.OverallAverageColor[2] /= (_coreObject.GetCaptureHeight()*_coreObject.GetCaptureWidth());
              if (lamp.PixelCount > 0)
              {
                lamp.AverageColor[0] /= lamp.PixelCount;
                lamp.AverageColor[1] /= lamp.PixelCount;
                lamp.AverageColor[2] /= lamp.PixelCount;

                if (Math.Abs(lamp.AverageColor[0] - lamp.PreviousColor[0]) >= _coreObject.atmoOrbThreshold ||
                    Math.Abs(lamp.AverageColor[1] - lamp.PreviousColor[1]) >= _coreObject.atmoOrbThreshold ||
                    Math.Abs(lamp.AverageColor[2] - lamp.PreviousColor[2]) >= _coreObject.atmoOrbThreshold)
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
                  HSL.HSL2RGB(hue, Math.Min(saturation + _coreObject.atmoOrbSaturation, 1),
                    (_coreObject.atmoOrbUseOverallLightness ? lightnessOverall : lightness), out lamp.AverageColor[0],
                    out lamp.AverageColor[1], out lamp.AverageColor[2]);

                  // Adjust gamma level and send to lamp
                  ChangeColor((byte) _gammaCurve[lamp.AverageColor[0]], (byte) _gammaCurve[lamp.AverageColor[1]],
                    (byte) _gammaCurve[lamp.AverageColor[2]]);
                }
              }
              else
              {
                if (Math.Abs(lamp.OverallAverageColor[0] - lamp.PreviousColor[0]) >= _coreObject.atmoOrbThreshold ||
                    Math.Abs(lamp.OverallAverageColor[1] - lamp.PreviousColor[1]) >= _coreObject.atmoOrbThreshold ||
                    Math.Abs(lamp.OverallAverageColor[2] - lamp.PreviousColor[2]) >= _coreObject.atmoOrbThreshold)
                {
                  // Save new color as previous color
                  lamp.PreviousColor = lamp.OverallAverageColor;

                  if (lamp.OverallAverageColor[0] <= _coreObject.atmoOrbBlackThreshold &&
                      lamp.OverallAverageColor[1] <= _coreObject.atmoOrbBlackThreshold &&
                      lamp.OverallAverageColor[2] <= _coreObject.atmoOrbBlackThreshold)
                  {
                    // Black threshold reached, forcing leds off as to clear smooth colors on the lamp side
                    ChangeColor(0, 0, 0, true);
                  }
                  else
                  {
                    // Adjust gamma level and send to lamp
                    ChangeColor((byte) _gammaCurve[lamp.OverallAverageColor[0]],
                      (byte) _gammaCurve[lamp.OverallAverageColor[1]], (byte) _gammaCurve[lamp.OverallAverageColor[2]]);
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

    private void ChangeColor(byte red, byte green, byte blue, bool forceLightsOff = false)
    {
      if (!IsConnected())
      {
        return;
      }

      foreach (var lamp in lamps)
      {
        lamp.ChangeColor(red, green, blue, forceLightsOff);
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
      _udpServer.BeginReceive(UdpBroadcastServerReceive, new object());
    }

    private void UdpBroadcastServerReceive(IAsyncResult ar)
    {
      try
      {
        var udpServerEndpoint = new IPEndPoint(IPAddress.Any, _coreObject.atmoOrbBroadcastPort);
        var bytes = _udpServer.EndReceive(ar, ref udpServerEndpoint);
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
      for (var i = 0; i < _gammaCurve.Length; i++)
      {
        _gammaCurve[i] = Math.Pow(i/((double) _gammaCurve.Length - 1.0f), _coreObject.atmoOrbGamma)*
                        (_gammaCurve.Length - 1.0f);
      }
    }

    #endregion
  }
}
