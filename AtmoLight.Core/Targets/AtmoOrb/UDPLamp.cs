using System;
using System.Net;
using System.Net.Sockets;

namespace AtmoLight.Targets
{
  internal class UDPLamp : ILamp
  {
    private bool _isConnected;
    private UdpClient _client;
    private IPEndPoint clientEndpoint;
    private readonly Core _coreObject = Core.GetInstance();

    public UDPLamp(string id, string ip, int port, int hScanStart, int hScanEnd, int vScanStart, int vScanEnd,
      bool zoneInverted)
    {
      ID = id;
      IP = ip;
      Port = port;
      HScanStart = hScanStart;
      HScanEnd = hScanEnd;
      VScanStart = vScanStart;
      VScanEnd = vScanEnd;
      ZoneInverted = zoneInverted;
    }

    public string ID { get; private set; }

    public LampType Type
    {
      get { return LampType.UDP; }
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

    public void Connect(string ip, int port)
    {
      IP = ip;
      Port = port;
      try
      {
        Disconnect();
        _client = new UdpClient();
        clientEndpoint = new IPEndPoint(IPAddress.Parse(ip), port);
        _isConnected = true;
        Log.Debug("AtmoOrbHandler - Successfully connected to lamp {0} ({1}:{2})", ID, ip, port);

        if (_coreObject.GetCurrentEffect() == ContentEffect.LEDsDisabled ||
            _coreObject.GetCurrentEffect() == ContentEffect.Undefined)
        {
          ChangeColor(0, 0, 0, true);
        }
        else if (_coreObject.GetCurrentEffect() == ContentEffect.StaticColor)
        {
          var red = byte.Parse(_coreObject.staticColor[0].ToString());
          var green = byte.Parse(_coreObject.staticColor[1].ToString());
          var blue = byte.Parse(_coreObject.staticColor[2].ToString());

          ChangeColor(red, green, blue, false);
        }
      }
      catch (Exception ex)
      {
        Log.Error("AtmoOrbHandler - Exception while connecting to lamp {0} ({1}:{2})", ID, ip, port);
        Log.Error("AtmoOrbHandler - Exception: {0}", ex.Message);
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
        Log.Error("AtmoOrbHandler - Exception while disconnecting from lamp {0} ({1}:{2})", ID, IP, Port);
        Log.Error("AtmoOrbHandler - Exception: {0}", ex.Message);
      }
    }

    public bool IsConnected()
    {
      return _isConnected;
    }

    public void ChangeColor(byte red, byte green, byte blue, bool forceLightsOff)
    {
      if (!IsConnected())
      {
        return;
      }

      try
      {
        byte commandCount = 24;
        byte[] bytes = new byte[3 + commandCount * 3];

        // Command identifier: C0FFEE
        bytes[0] = 0xC0;
        bytes[1] = 0xFF;
        bytes[2] = 0xEE;

        // Options parameter, force leds off = 1
        if (forceLightsOff)
        {
          bytes[3] = 1;
        }
        else
        {
          bytes[3] = 0;
        }

        // RED / GREEN / BLUE
        bytes[4] = red;
        bytes[5] = green;
        bytes[6] = blue;

        _client.Send(bytes, bytes.Length, clientEndpoint);
      }
      catch (Exception e)
      {
        Log.Error("Error during send message..");
      }
    }
  }
}
