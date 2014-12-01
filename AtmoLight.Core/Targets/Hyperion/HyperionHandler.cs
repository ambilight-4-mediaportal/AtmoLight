using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Linq;
using proto;
using Microsoft.Win32;

namespace AtmoLight.Targets
{
  class HyperionHandler : ITargets
  {
    #region Fields

    public Target Name { get { return Target.Hyperion; } }
    public TargetType Type { get { return TargetType.Network; } }
    public bool AllowDelay { get { return true; } }
    public List<ContentEffect> SupportedEffects
    {
      get
      {
        return new List<ContentEffect> {  ContentEffect.GIFReader,
                                          ContentEffect.LEDsDisabled,
                                          ContentEffect.MediaPortalLiveMode,
                                          ContentEffect.StaticColor,
                                          ContentEffect.VUMeter,
                                          ContentEffect.VUMeterRainbow
        };
      }
    }

    private static TcpClient Socket = new TcpClient();
    private Stream Stream;
    private bool isInit = false;
    private volatile bool initLock = false;
    private int hyperionReconnectCounter = 0;
    private Stopwatch liveReconnectSW = new Stopwatch();

    private Core coreObject;

    #endregion

    #region Hyperion
    public HyperionHandler()
    {
      coreObject = Core.GetInstance();
    }

    public void Initialise(bool force = false)
    {
      try
      {
        if (!initLock)
        {
          Log.Debug("HyperionHandler - Initialising");
          hyperionReconnectCounter = 0;
          initLock = true;
          isInit = true;

          if (coreObject.hyperionLiveReconnect)
          {
            liveReconnect();
          }
          else
          {
            Connect();
          }
        }
        else
        {
          Log.Debug("HyperionHandler - Initialising locked.");
        }
      }
      catch (Exception e)
      {
        Log.Error("HyperionHandler - Error during initialise");
        Log.Error("HyperionHandler - Exception: {0}", e.Message);
      }
    }

    public void ReInitialise(bool force = false)
    {
      Log.Debug("HyperionHandler - Reinitialising");
      if (coreObject.reInitOnError || force)
      {
        Initialise(force);
      }
    }

    public void Dispose()
    {
      //If connected close any remaining sockets and enabled set effect.
      if (Socket.Connected)
      {
        if (coreObject.GetCurrentEffect() == ContentEffect.LEDsDisabled || coreObject.GetCurrentEffect() == ContentEffect.Undefined)
        {
          ClearPriority(coreObject.hyperionPriority);
          ClearPriority(coreObject.hyperionPriorityStaticColor);
        }
        Disconnect();
      }

      //Stop live reconnect so it doesn't start new connect threads.
      if (coreObject.hyperionLiveReconnect)
      {
        coreObject.hyperionLiveReconnect = false;
      }
    }
    public bool IsConnected()
    {
      if (initLock)
      {
        return false;
      }

      return Socket.Connected;
    }

    private void Connect()
    {
      try
      {
        Thread t = new Thread(ConnectThread);
        t.IsBackground = true;
        t.Start();
      }
      catch (Exception e)
      {
        Log.Error("HyperionHandler - Error while starting connect thread");
        Log.Error("HyperionHandler - Exception: {0}", e.Message);
      }
    }
    private void liveReconnect()
    {
      try
      {
        Thread t = new Thread(liveReconnectThread);
        t.IsBackground = true;
        t.Start();
      }
      catch (Exception e)
      {
        Log.Error("HyperionHandler - Error while starting live reconnect thread");
        Log.Error("HyperionHandler - Exception: {0}", e.Message);
      }
    }

    private void liveReconnectThread()
    {
      liveReconnectSW.Start();

      //Start live reconnect with set delay in config
      while (coreObject.hyperionLiveReconnect)
      {
        if (liveReconnectSW.ElapsedMilliseconds >= coreObject.hyperionReconnectDelay && Socket.Connected == false)
        {
          Connect();
          liveReconnectSW.Restart();
        }
      }

      liveReconnectSW.Stop();
    }

    private void ConnectThread()
    {
      try
      {
        while (hyperionReconnectCounter <= coreObject.hyperionReconnectAttempts)
        {
          if (!Socket.Connected)
          {
            try
            {
              if (!coreObject.hyperionLiveReconnect)
              {
                Log.Debug("HyperionHandler - Trying to connect");
              }

              //Close old socket and create new TCP client which allows it to reconnect when calling Connect()
              Disconnect();

              Socket = new TcpClient();
              Socket.SendTimeout = 5000;
              Socket.ReceiveTimeout = 5000;
              Socket.Connect(coreObject.hyperionIP, coreObject.hyperionPort);
              Stream = Socket.GetStream();

              if (!coreObject.hyperionLiveReconnect)
              {
                Log.Debug("HyperionHandler - Connected");
              }
            }
            catch (Exception e)
            {
              if (!coreObject.hyperionLiveReconnect)
              {
                Log.Error("HyperionHandler - Error while connecting");
                Log.Error("HyperionHandler - Exception: {0}", e.Message);
              }
            }

            //if live connect enabled don't use this loop and let liveReconnectThread() fire up new connections
            if (coreObject.hyperionLiveReconnect)
            {
              break;
            }
            else
            {
              //Increment times tried
              hyperionReconnectCounter++;

              //Show error if reconnect attempts exhausted
              if (hyperionReconnectCounter > coreObject.hyperionReconnectAttempts && !Socket.Connected)
              {
                Log.Error("HyperionHandler - Error while connecting and connection attempts exhausted");
                coreObject.NewConnectionLost(Name);
                break;
              }

              //Sleep for specified time
              Thread.Sleep(coreObject.hyperionReconnectDelay);
            }
            //Log.Error("HyperionHandler - retry attempt {0} of {1}",hyperionReconnectCounter,hyperionReconnectAttempts);
          }
          else
          {
            //Log.Debug("HyperionHandler - Connected after {0} attempts.", hyperionReconnectCounter);
            break;
          }
        }

        //Reset Init lock
        initLock = false;

        //Reset counter when we have finished
        hyperionReconnectCounter = 0;

        //On first initialize set the effect after we are done trying to connect
        if (isInit && Socket.Connected)
        {
          ChangeEffect(coreObject.GetCurrentEffect());
          isInit = false;
        }
        else if (isInit)
        {
          isInit = false;
        }
      }
      catch (Exception e)
      {
        Log.Error("HyperionHandler - Error during connect thread");
        Log.Error("HyperionHandler - Exception: {0}", e.Message);

        //Reset Init lock
        initLock = false;
        isInit = false;
      }
    }
    private void Disconnect()
    {
      try
      {
        Socket.Close();
      }
      catch (Exception e)
      {
        Log.Error(string.Format("HyperionHandler - {0}", "Error during disconnect"));
        Log.Error(string.Format("HyperionHandler - {0}", e.Message));
      }
    }


    public void ChangeColor(int red, int green, int blue)
    {
      if (!IsConnected())
      {
        return;
      }
      ColorRequest colorRequest = ColorRequest.CreateBuilder()
        .SetRgbColor((red * 256 * 256) + (green * 256) + blue)
        .SetPriority(coreObject.hyperionPriorityStaticColor)
        .SetDuration(-1)
        .Build();

      HyperionRequest request = HyperionRequest.CreateBuilder()
        .SetCommand(HyperionRequest.Types.Command.COLOR)
        .SetExtension(ColorRequest.ColorRequest_, colorRequest)
        .Build();

      SendRequest(request);
    }
    public void ClearPriority(int priority)
    {
      if (!IsConnected())
      {
        return;
      }
      ClearRequest clearRequest = ClearRequest.CreateBuilder()
      .SetPriority(priority)
      .Build();

      HyperionRequest request = HyperionRequest.CreateBuilder()
      .SetCommand(HyperionRequest.Types.Command.CLEAR)
      .SetExtension(ClearRequest.ClearRequest_, clearRequest)
      .Build();

      SendRequest(request);
    }
    public void ClearAll()
    {
      if (!IsConnected())
      {
        return;
      }
      HyperionRequest request = HyperionRequest.CreateBuilder()
      .SetCommand(HyperionRequest.Types.Command.CLEARALL)
      .Build();

      SendRequest(request);
    }
    public bool ChangeEffect(ContentEffect effect)
    {
      if (!IsConnected())
      {
        return false;
      }
      switch (effect)
      {
        case ContentEffect.StaticColor:
          ChangeColor(coreObject.staticColor[0], coreObject.staticColor[1], coreObject.staticColor[2]);
          break;
        case ContentEffect.LEDsDisabled:
        case ContentEffect.Undefined:
        default:
          ClearPriority(coreObject.hyperionPriority);
          ClearPriority(coreObject.hyperionPriorityStaticColor);
          break;
      }
      return true;
    }
    public void ChangeProfile()
    {
      return;
    }
    public void ChangeImage(byte[] pixeldata, byte[] bmiInfoHeader)
    {
      if (!IsConnected())
      {
        return;
      }
      // Hyperion expects the bytestring to be the size of 3*width*height.
      // So 3 bytes per pixel, as in RGB.
      // Given pixeldata however is 4 bytes per pixel, as in RGBA.
      // So we need to remove the last byte per pixel.
      byte[] newpixeldata = new byte[coreObject.GetCaptureHeight() * coreObject.GetCaptureWidth() * 3];
      int x = 0;
      int i = 0;
      while (i <= (newpixeldata.GetLength(0) - 2))
      {
        newpixeldata[i] = pixeldata[i + x + 2];
        newpixeldata[i + 1] = pixeldata[i + x + 1];
        newpixeldata[i + 2] = pixeldata[i + x];
        i += 3;
        x++;
      }

      ImageRequest imageRequest = ImageRequest.CreateBuilder()
        .SetImagedata(Google.ProtocolBuffers.ByteString.CopyFrom(newpixeldata))
        .SetImageheight(coreObject.GetCaptureHeight())
        .SetImagewidth(coreObject.GetCaptureWidth())
        .SetPriority(coreObject.hyperionPriority)
        .SetDuration(-1)
        .Build();

      HyperionRequest request = HyperionRequest.CreateBuilder()
        .SetCommand(HyperionRequest.Types.Command.IMAGE)
        .SetExtension(ImageRequest.ImageRequest_, imageRequest)
        .Build();

      SendRequest(request);
    }

    private void SendRequest(HyperionRequest request)
    {
      try
      {
        if (Socket.Connected)
        {
          int size = request.SerializedSize;

          Byte[] header = new byte[4];
          header[0] = (byte)((size >> 24) & 0xFF);
          header[1] = (byte)((size >> 16) & 0xFF);
          header[2] = (byte)((size >> 8) & 0xFF);
          header[3] = (byte)((size) & 0xFF);

          int headerSize = header.Count();
          Stream.Write(header, 0, headerSize);
          request.WriteTo(Stream);
          Stream.Flush();

          // Enable reply message if needed (debugging only)
          //HyperionReply reply = receiveReply();
        }
      }
      catch (Exception e)
      {
        Log.Error("HyperionHandler - Error while sending proto request");
        Log.Error("HyperionHandler - Exception: {0}", e.Message);

        ReInitialise(false);
      }
    }

    private HyperionReply receiveReply()
    {
      try
      {
        Stream input = Socket.GetStream();
        byte[] header = new byte[4];
        input.Read(header, 0, 4);
        int size = (header[0] << 24) | (header[1] << 16) | (header[2] << 8) | (header[3]);
        byte[] data = new byte[size];
        input.Read(data, 0, size);
        HyperionReply reply = HyperionReply.ParseFrom(data);
        return reply;
      }
      catch (Exception e)
      {
        Log.Error("HyperionHandler - Error while receiving reply from proto request");
        Log.Error("HyperionHandler - Exception: {0}", e.Message);

        ReInitialise(false);
        return null;
      }
    }
    #endregion

    #region powerstate monitoring
    public void PowerModeChanged(PowerModes powerMode)
    {
      switch (powerMode)
      {
        case PowerModes.Resume:

          // Close old socket
          Disconnect();

          //Reconnect to Hyperion after standby
          Log.Debug("HyperionHandler - Initialising after standby");
          Initialise();

          break;
        case PowerModes.Suspend:

          ClearPriority(coreObject.hyperionPriorityStaticColor);
          ClearPriority(coreObject.hyperionPriority);

          break;
      }
    }
    #endregion
  }
}