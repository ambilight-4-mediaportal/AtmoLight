using System;
using System.Net.Sockets;
using System.Net;

namespace AtmoLight.Targets
{
  internal class UDPIPLamp : ILamp
  {
    private bool _isConnected;
    private UdpClient _client;
    private IPEndPoint _clientEndpoint;
    private readonly Core coreObject = Core.GetInstance();

    public UDPIPLamp(string id, string ip, int port, int hScanStart, int hScanEnd, int vScanStart, int vScanEnd,
      bool zoneInverted, int ledCount)
    {
      ID = id;
      IP = ip;
      Port = port;
      HScanStart = hScanStart;
      HScanEnd = hScanEnd;
      VScanStart = vScanStart;
      VScanEnd = vScanEnd;
      ZoneInverted = zoneInverted;
      LedCount = ledCount;
    }
    public string ID { get; private set; }

    public LampType Type
    {
      get { return LampType.UDPIP; }
    }

    public int[] OverallAverageColor { get; set; }

    public int[] AverageColor { get; set; }

    public int[] PreviousColor { get; set; }

    public int PixelCount { get; set; }

    public int HScanStart { get; private set; }

    public int HScanEnd { get; private set; }

    public int VScanStart { get; private set; }

    public int VScanEnd { get; private set; }

    public bool ZoneInverted { get; private set; }

    public string IP { get; private set; }

    public int Port { get; private set; }

    public int LedCount { get; private set; }

    public void Connect(string ip, int port)
    {
      IP = ip;
      Port = port;

      try
      {
        Disconnect();
        _client = new UdpClient();
        _clientEndpoint = new IPEndPoint(IPAddress.Parse(ip), port);
        _isConnected = true;
        Log.Debug("AtmoOrbHandler - [UDP IP] Successfully connected to lamp {0} ({1}:{2})", ID, ip, port);

        if (coreObject.GetCurrentEffect() == ContentEffect.LEDsDisabled ||
            coreObject.GetCurrentEffect() == ContentEffect.Undefined)
        {
          ChangeColor(0, 0, 0, true, true, ID);
        }
        else if (coreObject.GetCurrentEffect() == ContentEffect.StaticColor)
        {

          byte red = byte.Parse(coreObject.staticColor[0].ToString());
          byte green = byte.Parse(coreObject.staticColor[1].ToString());
          byte blue = byte.Parse(coreObject.staticColor[2].ToString());

          ChangeColor(red, green, blue, false, true, ID);
        }
      }
      catch (Exception ex)
      {
        Log.Error("AtmoOrbHandler - [UDP IP] Exception while connecting to lamp {0} ({1}:{2})", ID, ip, port);
        Log.Error("AtmoOrbHandler - [UDP IP] Exception: {0}", ex.Message);
      }
    }

    public void Disconnect()
    {
      try
      {
        if (_client != null)
        {
          _client.Close();
          _client = null;
        }
        _isConnected = false;
      }
      catch (Exception ex)
      {
        Log.Error("AtmoOrbHandler - [UDP IP] Exception while disconnecting from lamp {0} ({1}:{2})", ID, IP, Port);
        Log.Error("AtmoOrbHandler - [UDP IP] Exception: {0}", ex.Message);
      }
    }

    public bool IsConnected()
    {
      return _isConnected;
    }

    public void ChangeColor(byte red, byte green, byte blue, bool forceLightsOff, bool useLampSmoothing, string orbId)
    {
      if (!IsConnected())
      {
        return;
      }

      try
      {
        byte[] bytes = new byte[5 + LedCount * 3];

        // Command identifier: C0FFEE
        bytes[0] = 0xC0;
        bytes[1] = 0xFF;
        bytes[2] = 0xEE;

        // Options parameter: 1 = force off | 2 = validate command by Orb ID | 4 = use lamp smoothing and validate by Orb ID
        if (forceLightsOff)
        {
          bytes[3] = 1;
        }
        else
        {
          // Always validate by Orb ID
          if (useLampSmoothing)
          {
            bytes[3] = 4;
          }
          else
          {
            bytes[3] = 2;
          }
        }

        // Orb ID
        bytes[4] = byte.Parse(orbId);

        // RED / GREEN / BLUE
        bytes[5] = red;
        bytes[6] = green;
        bytes[7] = blue;

        _client.Send(bytes, bytes.Length, _clientEndpoint);
      }
      catch (Exception e)
      {
        Log.Error("Error during send message..");
      }
    }
  }
}
