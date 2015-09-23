using System;
using System.Net;
using System.Net.Sockets;

namespace AtmoLight.Targets
{
  internal class UDPMulticastLamp : ILamp
  {
    private bool isConnected;
    private Socket socket;
    private IPEndPoint clientEndpoint;
    private readonly Core coreObject = Core.GetInstance();

    public UDPMulticastLamp(string id, string ip, int port, int hScanStart, int hScanEnd, int vScanStart, int vScanEnd,
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

    public int LedCount { get; private set; }

    public void Connect(string ip, int port)
    {
      IP = ip;
      Port = port;
      try
      {
        Disconnect();

        var multiCastIp = IPAddress.Parse(ip);

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        clientEndpoint = new IPEndPoint(multiCastIp, port);
        socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
          new MulticastOption(multiCastIp));
        socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
        socket.Connect(clientEndpoint);

        isConnected = true;
        Log.Debug("AtmoOrbHandler - [UDP Multicast] Successfully joined UDP multicast group {0} ({1}:{2})", ID, ip, port);

        if (coreObject.GetCurrentEffect() == ContentEffect.LEDsDisabled ||
            coreObject.GetCurrentEffect() == ContentEffect.Undefined)
        {
          ChangeColor(0, 0, 0, true, ID);
        }
        else if (coreObject.GetCurrentEffect() == ContentEffect.StaticColor)
        {
          var red = byte.Parse(coreObject.staticColor[0].ToString());
          var green = byte.Parse(coreObject.staticColor[1].ToString());
          var blue = byte.Parse(coreObject.staticColor[2].ToString());

          ChangeColor(red, green, blue, false, ID);
        }
      }
      catch (Exception ex)
      {
        Log.Error("AtmoOrbHandler - [UDP Multicast] Exception while connecting to UDP multicast group {0} ({1}:{2})", ID, ip, port);
        Log.Error("AtmoOrbHandler - [UDP Multicast] Exception: {0}", ex.Message);
      }
    }

    public void Disconnect()
    {
      try
      {
        if (socket != null)
        {
          socket.Close();
          socket = null;
          clientEndpoint = null;
        }
        isConnected = false;
      }
      catch (Exception ex)
      {
        Log.Error("AtmoOrbHandler - [UDP Multicast] Exception while disconnecting from UDP multicast group {0} ({1}:{2})", ID, IP, Port);
        Log.Error("AtmoOrbHandler - [UDP Multicast] Exception: {0}", ex.Message);
      }
    }

    public bool IsConnected()
    {
      return isConnected;
    }

    public void ChangeColor(byte red, byte green, byte blue, bool forceLightsOff, string orbId)
    {
      if (!IsConnected())
      {
        return;
      }

      try
      {
        byte commandCount = 24;
        byte[] bytes = new byte[5 + LedCount * 3];

        // Command identifier: C0FFEE
        bytes[0] = 0xC0;
        bytes[1] = 0xFF;
        bytes[2] = 0xEE;

        // Options parameter: 1 = force off | 2 = validate command by Orb ID
        if (forceLightsOff)
        {
          bytes[3] = 1;
        }
        else
        {
          // Always validate by Orb ID
          bytes[3] = 2;
        }

        // Orb ID
        bytes[4] = byte.Parse(orbId);

        // RED / GREEN / BLUE
        bytes[5] = red;
        bytes[6] = green;
        bytes[7] = blue;

        socket.Send(bytes, bytes.Length, SocketFlags.None);
      }
      catch (Exception e)
      {
        Log.Error("Error during send message..");
      }
    }
  }
}
