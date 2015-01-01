﻿using System;
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
using Microsoft.Win32;
using proto;
using AtmoLight.Targets;

namespace AtmoLight
{
  public enum ContentEffect
  {
     
    LEDsDisabled = 0,
    MediaPortalLiveMode,
    StaticColor,
    GIFReader,
    VUMeter,
    VUMeterRainbow,
    ExternalLiveMode,
    AtmoWinColorchanger,
    AtmoWinColorchangerLR,
    Undefined = -1
  }

  public enum Target
  {
    AmbiBox,
    AtmoWin,
    Boblight,
    Hue,
    Hyperion
  }

  public enum TargetType
  {
    Local,
    Network
  }

  public class Core
  {
    #region Fields
    // Core Instance
    private static Core instance = null;

    // Threads
    private Thread setPixelDataThreadHelper;
    private Thread gifReaderThreadHelper;
    private Thread vuMeterThreadHelper;

    // States
    private ContentEffect currentEffect = ContentEffect.Undefined; // Current active effect

    // Lists
    private List<ITargets> targets = new List<ITargets>();
    private List<byte[]> pixelDataList = new List<byte[]>(); // List for pixelData (Delay)
    private List<byte[]> bmiInfoHeaderList = new List<byte[]>(); // List for bmiInfoHeader (Delay)
    private List<long> delayTimingList = new List<long>(); // List for timings (Delay)

    // Locks
    private readonly object listLock = new object(); // Lock object for the above lists
    private readonly object targetsLock = new object(); // Lock object for the target list
    private volatile bool setPixelDataLock = true; // Lock for SetPixelData thread
    private volatile bool gifReaderLock = true;
    private volatile bool vuMeterLock = true;

    // VU Meter
    private int[] vuMeterThresholds = new int[] { -2, -5, -8, -10, -11, -12, -14, -18, -20, -22 };

    // Event Handler
    public delegate void NewConnectionLostHandler(Target target);
    public static event NewConnectionLostHandler OnNewConnectionLost;

    public delegate double[] NewVUMeterHander();
    public static event NewVUMeterHander OnNewVUMeter;

    // Stopwatches
    private Stopwatch blackbarStopwatch = new Stopwatch();

    // Generic Fields
    private int captureWidth = 64; // Default fallback capture width
    private int captureHeight = 48; // Default fallback capture height
    private bool delayEnabled = false;
    private int delayTime = 0;
    private string gifPath = "";
    private Rectangle blackbarDetectionRect;

    // General settings for targets
    public int[] staticColor = { 0, 0, 0 }; // RGB code for static color
    public bool reInitOnError;
    public bool blackbarDetection;
    public int blackbarDetectionTime;
    public int blackbarDetectionThreshold;
    public int powerModeChangedDelay;

    // AmbiBox Settings Fields
    public string ambiBoxIP;
    public int ambiBoxPort;
    public int ambiBoxMaxReconnectAttempts;
    public int ambiBoxReconnectDelay;
    public string ambiBoxMediaPortalProfile;
    public string ambiBoxExternalProfile;
    public string ambiBoxPath;
    public bool ambiBoxAutoStart;
    public bool ambiBoxAutoStop;

    // AtmoWin Settings Fields
    public bool atmoWinAutoStart;
    public bool atmoWinAutoStop;
    public string atmoWinPath;
    public int atmoWinConnectionDelay;

    // Boblight Settings Fields
    public string boblightIP;
    public int boblightPort;
    public int boblightMaxFPS;
    public int boblightMaxReconnectAttempts;
    public int boblightReconnectDelay;
    public int boblightSpeed;
    public int boblightAutospeed;
    public bool boblightInterpolation;
    public int boblightSaturation;
    public int boblightValue;
    public int boblightThreshold;
    public double boblightGamma;

    // Hyperion Settings Fields
    public string hyperionIP;
    public int hyperionPort;
    public int hyperionPriority;
    public int hyperionReconnectDelay;
    public int hyperionReconnectAttempts;
    public int hyperionPriorityStaticColor;
    public bool hyperionLiveReconnect;

    // Hue Settings Fields
    public string huePath;
    public bool hueStart;
    public bool hueIsRemoteMachine;
    public string hueIP;
    public int huePort;
    public int hueReconnectDelay;
    public int hueReconnectAttempts;
    public int hueMinimalColorDifference;
    public bool hueBridgeEnableOnResume;
    public bool hueBridgeDisableOnSuspend;

    #endregion

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

    #region Constructor/Deconstructor
    /// <summary>
    /// Core Constructor
    /// </summary>
    private Core()
    {
      var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
      DateTime buildDate = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTime;
      Log.Debug("Core Version {0}.{1}.{2}.{3}, build on {4} at {5}.", version.Major, version.Minor, version.Build, version.Revision, buildDate.ToShortDateString(), buildDate.ToLongTimeString());
      return;
    }

    /// <summary>
    /// Disposes of all targets
    /// </summary>
    public void Dispose()
    {
      foreach (var target in targets)
      {
        target.Dispose();
      }
    }
    #endregion

    #region Initialisation
    /// <summary>
    /// Generate all targets and initialise them.
    /// </summary>
    /// <returns></returns>
    public void Initialise()
    {
      foreach (var target in targets)
      {
        if (!target.IsConnected())
        {
          target.Initialise(false);
        }
      }
    }

    /// <summary>
    /// Reinitialise all targets that are not connected.
    /// </summary>
    public void ReInitialise()
    {
      foreach (var target in targets)
      {
        if (!target.IsConnected())
        {
          target.ReInitialise(true);
        }
      }
    }
    #endregion

    #region Configuration Methods (set)
    /// <summary>
    /// Set capture dimensions that should be used by everbody.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public bool SetCaptureDimensions(int width, int height)
    {
      if (width >= 0 && height >= 0)
      {
        captureWidth = width;
        captureHeight = height;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Add a target to be used.
    /// </summary>
    /// <param name="target"></param>
    public bool AddTarget(Target target)
    {
      // Dont allow the same target to be added more than once.
      lock (targetsLock)
      {
        foreach (var t in targets)
        {
          if (t.Name == target)
          {
            return false;
          }
        }
      }
      if (target == Target.AtmoWin)
      {
        targets.Add(new AtmoWinHandler());
      }
      else if (target == Target.Hue)
      {
        targets.Add(new HueHandler());
      }
      else if (target == Target.Hyperion)
      {
        targets.Add(new HyperionHandler());
      }
      else if (target == Target.AmbiBox)
      {
        targets.Add(new AmbiBoxHandler());
      }
      else if (target == Target.Boblight)
      {
        targets.Add(new BoblightHandler());
      }
      return true;
    }

    /// <summary>
    /// Removes a target.
    /// </summary>
    /// <param name="target"></param>
    public bool RemoveTarget(Target target)
    {
      lock (targetsLock)
      {
        foreach (var t in targets)
        {
          if (t.Name == target)
          {
            Log.Debug("Removing {0} as target.", target.ToString());
            t.Dispose();
            targets.Remove(t);
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    /// Set the effect that should be switched to after connection has be established.
    /// </summary>
    /// <param name="effect"></param>
    /// <returns></returns>
    public void SetInitialEffect(ContentEffect effect)
    {
      currentEffect = effect;
    }

    /// <summary>
    /// Define if the handlers should try to reinitialise when the connection is lost
    /// or and error occurs.
    /// </summary>
    /// <param name="reInit"></param>
    public void SetReInitOnError(bool reInit)
    {
      reInitOnError = reInit;
    }

    /// <summary>
    /// Set the path to the gif file that should be used by the GIFReader
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool SetGIFPath(string path)
    {
      if (path.Length > 4)
      {
        if (path.Substring(path.Length - 3, 3).ToLower() == "gif")
        {
          gifPath = path;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Changes the delay time.
    /// </summary>
    /// <param name="delay">Delay in ms.</param>
    /// <returns>true or false</returns>
    public bool SetDelay(int delay)
    {
      if (delay > 0)
      {
        Log.Debug("Changing delay to {0}ms.", delay);
        delayTime = delay;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Changes the static color.
    /// </summary>
    /// <param name="red">Red in RGB format.</param>
    /// <param name="green">Green  in RGB format.</param>
    /// <param name="blue">Blue  in RGB format.</param>
    /// <returns>true or false</returns>
    public bool SetStaticColor(int red, int green, int blue)
    {
      if ((red >= 0 && red <= 255) && (green >= 0 && green <= 255) && (blue >= 0 && blue <= 255))
      {
        staticColor[0] = red;
        staticColor[1] = green;
        staticColor[2] = blue;
        return true;
      }
      return false;
    }
    #endregion

    #region Information Methods (get)
    /// <summary>
    /// Returns the instance of the core.
    /// </summary>
    /// <returns></returns>
    public static Core GetInstance()
    {
      if (instance == null)
      {
        instance = new Core();
      }
      return instance;
    }

    /// <summary>
    /// Returns if there are targets that are connected.
    /// </summary>
    /// <returns></returns>
    public bool IsConnected()
    {
      lock (targetsLock)
      {
        foreach (var target in targets)
        {
          if (target.IsConnected())
          {
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    /// Returns if all targets are connected.
    /// </summary>
    /// <returns></returns>
    public bool AreAllConnected()
    {
      lock (targetsLock)
      {
        foreach (var target in targets)
        {
          if (!target.IsConnected())
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Returns if AtmoLight/LEDs are on.
    /// </summary>
    /// <returns>true or false</returns>
    public bool IsAtmoLightOn()
    {
      if (!IsConnected())
      {
        return false;
      }
      return GetCurrentEffect() == ContentEffect.LEDsDisabled || GetCurrentEffect() == ContentEffect.LEDsDisabled ? false : true;
    }

    /// <summary>
    /// Returns if the delay in enabled.
    /// </summary>
    /// <returns>true or false</returns>
    public bool IsDelayEnabled()
    {
      return delayEnabled;
    }

    /// <summary>
    /// Returns the delay time.
    /// </summary>
    /// <returns>Delay in ms.</returns>
    public int GetDelayTime()
    {
      return delayTime;
    }

    /// <summary>
    /// Returns the static color.
    /// </summary>
    /// <returns>Static Color as int array</returns>
    public int[] GetStaticColor()
    {
      return staticColor;
    }

    /// <summary>
    /// Returns the capture width
    /// </summary>
    /// <returns></returns>
    public int GetCaptureWidth()
    {
      return captureWidth;
    }

    /// <summary>
    /// Returns the capture height
    /// </summary>
    /// <returns></returns>
    public int GetCaptureHeight()
    {
      return captureHeight;
    }

    /// <summary>
    /// Returns the current effect.
    /// </summary>
    /// <returns>Current effect</returns>
    public ContentEffect GetCurrentEffect()
    {
      return currentEffect;
    }

    /// <summary>
    /// Returns the number of active targets.
    /// </summary>
    /// <returns></returns>
    public int GetTargetCount()
    {
      return targets.Count();
    }

    public List<ContentEffect> GetSupportedEffects()
    {
      List<ContentEffect> tempList = new List<ContentEffect>();
      lock (targetsLock)
      {
        foreach (var target in targets)
        {
          for (int i = 0; i < target.SupportedEffects.Count; i++)
          {
            if (!tempList.Contains(target.SupportedEffects[i]))
            {
              tempList.Add(target.SupportedEffects[i]);
            }
          }
        }
      }
      return tempList;
    }

    /// <summary>
    /// Returns if at least one target allows the use of a delay
    /// </summary>
    /// <returns></returns>
    public bool IsAllowDelayTargetPresent()
    {
      lock (targetsLock)
      {
        foreach (var target in targets)
        {
          if (target.AllowDelay)
          {
            return true;
          }
        }
      }
      return false;
    }
    public ITargets GetTarget(Target target)
    {
      lock (targetsLock)
      {
        foreach (var t in targets)
        {
          if (t.Name == target)
          {
            return t;
          }
        }
      }
      return null;
    }
    #endregion

    #region Events
    /// <summary>
    /// Method to allow the handlers to raise the NewConnectionLost event.
    /// </summary>
    /// <param name="target"></param>
    public void NewConnectionLost(Target target)
    {
      OnNewConnectionLost(target);
    }
    #endregion

    #region Delay Lists
    /// <summary>
    /// Add new Items to the delay lists.
    /// </summary>
    /// <param name="bmiInfoHeader">Info Header</param>
    /// <param name="pixelData">Pixel Data</param>
    private void AddDelayListItem(byte[] pixelData, byte[] bmiInfoHeader)
    {
      if (delayTimingList.Count <= 60)
      {
        lock (listLock)
        {
          delayTimingList.Add(Win32API.GetTickCount());
          pixelDataList.Add(pixelData);
          bmiInfoHeaderList.Add(bmiInfoHeader);
        }
      }
      else
      {
        Log.Error("Delay buffer overflow.");
      }
    }

    /// <summary>
    /// Clear all delay lists.
    /// </summary>
    private void ClearDelayLists()
    {
      Log.Debug("Clearing delay lists.");
      lock (listLock)
      {
        delayTimingList.Clear();
        pixelDataList.Clear();
        bmiInfoHeaderList.Clear();
      }
      Log.Debug("Delay lists cleared.");
    }

    /// <summary>
    /// Trim all delay lists.
    /// </summary>
    private void TrimDelayLists()
    {
      lock (listLock)
      {
        delayTimingList.TrimExcess();
        pixelDataList.TrimExcess();
        bmiInfoHeaderList.TrimExcess();
      }
    }

    /// <summary>
    /// Delete first entry in all delay lists.
    /// </summary>
    private void DeleteFirstDelayListsItems()
    {
      lock (listLock)
      {
        delayTimingList.RemoveAt(0);
        pixelDataList.RemoveAt(0);
        bmiInfoHeaderList.RemoveAt(0);
      }
    }
    #endregion

    #region Utilities
    /// <summary>
    /// Calculates the needed information from a bitmap stream and sends them to SendPixelData().
    /// </summary>
    /// <param name="stream"></param>
    public void CalculateBitmap(Stream stream)
    {
      // Debug file output
      // new Bitmap(stream).Save("C:\\ProgramData\\Team MediaPortal\\MediaPortal\\" + Win32API.GetTickCount() + ".bmp");
      if (blackbarDetection && currentEffect == ContentEffect.MediaPortalLiveMode)
      {
        stream = BlackbarDetection(stream);
      }
      // Debug file output after blackbar detection
      // new Bitmap(stream).Save("C:\\ProgramData\\Team MediaPortal\\MediaPortal\\" + Win32API.GetTickCount() + ".bmp");

      BinaryReader reader = new BinaryReader(stream);
      stream.Position = 0; // ensure that what start at the beginning of the stream. 
      reader.ReadBytes(14); // skip bitmap file info header
      byte[] bmiInfoHeader = reader.ReadBytes(4 + 4 + 4 + 2 + 2 + 4 + 4 + 4 + 4 + 4 + 4);

      int rgbL = (int)(stream.Length - stream.Position);
      int rgb = (int)(rgbL / (GetCaptureWidth() * GetCaptureHeight()));

      byte[] pixelData = reader.ReadBytes((int)(stream.Length - stream.Position));

      byte[] h1pixelData = new byte[GetCaptureWidth() * rgb];
      byte[] h2pixelData = new byte[GetCaptureWidth() * rgb];

      // We need to flip the image horizontally.
      // Because after reading the bytes into the bytearray with BinaryReader the image is upside down (bmp characteristic).
      int i;
      for (i = 0; i < ((GetCaptureHeight() / 2) - 1); i++)
      {
        Array.Copy(pixelData, i * GetCaptureWidth() * rgb, h1pixelData, 0, GetCaptureWidth() * rgb);
        Array.Copy(pixelData, (GetCaptureHeight() - i - 1) * GetCaptureWidth() * rgb, h2pixelData, 0, GetCaptureWidth() * rgb);
        Array.Copy(h1pixelData, 0, pixelData, (GetCaptureHeight() - i - 1) * GetCaptureWidth() * rgb, GetCaptureWidth() * rgb);
        Array.Copy(h2pixelData, 0, pixelData, i * GetCaptureWidth() * rgb, GetCaptureWidth() * rgb);
      }

      SendPixelData(pixelData, bmiInfoHeader);
    }

    /// <summary>
    /// Sends picture information either to the delay thread or directly to the targets.
    /// </summary>
    /// <param name="pixelData"></param>
    /// <param name="bmiInfoHeader"></param>
    /// <param name="force"></param>
    private void SendPixelData(byte[] pixelData, byte[] bmiInfoHeader, bool force = false)
    {
      if (IsDelayEnabled() && !force && GetCurrentEffect() == ContentEffect.MediaPortalLiveMode && IsAllowDelayTargetPresent())
      {
        AddDelayListItem(pixelData, bmiInfoHeader);
        lock (targetsLock)
        {
          foreach (var target in targets)
          {
            if (!target.AllowDelay && target.IsConnected())
            {
              target.ChangeImage(pixelData, bmiInfoHeader);
            }
          }
        }
      }
      else
      {
        lock (targetsLock)
        {
          foreach (var target in targets)
          {
            if (target.IsConnected() && (target.AllowDelay || !force || !IsDelayEnabled() || GetCurrentEffect() != ContentEffect.MediaPortalLiveMode))
            {
              target.ChangeImage(pixelData, bmiInfoHeader);
            }
          }
        }
      }
    }

    private Stream BlackbarDetection(Stream stream)
    {
      if (!blackbarStopwatch.IsRunning)
      {
        blackbarStopwatch.Start();
      }
      if (blackbarStopwatch.ElapsedMilliseconds >= blackbarDetectionTime)
      {
        Bitmap blackBarBitmap = new Bitmap(stream);
        Color colorTemp;
        int yTopBound = -1;
        int yBottomBound = -1;
        int xLeftBound = -1;
        int xRightBound = -1;

        // Horizontal Scan
        for (int y = 0; y < (int)(blackBarBitmap.Height / 3); y++)
        {
          if (yTopBound != -1 && yBottomBound != -1)
          {
            break;
          }
          for (int x = (int)(blackBarBitmap.Width * 0.33); x < (int)(blackBarBitmap.Width * 0.66); x++)
          {
            if (yTopBound != -1 && yBottomBound != -1)
            {
              break;
            }

            if (yTopBound == -1)
            {
              colorTemp = blackBarBitmap.GetPixel(x, y);
              if (colorTemp.R > blackbarDetectionThreshold || colorTemp.G > blackbarDetectionThreshold || colorTemp.B > blackbarDetectionThreshold)
              {
                yTopBound = y;
              }
            }

            if (yBottomBound == -1)
            {
              colorTemp = blackBarBitmap.GetPixel(x, blackBarBitmap.Height - 1 - y);
              if (colorTemp.R > blackbarDetectionThreshold || colorTemp.G > blackbarDetectionThreshold || colorTemp.B > blackbarDetectionThreshold)
              {
                yBottomBound = blackBarBitmap.Height - y;
              }
            }
          }
        }

        // Vertical Scan
        for (int x = 0; x < (int)(blackBarBitmap.Width / 3); x++)
        {
          if (xLeftBound != -1 && xRightBound != -1)
          {
            break;
          }
          for (int y = (int)(blackBarBitmap.Height * 0.33); y < (int)(blackBarBitmap.Height * 0.66); y++)
          {
            if (xLeftBound != -1 && xRightBound != -1)
            {
              break;
            }

            if (xLeftBound == -1)
            {
              colorTemp = blackBarBitmap.GetPixel(x, y);
              if (colorTemp.R > blackbarDetectionThreshold || colorTemp.G > blackbarDetectionThreshold || colorTemp.B > blackbarDetectionThreshold)
              {
                xLeftBound = x;
              }
            }

            if (xRightBound == -1)
            {
              colorTemp = blackBarBitmap.GetPixel(blackBarBitmap.Width - 1 - x, y);
              if (colorTemp.R > blackbarDetectionThreshold || colorTemp.G > blackbarDetectionThreshold || colorTemp.B > blackbarDetectionThreshold)
              {
                xRightBound = blackBarBitmap.Width - x;
              }
            }
          }
        }
        if (yTopBound != -1 && yBottomBound != -1 && xLeftBound != -1 && xRightBound != -1)
        {
          blackbarDetectionRect = new Rectangle(xLeftBound, yTopBound, xRightBound - xLeftBound, yBottomBound - yTopBound);
        }
        blackBarBitmap.Dispose();
        blackbarStopwatch.Restart();
      }

      if (blackbarDetectionRect != new Rectangle(0, 0, GetCaptureWidth(), GetCaptureHeight()) && blackbarDetectionRect != new Rectangle(0, 0, 0, 0))
      {
        Bitmap blackBarBitmapOutput = new Bitmap(GetCaptureWidth(), GetCaptureHeight());

        using (Graphics g = Graphics.FromImage(blackBarBitmapOutput))
        {
          // Cropping and resizing the original bitmap
          g.DrawImage(new Bitmap(stream), new Rectangle(0, 0, GetCaptureWidth(), GetCaptureHeight()), blackbarDetectionRect, GraphicsUnit.Pixel);
        }

        // Saving cropped and resized bitmap to stream
        blackBarBitmapOutput.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
        blackBarBitmapOutput.Dispose();
      }
      return stream;
    }
    #endregion

    #region Control Methods
    /// <summary>
    /// Change effect.
    /// </summary>
    /// <param name="effect">Effect to change to</param>
    /// <param name="force">Force the effect change</param>
    /// <returns></returns>
    public bool ChangeEffect(ContentEffect effect, bool force = false)
    {
      if (!IsConnected())
      {
        return false;
      }
      // Static color gets excluded so we can actually change it.
      if ((effect == currentEffect) && (!force))
      {
        Log.Debug("Effect \"{0}\" is already active. Nothing to do.", effect);
        return false;
      }
      currentEffect = effect;
      Log.Info("Changing AtmoLight effect to: {0}", effect.ToString());
      StopAllThreads();

      lock (targetsLock)
      {
        foreach (var target in targets)
        {
          if (target.IsConnected())
          {
            target.ChangeEffect(effect);
          }
        }
      }

      if (effect == ContentEffect.MediaPortalLiveMode)
      {
        blackbarDetectionRect = new Rectangle(0, 0, GetCaptureWidth(), GetCaptureHeight());
        if (delayEnabled)
        {
          Log.Debug("Adding {0}ms delay to the LEDs.", delayTime);
          StartSetPixelDataThread();
        }
      }
      else if (effect == ContentEffect.GIFReader)
      {
        StartGIFReaderThread();
      }
      else if (effect == ContentEffect.VUMeter || effect == ContentEffect.VUMeterRainbow)
      {
        StartVUMeterThread();
      }
      return true;
    }

    /// <summary>
    /// Change profile.
    /// </summary>
    /// <returns>true or false</returns>
    public void ChangeProfile()
    {
      lock (targetsLock)
      {
        foreach (var target in targets)
        {
          if (target.IsConnected())
          {
            target.ChangeProfile();
          }
        }
      }
    }

    /// <summary>
    /// Enables the delay.
    /// </summary>
    /// <param name="delay">Delay in ms.</param>
    /// <returns>true or false</returns>
    public bool EnableDelay(int delay = -1)
    {
      if (delay > 0)
      {
        delayTime = delay;
      }
      Log.Info("Adding {0}ms delay to LEDs.", delayTime);
      delayEnabled = true;
      StartSetPixelDataThread();
      return false;
    }

    /// <summary>
    /// Disables the delay.
    /// </summary>
    public void DisableDelay()
    {
      Log.Info("Removing delay.");
      delayEnabled = false;
      StopSetPixelDataThread();
    }

    public void PowerModeChanged(PowerModes powerMode)
    {
      if (powerMode == PowerModes.Resume)
      {
        System.Threading.Thread.Sleep(powerModeChangedDelay);
      }
      lock (targetsLock)
      {
        foreach (var target in targets)
        {
          target.PowerModeChanged(powerMode);
        }
      }
    }
    #endregion

    #region Threads
    /// <summary>
    /// Start the SetPixelData thread.
    /// </summary>
    private void StartSetPixelDataThread()
    {
      setPixelDataLock = false;
      setPixelDataThreadHelper = new Thread(() => SetPixelDataThread());
      setPixelDataThreadHelper.Name = "AtmoLight SetPixelData";
      setPixelDataThreadHelper.IsBackground = true;
      setPixelDataThreadHelper.Start();
    }

    /// <summary>
    /// Stop the SetPixelData thread.
    /// </summary>
    private void StopSetPixelDataThread()
    {
      setPixelDataLock = true;
    }

    /// <summary>
    /// Start the GIFReader thread.
    /// </summary>
    private void StartGIFReaderThread()
    {
      gifReaderLock = false;
      gifReaderThreadHelper = new Thread(() => GIFReaderThread());
      gifReaderThreadHelper.Name = "AtmoLight GIFReader";
      gifReaderThreadHelper.IsBackground = true;
      gifReaderThreadHelper.Start();
    }

    /// <summary>
    /// Stop the GIFReader thread.
    /// </summary>
    private void StopGIFReaderThread()
    {
      gifReaderLock = true;
    }

    /// <summary>
    /// Start the VUMeter thread.
    /// </summary>
    private void StartVUMeterThread()
    {
      vuMeterLock = false;
      vuMeterThreadHelper = new Thread(() => VUMeterThread());
      vuMeterThreadHelper.Name = "AtmoLight VUMeter";
      vuMeterThreadHelper.IsBackground = true;
      vuMeterThreadHelper.Start();
    }

    /// <summary>
    /// Stop the VUMeter thread.
    /// </summary>
    private void StopVUMeterThread()
    {
      vuMeterLock = true;
    }

    /// <summary>
    /// Stop all core threads.
    /// </summary>
    private void StopAllThreads()
    {
      StopSetPixelDataThread();
      StopGIFReaderThread();
      StopVUMeterThread();
    }

    /// <summary>
    /// Send pixel data to targets when MediaPortal liveview is used.
    /// Also add a delay specified in settings.
    /// This method is designed to run as its own thread.
    /// </summary>
    private void SetPixelDataThread()
    {
      if (!IsConnected())
      {
        return;
      }
      try
      {
        Log.Debug("Starting delay thread.");
        while (!setPixelDataLock && IsConnected())
        {
          if (delayTimingList.Count >= 1)
          {
            if (Win32API.GetTickCount() >= (delayTimingList[0] + delayTime))
            {
              SendPixelData(pixelDataList[0], bmiInfoHeaderList[0], true);
              DeleteFirstDelayListsItems();

              // Trim the lists, to prevent a memory leak.
              TrimDelayLists();
            }
          }
          // Sleep 5ms to reduce cpu load.
          System.Threading.Thread.Sleep(5);
        }
        ClearDelayLists();
      }
      catch (Exception ex)
      {
        Log.Error("Could not send pixeldata to targets.");
        Log.Error("Exception: {0}", ex.Message);

        ClearDelayLists();
      }
    }

    /// <summary>
    /// Decode the gif file, transform it and send it to CalculateBitmap().
    /// </summary>
    private void GIFReaderThread()
    {
      try
      {
        // Get gif as stream
        Stream gifSource = new FileStream(gifPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        
        // Decode gif
        GifBitmapDecoder gifDecoder = new GifBitmapDecoder(gifSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
        BitmapMetadata gifDecoderMetadata = (BitmapMetadata)gifDecoder.Metadata;
        int gifWidth = Convert.ToInt32(gifDecoderMetadata.GetQuery("/logscrdesc/Width"));
        int gifHeight = Convert.ToInt32(gifDecoderMetadata.GetQuery("/logscrdesc/Height"));
        int gifBackgroundColor = Convert.ToInt32(gifDecoderMetadata.GetQuery("/logscrdesc/BackgroundColorIndex"));
        while (!gifReaderLock)
        {
          for (int index = 0; index < gifDecoder.Frames.Count; index++)
          {
            if (gifReaderLock)
            {
              break;
            }
            // Select frame
            BitmapSource gifBitmapSource = gifDecoder.Frames[index];
            gifBitmapSource.Freeze();
            BitmapMetadata gifBitmapMetadata = (BitmapMetadata)gifBitmapSource.Metadata;
            int gifDelay = Convert.ToInt32(gifBitmapMetadata.GetQuery("/grctlext/Delay")) * 10;
            int gifOffsetLeft = Convert.ToInt32(gifBitmapMetadata.GetQuery("/imgdesc/Left"));
            int gifOffsetTop = Convert.ToInt32(gifBitmapMetadata.GetQuery("/imgdesc/Top"));

            if (gifDelay == 0)
            {
              gifDelay = 20;
            }

            Bitmap gifBitmap;
            // Convert frame to Bitmap
            using (MemoryStream outStream = new MemoryStream())
            {
              BitmapEncoder gifEncoder = new BmpBitmapEncoder();
              gifEncoder.Frames.Add(BitmapFrame.Create(gifBitmapSource));
              gifEncoder.Save(outStream);
              gifBitmap = new Bitmap(outStream);
            }
            // Correct position of this frame, as gifs dont have to have fixed dimensions and positions
            if (gifBitmap.Width != gifWidth || gifBitmap.Height != gifHeight || gifOffsetLeft > 0 || gifOffsetTop > 0)
            {
              using (Bitmap gifBitmapOffset = new Bitmap(gifWidth, gifHeight))
              {
                using (Graphics gifBitmapOffsetGFX = Graphics.FromImage(gifBitmapOffset))
                {
                  // Fill Bitmap with background color
                  gifBitmapOffsetGFX.FillRectangle(new SolidBrush(Color.FromArgb(gifDecoder.Palette.Colors[gifBackgroundColor].A, gifDecoder.Palette.Colors[gifBackgroundColor].R, gifDecoder.Palette.Colors[gifBackgroundColor].G, gifDecoder.Palette.Colors[gifBackgroundColor].B)), 0, 0, gifWidth, gifHeight);
                  // Draw in original picture
                  gifBitmapOffsetGFX.DrawImage(gifBitmap, gifOffsetLeft, gifOffsetTop);
                  // Copy Bitmap
                  gifBitmap = gifBitmapOffset.Clone(new Rectangle(0, 0, gifWidth, gifHeight), PixelFormat.Undefined);
                }
              }
            }
            
            // Resize Bitmap
            gifBitmap = new Bitmap(gifBitmap, new Size(GetCaptureWidth(), GetCaptureHeight()));

            // Convert Bitmap to stream
            MemoryStream gifStream = new MemoryStream();
            gifBitmap.Save(gifStream, ImageFormat.Bmp);

            // Calculations to prepare data and then send data
            CalculateBitmap(gifStream);

            // Cleanup
            gifStream.Close();
            gifStream.Dispose();
            gifBitmap.Dispose();

            // Sleep until next frame
            System.Threading.Thread.Sleep(gifDelay);
          }
        }
        gifSource.Close();
        gifSource.Dispose();
      }
      catch (Exception ex)
      {
        Log.Error("Error in GIFReaderThread.");
        Log.Error("Exception: {0}", ex.Message);
      }
    }

    /// <summary>
    /// Receives dblevel data from MediaPortal and generates bitmaps out of that.
    /// Then sends these bitmaps to CalculateBitmap().
    /// </summary>
    private void VUMeterThread()
    {
      List<SolidBrush> vuMeterBrushes = new List<SolidBrush>();
      try
      {
        if (currentEffect == ContentEffect.VUMeterRainbow)
        {
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 0, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(255, 0, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(255, 77, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(255, 128, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(255, 204, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(230, 255, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(102, 255, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 255, 153)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 230, 255)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 128, 255)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 0, 255)));
        }
        else
        {
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 0, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(255, 0, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(255, 0, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(255, 128, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(255, 255, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 255, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 255, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 255, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 255, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 255, 0)));
          vuMeterBrushes.Add(new SolidBrush(Color.FromArgb(0, 255, 0)));
        }

        Rectangle rectFull = new Rectangle(0, 0, GetCaptureWidth(), GetCaptureHeight());

        Bitmap vuMeterBitmap = new Bitmap(GetCaptureWidth(), GetCaptureHeight());
        Graphics vuMeterGFX = Graphics.FromImage(vuMeterBitmap);

        double[] dbLevel = new double[] { 0.0, 0.0 };

        while (!vuMeterLock)
        {
          vuMeterGFX.FillRectangle(vuMeterBrushes[0], rectFull);
          dbLevel = OnNewVUMeter();

          for (int channel = 0; channel <= 1; channel++)
          {
            for (int index = 0; index < vuMeterThresholds.Length; index++)
            {
              if (dbLevel[channel] >= vuMeterThresholds[index])
              {
                vuMeterGFX.FillRectangle(vuMeterBrushes[index + 1], (int)((double)channel * (double)vuMeterBitmap.Width / (double)4 * (double)3), (int)((double)index * (double)vuMeterBitmap.Height / (double)10), (int)((double)vuMeterBitmap.Width / (double)4), (int)(((double)vuMeterBitmap.Height / (double)10) + (double)1));
              }
            }
          }

          MemoryStream vuMeterStream = new MemoryStream();
          vuMeterBitmap.Save(vuMeterStream, ImageFormat.Bmp);
          CalculateBitmap(vuMeterStream);
          vuMeterStream.Close();
          vuMeterStream.Dispose();

          System.Threading.Thread.Sleep(50);
        }
        vuMeterBitmap.Dispose();
        vuMeterGFX.Dispose();
        vuMeterBrushes.Clear();
      }
      catch (Exception ex)
      {
        Log.Error("Error in VUMeterThread.");
        Log.Error("Exception: {0}", ex.Message);
      }
    }
    #endregion
  }
}
