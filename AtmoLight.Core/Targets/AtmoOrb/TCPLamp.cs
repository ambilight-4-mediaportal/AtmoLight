using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace AtmoLight.Targets
{
  class TCPLamp : ILamp
  {
    private string id;
    private string ip;
    private int port;
    private int hScanStart;
    private int hScanEnd;
    private int vScanStart;
    private int vScanEnd;
    private bool isConnected = false;
    private TcpClient tcpClient;
    private Core coreObject = Core.GetInstance();

    public TCPLamp(string id, string ip, int port, int hScanStart, int hScanEnd, int vScanStart, int vScanEnd)
    {
      this.id = id;
      this.ip = ip;
      this.port = port;
      this.hScanStart = hScanStart;
      this.hScanEnd = hScanEnd;
      this.vScanStart = vScanStart;
      this.vScanEnd = vScanEnd;

      Connect(ip, port);
    }

    public string ID { get { return id; } }

    public string Type { get { return "TCP"; } }

    public int[] OverallAverageColor { get; set; }

    public int[] AverageColor { get; set; }

    public int[] PreviousColor { get; set; }

    public int PixelCount { get; set; }

    public int HScanStart { get { return hScanStart; } }

    public int HScanEnd { get { return hScanEnd; } }

    public int VScanStart { get { return vScanStart; } }

    public int VScanEnd { get { return vScanEnd; } }

    public void Connect(string ip, int port)
    {
      this.ip = ip;
      this.port = port;
      try
      {
        Disconnect();
        tcpClient = new TcpClient(ip,port);
        isConnected = true;
        Log.Debug("AtmoOrbHandler - Successfully connected to lamp {0} ({1}:{2})", id, ip, port);
        if (coreObject.GetCurrentEffect() == ContentEffect.LEDsDisabled || coreObject.GetCurrentEffect() == ContentEffect.Undefined)
        {
          ChangeColor("000000");
        }
        else if (coreObject.GetCurrentEffect() == ContentEffect.StaticColor)
        {
          string redHex = coreObject.staticColor[0].ToString("X");
          string greenHex = coreObject.staticColor[1].ToString("X");
          string blueHex = coreObject.staticColor[2].ToString("X");

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

          ChangeColor(redHex + greenHex + blueHex);
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
        if (tcpClient != null)
        {
          tcpClient.Close();
          tcpClient = null;
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

    public void ChangeColor(string color)
    {
      if (!IsConnected())
      {
        return;
      }
      byte[] bytes = Encoding.ASCII.GetBytes("setcolor:" + color + ";");
      NetworkStream stream = tcpClient.GetStream();
      stream.Write(bytes, 0, bytes.Length);
      stream.Close();
    }
  }
}
