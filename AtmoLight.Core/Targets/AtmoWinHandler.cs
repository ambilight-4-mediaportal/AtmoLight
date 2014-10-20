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
  class AtmoWinHandler
  {
    #region Fields
    private string atmoWinPath = "";
    private bool reInitOnError = true;
    private bool startAtmoWin = true;
    private int[] staticColor = { 0, 0, 0 };

    // Com  Objects
    private IAtmoRemoteControl2 atmoRemoteControl = null; // Com Object to control AtmoWin
    private IAtmoLiveViewControl atmoLiveViewControl = null; // Com Object to control AtmoWins liveview
    private ComLiveViewSource atmoLiveViewSource; // Current liveview source

    // Timings
    private const int timeoutComInterface = 5000; // Timeout for the COM interface
    private const int delaySetStaticColor = 20; // SEDU workaround delay time
    private const int delayAtmoWinConnect = 1000; // Delay between starting AtmoWin and connection to it
    private const int delayGetAtmoLiveViewSource = 1000; // Delay between liveview source checks

    // Locks
    private volatile bool reinitialiseLock = false;

    #endregion

    #region Constructor
    public AtmoWinHandler()
    {
      var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
      DateTime buildDate = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTime;
      Log.Debug("Core Version {0}.{1}.{2}.{3}, build on {4} at {5}.", version.Major, version.Minor, version.Build, version.Revision, buildDate.ToShortDateString(), buildDate.ToLongTimeString());
    }
    #endregion

    #region Public methods
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
      if (!reInitOnError && !force)
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

    public bool ChangeEffect(ContentEffect effect)
    {
      return true;
    }

    public bool ChangeColor(int red, int green, int blue)
    {
      return true;
    }

    public bool ChangeImage(byte[] pixeldata)
    {
      return true;
    }

    /// <summary>
    /// Change to AtmoWin profile.
    /// </summary>
    /// <returns>true or false</returns>
    public bool ChangeProfile()
    {
      if (!SetColorMode(ComEffectMode.cemColorMode)) return false;

      // Change the effect to the desired effect.
      // Needed for AtmoWin 1.0.0.5+
      if (!ChangeEffect(currentEffect, true)) return false;
      return true;
    }

    public void UpdateAtmoWinPath(string path)
    {
      atmoWinPath = path;
    }

    public void UpdateReInitOnError(bool reInit)
    {
      reInitOnError = reInit;
    }

    public void UpdateStartAtmoWin(bool startAtmoWin)
    {
      this.startAtmoWin = startAtmoWin;
    }

    public void UpdateStaticColor(int[] staticColor)
    {
      this.staticColor = staticColor;
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
    /// Start reinitialising in a new thread.
    /// </summary>
    /// <param name="force">Force the reinitialising and discard user settings.</param>
    private void ReinitialiseThreaded(bool force = false)
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
