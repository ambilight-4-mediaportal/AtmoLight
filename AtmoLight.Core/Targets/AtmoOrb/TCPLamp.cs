using System;
using System.Net.Sockets;
using System.Threading;

namespace AtmoLight.Targets
{
  internal class TCPLamp : ILamp
  {
    private TcpClient _client;
    private Socket socket;
    private volatile bool connectLock;
    private Thread connectThreadHelper;
    private readonly Core coreObject = Core.GetInstance();

    public TCPLamp(string id, string ip, int port, int hScanStart, int hScanEnd, int vScanStart, int vScanEnd,
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
      get { return LampType.TCP; }
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
      if (!connectLock)
      {
        connectThreadHelper = new Thread(() => ConnectThreaded(ip, port));
        connectThreadHelper.Name = "AtmoLight AtmoOrb Connect " + ID;
        connectThreadHelper.IsBackground = true;
        connectThreadHelper.Start();
      }
    }

    private void ConnectThreaded(string ip, int port)
    {
      if (connectLock)
      {
        Log.Debug("AtmoOrbHandler - Connect locked for lamp {0}.", ID);
        return;
      }
      connectLock = true;
      try
      {
        Disconnect();
        _client = new TcpClient(ip, port);
        socket = _client.Client;
        Log.Debug("AtmoOrbHandler - Socket connected: " + socket.Connected);
        Log.Debug("AtmoOrbHandler - TCP connected: " + _client.Connected);
        Log.Debug("AtmoOrbHandler - Successfully connected to lamp {0} ({1}:{2})", ID, ip, port);

        if (coreObject.GetCurrentEffect() == ContentEffect.LEDsDisabled ||
            coreObject.GetCurrentEffect() == ContentEffect.Undefined)
        {
          ChangeColor(0, 0, 0, true, false, ID);
        }
        else if (coreObject.GetCurrentEffect() == ContentEffect.StaticColor)
        {
          var red = byte.Parse(coreObject.staticColor[0].ToString());
          var green = byte.Parse(coreObject.staticColor[1].ToString());
          var blue = byte.Parse(coreObject.staticColor[2].ToString());

          ChangeColor(red, green, blue, false, false, ID);
        }

        // Reset lock
        connectLock = false;
      }
      catch (Exception ex)
      {
        connectLock = false;
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
      }
      catch (Exception ex)
      {
        Log.Error("AtmoOrbHandler - Exception while disconnecting from lamp {0} ({1}:{2})", ID, IP, Port);
        Log.Error("AtmoOrbHandler - Exception: {0}", ex.Message);
      }
    }

    public bool IsConnected()
    {
      if (_client == null)
      {
        return false;
      }

      return _client.Connected && !connectLock;
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

        // Options parameter: 1 = force off | 2 = use lamp smoothing and validate by Orb ID | 4 = validate by Orb ID
        if (forceLightsOff)
        {
          bytes[3] = 1;
        }
        else
        {
          // Always validate by Orb ID
          if (useLampSmoothing)
          {
            bytes[3] = 2;
          }
          else
          {
            bytes[3] = 4;
          }
        }

        // Orb ID
        bytes[4] = byte.Parse(orbId);

        // RED / GREEN / BLUE
        bytes[5] = red;
        bytes[6] = green;
        bytes[7] = blue;

        Send(socket, bytes, 0, bytes.Length, 50);
      }
      catch (Exception)
      {
        //Log.Error("Error during send message..");
      }
    }

    public static void Send(Socket socket, byte[] buffer, int offset, int size, int timeout)
    {
      var startTickCount = Environment.TickCount;
      var sent = 0; // how many bytes is already sent
      do
      {
        if (Environment.TickCount > startTickCount + timeout)
          return;
        try
        {
          sent += socket.Send(buffer, offset + sent, size - sent, SocketFlags.None);
        }
        catch (Exception e)
        {
          //
        }
      } while (sent < size);
    }
  }
}