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
using Microsoft.Win32;

namespace AtmoLight.Targets
{
  class HueHandler : ITargets
  {
    #region Fields

    public Target Name { get { return Target.Hue; } }
    public TargetType Type { get { return TargetType.Network; } }

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

    // CORE
    private Core coreObject;
    private int hueDelayAtmoHue = 5000; 
    private int hueReconnectCounter = 0;

    // TCP
    private static TcpClient Socket = new TcpClient();
    private Stream Stream;
    private Boolean Connected = false;

    // Color checks
    private int avgR_previous = 0;
    private int avgG_previous = 0;
    private int avgB_previous = 0;

    // Locks
    private Boolean isInit = false;
    private Boolean isAtmoHueRunning = false;

    #endregion

    #region Hue
    public HueHandler()
    {
      coreObject = Core.GetInstance();
    }

    public void Initialise(bool force = false)
    {
      isInit = true;

      //Start monitoring powerstate for on resume reconnection
      monitorPowerState();

      Thread t = new Thread(() => InitialiseThread(force));
      t.IsBackground = true;
      t.Start();
    }

    private bool InitialiseThread(bool force = false)
    {
      if (!Win32API.IsProcessRunning("atmohue.exe") && coreObject.hueIsRemoteMachine == false)
      {
        if (coreObject.hueStart)
        {
          isAtmoHueRunning = StartHue();
          System.Threading.Thread.Sleep(hueDelayAtmoHue);
          if (isAtmoHueRunning)
          {
            Connect();
          }
        }
        else
        {
          Log.Error("HueHandler - AtmoHue is not running.");
          return false;
        }
      }
      else
      {
        isAtmoHueRunning = true;
        if (Socket.Connected)
        {
          Log.Debug("HueHandler - already connect to AtmoHue");
          return true;
        }
        else
        {
          Connect();
          return true;
        }
      }
      return true;
    }


    public void ReInitialise(bool force = false)
    {
      Thread t = new Thread(() => InitialiseThread(force));
      t.IsBackground = true;
      t.Start();
    }

    public void Dispose()
    {
      if (Socket.Connected)
      {
        try
        {
          Socket.Close();
        }
        catch (Exception e)
        {
          Log.Error(string.Format("Hue - {0}", "Error during dispose"));
          Log.Error(string.Format("Hue - {0}", e.Message));
        }
      }
    }

    public bool StartHue()
    {
      Log.Debug("HueHandler - Trying to start AtmoHue.");
      if (!System.IO.File.Exists(coreObject.huePath))
      {
        Log.Error("HueHandler - AtmoHue.exe not found!");
        return false;
      }
      
      Process Hue = new Process();
      Hue.StartInfo.FileName = coreObject.huePath;
      Hue.StartInfo.WorkingDirectory = Path.GetDirectoryName(coreObject.huePath);
      Hue.StartInfo.UseShellExecute = true;
      try
      {
        Hue.Start();
      }
      catch (Exception)
      {
        Log.Error("HueHander - Starting Hue failed.");
        return false;
      }
      Log.Info("HueHander - AtmoHue successfully started.");
      return true;
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
      t.IsBackground = true;
      t.Start();
    }

    private void ConnectThread()
    {

      while (hueReconnectCounter <= coreObject.hueReconnectAttempts)
      {
        if (Connected == false)
        {
          //Close old socket and create new TCP client which allows it to reconnect when calling Connect()
          try
          {
            Socket.Close();
          }
          catch (Exception e)
          {
            Log.Error("HueHandler - Error while closing socket");
            Log.Error("HueHandler - Exception: {0}", e.Message);
          }
          try
          {
            Socket = new TcpClient();

            Socket.SendTimeout = 5000;
            Socket.ReceiveTimeout = 5000;
            Socket.Connect(coreObject.hueIP, coreObject.huePort);
            Stream = Socket.GetStream();
            Connected = Socket.Connected;
            Log.Debug("HueHandler - Connected to AtmoHue");
          }
          catch (Exception e)
          {
            Connected = false;
            Log.Error("HyperionHandler - Error while connecting");
            Log.Error("HyperionHandler - Exception: {0}", e.Message);
          }

          //Increment times tried
          hueReconnectCounter++;

          //Show error if reconnect attempts exhausted
          if (hueReconnectCounter > coreObject.hueReconnectAttempts && Connected == false)
          {
            Log.Error("HueHandler - Error while connecting and connection attempts exhausted");
            coreObject.NewConnectionLost(Name);
            break;
          }

          //Sleep for specified time
          Thread.Sleep(coreObject.hyperionReconnectDelay);
        }
        else
        {
          //Log.Debug("HueHandler - Connected after {0} attempts.", hyperionReconnectCounter);
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

      //Reset counter when we have finished
      hueReconnectCounter = 0;
    }

    public void ChangeColor(int red, int green, int blue, int priority)
    {
      Thread t = new Thread(() => ChangeColorThread(red,green,blue, priority));
      t.IsBackground = true;
      t.Start();

    }
    public void ChangeColorThread(int red, int green, int blue, int priority)
    {
      try
      {
          string message = string.Format("{0},{1},{2},{3}", red.ToString(), green.ToString(), blue.ToString(), priority.ToString());
          ASCIIEncoding encoder = new ASCIIEncoding();
          byte[] buffer = encoder.GetBytes(message);

          Stream.Write(buffer, 0, buffer.Length);
          Stream.Flush();
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
        case ContentEffect.StaticColor:
          ChangeColor(coreObject.staticColor[0], coreObject.staticColor[1], coreObject.staticColor[2], 10);
          break;
        case ContentEffect.LEDsDisabled:
        case ContentEffect.Undefined:
        default:
          ChangeColor(0, 0, 0, 1);
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
              if (coreObject.GetCurrentEffect() == ContentEffect.VUMeter || coreObject.GetCurrentEffect() == ContentEffect.VUMeterRainbow)
              {
                CalculateVUMeterColorAndSendToHue(image);
              }
              else
              {
                CalculateAverageColorAndSendToHue(image);
              }
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
    public void CalculateAverageColorAndSendToHue(Bitmap bm)
    {
      int width = bm.Width;
      int height = bm.Height;
      int red = 0;
      int green = 0;
      int blue = 0;
      int minDiversion = 15; // drop pixels that do not differ by at least minDiversion between color values (white, gray or black)
      int dropped = 0; // keep track of dropped pixels
      long[] totals = new long[] { 0, 0, 0 };
      int bppModifier = bm.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb ? 3 : 4; // cutting corners, will fail on anything else but 32 and 24 bit images

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

      int minDifferencePreviousColors = coreObject.hueMinimalColorDifference;


      int avgR = 0;
      int avgG = 0;
      int avgB = 0;
      bool invalidColorValue = false;

      // Doesn't work all the time, will return divide by zero errors sometimes due to invalid values.
      // If we get an invalid value we return 0 and skip that image
      try
      {
        avgR = (int)(totals[2] / count);
      }
      catch
      {
        invalidColorValue = true;
      }

      try
      {
        avgG = (int)(totals[1] / count);
      }
      catch
      {
        invalidColorValue = true;
      }

      try
      {
        avgB = (int)(totals[0] / count);
      }
      catch
      {
        invalidColorValue = true;
      }

      //If users sets minimal difference to 0 disable the average color check
      if (minDifferencePreviousColors == 0 && invalidColorValue == false)
      {
        //Send average colors to Bridge
        ChangeColor(avgR, avgG, avgB, 200);
      }
      else
      {
        //Minimal differcence new compared to previous colors
        if (Math.Abs(avgR_previous - avgR) > minDifferencePreviousColors || Math.Abs(avgG_previous - avgG) > minDifferencePreviousColors || Math.Abs(avgG_previous - avgG_previous) > minDifferencePreviousColors)
        {
          avgR_previous = avgR;
          avgG_previous = avgG;
          avgB_previous = avgB;

          //Send average colors to Bridge
          if (invalidColorValue == false)
          {
            ChangeColor(avgR, avgG, avgB, 200);
          }
        }
      }
    }

    private void CalculateVUMeterColorAndSendToHue(Bitmap vuMeterBitmap)
    {
      for (int i = 0; i < vuMeterBitmap.Height; i++)
      {
        if (vuMeterBitmap.GetPixel(0, i).R != 0 || vuMeterBitmap.GetPixel(0, i).G != 0 || vuMeterBitmap.GetPixel(0, i).B != 0)
        {
          ChangeColor(vuMeterBitmap.GetPixel(0, i).R, vuMeterBitmap.GetPixel(0, i).G, vuMeterBitmap.GetPixel(0, i).B, 200);
          return;
        }
        else if (vuMeterBitmap.GetPixel(vuMeterBitmap.Width - 1, i).R != 0 || vuMeterBitmap.GetPixel(vuMeterBitmap.Width - 1, i).G != 0 || vuMeterBitmap.GetPixel(vuMeterBitmap.Width - 1, i).B != 0)
        {
          ChangeColor(vuMeterBitmap.GetPixel(vuMeterBitmap.Width - 1, i).R, vuMeterBitmap.GetPixel(vuMeterBitmap.Width - 1, i).G, vuMeterBitmap.GetPixel(vuMeterBitmap.Width - 1, i).B, 200);
          return;
        }
      }
      ChangeColor(0, 0, 0,200);
    }
    #endregion

    #region powerstate monitoring
    private void monitorPowerState()
    {
      SystemEvents.PowerModeChanged += OnPowerModeChanged;
    }
    private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
      switch (e.Mode)
      {
        case PowerModes.Resume:
          // Close old socket and create new TCP client which allows it to reconnect when calling Connect()
          try
          {
            Socket.Close();
          }
          catch { };
          //Reconnect AtmoHue after standby
          Log.Debug("HueHandler - Reconnecting after standby");

          //reset Init so we restore the effect on resume
          isInit = true;
          Connected = false;
          Connect();
          break;
        case PowerModes.Suspend:
          break;
      }
    }
    #endregion

  }
  #region class Win32API
  public sealed class Win32API
  {
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
      public int left;
      public int top;
      public int right;
      public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PROCESSENTRY32
    {
      public uint dwSize;
      public uint cntUsage;
      public uint th32ProcessID;
      public IntPtr th32DefaultHeapID;
      public uint th32ModuleID;
      public uint cntThreads;
      public uint th32ParentProcessID;
      public int pcPriClassBase;
      public uint dwFlags;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
      public string szExeFile;
    }

    private const uint TH32CS_SNAPPROCESS = 0x00000002;

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, String lpWindowName);

    [DllImport("user32.dll")]
    public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

    [DllImport("user32.dll")]
    public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

    private const int WM_CLOSE = 0x10;
    private const int WM_DESTROY = 0x2;

    [DllImport("user32.dll")]
    public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern Int64 GetTickCount();

    [DllImport("kernel32.dll")]
    private static extern int Process32First(IntPtr hSnapshot,
                                     ref PROCESSENTRY32 lppe);

    [DllImport("kernel32.dll")]
    private static extern int Process32Next(IntPtr hSnapshot,
                                    ref PROCESSENTRY32 lppe);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags,
                                                   uint th32ProcessID);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hSnapshot);
    private const int WM_MouseMove = 0x0200;

    public static void RefreshTrayArea()
    {

      RECT rect;

      IntPtr systemTrayContainerHandle = FindWindow("Shell_TrayWnd", null);
      IntPtr systemTrayHandle = FindWindowEx(systemTrayContainerHandle, IntPtr.Zero, "TrayNotifyWnd", null);
      IntPtr sysPagerHandle = FindWindowEx(systemTrayHandle, IntPtr.Zero, "SysPager", null);
      IntPtr notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", null);
      GetClientRect(notificationAreaHandle, out rect);
      for (var x = 0; x < rect.right; x += 5)
        for (var y = 0; y < rect.bottom; y += 5)
          SendMessage(notificationAreaHandle, WM_MouseMove, 0, (y << 16) + x);
    }

    public static bool IsProcessRunning(string applicationName)
    {
      IntPtr handle = IntPtr.Zero;
      try
      {
        // Create snapshot of the processes
        handle = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
        PROCESSENTRY32 info = new PROCESSENTRY32();
        info.dwSize = (uint)System.Runtime.InteropServices.
                      Marshal.SizeOf(typeof(PROCESSENTRY32));

        // Get the first process
        int first = Process32First(handle, ref info);

        // While there's another process, retrieve it
        do
        {
          if (string.Compare(info.szExeFile,
                applicationName, true) == 0)
          {
            return true;
          }
        }
        while (Process32Next(handle, ref info) != 0);
      }
      catch
      {
        throw;
      }
      finally
      {
        // Release handle of the snapshot
        CloseHandle(handle);
        handle = IntPtr.Zero;
      }
      return false;
    }
  }
  #endregion
}
