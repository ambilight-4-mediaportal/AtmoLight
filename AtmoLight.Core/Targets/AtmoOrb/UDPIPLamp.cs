using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace AtmoLight.Targets
{
  class UDPIPLamp : ILamp
  {
    private string id;
    private string ip;
    private int port;
    private int hScanStart;
    private int hScanEnd;
    private int vScanStart;
    private int vScanEnd;
    private bool zoneInverted;
    private bool isConnected = false;
    private UdpClient udpClient;
    private IPEndPoint udpClientEndpoint;
    private Core coreObject = Core.GetInstance();

    public UDPIPLamp(string id, string ip, int port, int hScanStart, int hScanEnd, int vScanStart, int vScanEnd, bool zoneInverted)
    {
      this.id = id;
      this.ip = ip;
      this.port = port;
      this.hScanStart = hScanStart;
      this.hScanEnd = hScanEnd;
      this.vScanStart = vScanStart;
      this.vScanEnd = vScanEnd;
      this.zoneInverted = zoneInverted;
    }

    public string ID { get { return id; } }

    public LampType Type { get { return LampType.UDPIP; } }

    public int[] OverallAverageColor { get; set; }

    public int[] AverageColor { get; set; }

    public int[] PreviousColor { get; set; }

    public int PixelCount { get; set; }

    public int HScanStart { get { return hScanStart; } }

    public int HScanEnd { get { return hScanEnd; } }

    public int VScanStart { get { return vScanStart; } }

    public int VScanEnd { get { return vScanEnd; } }

    public bool ZoneInverted { get { return zoneInverted; } }

    public string IP { get { return ip; } }

    public int Port { get { return port; } }

    public void Connect(string ip, int port)
    {
      this.ip = ip;
      this.port = port;
      try
      {
        Disconnect();
        udpClient = new UdpClient();
        udpClientEndpoint = new IPEndPoint(IPAddress.Parse(ip), port);
        isConnected = true;
        Log.Debug("AtmoOrbHandler - Successfully connected to lamp {0} ({1}:{2})", id, ip, port);

        if (coreObject.GetCurrentEffect() == ContentEffect.LEDsDisabled || coreObject.GetCurrentEffect() == ContentEffect.Undefined)
        {
          ChangeColor(0, 0, 0, true);
        }
        else if (coreObject.GetCurrentEffect() == ContentEffect.StaticColor)
        {

          byte red = byte.Parse(coreObject.staticColor[0].ToString());
          byte green = byte.Parse(coreObject.staticColor[1].ToString());
          byte blue = byte.Parse(coreObject.staticColor[2].ToString());

          ChangeColor(red, green, blue, false);
        }
      }
      catch (Exception ex)
      {
        Log.Error("AtmoOrbHandler - Exception while connecting to lamp {0} ({1}:{2})", id, ip, port);
        Log.Error("AtmoOrbHandler - Exception: {0}", ex.Message);
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
        Log.Error("AtmoOrbHandler - Exception while disconnecting from lamp {0} ({1}:{2})", id, ip, port);
        Log.Error("AtmoOrbHandler - Exception: {0}", ex.Message);
      }
    }

    public bool IsConnected()
    {
      return isConnected;
    }

    public void ChangeColor(byte red, byte green, byte blue, bool forceLightsOff)
    {
      if (!IsConnected())
      {
        return;
      }

      if (forceLightsOff)
      {
        ForceLightsOff();
      }
      else
      {
        try
        {
          // Fixed led count to expand later as its optional by default
          byte ledCount = 24;
          byte[] bytes = new byte[3 + ledCount * 3];

          // Command identifier: C0FFEE
          bytes[0] = 0xC0;
          bytes[1] = 0xFF;
          bytes[2] = 0xEE;

          // Force OFF if value greater than 1
          bytes[3] = 0;

          // RED / GREEN / BLUE
          bytes[4] = red;
          bytes[5] = green;
          bytes[6] = blue;

          udpClient.Send(bytes, bytes.Length, udpClientEndpoint);
          //Send(socket, bytes, 0, bytes.Length, 50);
        }
        catch (Exception e)
        {
          Log.Error("Error during send message..");
        }
      }
    }
    private void ForceLightsOff()
    {
      try
      {
        // Fixed led count to expand later as its optional by default
        byte ledCount = 24;
        byte[] bytes = new byte[3 + ledCount * 3];

        // Command identifier: C0FFEE
        bytes[0] = 0xC0;
        bytes[1] = 0xFF;
        bytes[2] = 0xEE;

        // Force OFF if value greater than 1
        bytes[3] = 255;

        // RED / GREEN / BLUE
        bytes[4] = 0;
        bytes[5] = 0;
        bytes[6] = 0;

        udpClient.Send(bytes, bytes.Length, udpClientEndpoint);
        //Send(socket, bytes, 0, bytes.Length, 50);
      }
      catch (Exception e)
      {
        Log.Error("Error during send message..");
      }
    }

  }
}
