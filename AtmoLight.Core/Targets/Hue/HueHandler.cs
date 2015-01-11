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
using System.Xml;
using Microsoft.Win32;

namespace AtmoLight.Targets
{
  public class HueHandler : ITargets
  {
    #region Fields

    public Target Name { get { return Target.Hue; } }
    public TargetType Type { get { return TargetType.Network; } }
    public bool AllowDelay { get { return false; } }
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

    // HUE
    private int hueDelayAtmoHue = 5000; 
    private int hueReconnectCounter = 0;
    private Boolean HueBridgeStartOnResume = false;
    private Thread changeColorThreadHelper;
    private volatile int[] changeColorBuffer = new int[5];

    // TCP
    private static TcpClient Socket = new TcpClient();
    private Stream Stream;

    // Color checks
    private int avgR_previousVU = 0;
    private int avgG_previousVU = 0;
    private int avgB_previousVU = 0;

    // Locks
    private bool isInit = false;
    private volatile bool initLock = false;
    private volatile bool changeColorThreadLock = true;
    private bool isAtmoHueRunning = false;
    private readonly object changeColorBufferLock = new object();

    private enum APIcommandType
    {
      Color,
      Group,
      Power,
      Room,
    }

    #endregion

    #region Hue
    public HueHandler()
    {
      coreObject = Core.GetInstance();
    }

    public void Initialise(bool force = false)
    {
      if (!initLock)
      {
        //Set Init lock
        initLock = true;
        isInit = true;
        hueReconnectCounter = 0;
        Thread t = new Thread(() => InitialiseThread(force));
        t.IsBackground = true;
        t.Start();
      }
      else
      {
        Log.Debug("HueHandler - Initialising locked.");
      }

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
          initLock = false;
          return false;
        }
      }
      else
      {
        isAtmoHueRunning = true;
        if (Socket.Connected)
        {
          Log.Debug("HueHandler - already connect to AtmoHue");
          initLock = false;
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
      if (coreObject.reInitOnError || force)
      {
        Initialise(force);
      }
    }

    public void Dispose()
    {
      if (Socket.Connected)
      {
        Disconnect();
      }
    }

    public bool StartHue()
    {
      Log.Debug("HueHandler - Trying to start AtmoHue.");
      if (!System.IO.File.Exists(coreObject.huePath))
      {
        Log.Error("HueHandler - AtmoHue.exe not found!");
        initLock = false;
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
        initLock = false;
        return false;
      }
      Log.Info("HueHander - AtmoHue successfully started.");
      return true;
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
      Thread t = new Thread(ConnectThread);
      t.IsBackground = true;
      t.Start();
    }
    private void ConnectThread()
    {

      while (hueReconnectCounter <= coreObject.hueReconnectAttempts)
      {
        if (!Socket.Connected)
        {
          //Close old socket and create new TCP client which allows it to reconnect when calling Connect()
          Disconnect();

          try
          {
            Socket = new TcpClient();

            Socket.SendTimeout = 5000;
            Socket.ReceiveTimeout = 5000;
            Socket.Connect(coreObject.hueIP, coreObject.huePort);
            Stream = Socket.GetStream();
            StartChangeColorThread();
            Log.Debug("HueHandler - Connected to AtmoHue");
          }
          catch (Exception e)
          {
            Log.Error("HueHandler - Error while connecting");
            Log.Error("HueHandler - Exception: {0}", e.Message);
          }

          //Increment times tried
          hueReconnectCounter++;

          //Show error if reconnect attempts exhausted
          if (hueReconnectCounter > coreObject.hueReconnectAttempts && !Socket.Connected)
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

      //Reset Init lock
      initLock = false;

      //Reset counter when we have finished
      hueReconnectCounter = 0;

      //Power ON bridge if connected and enabled
      if (HueBridgeStartOnResume)
      {
        //Reset start variable
        HueBridgeStartOnResume = false;

        if (Socket.Connected)
        {
          //Send Power ON command
          HueBridgePower("ON");

          //Sleep for 2s to allow for Hue Bridge startup
          Thread.Sleep(2000);
        }
      }
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
    private void Disconnect()
    {
      try
      {
        StopChangeColorThread();
        Socket.Close();
      }
      catch (Exception e)
      {
        Log.Error(string.Format("HueHandler - {0}", "Error during disconnect"));
        Log.Error(string.Format("HueHandler - {0}", e.Message));
      }
    }

    private void sendAPIcommand(string message)
    {
      try
      {
        ASCIIEncoding encoder = new ASCIIEncoding();
        byte[] buffer = encoder.GetBytes(message);

        Stream.Write(buffer, 0, buffer.Length);
        Stream.Flush();
      }
      catch (Exception e)
      {
        Log.Error("HueHandler - error during sending power command");
        Log.Error(string.Format("HueHandler - {0}", e.Message));
        ReInitialise(false);
      }
    }

    public void ChangeColor(int red, int green, int blue, int priority, int brightness)
    {
      lock (changeColorBufferLock)
      {
        changeColorBuffer[0] = red;
        changeColorBuffer[1] = green;
        changeColorBuffer[2] = blue;
        changeColorBuffer[3] = priority;
        changeColorBuffer[4] = brightness;
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
          ChangeColor(coreObject.staticColor[0], coreObject.staticColor[1], coreObject.staticColor[2], 10, 0);
          break;
        case ContentEffect.LEDsDisabled:
        case ContentEffect.Undefined:
        default:
          ChangeColor(0, 0, 0, 1, 0);
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
      try
      {
        if (coreObject.GetCurrentEffect() == ContentEffect.MediaPortalLiveMode || coreObject.GetCurrentEffect() == ContentEffect.GIFReader)
        {
          int[] overallAverageColor = new int[3];
          int[] averageColor = new int[3];
          int[] previousColor = new int[3];
          int pixelCount = 0;

          for (int y = 0; y < coreObject.GetCaptureHeight(); y++)
          {
            int row = coreObject.GetCaptureWidth() * y * 4;
            for (int x = 0; x < coreObject.GetCaptureWidth(); x++)
            {
              overallAverageColor[0] += pixeldata[row + x * 4 + 2];
              overallAverageColor[1] += pixeldata[row + x * 4 + 1];
              overallAverageColor[2] += pixeldata[row + x * 4];
              if (Math.Abs(pixeldata[row + x * 4 + 2] - pixeldata[row + x * 4 + 1]) > coreObject.hueMinDiversion || Math.Abs(pixeldata[row + x * 4 + 2] - pixeldata[row + x * 4]) > coreObject.hueMinDiversion || Math.Abs(pixeldata[row + x * 4 + 1] - pixeldata[row + x * 4]) > coreObject.hueMinDiversion)
              {
                averageColor[0] += pixeldata[row + x * 4 + 2];
                averageColor[1] += pixeldata[row + x * 4 + 1];
                averageColor[2] += pixeldata[row + x * 4];
                pixelCount++;
              }
            }
          }
          overallAverageColor[0] /= (coreObject.GetCaptureHeight() * coreObject.GetCaptureWidth());
          overallAverageColor[1] /= (coreObject.GetCaptureHeight() * coreObject.GetCaptureWidth());
          overallAverageColor[2] /= (coreObject.GetCaptureHeight() * coreObject.GetCaptureWidth());
          if (pixelCount > 0)
          {
            averageColor[0] /= pixelCount;
            averageColor[1] /= pixelCount;
            averageColor[2] /= pixelCount;

            if (Math.Abs(averageColor[0] - previousColor[0]) >= coreObject.hueThreshold || Math.Abs(averageColor[1] - previousColor[1]) >= coreObject.hueThreshold || Math.Abs(averageColor[2] - previousColor[2]) >= coreObject.hueThreshold)
            {
              double hue;
              double saturation;
              double lightness;
              double hueOverall;
              double saturationOverall;
              double lightnessOverall;

              // Save new color as previous color
              previousColor = averageColor;

              // Convert to hsl color model
              HSL.RGB2HSL(averageColor[0], averageColor[1], averageColor[2], out hue, out saturation, out lightness);
              HSL.RGB2HSL(overallAverageColor[0], overallAverageColor[1], overallAverageColor[2], out hueOverall, out saturationOverall, out lightnessOverall);

              // Convert back to rgb with adjusted saturation and lightness
              HSL.HSL2RGB(hue, Math.Min(saturation + coreObject.hueSaturation, 1), (coreObject.hueUseOverallLightness ? lightnessOverall : lightness), out averageColor[0], out averageColor[1], out averageColor[2]);

              // Send to lamp
              ChangeColor(averageColor[0], averageColor[1], averageColor[2], 200, (int)(Math.Min(lightnessOverall, 0.5) * 510));
            }
          }
          else
          {
            if (Math.Abs(overallAverageColor[0] - previousColor[0]) >= coreObject.hueThreshold || Math.Abs(overallAverageColor[1] - previousColor[1]) >= coreObject.hueThreshold || Math.Abs(overallAverageColor[2] - previousColor[2]) >= coreObject.hueThreshold)
            {
              // Save new color as previous color
              previousColor = overallAverageColor;

              if (overallAverageColor[0] <= coreObject.hueBlackThreshold && overallAverageColor[1] <= coreObject.hueBlackThreshold && overallAverageColor[2] <= coreObject.hueBlackThreshold)
              {
                ChangeColor(0, 0, 0, 200, 0);
              }
              else
              {
                double hueOverall;
                double saturationOverall;
                double lightnessOverall;
                HSL.RGB2HSL(overallAverageColor[0], overallAverageColor[1], overallAverageColor[2], out hueOverall, out saturationOverall, out lightnessOverall);
                // Adjust gamma level and send to lamp
                ChangeColor(overallAverageColor[0], overallAverageColor[1], overallAverageColor[2], 200, (int)(Math.Min(lightnessOverall, 0.5) * 510));
              }
            }
          }
        }
        // VUMeter
        else
        {
          unsafe
          {
            fixed (byte* ptr = pixeldata)
            {

              using (Bitmap image = new Bitmap(coreObject.GetCaptureWidth(), coreObject.GetCaptureHeight(), coreObject.GetCaptureWidth() * 4,
                          PixelFormat.Format32bppRgb, new IntPtr(ptr)))
              {
                CalculateVUMeterColorAndSendToHue(image);
              }
            }
          }
        }
      }
      catch (Exception e)
      {
        Log.Error(string.Format("HueHandler - {0}", "Error during average color calculations"));
        Log.Error(string.Format("HueHandler - {0}", e.Message));
      }
    }
    public void setActiveGroup(string groupName)
    {
      Log.Debug(APIcommandType.Group + " --> " + groupName);
      string message = string.Format("{0},{1},{2},{3}", "ATMOLIGHT", APIcommandType.Group, "OnlyActivate", groupName);
      sendAPIcommand(message);
    }
    public void setGroupStaticColor(string groupName, string colorName)
    {
      Log.Debug(APIcommandType.Group + " --> " + groupName);
      string message = string.Format("{0},{1},{2},{3},{4}", "ATMOLIGHT", APIcommandType.Group, "SetStaticColor", groupName, colorName);
      sendAPIcommand(message);
    }
    public List<string> Loadgroups()
    {
      List<string> groups = new List<string>();

      try
      {
        string settingsLocation = Path.GetDirectoryName(coreObject.huePath) + "\\settings.xml";
        if (File.Exists(settingsLocation))
        {
          using (XmlReader reader = XmlReader.Create(settingsLocation))
          {
            while (reader.Read())
            {
              // LED Locations

              if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "LedLocation"))
              {
                reader.ReadToDescendant("Location");
                groups.Add(reader.ReadString());
              }

            }
          }
        }
      }
      catch(Exception e)
      {
        Log.Error(string.Format("HueHandler - {0}", "Error during reading group config"));
        Log.Error(string.Format("HueHandler - {0}", e.Message));
      }
      return groups;
    }
    public List<string> LoadStaticColors()
    {
      List<string> staticColors = new List<string>();

      try
      {
        string settingsLocation = Path.GetDirectoryName(coreObject.huePath) + "\\settings.xml";
        if (File.Exists(settingsLocation))
        {
          using (XmlReader reader = XmlReader.Create(settingsLocation))
          {
            while (reader.Read())
            {
              // LED Locations

              if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "LedStaticColor"))
              {
                reader.ReadToDescendant("Name");
                staticColors.Add(reader.ReadString());
              }
            }
          }
        }
      }
      catch (Exception e)
      {
        Log.Error(string.Format("HueHandler - {0}", "Error during reading static color config"));
        Log.Error(string.Format("HueHandler - {0}", e.Message));
      }
      return staticColors;
    }

    private void CalculateVUMeterColorAndSendToHue(Bitmap vuMeterBitmap)
    {
      int minDifferencePreviousColors = coreObject.hueThreshold;

      for (int i = 0; i < vuMeterBitmap.Height; i++)
      {
        if (vuMeterBitmap.GetPixel(0, i).R != 0 || vuMeterBitmap.GetPixel(0, i).G != 0 || vuMeterBitmap.GetPixel(0, i).B != 0)
        {
          int red = vuMeterBitmap.GetPixel(0, i).R;
          int green = vuMeterBitmap.GetPixel(0, i).G;
          int blue = vuMeterBitmap.GetPixel(0, i).B;

          if (Math.Abs(avgR_previousVU - red) > minDifferencePreviousColors || Math.Abs(avgG_previousVU - green) > minDifferencePreviousColors || Math.Abs(avgB_previousVU - blue) > minDifferencePreviousColors)
          {
            avgR_previousVU = red;
            avgG_previousVU = green;
            avgB_previousVU = blue;
            ChangeColor(red, green, blue, 200, 0);
          }
          return;
        }
        else if (vuMeterBitmap.GetPixel(vuMeterBitmap.Width - 1, i).R != 0 || vuMeterBitmap.GetPixel(vuMeterBitmap.Width - 1, i).G != 0 || vuMeterBitmap.GetPixel(vuMeterBitmap.Width - 1, i).B != 0)
        {
          int red = vuMeterBitmap.GetPixel(vuMeterBitmap.Width - 1, i).R;
          int green = vuMeterBitmap.GetPixel(vuMeterBitmap.Width - 1, i).G;
          int blue = vuMeterBitmap.GetPixel(vuMeterBitmap.Width - 1, i).B;

          if (Math.Abs(avgR_previousVU - red) > minDifferencePreviousColors || Math.Abs(avgG_previousVU - green) > minDifferencePreviousColors || Math.Abs(avgB_previousVU - blue) > minDifferencePreviousColors)
          {
            avgR_previousVU = red;
            avgG_previousVU = green;
            avgB_previousVU = blue;
            ChangeColor(red, green, blue, 200, 0);
          }
          return;
        }
      }
      ChangeColor(0, 0, 0,200, 0);
    }

    private void HueBridgePower(string powerCommand)
    {
      string message = string.Format("{0},{1},{2}", "ATMOLIGHT", APIcommandType.Power, powerCommand);
      sendAPIcommand(message);
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

          //Reconnect to AtmoHue after standby
          Log.Debug("HueHandler - Initialising after standby");

          if (coreObject.hueBridgeEnableOnResume)
          {
            HueBridgeStartOnResume = true;
            Initialise();
          }
          else
          {
            Initialise();
          }
          break;
        case PowerModes.Suspend:
          if (coreObject.hueBridgeDisableOnSuspend)
          {
            //Send Power OFF command
            if (Socket.Connected)
            {
              HueBridgePower("OFF");
            }
          }
          break;
      }
    }
    #endregion

    #region Change Color Thread
    private void StartChangeColorThread()
    {
      changeColorThreadLock = false;
      changeColorThreadHelper = new Thread(() => ChangeColorThread());
      changeColorThreadHelper.Name = "AtmoLight Hue ChangeColor";
      changeColorThreadHelper.IsBackground = true;
      changeColorThreadHelper.Start();
    }

    private void StopChangeColorThread()
    {
      changeColorThreadLock = true;
    }

    private void ChangeColorThread()
    {
      int[] changeColorPrevColor = new int[5];
      while (!changeColorThreadLock && IsConnected())
      {
        lock (changeColorBufferLock)
        {
          try
          {
            if (changeColorBuffer != changeColorPrevColor)
            {
              changeColorPrevColor = changeColorBuffer;
              sendAPIcommand(string.Format("{0},{1},{2},{3},{4},{5},{6}", "ATMOLIGHT", APIcommandType.Color, changeColorBuffer[0].ToString(), changeColorBuffer[1].ToString(), changeColorBuffer[2].ToString(), changeColorBuffer[3].ToString(), changeColorBuffer[4].ToString()));
            }
          }
          catch (Exception e)
          {
            Log.Error("HueHandler - Error in ChangeColorThread");
            Log.Error(string.Format("HueHandler - Exception: {0}", e.Message));
            ReInitialise(false);
          }
        }
        System.Threading.Thread.Sleep(5);
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
