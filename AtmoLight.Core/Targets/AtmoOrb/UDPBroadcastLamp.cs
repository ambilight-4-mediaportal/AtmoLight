using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AtmoLight.Targets
{
  internal class UDPBroadcastLamp : ILamp
  {
    private bool isConnected;
    private UdpClient udpClient;
    private IPEndPoint udpClientEndpoint;
    private readonly Core coreObject = Core.GetInstance();

    public UDPBroadcastLamp(string id, int hScanStart, int hScanEnd, int vScanStart, int vScanEnd, bool zoneInverted)
    {
      ID = id;
      HScanStart = hScanStart;
      HScanEnd = hScanEnd;
      VScanStart = vScanStart;
      VScanEnd = vScanEnd;
      ZoneInverted = zoneInverted;
    }

    public string ID { get; private set; }

    public LampType Type
    {
      get { return LampType.UDPBroadcast; }
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
        udpClient = new UdpClient();
        udpClientEndpoint = new IPEndPoint(IPAddress.Parse(ip), port);
        isConnected = true;
        Log.Debug("AtmoOrbHandler -[UDP Broadcast] Successfully connected to lamp {0} ({1}:{2})", ID, ip, port);
        if (coreObject.GetCurrentEffect() == ContentEffect.LEDsDisabled ||
            coreObject.GetCurrentEffect() == ContentEffect.Undefined)
        {
          ChangeColor(0, 0, 0, true, ID);
        }
        else if (coreObject.GetCurrentEffect() == ContentEffect.StaticColor)
        {
          ChangeColor(0, 0, 0, false, ID);
        }
      }
      catch (Exception ex)
      {
        Log.Error("AtmoOrbHandler - [UDP Broadcast] Exception while connecting to lamp {0} ({1}:{2})", ID, ip, port);
        Log.Error("AtmoOrbHandler - [UDP Broadcast] Exception: {0}", ex.Message);
      }
    }

    public void Disconnect()
    {
      try
      {
        if (udpClient != null)
        {
          udpClient.Close();
          udpClient = null;
        }
        isConnected = false;
      }
      catch (Exception ex)
      {
        Log.Error("AtmoOrbHandler - [UDP Broadcast] Exception while disconnecting from lamp {0} ({1}:{2})", ID, IP, Port);
        Log.Error("AtmoOrbHandler - [UDP Broadcast] Exception: {0}", ex.Message);
      }
    }

    public bool IsConnected()
    {
      return isConnected;
    }

    public void ChangeColor(byte red, byte green, byte blue, bool forceLightsOff, string orbId)
    {
      var color = string.Format("{0,2:X}{1,2:X}{2,2:X}", red, green, blue);

      if (!IsConnected())
      {
        return;
      }
      var bytes = Encoding.ASCII.GetBytes("setcolor:" + color + ";");
      udpClient.Send(bytes, bytes.Length, udpClientEndpoint);
    }
  }
}
