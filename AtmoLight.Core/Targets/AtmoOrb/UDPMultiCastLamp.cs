﻿using System;
using System.Net;
using System.Net.Sockets;

namespace AtmoLight.Targets
{
  internal class UDPMulticastLamp : ILamp
  {
    private bool _isConnected;
    private Socket _socket;
    private IPEndPoint _ipClientEndpoint;
    private readonly Core _coreObject = Core.GetInstance();

    public UDPMulticastLamp(string id, string ip, int port, int hScanStart, int hScanEnd, int vScanStart, int vScanEnd,
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
      get { return LampType.UDPMultiCast; }
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

        var multiCastIp = IPAddress.Parse(ip);

        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _ipClientEndpoint = new IPEndPoint(multiCastIp, port);
        _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
          new MulticastOption(multiCastIp));
        _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
        _socket.Connect(_ipClientEndpoint);

        _isConnected = true;
        Log.Debug("AtmoOrbHandler - Successfully joined UDP multicast group {0} ({1}:{2})", ID, ip, port);

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
        Log.Error("AtmoOrbHandler - Exception while connecting to UDP multicast group {0} ({1}:{2})", ID, ip, port);
        Log.Error("AtmoOrbHandler - Exception: {0}", ex.Message);
      }
    }

    public void Disconnect()
    {
      try
      {
        if (_socket != null)
        {
          _socket.Close();
          _socket = null;
          _ipClientEndpoint = null;
        }
        _isConnected = false;
      }
      catch (Exception ex)
      {
        Log.Error("AtmoOrbHandler - Exception while disconnecting from UDP multicast group {0} ({1}:{2})", ID, IP, Port);
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

        _socket.Send(bytes, bytes.Length, SocketFlags.None);
      }
      catch (Exception e)
      {
        Log.Error("Error during send message..");
      }
    }
  }
}
