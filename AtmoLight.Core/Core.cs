using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using AtmoWinRemoteControl;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;

namespace AtmoLight
{
  public enum ContentEffect
  {
    LEDsDisabled = 0,
    AtmoWinLiveMode,
    Colorchanger,
    ColorchangerLR,
    MediaPortalLiveMode,
    StaticColor,
    GIFReader,
    VUMeter,
    VUMeterRainbow,
    Undefined = -1
  }
  public class Core
  {

    #region Fields

    private string pathAtmoWin = "";
    private bool delayEnabled = false;
    private int delayTime = 0;    
    private bool reinitialiseOnError = true;
    private bool startAtmoWin = true;
    private int[] staticColor = { 0, 0, 0 }; // RGB code for static color
    private string gifPath = "";

    private Thread setPixelDataThreadHelper;
    private Thread getAtmoLiveViewSourceThreadHelper;
    private Thread reinitialiseThreadHelper;
    private Thread gifReaderThreadHelper;
    private Thread vuMeterThreadHelper;

    // Com  Objects
    private IAtmoRemoteControl2 atmoRemoteControl = null; // Com Object to control AtmoWin
    private IAtmoLiveViewControl atmoLiveViewControl = null; // Com Object to control AtmoWins liveview

    // States
    private bool currentState = false; // State of the LEDs
    private ContentEffect currentEffect = ContentEffect.Undefined; // Current aktive effect
    private ContentEffect changeEffect = ContentEffect.Undefined; // Effect ChangeEffect() should change to (need for Reinitialise() if ChangeEffect() fails)
    private ComLiveViewSource atmoLiveViewSource; // Current liveview source

    // Timings
    private const int timeoutComInterface = 5000; // Timeout for the COM interface
    private const int delaySetStaticColor = 20; // SEDU workaround delay time
    private const int delayAtmoWinConnect = 1000; // Delay between starting AtmoWin and connection to it
    private const int delayGetAtmoLiveViewSource = 1000; // Delay between liveview source checks

    // Lists
    private List<byte[]> pixelDataList = new List<byte[]>(); // List for pixelData (Delay)
    private List<byte[]> bmiInfoHeaderList = new List<byte[]>(); // List for bmiInfoHeader (Delay)
    private List<long> delayTimingList = new List<long>(); // List for timings (Delay)

    // Locks
    private readonly object listLock = new object(); // Lock object for the above lists
    private volatile bool getAtmoLiveViewSourceLock = true; // Lock for liveview source checks
    private volatile bool setPixelDataLock = true; // Lock for SetPixelData thread
    private volatile bool reinitialiseLock = false;
    private volatile bool gifReaderLock = true;
    private volatile bool vuMeterLock = true;

    // VU Meter
    private int[] vuMeterThresholds = new int[] { -2, -5, -8, -10, -11, -12, -14, -18, -20, -22 };
    private bool vuMeterRainbowColorScheme = false;
    private List<SolidBrush> vuMeterBrushes = new List<SolidBrush>();

    private int captureWidth = 0; // AtmoWins capture width
    private int captureHeight = 0; // AtmoWins capture height

    public delegate void NewConnectionLostHandler();
    public static event NewConnectionLostHandler OnNewConnectionLost;

    public delegate double[] NewVUMeterHander();
    public static event NewVUMeterHander OnNewVUMeter;

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

    #region Constructor
    public Core(string pathAtmoWin, bool reinitialiseOnError, bool startAtmoWin, int[] staticColor, bool delayEnabled, int delayTime)
    {
      var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
      DateTime buildDate = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTime;
      Log.Debug("Core Version {0}.{1}.{2}.{3}, build on {4} at {5}.", version.Major, version.Minor, version.Build, version.Revision, buildDate.ToShortDateString(), buildDate.ToLongTimeString());

      this.pathAtmoWin = pathAtmoWin;
      this.reinitialiseOnError = reinitialiseOnError;
      this.startAtmoWin = startAtmoWin;
      this.staticColor = staticColor;
      this.delayEnabled = delayEnabled;
      this.delayTime = delayTime;
    }
    #endregion

    #region Connect
    /// <summary>
    /// Connect to AtmoWin.
    /// </summary>
    /// <returns>true or false</returns>
    public bool Connect()
    {
      Log.Debug("Trying to connect to AtmoWin.");
      if (!GetAtmoRemoteControl()) return false;
      if (!SetAtmoEffect(ComEffectMode.cemLivePicture, true)) return false;
      if (!GetAtmoLiveViewControl()) return false;
      if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal)) return false;
      if (!GetAtmoLiveViewRes()) return false;

      Log.Debug("Successfully connected to AtmoWin.");
      return true;
    }

    /// <summary>
    /// Disconnect from AtmoWin.
    /// </summary>
    /// <returns>true or false</returns>
    public bool Disconnect()
    {
      Log.Debug("Disconnecting from AtmoWin.");

      StopAllThreads();

      if (atmoRemoteControl != null)
      {
        Marshal.ReleaseComObject(atmoRemoteControl);
        atmoRemoteControl = null;
      }
      if (atmoLiveViewControl != null)
      {
        Marshal.ReleaseComObject(atmoLiveViewControl);
        atmoLiveViewControl = null;
      }
      return true;
    }

    /// <summary>
    /// Reconnect to AtmoWin.
    /// </summary>
    /// <returns>true or false</returns>
    public bool Reconnect()
    {
      Log.Debug("Trying to reconnect to AtmoWin.");
      Disconnect();
      Connect();
      return true;
    }

    /// <summary>
    /// Return if a connection to AtmoWin is established.
    /// </summary>
    /// <returns>true or false</returns>
    public bool IsConnected()
    {
      if (atmoRemoteControl == null || atmoLiveViewControl == null)
      {
        return false;
      }
      return true;
    }
    #endregion

    #region Initialise
    /// <summary>
    /// Start AtmoWin and connects to it.
    /// </summary>
    /// <returns>true or false</returns>
    public bool Initialise(bool force = false)
    {
      Log.Debug("Initialising.");
      if (!Win32API.IsProcessRunning("atmowina.exe"))
      {
        if (startAtmoWin || force)
        {
          if (!StartAtmoWin()) return false;
          System.Threading.Thread.Sleep(delayAtmoWinConnect);
        }
        else
        {
          Log.Error("AtmoWin is not running.");
          return false;
        }
      }

      if (!Connect()) return false;

      Log.Debug("Initialising successfull.");
      return true;
    }

    /// <summary>
    /// Restart AtmoWin and reconnects to it.
    /// </summary>
    /// <param name="force">Force the reinitialising and discard user settings.</param>
    public void Reinitialise(bool force = false)
    {
      if (reinitialiseLock)
      {
        Log.Debug("Reinitialising locked.");
        return;
      }
      if (!reinitialiseOnError && !force)
      {
        Disconnect();
        OnNewConnectionLost();
        return;
      }

      reinitialiseLock = true;
      Log.Debug("Reinitialising.");

      if (!Disconnect() || !StopAtmoWin() || !Initialise(force) || !ChangeEffect(changeEffect != ContentEffect.Undefined ? changeEffect : currentEffect, true))
      {
        Disconnect();
        StopAtmoWin();
        Log.Error("Reinitialising failed.");
        reinitialiseLock = false;
        OnNewConnectionLost();
        return;
      }

      Log.Debug("Reinitialising successfull.");
      reinitialiseLock = false;
      return;
    }

    /// <summary>
    /// Start reinitialising in a new thread.
    /// </summary>
    /// <param name="force">Force the reinitialising and discard user settings.</param>
    public void ReinitialiseThreaded(bool force = false)
    {
      if (!reinitialiseLock)
      {
        reinitialiseThreadHelper = new Thread(() => Reinitialise(force));
        reinitialiseThreadHelper.Name = "AtmoLight Reinitialise";
        reinitialiseThreadHelper.Start();
      }
      else
      {
        Log.Debug("Reinitialising Thread already running.");
      }
    }

    #endregion

    #region AtmoWin
    /// <summary>
    /// Start AtmoWin.
    /// </summary>
    /// <returns>true or false</returns>
    public bool StartAtmoWin()
    {
      Log.Debug("Trying to start AtmoWin.");
      if (!System.IO.File.Exists(pathAtmoWin))
      {
        return false;
      }
      Process AtmoWinA = new Process();
      AtmoWinA.StartInfo.FileName = pathAtmoWin;
      AtmoWinA.StartInfo.UseShellExecute = true;
      AtmoWinA.StartInfo.Verb = "open";
      try
      {
        AtmoWinA.Start();
      }
      catch (Exception)
      {
        Log.Error("Starting AtmoWin failed.");
        return false;
      }
      Log.Info("AtmoWin successfully started.");
      return true;
    }

    /// <summary>
    /// Stop AtmoWin.
    /// </summary>
    /// <returns>true or false</returns>
    public bool StopAtmoWin()
    {
      Log.Info("Trying to stop AtmoWin.");
      foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("atmowina")))
      {
        try
        {
          process.Kill();
          // Wait if the kill succeeded, because sometimes it does not.
          // If it does not, we stop the whole initialization.
          if (!TimeoutHandler(() => process.WaitForExit()))
          {
            Log.Error("Stopping AtmoWin failed.");
            return false;
          }
          Win32API.RefreshTrayArea();
        }
        catch (Exception ex)
        {
          Log.Error("Stopping AtmoWin failed.");
          Log.Error("Exception: {0}", ex.Message);
          return false;
        }
      }
      Log.Info("AtmoWin successfully stopped.");
      return true;
    }

    /// <summary>
    /// Restart AtmoWin.
    /// </summary>
    public void RestartAtmoWin()
    {
      Log.Debug("Trying to restart AtmoWin.");
      StopAtmoWin();
      StartAtmoWin();
    }

    public void ChangeAtmoWinRestartOnError(bool restartOnError)
    {
      reinitialiseOnError = restartOnError;
    }
    #endregion

    #region Delay Lists
    /// <summary>
    /// Add new Items to the delay lists.
    /// </summary>
    /// <param name="bmiInfoHeader">Info Header</param>
    /// <param name="pixelData">Pixel Data</param>
    private void AddDelayListItem(byte[] bmiInfoHeader, byte[] pixelData)
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
        ReinitialiseThreaded();
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
    /// Check if a method times out and starts to reinitialise AtmoWin if needed.
    /// </summary>
    /// <param name="method">Method that needs checking for a timeout.</param>
    /// <param name="timeout">Timeout in ms.</param>
    /// <returns>true if not timed out and false if timed out.</returns>
    private bool TimeoutHandler(System.Action method, int timeout = timeoutComInterface)
    {
      try
      {
#if DEBUG
        method();
        return true;
#else
        long timeoutStart = Win32API.GetTickCount();
        var tokenSource = new CancellationTokenSource();
        CancellationToken token = tokenSource.Token;
        var task = Task.Factory.StartNew(() => method(), token);

        if (!task.Wait(timeout, token))
        {
          // Stacktrace is needed so we can output the name of the method that timed out.
          StackTrace trace = new StackTrace();
          Log.Error("{0} timed out after {1}ms!", trace.GetFrame(1).GetMethod().Name, Win32API.GetTickCount() - timeoutStart);
          ReinitialiseThreaded();
          return false;
        }
        return true;
#endif
      }
      catch (AggregateException ex)
      {
        StackTrace trace = new StackTrace();
        Log.Error("Error with {0}!", trace.GetFrame(1).GetMethod().Name);
        foreach (var innerEx in ex.InnerExceptions)
        {
          Log.Error("Exception: {0}", innerEx.Message);
        }
        ReinitialiseThreaded();
        return false;
      }
    }

    public void CalculateBitmap(Stream stream)
    {
      // Debug file output
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
      //now flip horizontally, we do it always to prevent microstudder
      int i;
      for (i = 0; i < ((GetCaptureHeight() / 2) - 1); i++)
      {
        Array.Copy(pixelData, i * GetCaptureWidth() * rgb, h1pixelData, 0, GetCaptureWidth() * rgb);
        Array.Copy(pixelData, (GetCaptureHeight() - i - 1) * GetCaptureWidth() * rgb, h2pixelData, 0, GetCaptureWidth() * rgb);
        Array.Copy(h1pixelData, 0, pixelData, (GetCaptureHeight() - i - 1) * GetCaptureWidth() * rgb, GetCaptureWidth() * rgb);
        Array.Copy(h2pixelData, 0, pixelData, i * GetCaptureWidth() * rgb, GetCaptureWidth() * rgb);
      }
      //send scaled and fliped frame to atmowin

      SetPixelData(bmiInfoHeader, pixelData);
    }

    public void UpdateGIFPath(string path)
    {
      gifPath = path;
    }
    #endregion

    #region COM Interface

    private bool GetAtmoRemoteControl()
    {
      Log.Debug("Getting AtmoWin Remote Control.");
      if (TimeoutHandler(() => atmoRemoteControl = (IAtmoRemoteControl2)Marshal.GetActiveObject("AtmoRemoteControl.1")))
      {
        Log.Debug("Successfully got AtmoWin Remote Control.");
        return true;
      }
      return false;
    }

    /// <summary>
    /// Opens the COM interface to AtmoWin.
    /// </summary>
    /// <returns>true if successfull and false if not.</returns>
    private bool GetAtmoLiveViewControl()
    {
      if (atmoRemoteControl == null)
      {
        return false;
      }

      Log.Debug("Getting AtmoWin Live View Control.");
      if (TimeoutHandler(() => atmoLiveViewControl = (IAtmoLiveViewControl)Marshal.GetActiveObject("AtmoRemoteControl.1")))
      {
        Log.Debug("Successfully got AtmoWin Live View Control.");
        return true;
      }
      return false;
    }

    /// <summary>
    /// Gets the current AtmoWin liveview source.
    /// </summary>
    /// <returns>true if successfull and false if not.</returns>
    private bool GetAtmoLiveViewSource()
    {
      if (!IsConnected())
      {
        return false;
      }

      if (TimeoutHandler(() => atmoLiveViewControl.getCurrentLiveViewSource(out atmoLiveViewSource)))
      {
        return true;
      }
      return false;
    }

    /// <summary>
    /// Returns the capture width of AtmoWin
    /// </summary>
    /// <returns>Capture width of AtmoWin</returns>
    public int GetCaptureWidth()
    {
      return captureWidth;
    }

    /// <summary>
    /// Returns the capture height of AtmoWin
    /// </summary>
    /// <returns>Capture height of AtmoWin</returns>
    public int GetCaptureHeight()
    {
      return captureHeight;
    }

    /// <summary>
    /// Changes the AtmoWin profile.
    /// </summary>
    /// <returns>true if successfull and false if not.</returns>
    private bool SetColorMode(ComEffectMode effect)
    {
      if (!IsConnected())
      {
        return false;
      }

      Log.Debug("Changing AtmoWin profile (SetColorMode).");
      ComEffectMode oldEffect;
      if (TimeoutHandler(() => atmoRemoteControl.setEffect(effect, out oldEffect)))
      {
        Log.Info("Successfully changed AtmoWin profile.");
        return true;
      }
      return false;
    }

    /// <summary>
    /// Changes the AtmoWin effect.
    /// </summary>
    /// <param name="effect">Effect to change to.</param>
    /// <param name="force">Currently initialising.</param>
    /// <returns>true if successfull and false if not.</returns>
    private bool SetAtmoEffect(ComEffectMode effect, bool init = false)
    {
      if (!IsConnected() && !init)
      {
        return false;
      }

      Log.Debug("Changing AtmoWin effect to: {0}", effect.ToString());
      ComEffectMode oldEffect;
      if (TimeoutHandler(() => atmoRemoteControl.setEffect(effect, out oldEffect)))
      {
        Log.Info("Successfully changed AtmoWin effect to: {0}", effect.ToString());
        return true;
      }
      return false;
    }

    /// <summary>
    /// Changes the static color in AtmoWin.
    /// </summary>
    /// <param name="red">RGB value for red.</param>
    /// <param name="green">RGB value for green.</param>
    /// <param name="blue">RGB value for blue.</param>
    /// <returns>true if successfull and false if not.</returns>
    private bool SetAtmoColor(byte red, byte green, byte blue)
    {
      if (!IsConnected())
      {
        return false;
      }

      Log.Debug("Setting static color to R:{0} G:{1} B:{2}.", red, green, blue);
      if (TimeoutHandler(() => atmoRemoteControl.setStaticColor(red, green, blue)))
      {
        Log.Info("Successfully set static color to R:{0} G:{1} B:{2}.", red, green, blue);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Changes the AtmoWin liveview source.
    /// </summary>
    /// <param name="viewSource">The liveview source.</param>
    /// <returns>true if successfull and false if not.</returns>
    private bool SetAtmoLiveViewSource(ComLiveViewSource viewSource)
    {
      if (!IsConnected())
      {
        return false;
      }

      Log.Debug("Changing AtmoWin Liveview Source to: {0}", viewSource.ToString());
      if (TimeoutHandler(() => atmoLiveViewControl.setLiveViewSource(viewSource)))
      {
        Log.Info("Successfully changed AtmoWin Liveview Source to: {0}", viewSource.ToString());
        return true;
      }
      return false;
    }



    /// <summary>
    /// Gets the current liveview resolution.
    /// </summary>
    /// <returns>true if successfull and false if not.</returns>
    private bool GetAtmoLiveViewRes()
    {
      if (!IsConnected())
      {
        return false;
      }

      Log.Debug("Getting Liveview Resolution.");
      if (TimeoutHandler(() => atmoRemoteControl.getLiveViewRes(out captureWidth, out captureHeight)))
      {
        Log.Debug("Liveview capture resolution is {0}x{1}. Screenshot will be resized to this dimensions.", captureWidth, captureHeight);
        return true;
      }
      return false;
    }

    public void SetPixelData(byte[] bmiInfoHeader, byte[] pixelData, bool force = false)
    {
      if (!IsConnected())
      {
        return;
      }
      try
      {
        if (IsDelayEnabled() && !force && GetCurrentEffect() == ContentEffect.MediaPortalLiveMode)
        {
          AddDelayListItem(bmiInfoHeader, pixelData);
        }
        else
        {
          atmoLiveViewControl.setPixelData(bmiInfoHeader, pixelData);
        }
      }
      catch (Exception ex)
      {
        Log.Error("Error with SetPixelData.");
        Log.Error("Exception: {0}", ex.Message);
        ReinitialiseThreaded();
      }
    }


    #endregion

    #region Control LEDs
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
      currentEffect = ContentEffect.Undefined;
      changeEffect = effect;
      Log.Info("Changing AtmoLight effect to: {0}", effect.ToString());
      StopAllThreads();
      switch (effect)
      {
        case ContentEffect.AtmoWinLiveMode:
          currentState = true;
          if (!SetAtmoEffect(ComEffectMode.cemLivePicture)) return false;
          if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsGDI)) return false;
          break;
        case ContentEffect.Colorchanger:
          currentState = true;
          if (!SetAtmoEffect(ComEffectMode.cemColorChange)) return false;
          break;
        case ContentEffect.ColorchangerLR:
          currentState = true;
          if (!SetAtmoEffect(ComEffectMode.cemLrColorChange)) return false;
          break;
        case ContentEffect.LEDsDisabled:
        case ContentEffect.Undefined:
          currentState = false;
          if (!SetAtmoEffect(ComEffectMode.cemDisabled)) return false;
          // Workaround for SEDU.
          // Without the sleep it would not change to color.
          System.Threading.Thread.Sleep(delaySetStaticColor);
          if (!SetAtmoColor(0, 0, 0)) return false;
          break;
        case ContentEffect.MediaPortalLiveMode:
          currentState = true;
          if (!SetAtmoEffect(ComEffectMode.cemLivePicture)) return false;
          if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal)) return false;

          if (delayEnabled)
          {
            Log.Debug("Adding {0}ms delay to the LEDs.", delayTime);
            StartSetPixelDataThread();
          }

          StartGetAtmoLiveViewSourceThread();
          break;
        case ContentEffect.StaticColor:
          currentState = true;
          if (!SetAtmoEffect(ComEffectMode.cemDisabled)) return false;
          if (!SetAtmoColor((byte)staticColor[0], (byte)staticColor[1], (byte)staticColor[2])) return false;
          // Workaround for SEDU.
          // Without the sleep it would not change to color.
          System.Threading.Thread.Sleep(delaySetStaticColor);
          if (!SetAtmoColor((byte)staticColor[0], (byte)staticColor[1], (byte)staticColor[2])) return false;
          break;
        case ContentEffect.GIFReader:
          currentState = true;
          if (!SetAtmoEffect(ComEffectMode.cemLivePicture)) return false;
          if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal)) return false;
          StartGetAtmoLiveViewSourceThread();
          StartGIFReaderThread();
          break;
        case ContentEffect.VUMeter:
        case ContentEffect.VUMeterRainbow:
          currentState = true;
          vuMeterRainbowColorScheme = (effect == ContentEffect.VUMeterRainbow) ? true : false;
          if (!SetAtmoEffect(ComEffectMode.cemLivePicture)) return false;
          if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal)) return false;
          StartGetAtmoLiveViewSourceThread();
          StartVUMeterThread();
          break;
      }
      currentEffect = changeEffect;
      changeEffect = ContentEffect.Undefined;
      return true;
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
    /// Returns if AtmoLight/LEDs are on.
    /// </summary>
    /// <returns>true or false</returns>
    public bool IsAtmoLightOn()
    {
      return currentState;
    }
 
    /// <summary>
    /// Change to AtmoWin profile.
    /// </summary>
    /// <returns>true or false</returns>
    public bool ChangeAtmoWinProfile()
    {
      if (!SetColorMode(ComEffectMode.cemColorMode)) return false;

      // Change the effect to the desired effect.
      // Needed for AtmoWin 1.0.0.5+
      if (!ChangeEffect(currentEffect, true)) return false;
      return true;
    }

    /// <summary>
    /// Changes the delay time.
    /// </summary>
    /// <param name="delay">Delay in ms.</param>
    /// <returns>true or false</returns>
    public bool ChangeDelay(int delay)
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
    /// Changes the static color.
    /// </summary>
    /// <param name="red">Red in RGB format.</param>
    /// <param name="green">Green  in RGB format.</param>
    /// <param name="blue">Blue  in RGB format.</param>
    /// <returns>true or false</returns>
    public bool ChangeStaticColor(int red, int green, int blue)
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

    /// <summary>
    /// Returns the static color.
    /// </summary>
    /// <returns>Static Color as int array</returns>
    public int[] GetStaticColor()
    {
      return staticColor;
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
    /// Start the GetAtmoLiveViewSource thread.
    /// </summary>
    private void StartGetAtmoLiveViewSourceThread()
    {
      getAtmoLiveViewSourceLock = false;
      getAtmoLiveViewSourceThreadHelper = new Thread(() => GetAtmoLiveViewSourceThread());
      getAtmoLiveViewSourceThreadHelper.Name = "AtmoLight GetAtmoLiveViewSource";
      getAtmoLiveViewSourceThreadHelper.Start();
    }

    /// <summary>
    /// Stop the GetAtmoLiveViewSource thread.
    /// </summary>
    private void StopGetAtmoLiveViewSourceThread()
    {
      getAtmoLiveViewSourceLock = true;
    }

    private void StartGIFReaderThread()
    {
      gifReaderLock = false;
      gifReaderThreadHelper = new Thread(() => GIFReaderThread());
      gifReaderThreadHelper.Name = "AtmoLight GIFReader";
      gifReaderThreadHelper.Start();
    }

    private void StopGIFReaderThread()
    {
      gifReaderLock = true;
    }

    private void StartVUMeterThread()
    {
      vuMeterLock = false;
      vuMeterThreadHelper = new Thread(() => VUMeterThread());
      vuMeterThreadHelper.Name = "AtmoLight VUMeter";
      vuMeterThreadHelper.Start();
    }

    private void StopVUMeterThread()
    {
      vuMeterLock = true;
    }

    private void StopAllThreads()
    {
      StopSetPixelDataThread();
      StopGetAtmoLiveViewSourceThread();
      StopGIFReaderThread();
      StopVUMeterThread();
    }

    /// <summary>
    /// Send pixel data to AtmoWin when MediaPortal liveview is used (external liveview source).
    /// Also add a delay specified in settings.
    /// This method is designed to run as its own thread.
    /// </summary>
    private void SetPixelDataThread()
    {
      if (atmoRemoteControl == null)
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
              SetPixelData(bmiInfoHeaderList[0], pixelDataList[0], true);
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
        Log.Error("Could not send data to AtmoWin.");
        Log.Error("Exception: {0}", ex.Message);

        ClearDelayLists();
      }
    }

    /// <summary>
    /// Check if the AtmoWin liveview source is set to external when MediaPortal liveview is used.
    /// Set liveview source back to external if needed.
    /// This method is designed to run as its own thread.
    /// </summary>
    private void GetAtmoLiveViewSourceThread()
    {
      try
      {
        while (!getAtmoLiveViewSourceLock && IsConnected())
        {
          GetAtmoLiveViewSource();
          if (atmoLiveViewSource != ComLiveViewSource.lvsExternal)
          {
            Log.Debug("AtmoWin Liveview Source is not lvsExternal");
            SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal);
          }
          System.Threading.Thread.Sleep(delayGetAtmoLiveViewSource);
        }
      }
      catch (Exception ex)
      {
        Log.Error("Error in GetAtmoLiveViewSourceThread.");
        Log.Error("Exception: {0}", ex.Message);
      }
    }

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

            // Calculations to prepare data for AtmoWin and then send data
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

    private void VUMeterThread()
    {
      try
      {
        if (vuMeterRainbowColorScheme)
        {
          vuMeterBrushes.Clear();
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
          vuMeterBrushes.Clear();
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
