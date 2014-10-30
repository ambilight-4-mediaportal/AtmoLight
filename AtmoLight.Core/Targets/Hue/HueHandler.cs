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
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace AtmoLight.Targets
{
  class HueHandler : ITargets
  {
    #region Fields

    public Target Name { get { return Target.Hue; } }
    public TargetType Type { get { return TargetType.Network; } }

    public static TcpClient client = new TcpClient();
    public static IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000);
    private Boolean Connected = false;
    private Core coreObject;

    #endregion

    #region Hue
    public HueHandler()
    {
      coreObject = Core.GetInstance();
    }

    public void Initialise(bool force = false)
    {
      Connect();
    }

    public void ReInitialise(bool force = false)
    {
      Connect();
    }

    public void Dispose()
    {
      if (client.Connected)
      {
        try
        {
          client.Close();
        }
        catch (Exception e)
        {
          Log.Error(string.Format("Hue - {0}", "Error during dispose"));
          Log.Error(string.Format("Hue - {0}", e.Message));
        }
      }
    }
    public bool IsConnected()
    {
      if (client.Connected)
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
      if (Connected == false)
      {
        Thread t = new Thread(ConnectThread);
        t.Start();
      }
    }

    private void ConnectThread()
    {
      try
      {
        client.Connect(serverEndPoint);
      }
      catch (Exception e)
      {
        Log.Error("Hue - error during connect");
        Log.Error(string.Format("Hue - {0}", e.Message));
      }
    }

    public void ChangeColor(int red, int green, int blue)
    {
      Thread t = new Thread(() => ChangeColorThread(red,green,blue));
      t.Start();
    }
    public void ChangeColorThread(int red, int green, int blue)
    {
      try
      {
          string message = string.Format("{0},{1},{2}", red.ToString(), green.ToString(), blue.ToString());
          NetworkStream clientStream = client.GetStream();
          ASCIIEncoding encoder = new ASCIIEncoding();
          byte[] buffer = encoder.GetBytes(message);

          clientStream.Write(buffer, 0, buffer.Length);
          clientStream.Flush();
      }
      catch (Exception e)
      {
        Log.Error("Hue - error during sending color");
        Log.Error(string.Format("Hue - {0}", e.Message));
      }
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
          ChangeColor(0, 0, 0);
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
      int red = 0;
      int green = 0;
      int blue = 0;
      while (i <= (newpixeldata.GetLength(0) - 2))
      {
        newpixeldata[i] = pixeldata[i + x + 2];
        newpixeldata[i + 1] = pixeldata[i + x + 1];
        newpixeldata[i + 2] = pixeldata[i + x];
        red = newpixeldata[i];
        green = newpixeldata[i + 1];
        blue = newpixeldata[i + 2];
        i += 3;
        x++;
       
      }

      //Convert pixeldata to bitmap and calculate average color afterwards
      try
      {
        unsafe
        {
          fixed (byte* ptr = pixeldata)
          {

            using (Bitmap image = new Bitmap(coreObject.GetCaptureWidth(), coreObject.GetCaptureHeight(), coreObject.GetCaptureWidth() * 4,
                        PixelFormat.Format32bppRgb, new IntPtr(ptr)))
            {
              CalculateAverageColor(image);
            }
          }
        }
      }
      catch(Exception e)
      {
        Log.Error(string.Format("Hue - {0}", "Error during average color calculations"));
        Log.Error(string.Format("Hue - {0}", e.Message));
      }
    }
    public void CalculateAverageColor(Bitmap bm)
    {
      int width = bm.Width;
      int height = bm.Height;
      int red = 0;
      int green = 0;
      int blue = 0;
      int minDiversion = 15; // drop pixels that do not differ by at least minDiversion between color values (white, gray or black)
      int dropped = 0; // keep track of dropped pixels
      long[] totals = new long[] { 0, 0, 0 };
      int bppModifier = bm.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppRgb ? 3 : 4; // cutting corners, will fail on anything else but 32 and 24 bit images

      BitmapData srcData = bm.LockBits(new System.Drawing.Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, bm.PixelFormat);
      int stride = srcData.Stride;
      IntPtr Scan0 = srcData.Scan0;

      unsafe
      {
        byte* p = (byte*)(void*)Scan0;

        for (int y = 0; y < height; y++)
        {
          for (int x = 0; x < width; x++)
          {
            int idx = (y * stride) + x * bppModifier;
            red = p[idx + 2];
            green = p[idx + 1];
            blue = p[idx];
            if (Math.Abs(red - green) > minDiversion || Math.Abs(red - blue) > minDiversion || Math.Abs(green - blue) > minDiversion)
            {
              totals[2] += red;
              totals[1] += green;
              totals[0] += blue;
            }
            else
            {
              dropped++;
            }
          }
        }
      }

      int count = width * height - dropped;
      int avgR = (int)(totals[2] / count);
      int avgG = (int)(totals[1] / count);
      int avgB = (int)(totals[0] / count);


      //Send average colors to Bridge
      ChangeColor(avgR, avgG, avgB);
    }
    #endregion
  }
}
