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

namespace AtmoLight.Targets
{
  class HyperionHandler : ITargets
  {
    #region Fields

    public Target Name { get { return Target.Hyperion; } }
    public TargetType Type { get { return TargetType.Network; } }

    public List<ContentEffect> SupportedEffects
    {
      get
      {
        return new List<ContentEffect> {  ContentEffect.GIFReader,
                                          ContentEffect.LEDsDisabled,
                                          ContentEffect.MediaPortalLiveMode,
                                          ContentEffect.StaticColor,
                                          ContentEffect.Undefined,
                                          ContentEffect.VUMeter,
                                          ContentEffect.VUMeterRainbow
        };
      }
    }

    private static TcpClient Socket = new TcpClient();
    private Stream Stream;
    private Boolean Connected = false;
    private Boolean isInit = false;
    int hyperionReconnectCounter = 0;
    public int hyperionReconnectAttempts = 0;
    private Stopwatch liveReconnectSW = new Stopwatch();

    private Core coreObject;

    #endregion

    public HyperionHandler()
    {
      coreObject = Core.GetInstance();
    }
    #region Hyperion

    public void Initialise(bool force = false)
    {
      try
      {
        isInit = true;
        Connect();
        ClearPriority(coreObject.hyperionPriority);

        if (coreObject.hyperionLiveReconnect)
        {
          liveReconnect();
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
      //Reset reconnect counter if needed
      hyperionReconnectCounter = 0;
      Connect();
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
        Socket.Close();
      }

      //Stop live reconnect so it doesn't start new connect threads.
      if (coreObject.hyperionLiveReconnect)
      {
        coreObject.hyperionLiveReconnect = false;
      }
    }
    public bool IsConnected()
    {
      if (Socket.Connected)
      {
        Connected = true;
      }
      else
      {
        Connected = false;
      }

      return Connected;
    }

    private void Connect()
    {
      Thread t = new Thread(ConnectThread);
      t.Start();
    }
    private void liveReconnect()
    {
      Thread t = new Thread(liveReconnectThread);
      t.Start();
    }

    private void liveReconnectThread()
    {
      liveReconnectSW.Start();

      //Start live reconnect with set delay in config
      while (coreObject.hyperionLiveReconnect)
      {
        if (liveReconnectSW.ElapsedMilliseconds >= coreObject.hyperionReconnectDelay && Connected == false)
        {
          Connect();
          liveReconnectSW.Restart();
        }
      }

      liveReconnectSW.Stop();
    }

    private void ConnectThread()
    {
      while (hyperionReconnectCounter <= coreObject.hyperionReconnectAttempts)
      {
        if (Connected == false)
        {
          try
          {
            if (coreObject.hyperionLiveReconnect == false)
            {
              Log.Debug("HyperionHandler - Trying to connect");
            }

            //Close old socket and create new TCP client which allows it to reconnect when calling Connect()
            try
            {
              Socket.Close();
            }
            catch (Exception e)
            {
              if (coreObject.hyperionLiveReconnect == false)
              {
                Log.Error("HyperionHandler - Error while closing socket");
                Log.Error("HyperionHandler - Exception: {0}", e.Message);
              }
            }
            Socket = new TcpClient();

            Socket.SendTimeout = 5000;
            Socket.ReceiveTimeout = 5000;
            Socket.Connect(coreObject.hyperionIP, coreObject.hyperionPort);
            Stream = Socket.GetStream();
            Connected = Socket.Connected;

            if (coreObject.hyperionLiveReconnect == false)
            {
              Log.Debug("HyperionHandler - Connected");
            }
          }
          catch (Exception e)
          {
            if (coreObject.hyperionLiveReconnect == false)
            {

              Log.Error("HyperionHandler - Error while connecting");
              Log.Error("HyperionHandler - Exception: {0}", e.Message);
            }
            Connected = false;
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

            if (hyperionReconnectCounter > coreObject.hyperionReconnectAttempts)
            {
                coreObject.NewConnectionLost(Name);
            }

            //Sleep for specified time
            Thread.Sleep(coreObject.hyperionReconnectDelay);
          }
          //Log.Error("HyperionHandler - retry attempt {0} of {1}",hyperionReconnectCounter,hyperionReconnectAttempts);
        }
        else
        {
          //Log.Debug("HyperionHandler - Connected after {0} attempts.", hyperionReconnectCounter);
          hyperionReconnectCounter = 0;
          break;
        }
      }

      //On first initialize set the effect after we are done trying to connect
      if (isInit && Connected)
      {
        ChangeEffect(coreObject.GetCurrentEffect());
        coreObject.SetAtmoLightOn(coreObject.GetCurrentEffect() == ContentEffect.LEDsDisabled || coreObject.GetCurrentEffect() == ContentEffect.LEDsDisabled ? false : true);

        isInit = false;
      }
      else if (isInit)
      {
        isInit = false;
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
        case ContentEffect.LEDsDisabled:
        case ContentEffect.Undefined:
          ClearPriority(coreObject.hyperionPriority);
          ClearPriority(coreObject.hyperionPriorityStaticColor);
          break;
        case ContentEffect.StaticColor:
          ChangeColor(coreObject.staticColor[0], coreObject.staticColor[1], coreObject.staticColor[2]);
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
          HyperionReply reply = receiveReply();
        }
        else
        {
          Connected = false;
          Connect();
        }
      }
      catch (Exception e)
      {
        Log.Error("HyperionHandler - Error while sending proto request");
        Log.Error("HyperionHandler - Exception: {0}", e.Message);
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
      catch(Exception e)
      {
        Log.Error("HyperionHandler - Error while receiving reply from proto request");
        Log.Error("HyperionHandler - Exception: {0}", e.Message);
        return null;
      }
    }
    #endregion
  }
}
