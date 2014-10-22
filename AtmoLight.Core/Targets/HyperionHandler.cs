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

    private static TcpClient Socket = new TcpClient();
    private Stream Stream;
    private Boolean Connected = false;

    private int captureWidth = 64;
    private int captureHeight = 64;


    private string hyperionIP = "";
    private int hyperionPort = 0;
    private int hyperionStaticColor = 0;
    private int hyperionPriority = 0;
    private Boolean hyperionReconnectOnError = false;

    #endregion
    #region Hyperion

    public bool Initialise()
    {
      Boolean IsInitialised = true;

      try
      {
        Connect();
        ClearPriority(hyperionPriority);
      }
      catch (Exception e)
      {
        Log.Debug("Error during initialise of Hyperion");
        IsInitialised = false;
      }

      return IsInitialised;
    }

    public bool Dispose()
    {
      if (Socket.Connected)
      {
        ClearPriority(hyperionPriority);
        Socket.Close();
      }
      return true;
    }
    public bool IsConnected()
    {
      return Connected;
    }

    public int GetCaptureWidth()
    {
      return captureWidth;
    }
    public int GetCaptureHeight()
    {
      return captureHeight;
    }
    public void Connect()
    {
      //Use connection thread to prevent Mediaportal lag due to connect errors
      Thread t = new Thread(ConnectThread);
      t.Start();
    }
    private void ConnectThread()
    {
      try
      {
        Log.Debug("Trying to connect to Hyperion");

        //Close old socket and create new TCP client which allows it to reconnect when calling Connect()
        try
        {
          Socket.Close();
        }
        catch { };
        Socket = new TcpClient();

        Socket.SendTimeout = 5000;
        Socket.ReceiveTimeout = 5000;
        Socket.Connect(hyperionIP, hyperionPort);
        Log.Debug("Connected to Hyperion.");
        Stream = Socket.GetStream();
        Connected = Socket.Connected;
      }
      catch (Exception e)
      {
        Log.Debug("Error while connecting to Hyperion");
        Connected = false;
      }

    }
    public void ChangeColor(int red, int green, int blue)
    {
      ColorRequest colorRequest = ColorRequest.CreateBuilder()
        .SetRgbColor((red * 256 * 256) + (green * 256) + blue)
        .SetPriority(hyperionPriority)
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
      HyperionRequest request = HyperionRequest.CreateBuilder()
      .SetCommand(HyperionRequest.Types.Command.CLEARALL)
      .Build();

      SendRequest(request);
    }
    public bool ChangeEffect(ContentEffect effect)
    {
      return true;
    }
    public bool ChangeProfile()
    {
      return true;
    }
    public bool ChangeImage(byte[] pixeldata, byte[] dummy)
    {
      // Hyperion expects the bytestring to be the size of 3*width*height.
      // So 3 bytes per pixel, as in RGB.
      // Given pixeldata however is 4 bytes per pixel, as in RGBA.
      // So we need to remove the last byte per pixel.
      byte[] newpixeldata = new byte[captureHeight * captureWidth * 3];
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
        .SetImageheight(captureHeight)
        .SetImagewidth(captureWidth)
        .SetPriority(hyperionPriority)
        .SetDuration(-1)
        .Build();

      HyperionRequest request = HyperionRequest.CreateBuilder()
        .SetCommand(HyperionRequest.Types.Command.IMAGE)
        .SetExtension(ImageRequest.ImageRequest_, imageRequest)
        .Build();

      SendRequest(request);
      return true;
    }

    private void SendRequest(HyperionRequest request)
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
        Log.Debug("Hyperion data written.");

        Stream.Flush();
        Log.Debug("Hyperion data flushed.");

        HyperionReply reply = receiveReply();
        Log.Debug("Hyperion reply: {0}", reply);
      }
      else
      {
        Connected = false;
        Connect();
      }
    }

    private HyperionReply receiveReply()
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
    #endregion

    #region settings
    public void setHyperionIP(string ip)
    {
      hyperionIP = ip;
    }
    public void setHyperionPort(int port)
    {
      hyperionPort = port;
    }
    public void setCaptureWidth(int width)
    {
      captureWidth = width;
    }
    public void setCaptureHeight(int height)
    {
      captureHeight = height;
    }
    public void setHyperionStaticColor(int staticColor)
    {
      hyperionStaticColor = staticColor;
    }
    public void setHyperionPriority(int priority)
    {
      hyperionPriority = priority;
    }
    public void setReconnectOnError(Boolean reconnectOnError)
    {
      hyperionReconnectOnError = reconnectOnError;
    }

    #endregion
  }
}
