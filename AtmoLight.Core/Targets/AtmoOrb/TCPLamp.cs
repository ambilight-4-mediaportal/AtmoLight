using System;
using System.Net.Sockets;
using System.Threading;

namespace AtmoLight.Targets
{
  internal class TCPLamp : ILamp
  {
    private TcpClient _tcpClient;
    private Socket _socket;
    private volatile bool _connectLock;
    private Thread _connectThreadHelper;
    private readonly Core _coreObject = Core.GetInstance();

    public TCPLamp(string id, string ip, int port, int hScanStart, int hScanEnd, int vScanStart, int vScanEnd,
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

    public void Connect(string ip, int port)
    {
      IP = ip;
      Port = port;
      if (!_connectLock)
      {
        _connectThreadHelper = new Thread(() => ConnectThreaded(ip, port));
        _connectThreadHelper.Name = "AtmoLight AtmoOrb Connect " + ID;
        _connectThreadHelper.IsBackground = true;
        _connectThreadHelper.Start();
      }
    }

    private void ConnectThreaded(string ip, int port)
    {
      if (_connectLock)
      {
        Log.Debug("AtmoOrbHandler - Connect locked for lamp {0}.", ID);
        return;
      }
      _connectLock = true;
      try
      {
        Disconnect();
        _tcpClient = new TcpClient(ip, port);
        _socket = _tcpClient.Client;
        Log.Debug("AtmoOrbHandler - Socket connected: " + _socket.Connected);
        Log.Debug("AtmoOrbHandler - TCP connected: " + _tcpClient.Connected);
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

        // Reset lock
        _connectLock = false;
      }
      catch (Exception ex)
      {
        _connectLock = false;
        Log.Error("AtmoOrbHandler - Exception while connecting to lamp {0} ({1}:{2})", ID, ip, port);
        Log.Error("AtmoOrbHandler - Exception: {0}", ex.Message);
      }
    }

    public void Disconnect()
    {
      try
      {
        if (_tcpClient != null)
        {
          _tcpClient.Close();
          _tcpClient = null;
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
      if (_tcpClient == null)
      {
        return false;
      }

      return _tcpClient.Connected && !_connectLock;
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

        Send(_socket, bytes, 0, bytes.Length, 50);
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