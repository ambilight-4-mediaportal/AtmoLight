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
using System.Net.Sockets;
using System.Linq;

namespace AtmoLight
{
  class AtmoWinHandler : ITargets
  {
    #region Fields
    public Target Name { get { return Target.AtmoWin; } }
    public TargetType Type { get { return TargetType.Local; } }

    // Threads
    private Thread reinitialiseThreadHelper;
    private Thread initialiseThreadHelper;
    private Thread getAtmoLiveViewSourceThreadHelper;

    // Locks
    private volatile bool reInitialiseLock = false;
    private volatile bool initialiseLock = false;
    private volatile bool getAtmoLiveViewSourceLock = false;

    // Com  Objects
    private IAtmoRemoteControl2 atmoRemoteControl = null; // Com Object to control AtmoWin
    private IAtmoLiveViewControl atmoLiveViewControl = null; // Com Object to control AtmoWins liveview
    private ComLiveViewSource atmoLiveViewSource; // Current liveview source

    // Timings
    private const int timeoutComInterface = 5000; // Timeout for the COM interface
    private const int delaySetStaticColor = 20; // SEDU workaround delay time
    private const int delayAtmoWinConnect = 1000; // Delay between starting AtmoWin and connection to it
    private const int delayGetAtmoLiveViewSource = 1000; // Delay between liveview source checks

    // Core Object
    private Core coreObject;

    // Other Fields
    private int captureWidth;
    private int captureHeight;
    #endregion

    #region Constructor
    /// <summary>
    /// AtmoWinHandler constructor
    /// </summary>
    public AtmoWinHandler()
    {
      Log.Debug("AtmoWinHandler - AtmoWin as target added.");
      // Get Core instance to use core inside AtmoWinHandler
      coreObject = Core.GetInstance();
    }
    #endregion

    #region ITargets Methods
    /// <summary>
    /// Initialise AtmoWin by starting (if needed and wanted) it and connecting.
    /// This happens threaded.
    /// </summary>
    /// <param name="force"></param>
    public void Initialise(bool force = false)
    {
      if (!initialiseLock)
      {
        initialiseThreadHelper = new Thread(() => InitialiseThreaded(force));
        initialiseThreadHelper.Name = "AtmoLight Initialise";
        initialiseThreadHelper.Start();
      }
      else
      {
        Log.Debug("AtmoWinHandler - Initialising Thread already running.");
      }
    }

    /// <summary>
    /// Reinitialisation of AtmoWin if connection got lost.
    /// This happens threaded.
    /// </summary>
    /// <param name="force">force the reinit or not</param>
    public void ReInitialise(bool force = false)
    {
      if (!reInitialiseLock)
      {
        reinitialiseThreadHelper = new Thread(() => ReInitialiseThreaded(force));
        reinitialiseThreadHelper.Name = "AtmoLight Reinitialise";
        reinitialiseThreadHelper.Start();
      }
      else
      {
        Log.Debug("AtmoWinHandler - Reinitialising Thread already running.");
      }
    }

    /// <summary>
    /// Disconnects from AtmoWin and stops it if wanted.
    /// </summary>
    public void Dispose()
    {
      Log.Debug("AtmoWinHandler - Disposing AtmoWin handler.");
      Disconnect();
      if (coreObject.atmoWinAutoStop)
      {
        StopAtmoWin();
      }
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

    /// <summary>
    /// Changes to desired effect.
    /// </summary>
    /// <param name="effect"></param>
    /// <returns></returns>
    public bool ChangeEffect(ContentEffect effect)
    {
      if (!IsConnected())
      {
        return false;
      }
      StopGetAtmoLiveViewSourceThread();
      switch (effect)
      {
        case ContentEffect.AtmoWinLiveMode:
          if (!SetAtmoEffect(ComEffectMode.cemLivePicture)) return false;
          if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsGDI)) return false;
          break;
        case ContentEffect.Colorchanger:
          if (!SetAtmoEffect(ComEffectMode.cemColorChange)) return false;
          break;
        case ContentEffect.ColorchangerLR:
          if (!SetAtmoEffect(ComEffectMode.cemLrColorChange)) return false;
          break;
        case ContentEffect.LEDsDisabled:
        case ContentEffect.Undefined:
          if (!SetAtmoEffect(ComEffectMode.cemDisabled)) return false;
          // Workaround for SEDU.
          // Without the sleep it would not change to color.
          System.Threading.Thread.Sleep(delaySetStaticColor);
          if (!SetAtmoColor(0, 0, 0)) return false;
          break;
        case ContentEffect.MediaPortalLiveMode:
          if (!SetAtmoEffect(ComEffectMode.cemLivePicture)) return false;
          if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal)) return false;

          StartGetAtmoLiveViewSourceThread();
          break;
        case ContentEffect.StaticColor:
          if (!SetAtmoEffect(ComEffectMode.cemDisabled)) return false;
          if (!SetAtmoColor((byte)coreObject.staticColor[0], (byte)coreObject.staticColor[1], (byte)coreObject.staticColor[2])) return false;
          // Workaround for SEDU.
          // Without the sleep it would not change to color.
          System.Threading.Thread.Sleep(delaySetStaticColor);
          if (!SetAtmoColor((byte)coreObject.staticColor[0], (byte)coreObject.staticColor[1], (byte)coreObject.staticColor[2])) return false;
          break;
        case ContentEffect.GIFReader:
          if (!SetAtmoEffect(ComEffectMode.cemLivePicture)) return false;
          if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal)) return false;
          StartGetAtmoLiveViewSourceThread();
          break;
        case ContentEffect.VUMeter:
        case ContentEffect.VUMeterRainbow:
          if (!SetAtmoEffect(ComEffectMode.cemLivePicture)) return false;
          if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal)) return false;
          StartGetAtmoLiveViewSourceThread();
          break;
      }
      return true;
    }

    /// <summary>
    /// Sends current image/frame to AtmoWin to be displayed.
    /// </summary>
    /// <param name="pixeldata">bytearray of the picture</param>
    /// <param name="bmiInfoHeader">bytearray of the picture header</param>
    public void ChangeImage(byte[] pixeldata, byte[] bmiInfoHeader)
    {
      if (!IsConnected())
      {
        return;
      }
      SetPixelData(bmiInfoHeader, pixeldata);
    }

    /// <summary>
    /// Change the AtmoWin profile.
    /// </summary>
    /// <returns>true or false</returns>
    public void ChangeProfile()
    {
      if (!IsConnected())
      {
        return;
      }
      Log.Info("AtmoWinHandler - Changing AtmoWin profile.");
      if (!SetColorMode(ComEffectMode.cemColorMode)) return;

      // Change the effect to the desired effect.
      // Needed for AtmoWin 1.0.0.5+
      if (!ChangeEffect(coreObject.GetCurrentEffect())) return;
    }
    #endregion

    #region Initialise
    /// <summary>
    /// Start AtmoWin and connects to it.
    /// </summary>
    /// <returns>true or false</returns>
    private bool InitialiseThreaded(bool force = false)
    {
      if (initialiseLock)
      {
        Log.Debug("AtmoWinHandler - Initialising locked.");
        return false;
      }
      initialiseLock = true;
      Log.Debug("AtmoWinHandler - Initialising.");
      if (!Win32API.IsProcessRunning("atmowina.exe"))
      {
        if (coreObject.atmoWinAutoStart || force)
        {
          if (!StartAtmoWin())
          {
            initialiseLock = false;
            return false;
          }
          System.Threading.Thread.Sleep(delayAtmoWinConnect);
        }
        else
        {
          Log.Error("AtmoWinHandler - AtmoWin is not running.");
          initialiseLock = false;
          return false;
        }
      }

      if (!Connect())
      {
        initialiseLock = false;
        return false;
      }

      Log.Debug("AtmoWinHandler - Initialising successfull.");

      coreObject.ChangeEffect(coreObject.GetCurrentEffect(), true);

      initialiseLock = false;
      return true;
    }

    /// <summary>
    /// Restart AtmoWin and reconnects to it.
    /// </summary>
    /// <param name="force">Force the reinitialising and discard user settings.</param>
    public void ReInitialiseThreaded(bool force = false)
    {
      if (reInitialiseLock)
      {
        Log.Debug("AtmoWinHandler - Reinitialising locked.");
        return;
      }
      if (!coreObject.reInitOnError && !force)
      {
        Disconnect();
        Log.Error("AtmoWinHandler - Connection to AtmoWin lost.");
        coreObject.NewConnectionLost(Name);
        return;
      }

      reInitialiseLock = true;
      Log.Debug("AtmoWinHandler - Reinitialising.");

      if (!Disconnect() || !StopAtmoWin() || !InitialiseThreaded(force) || !ChangeEffect(coreObject.GetChangeEffect() != ContentEffect.Undefined ? coreObject.GetChangeEffect() : coreObject.GetCurrentEffect()))
      {
        Disconnect();
        StopAtmoWin();
        Log.Error("AtmoWinHandler - Reinitialising failed.");
        reInitialiseLock = false;
        coreObject.NewConnectionLost(Name);
        return;
      }

      Log.Debug("AtmoWinHandler - Reinitialising successfull.");
      reInitialiseLock = false;
      return;
    }
    #endregion

    #region Connect
    /// <summary>
    /// Connect to AtmoWin.
    /// </summary>
    /// <returns>true or false</returns>
    public bool Connect()
    {
      Log.Debug("AtmoWinHandler - Trying to connect to AtmoWin.");
      if (!GetAtmoRemoteControl()) return false;
      if (!SetAtmoEffect(ComEffectMode.cemLivePicture, true)) return false;
      if (!GetAtmoLiveViewControl()) return false;
      if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal)) return false;
      if (!GetAtmoLiveViewRes()) return false;

      Log.Debug("AtmoWinHandler - Successfully connected to AtmoWin.");
      return true;
    }

    /// <summary>
    /// Disconnect from AtmoWin.
    /// </summary>
    /// <returns>true or false</returns>
    public bool Disconnect()
    {
      Log.Debug("AtmoWinHandler - Disconnecting from AtmoWin.");

      StopGetAtmoLiveViewSourceThread();

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
      Log.Debug("AtmoWinHandler - Trying to reconnect to AtmoWin.");
      Disconnect();
      Connect();
      return true;
    }
    #endregion

    #region AtmoWin
    /// <summary>
    /// Start AtmoWin.
    /// </summary>
    /// <returns>true or false</returns>
    public bool StartAtmoWin()
    {
      Log.Debug("AtmoWinHandler - Trying to start AtmoWin.");
      if (!System.IO.File.Exists(coreObject.atmoWinPath))
      {
        Log.Error("AtmoWinHandler - AtmoWinA.exe not found!");
        return false;
      }
      Process AtmoWinA = new Process();
      AtmoWinA.StartInfo.FileName = coreObject.atmoWinPath;
      AtmoWinA.StartInfo.UseShellExecute = true;
      AtmoWinA.StartInfo.Verb = "open";
      try
      {
        AtmoWinA.Start();
      }
      catch (Exception)
      {
        Log.Error("AtmoWinHandler - Starting AtmoWin failed.");
        return false;
      }
      Log.Info("AtmoWinHandler - AtmoWin successfully started.");
      return true;
    }

    /// <summary>
    /// Stop AtmoWin.
    /// </summary>
    /// <returns>true or false</returns>
    public bool StopAtmoWin()
    {
      Log.Info("AtmoWinHandler - Trying to stop AtmoWin.");
      foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("atmowina")))
      {
        try
        {
          process.Kill();
          // Wait if the kill succeeded, because sometimes it does not.
          // If it does not, we stop the whole initialization.
          if (!TimeoutHandler(() => process.WaitForExit()))
          {
            Log.Error("AtmoWinHandler - Stopping AtmoWin failed.");
            return false;
          }
          Win32API.RefreshTrayArea();
        }
        catch (Exception ex)
        {
          Log.Error("AtmoWinHandler - Stopping AtmoWin failed.");
          Log.Error("AtmoWinHandler - Exception: {0}", ex.Message);
          return false;
        }
      }
      Log.Info("AtmoWinHandler - AtmoWin successfully stopped.");
      return true;
    }

    /// <summary>
    /// Restart AtmoWin.
    /// </summary>
    public void RestartAtmoWin()
    {
      Log.Debug("AtmoWinHandler - Trying to restart AtmoWin.");
      StopAtmoWin();
      StartAtmoWin();
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
          Log.Error("AtmoWinHandler - {0} timed out after {1}ms!", trace.GetFrame(1).GetMethod().Name, Win32API.GetTickCount() - timeoutStart);
          ReInitialise();
          return false;
        }
        return true;
#endif
      }
      catch (AggregateException ex)
      {
        StackTrace trace = new StackTrace();
        Log.Error("AtmoWinHandler - Error with {0}!", trace.GetFrame(1).GetMethod().Name);
        foreach (var innerEx in ex.InnerExceptions)
        {
          Log.Error("AtmoWinHandler - Exception: {0}", innerEx.Message);
        }
        ReInitialise();
        return false;
      }
    }
    #endregion

    #region COM Interface
    /// <summary>
    /// Opens the COM interface to AtmoWin.
    /// </summary>
    /// <returns></returns>
    private bool GetAtmoRemoteControl()
    {
      Log.Debug("AtmoWinHandler - Getting AtmoWin Remote Control.");
      if (TimeoutHandler(() => atmoRemoteControl = (IAtmoRemoteControl2)Marshal.GetActiveObject("AtmoRemoteControl.1")))
      {
        Log.Debug("AtmoWinHandler - Successfully got AtmoWin Remote Control.");
        return true;
      }
      return false;
    }

    /// <summary>
    /// Opens the COM interface for live view control.
    /// </summary>
    /// <returns>true if successfull and false if not.</returns>
    private bool GetAtmoLiveViewControl()
    {
      if (atmoRemoteControl == null)
      {
        return false;
      }

      Log.Debug("AtmoWinHandler - Getting AtmoWin Live View Control.");
      if (TimeoutHandler(() => atmoLiveViewControl = (IAtmoLiveViewControl)Marshal.GetActiveObject("AtmoRemoteControl.1")))
      {
        Log.Debug("AtmoWinHandler - Successfully got AtmoWin Live View Control.");
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
    /// Changes the AtmoWin profile.
    /// </summary>
    /// <returns>true if successfull and false if not.</returns>
    private bool SetColorMode(ComEffectMode effect)
    {
      if (!IsConnected())
      {
        return false;
      }

      Log.Debug("AtmoWinHandler - Changing AtmoWin profile (SetColorMode).");
      ComEffectMode oldEffect;
      if (TimeoutHandler(() => atmoRemoteControl.setEffect(effect, out oldEffect)))
      {
        Log.Info("AtmoWinHandler - Successfully changed AtmoWin profile.");
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

      Log.Debug("AtmoWinHandler - Changing AtmoWin effect to: {0}", effect.ToString());
      ComEffectMode oldEffect;
      if (TimeoutHandler(() => atmoRemoteControl.setEffect(effect, out oldEffect)))
      {
        Log.Info("AtmoWinHandler - Successfully changed AtmoWin effect to: {0}", effect.ToString());
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

      Log.Debug("AtmoWinHandler - Setting static color to R:{0} G:{1} B:{2}.", red, green, blue);
      if (TimeoutHandler(() => atmoRemoteControl.setStaticColor(red, green, blue)))
      {
        Log.Info("AtmoWinHandler - Successfully set static color to R:{0} G:{1} B:{2}.", red, green, blue);
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

      Log.Debug("AtmoWinHandler - Changing AtmoWin Liveview Source to: {0}", viewSource.ToString());
      if (TimeoutHandler(() => atmoLiveViewControl.setLiveViewSource(viewSource)))
      {
        Log.Info("AtmoWinHandler - Successfully changed AtmoWin Liveview Source to: {0}", viewSource.ToString());
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

      Log.Debug("AtmoWinHandler - Getting Liveview Resolution.");
      if (TimeoutHandler(() => atmoRemoteControl.getLiveViewRes(out captureWidth, out captureHeight)))
      {
        Log.Debug("AtmoWinHandler - Liveview capture resolution is {0}x{1}. Screenshot will be resized to this dimensions.", captureWidth, captureHeight);
        coreObject.SetCaptureDimensions(captureWidth, captureHeight);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Sends picture information directly to AtmoWin
    /// </summary>
    /// <param name="bmiInfoHeader"></param>
    /// <param name="pixelData"></param>
    private void SetPixelData(byte[] bmiInfoHeader, byte[] pixelData)
    {
      if (!IsConnected())
      {
        return;
      }
      try
      {
        atmoLiveViewControl.setPixelData(bmiInfoHeader, pixelData);
      }
      catch (Exception ex)
      {
        Log.Error("AtmoWinHandler - Error with SetPixelData.");
        Log.Error("AtmoWinHandler - Exception: {0}", ex.Message);
        ReInitialise();
      }
    }
    #endregion

    #region Threads
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
            Log.Debug("AtmoWinHandler - AtmoWin Liveview Source is not lvsExternal");
            SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal);
          }
          System.Threading.Thread.Sleep(delayGetAtmoLiveViewSource);
        }
      }
      catch (Exception ex)
      {
        Log.Error("AtmoWinHandler - Error in GetAtmoLiveViewSourceThread.");
        Log.Error("AtmoWinHandler - Exception: {0}", ex.Message);
      }
    }
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
  }
}
