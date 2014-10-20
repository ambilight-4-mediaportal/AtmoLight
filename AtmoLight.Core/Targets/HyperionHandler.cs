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
  class HyperionHandler
  {
    #region Fields

    public static TcpClient hyperionSocket = new TcpClient();
    public Stream hyperionStream;
    public Boolean hyperionIsConnected = false;

    private int captureWidth = 64;
    private int captureHeight = 48;

    #endregion
    #region Hyperion

    public Boolean SetupConnection()
    {
        if (hyperionIsConnected == false)
        {
            Log.Debug("Trying to connect to Hyperion");
            hyperionSocket.Connect(Settings.hyperionIP, Settings.hyperionPort);
            Log.Debug("Connected to Hyperion.");
            hyperionStream = hyperionSocket.GetStream();
        }
    }
    public void ChangeColor(int red, int green, int blue, int priority)
    {
      ColorRequest colorRequest = ColorRequest.CreateBuilder()
        .SetRgbColor((red * 256 * 256) + (green * 256) + blue)
        .SetPriority(priority)
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

    public void SendImage(byte[] pixeldata, int priority)
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
        .SetPriority(priority)
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
      if (hyperionSocket.Connected)
      {
        int size = request.SerializedSize;

        Byte[] header = new byte[4];
        header[0] = (byte)((size >> 24) & 0xFF);
        header[1] = (byte)((size >> 16) & 0xFF);
        header[2] = (byte)((size >> 8) & 0xFF);
        header[3] = (byte)((size) & 0xFF);

        int headerSize = header.Count();

        hyperionStream.Write(header, 0, headerSize);
        request.WriteTo(hyperionStream);
        Log.Debug("Hyperion data written.");

        hyperionStream.Flush();
        Log.Debug("Hyperion data flushed.");

        HyperionReply reply = receiveReply();
        Log.Debug("Hyperion reply: {0}", reply);
      }
    }

    private HyperionReply receiveReply()
    {
      Stream input = hyperionSocket.GetStream();
      byte[] header = new byte[4];
      input.Read(header, 0, 4);
      int size = (header[0] << 24) | (header[1] << 16) | (header[2] << 8) | (header[3]);
      byte[] data = new byte[size];
      input.Read(data, 0, size);
      HyperionReply reply = HyperionReply.ParseFrom(data);
      return reply;
    }
    #endregion
  }
}
