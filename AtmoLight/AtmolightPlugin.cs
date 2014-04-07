using System;
using System.Collections.Generic;
using System.Text;
using MediaPortal.GUI.Library;
using MediaPortal.Profile;
using MediaPortal.Player;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using AtmoWinRemoteControl;
using System.Drawing;
using System.IO;
using Microsoft.DirectX.Direct3D;
using MediaPortal.Dialogs;
using MediaPortal.Configuration;
using Language;

namespace MediaPortal.ProcessPlugins.Atmolight
{
  [PluginIcons("Atmolight.Enabled.png", "Atmolight.Disabled.png")]
  public class AtmolightPlugin : ISetupForm, IPlugin
  {
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

    #region AtmoDXUtil Import
    [DllImport("AtmoDXUtil.dll", PreserveSig = false, CharSet = CharSet.Auto)]
    private static extern void VideoSurfaceToRGBSurfaceExt(IntPtr src, int srcWidth, int srcHeight, IntPtr dst, int dstWidth, int dstHeight);
    #endregion

    #region Variables
    private bool atmoLightPluginStarted = false; // State of the plugin
    public bool atmoOff = true; // State of the LEDs
    public Int64 lastFrame = 0; // Tick count of the last frame
    private IAtmoRemoteControl2 atmoCtrl = null; // Com Object to control AtmoWin
    private IAtmoLiveViewControl atmoLiveViewCtrl = null; // Com Object to control AtmoWins liveview
    private int captureWidth = 0; // AtmoWins capture width
    private int captureHeight = 0; // AtmoWins capture height
    private Surface rgbSurface = null; // RGB Surface
    private ContentEffect playbackEffect = ContentEffect.Undefined; // Effect for current placback
    private ContentEffect menuEffect = ContentEffect.Undefined; // Effect in GUI (no playback)
    private ContentEffect currentEffect = ContentEffect.Undefined; // Current aktive effect
    private int[] staticColor = { 0, 0, 0 }; // RGB code for static color
    private int[] staticColorTemp = { 0, 0, 0 }; // Temp array to change static color
    private int staticColorHelper; // Helper var for static color change
    private const int timeoutComInterface = 5000; // Timeout for the COM interface
    private const int delaySetStaticColor = 20; // SEDU workaround delay time
    private const int delayAtmoWinConnecting = 1000; // Delay between starting AtmoWin and connection to it
    private const int delayGetAtmoLiveViewSource = 1000; // Delay between liveview source checks
    private bool reInitializeLock = false; // Lock for the reinitialization process
    private bool getAtmoLiveViewSourceLock = true; // Lock for liveview source checks
    private bool setPixelDataLock = true; // Lock for SetPixelData thread
    private bool onRestartAtmoWin = false; // Need to be set only after the first start of plugin
    private ComLiveViewSource atmoLiveViewSource; // Current liveview source
    private int delayTimeHelper; // Helper var for delay time change
    private int delayRefreshRateDependant; // Variable that holds the actual delay
    #endregion

    #region Utilities
    /// <summary>
    /// Checks if a method times out and starts to reinitialize AtmoWin if needed.
    /// </summary>
    /// <param name="method">Method that needs checking for a timeout.</param>
    /// <param name="timeout">Timeout in ms.</param>
    /// <returns>true if not timed out and false if timed out.</returns>
    private bool TimeoutHandler(System.Action method, int timeout = timeoutComInterface)
    {
      try
      {
        long timeoutStart = Win32API.GetTickCount();
        var tokenSource = new CancellationTokenSource();
        CancellationToken token = tokenSource.Token;
        var task = Task.Factory.StartNew(() => method(), token);

        if (!task.Wait(timeout, token))
        {
          // Stacktrace is needed so we can output the name of the method that timed out.
          StackTrace trace = new StackTrace();
          Log.Error("AtmoLight: {0} timed out after {1}ms!", trace.GetFrame(1).GetMethod().Name, Win32API.GetTickCount() - timeoutStart);

          // Try to reconnect to AtmoWin.
          // This is done in a new thread, to not halt anything else.
          Thread ReInitializeAtmoWinConnectionHelperThread = new Thread(() => ReInitializeAtmoWinConnection());
          ReInitializeAtmoWinConnectionHelperThread.Start();

          return false;
        }
        return true;
      }
      catch (AggregateException ex)
      {
        StackTrace trace = new StackTrace();
        Log.Error("AtmoLight: Error with {0}!", trace.GetFrame(1).GetMethod().Name);
        foreach (var innerEx in ex.InnerExceptions)
        {
          Log.Error("AtmoLight: Exception: {0}", innerEx.Message);
        }
        Thread ReInitializeAtmoWinConnectionHelperThread = new Thread(() => ReInitializeAtmoWinConnection());
        ReInitializeAtmoWinConnectionHelperThread.Start();
        return false;
      }
    }

    /// <summary>
    /// Returns the current refresh rate.
    /// </summary>
    /// <returns>Current refresh rate.</returns>
    private int GetRefreshRate()
    {
      if (GUIGraphicsContext.currentMonitorIdx == -1)
      {
        return Manager.Adapters[GUIGraphicsContext.DX9Device.DeviceCaps.AdapterOrdinal].CurrentDisplayMode.RefreshRate;
      }
      else
      {
        return Manager.Adapters[GUIGraphicsContext.currentMonitorIdx].CurrentDisplayMode.RefreshRate;
      }
    }

    /// <summary>
    /// Checks if the LEDs may be activated or deactivated bacause of the settings.
    /// </summary>
    /// <returns>true if LEDs may be activated and false if not.</returns>
    private bool CheckForStartRequirements()
    {
      if (atmoCtrl == null)
      {
        return false;
      }
      if (AtmolightSettings.manualMode)
      {
        Log.Debug("AtmoLight: LEDs should be deactivated. (Manual Mode)");
        atmoOff = true;
        return false;
      }
      else if ((DateTime.Now.TimeOfDay >= AtmolightSettings.excludeTimeStart.TimeOfDay && DateTime.Now.TimeOfDay <= AtmolightSettings.excludeTimeEnd.TimeOfDay))
      {
        Log.Debug("AtmoLight: LEDs should be deactivated. (Timeframe)");
        atmoOff = true;
        return false;
      }
      else
      {
        Log.Debug("AtmoLight: LEDs can be activated.");
        atmoOff = false;
        return true;
      }
    }
    #endregion

    #region Initialize AtmoLight

    /// <summary>
    /// AtmoLight constructor.
    /// Loads the plugin, loads the settings and initializes AtmoWin and the connection.
    /// </summary>
    public AtmolightPlugin()
    {
      if (MPSettings.Instance.GetValueAsBool("plugins", "Atmolight", true))
      {
        Log.Debug("AtmoLight: Loading Settings.");
        AtmolightSettings.LoadSettings();
        InitializeAtmoWinConnection();
      }
    }


    /// <summary>
    /// Starts AmtoWin (if needed and wanted) and connects to it.
    /// </summary>
    /// <returns>true if successfull and false if not.</returns>
    private bool InitializeAtmoWinConnection()
    {
      // Dont start the initialization when in MP Config.
      if (!Win32API.IsProcessRunning("configuration.exe"))
      {
        if (AtmolightSettings.startAtmoWin)
        {
          Log.Debug("AtmoLight: Checking if AtmoWinA.exe is running.");
          if (!Win32API.IsProcessRunning("atmowina.exe"))
          {
            Log.Debug("AtmoLight: AtmoWinA.exe not running. Starting it.");
            if (StartAtmoWinA())
            {
              // Wait 1 second before trying to connect.
              System.Threading.Thread.Sleep(delayAtmoWinConnecting);
              return ConnectToAtmoWinA();
            }
          }
          else
          {
            Log.Debug("AtmoLight: AtmoWinA is allready running.");
            return ConnectToAtmoWinA();
          }
        }
        else
        {
          Log.Debug("AtmoLight: Checking if AtmoWinA.exe is running.");
          if (Win32API.IsProcessRunning("atmowina.exe"))
          {
            Log.Debug("AtmoLight: AtmoWinA is running.");
            onRestartAtmoWin = true;
            return ConnectToAtmoWinA();
          }
          else if (AtmolightSettings.restartOnError && onRestartAtmoWin)
          {
            Log.Debug("AtmoLight: Checking if AtmoWinA.exe is running.");
            if (!Win32API.IsProcessRunning("atmowina.exe"))
            {
              Log.Debug("AtmoLight: AtmoWinA.exe not running. Starting it.");
              if (StartAtmoWinA())
              {
                // Wait 1 second before trying to connect.
                System.Threading.Thread.Sleep(delayAtmoWinConnecting);
                return ConnectToAtmoWinA();
              }
            }
            else
            {
              Log.Debug("AtmoLight: AtmoWinA is allready running.");
              return ConnectToAtmoWinA();
            }
          }
          else
          {
            Log.Error("AtmoLight: AtmoWinA is not running.");
          }
        }
      }
      return false;
    }

    /// <summary>
    /// Connects to AtmoWin.
    /// </summary>
    /// <returns>true if successfull and false if not.</returns>
    private bool ConnectToAtmoWinA()
    {
      try
      {
        ReleaseAtmoControl();
        atmoCtrl = (IAtmoRemoteControl2)Marshal.GetActiveObject("AtmoRemoteControl.1");
      }
      catch (Exception ex)
      {
        Log.Error("AtmoLight: Failed to connect to AtmoWin.");
        Log.Error("AtmoLight: Exception: {0}", ex.Message);
        ReleaseAtmoControl();
        return false;
      }

      Log.Info("AtmoLight: Successfully connected to AtmoWin.");

      // If any of these fail, stop the whole initialization so we dont run into loops.
      if (!SetAtmoEffect(ComEffectMode.cemLivePicture))
      {
        return false;
      }
      if (!GetAtmoLiveViewCtrl())
      {
        return false;
      }
      if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal))
      {
        return false;
      }
      if (!GetAtmoLiveViewRes())
      {
        return false;
      }
      currentEffect = ContentEffect.Undefined;
      if (!DisableLEDs())
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Starts AtmoWin.
    /// </summary>
    /// <returns>true if successfull and false if not.</returns>
    private bool StartAtmoWinA()
    {
      if (!System.IO.File.Exists(AtmolightSettings.atmowinExe))
      {
        Log.Error("AtmoLight: AtmoWinA.exe not found.");
        return false;
      }
      Process app = new Process();
      app.StartInfo.FileName = AtmolightSettings.atmowinExe;
      app.StartInfo.UseShellExecute = true;
      app.StartInfo.Verb = "open";
      bool ret = true;
      try
      {
        ret = app.Start();
      }
      catch (Exception)
      {
        Log.Error("AtmoLight: Can't start AtmoWinA.exe.");
        return false;
      }
      Log.Info("AtmoLight: AtmoWinA.exe started.");
      return ret;
    }

    /// <summary>
    /// Kills AtmoWin.
    /// </summary>
    /// <returns>true if successfull and false if not.</returns>
    private bool KillAtmoWinA()
    {
      Log.Debug("AtmoLight: Stopping AtmoWinA.exe.");
      foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("atmowina")))
      {
        process.Kill();
        // Wait if the kill succeeded, because sometimes it does not.
        // If it does not, we stop the whole initialization.
        if (!TimeoutHandler(() => process.WaitForExit()))
        {
          return false;
        }
        Win32API.RefreshTrayArea();
        Log.Debug("AtmoLight: AtmoWinA.exe stopped.");
      }
      return true;
    }

    /// <summary>
    /// Kills AtmoWin, restarts it and connects to it.
    /// Disables AtmoLight if it fails and prompts the user with an error.
    /// </summary>
    /// <returns>true if successfull and false if not.</returns>
    private bool ReInitializeAtmoWinConnection(bool overwriteRestartOnError = false)
    {
      // Lock is needed so we dont try to initialize while another initialize is already running.
      if (reInitializeLock)
      {
        return false;
      }
      reInitializeLock = true;

      // Dont do anything if user doesnt want it.
      if (!AtmolightSettings.restartOnError && !overwriteRestartOnError)
      {
        ReleaseAtmoControl();
        atmoOff = true;
        reInitializeLock = false;
        DialogError(LanguageLoader.appStrings.ContextMenu_AtmoWinConnectionLost);
        return false;
      }

      GUIWaitCursor.Init();
      GUIWaitCursor.Show();
      
      currentEffect = ContentEffect.Undefined;
      Log.Debug("AtmoLight: Trying to restart AtmoWin and reconnect to it.");
      ReleaseAtmoControl();
      if (!KillAtmoWinA() || !InitializeAtmoWinConnection())
      {
        atmoOff = true;
        ReleaseAtmoControl();
        reInitializeLock = false;
        GUIWaitCursor.Hide();
        Log.Error("AtmoLight: Reconnecting to AtmoWin failed.");
        DialogError(LanguageLoader.appStrings.ContextMenu_AtmoWinConnectionLost);
        return false;
      }
      else
      {
        StartLEDs();
        GUIWaitCursor.Hide();
        reInitializeLock = false;
        return true;
      }
    }

    /// <summary>
    /// Releases atmoCtrl COM Object.
    /// </summary>
    private void ReleaseAtmoControl()
    {
      if (atmoCtrl != null)
      {
        Marshal.ReleaseComObject(atmoCtrl);
        atmoCtrl = null;
      }
    }

    /// <summary>
    /// Start method for AtmoLight.
    /// </summary>
    public void Start()
    {
      if (atmoCtrl != null)
      {
        Log.Debug("AtmoLight: Initializing Event Handler.");

        g_Player.PlayBackStarted += new g_Player.StartedHandler(g_Player_PlayBackStarted);
        g_Player.PlayBackStopped += new g_Player.StoppedHandler(g_Player_PlayBackStopped);
        g_Player.PlayBackEnded += new g_Player.EndedHandler(g_Player_PlayBackEnded);

        FrameGrabber.GetInstance().OnNewFrame += new FrameGrabber.NewFrameHandler(AtmolightPlugin_OnNewFrame);

        // Workaround
        // Enum says we choose MP Live Mode, but it is actually Static color.
        if (AtmolightSettings.effectMenu == ContentEffect.MediaPortalLiveMode)
        {
          AtmolightSettings.effectMenu = ContentEffect.StaticColor;
        }

        menuEffect = AtmolightSettings.effectMenu;

        // We use an extra array for the colors so we later can reload the originals if wanted
        staticColor[0] = AtmolightSettings.staticColorRed;
        staticColor[1] = AtmolightSettings.staticColorGreen;
        staticColor[2] = AtmolightSettings.staticColorBlue;

        if (CheckForStartRequirements())
        {
          MenuMode();
        }
        else
        {
          DisableLEDs();
        }
        atmoLightPluginStarted = true;
      }
      GUIWindowManager.OnNewAction += new OnActionHandler(OnNewAction);
    }

    /// <summary>
    /// Stop method for AtmoLight.
    /// </summary>
    public void Stop()
    {
      if (atmoCtrl != null)
      {
        FrameGrabber.GetInstance().OnNewFrame += new FrameGrabber.NewFrameHandler(AtmolightPlugin_OnNewFrame);

        g_Player.PlayBackStarted -= new g_Player.StartedHandler(g_Player_PlayBackStarted);
        g_Player.PlayBackStopped -= new g_Player.StoppedHandler(g_Player_PlayBackStopped);
        g_Player.PlayBackEnded -= new g_Player.EndedHandler(g_Player_PlayBackEnded);

        GUIWindowManager.OnNewAction -= new OnActionHandler(OnNewAction);

        if (AtmolightSettings.disableOnShutdown)
        {
          DisableLEDs();
        }

        if (AtmolightSettings.enableInternalLiveView)
        {
          SetAtmoEffect(ComEffectMode.cemLivePicture);
          SetAtmoLiveViewSource(ComLiveViewSource.lvsGDI);
        }

        atmoCtrl = null;

        if (AtmolightSettings.exitAtmoWin)
        {
          KillAtmoWinA();
        }
      }
      Log.Debug("AtmoLight: Plugin Stopped.");
    }
    #endregion

    #region COM Interface
    /// <summary>
    /// Changes the AtmoWin profile.
    /// </summary>
    /// <returns>true if successfull and false if not.</returns>
    private bool SetColorMode(ComEffectMode effect)
    {
      if (atmoCtrl == null)
      {
        return false;
      }

      Log.Debug("AtmoLight: Changing AtmoWin profile (SetColorMode).");
      ComEffectMode oldEffect;
      if (TimeoutHandler(() => atmoCtrl.setEffect(effect, out oldEffect)))
      {
        Log.Info("AtmoLight: Successfully changed AtmoWin profile.");
        return true;
      }
      return false;
    }

    /// <summary>
    /// Changes the AtmoWin effect.
    /// </summary>
    /// <param name="effect">Effect to change to.</param>
    /// <returns>true if successfull and false if not.</returns>
    private bool SetAtmoEffect(ComEffectMode effect)
    {
      if (atmoCtrl == null)
      {
        return false;
      }

      Log.Debug("AtmoLight: Changing AtmoWin effect to: {0}", effect.ToString());
      ComEffectMode oldEffect;
      if (TimeoutHandler(() => atmoCtrl.setEffect(effect, out oldEffect)))
      {
        Log.Info("AtmoLight: Successfully changed AtmoWin effect to: {0}", effect.ToString());
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
      if (atmoCtrl == null)
      {
        return false;
      }

      Log.Debug("AtmoLight: Setting static color to R:{0} G:{1} B:{2}.", red, green, blue);
      if (TimeoutHandler(() => atmoCtrl.setStaticColor(red, green, blue)))
      {
        Log.Info("AtmoLight: Successfully set static color to R:{0} G:{1} B:{2}.", red, green, blue);
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
      if (atmoCtrl == null)
      {
        return false;
      }

      Log.Debug("AtmoLight: Changing AtmoWin Liveview Source to: {0}", viewSource.ToString());
      if (TimeoutHandler(() => atmoLiveViewCtrl.setLiveViewSource(viewSource)))
      {
        Log.Info("AtmoLight: Successfully changed AtmoWin Liveview Source to: {0}", viewSource.ToString());
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
      if (atmoCtrl == null)
      {
        return false;
      }

      if (TimeoutHandler(() => atmoLiveViewCtrl.getCurrentLiveViewSource(out atmoLiveViewSource)))
      {
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
      if (atmoCtrl == null)
      {
        return false;
      }

      Log.Debug("AtmoLight: Getting Liveview Resolution.");
      if (TimeoutHandler(() => atmoCtrl.getLiveViewRes(out captureWidth, out captureHeight)))
      {
        Log.Debug("AtmoLight: Liveview capture resolution is {0}x{1}. Screenshot will be resized to this dimensions.", captureWidth, captureHeight);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Opens the COM interface to AtmoWin.
    /// </summary>
    /// <returns>true if successfull and false if not.</returns>
    private bool GetAtmoLiveViewCtrl()
    {
      if (atmoCtrl == null)
      {
        return false;
      }

      Log.Debug("AtmoLight: Getting AtmoWin Live View Control.");
      if (TimeoutHandler(() => atmoLiveViewCtrl = (IAtmoLiveViewControl)Marshal.GetActiveObject("AtmoRemoteControl.1")))
      {
        Log.Debug("AtmoLight: Successfully got AtmoWin Live View Control.");
        return true;
      }
      return false;      
    }
    #endregion

    #region Control LEDs
    /// <summary>
    /// Sets the AtmoWin effect to disabled and sets the color of the leds to black.
    /// </summary>
    /// <returns>true if successfull and false if not.</returns>
    private bool DisableLEDs()
    {
      if (atmoCtrl == null)
      {
        return false;
      }
      if (currentEffect == ContentEffect.LEDsDisabled)
      {
        Log.Debug("AtmoLight: LEDs already disabled. Nothing to do.");
        return true;
      }
      currentEffect = ContentEffect.Undefined;
      atmoOff = true;
      getAtmoLiveViewSourceLock = true;
      setPixelDataLock = true;
      try
      {
        Log.Debug("AtmoLight: Disabling LEDs.");
        if (!SetAtmoEffect(ComEffectMode.cemDisabled))
        {
          return false;
        }
        // Workaround for SEDU.
        // Without the sleep it would not change to color.
        System.Threading.Thread.Sleep(delaySetStaticColor);
        if (!SetAtmoColor(0, 0, 0))
        {
          return false;
        }
      }
      catch (Exception ex)
      {
        Log.Error("AtmoLight: Failed to disable LEDs.");
        Log.Error("AtmoLight: Exception: {0}", ex.Message);
        return false;
      }
      currentEffect = ContentEffect.LEDsDisabled;
      return true;
    }

    /// <summary>
    /// Changes the effect while in the GUI (no playback).
    /// </summary>
    private void MenuMode()
    {
      if (atmoCtrl == null)
      {
        return;
      }
      // Static color gets excluded so we can actually change it.
      if ((menuEffect == currentEffect) && (currentEffect != ContentEffect.StaticColor))
      {
        Log.Debug("AtmoLight: Effect is already active. Nothing to do.");
        return;
      }
      currentEffect = ContentEffect.Undefined;
      Log.Info("AtmoLight: Changing AtmoLight effect to: {0}", menuEffect.ToString());
      switch (menuEffect)
      {
        case ContentEffect.AtmoWinLiveMode:
          atmoOff = false;
          if (!SetAtmoEffect(ComEffectMode.cemLivePicture))
          {
            return;
          }
          if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsGDI))
          {
            return;
          }
          break;
        case ContentEffect.Colorchanger:
          atmoOff = false;
          if (!SetAtmoEffect(ComEffectMode.cemColorChange))
          {
            return;
          }
          break;
        case ContentEffect.ColorchangerLR:
          atmoOff = false;
          if (!SetAtmoEffect(ComEffectMode.cemLrColorChange))
          {
            return;
          }
          break;
        case ContentEffect.LEDsDisabled:
          DisableLEDs();
          break;
        // Effect can be called "MP_Live_view" but it actually is "Static Color".
        // This should not happen anymore, but the case for it stays in for now.
        case ContentEffect.MediaPortalLiveMode:
        case ContentEffect.StaticColor:
          atmoOff = false;
          if (!SetAtmoEffect(ComEffectMode.cemDisabled))
          {
            return;
          }
          if (!SetAtmoColor((byte)staticColor[0], (byte)staticColor[1], (byte)staticColor[2]))
          {
            return;
          }
          // Workaround for SEDU.
          // Without the sleep it would not change to color.
          System.Threading.Thread.Sleep(delaySetStaticColor);
          if (!SetAtmoColor((byte)staticColor[0], (byte)staticColor[1], (byte)staticColor[2]))
          {
            return;
          }
          break;
      }
      currentEffect = menuEffect;
    }

    /// <summary>
    /// Changes the effect during playback.
    /// </summary>
    private void PlaybackMode()
    {
      if (atmoCtrl == null)
      {
        return;
      }
      // Static color gets excluded so we can actually change it.
      if ((playbackEffect == currentEffect) && (currentEffect != ContentEffect.StaticColor))
      {
        Log.Debug("AtmoLight: Effect is already active. Nothing to do.");
        return;
      }
      currentEffect = ContentEffect.Undefined;
      Log.Info("AtmoLight: Changing AtmoLight effect to: {0}", playbackEffect.ToString());
      switch (playbackEffect)
      {
        case ContentEffect.AtmoWinLiveMode:
          atmoOff = false;
          getAtmoLiveViewSourceLock = true;
          setPixelDataLock = true;
          if (!SetAtmoEffect(ComEffectMode.cemLivePicture))
          {
            return;
          }
          if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsGDI))
          {
            return;
          }
          break;
        case ContentEffect.Colorchanger:
          atmoOff = false;
          getAtmoLiveViewSourceLock = true;
          setPixelDataLock = true;
          if (!SetAtmoEffect(ComEffectMode.cemColorChange))
          {
            return;
          }
          break;
        case ContentEffect.ColorchangerLR:
          atmoOff = false;
          getAtmoLiveViewSourceLock = true;
          setPixelDataLock = true;
          if (!SetAtmoEffect(ComEffectMode.cemLrColorChange))
          {
            return;
          }
          break;
        case ContentEffect.LEDsDisabled:
          getAtmoLiveViewSourceLock = true;
          setPixelDataLock = true;
          DisableLEDs();
          break;
        case ContentEffect.MediaPortalLiveMode:
          if (!SetAtmoEffect(ComEffectMode.cemLivePicture))
          {
            return;
          }
          if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal))
          {
            return;
          }
          if (AtmolightSettings.delay)
          {
            delayRefreshRateDependant = (int)(((float)AtmolightSettings.delayReferenceRefreshRate / (float)GetRefreshRate()) * (float)AtmolightSettings.delayReferenceTime);
            Log.Debug("AtmoLight: Adding {0}ms delay to the LEDs.", delayRefreshRateDependant);
          }
          atmoOff = false;
          getAtmoLiveViewSourceLock = false;
          setPixelDataLock = false;
          Thread GetAtmoLiveViewSourceThreadHelper = new Thread(() => GetAtmoLiveViewSourceThread());
          GetAtmoLiveViewSourceThreadHelper.Start();
          break;
        case ContentEffect.StaticColor:
          atmoOff = false;
          getAtmoLiveViewSourceLock = true;
          setPixelDataLock = true;
          if (!SetAtmoEffect(ComEffectMode.cemDisabled))
          {
            return;
          }
          if (!SetAtmoColor((byte)staticColor[0], (byte)staticColor[1], (byte)staticColor[2]))
          {
            return;
          }
          // Workaround for SEDU.
          // Without the sleep it would not change to color.
          System.Threading.Thread.Sleep(delaySetStaticColor);
          if (!SetAtmoColor((byte)staticColor[0], (byte)staticColor[1], (byte)staticColor[2]))
          {
            return;
          }
          break;
      }
      currentEffect = playbackEffect;
    }

    /// <summary>
    /// Checks if PlaybackMode() or MenuMode() should be used and calls them.
    /// </summary>
    private void StartLEDs()
    {
      if (atmoCtrl == null)
      {
        return;
      }
      if (g_Player.Playing)
      {
        PlaybackMode();
      }
      else
      {
        MenuMode();
      }
    }
    #endregion

    #region Threads

    /// <summary>
    /// Sends pixel data to AtmoWin when MediaPortal liveview is used (external liveview source).
    /// Also adds a delay if specified in settings.
    /// This method is designed to run as its own thread.
    /// </summary>
    /// <param name="bmiInfoHeader">Info Header.</param>
    /// <param name="pixelData">Pixel data.</param>
    private void SetPixelDataThread(byte[] bmiInfoHeader, byte[] pixelData)
    {
      if (atmoCtrl == null)
      {
        return;
      }
      try
      {
        if (AtmolightSettings.delay)
        {
          System.Threading.Thread.Sleep(delayRefreshRateDependant);
        }
        if (g_Player.Playing && !setPixelDataLock && !atmoOff && !reInitializeLock)
        {
            atmoLiveViewCtrl.setPixelData(bmiInfoHeader, pixelData);
        }
      }
      catch (Exception ex)
      {
        Log.Error("AtmoLight: Could not send data to AtmoWin.");
        Log.Error("AtmoLight: Exception: {0}", ex.Message);

        // Try to reconnect to AtmoWin.
        // No new thread needed, as this already is not the main thread.
        ReInitializeAtmoWinConnection();
      }
    }

    /// <summary>
    /// Checks if the AtmoWin liveview source is set to external when MediaPortal liveview is used.
    /// Sets liveview source back to external if needed.
    /// This method is designed to run as its own thread.
    /// </summary>
    private void GetAtmoLiveViewSourceThread()
    {
      while (g_Player.Playing && !getAtmoLiveViewSourceLock && !atmoOff)
      {
        if (!reInitializeLock)
        {
          GetAtmoLiveViewSource();
          if (atmoLiveViewSource != ComLiveViewSource.lvsExternal)
          {
            Log.Debug("AtmoLight: AtmoWin Liveview Source is not lvsExternal");
            SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal);
          }
        }
        System.Threading.Thread.Sleep(delayGetAtmoLiveViewSource);
      }
    }
    #endregion

    #region g_player Events
    /// <summary>
    /// Checks what to do when playback is ended.
    /// </summary>
    /// <param name="type">Media type.</param>
    /// <param name="filename">Media filename.</param>
    void g_Player_PlayBackEnded(g_Player.MediaType type, string filename)
    {
      if (atmoCtrl == null)
      {
        return;
      }
      try
      {
        getAtmoLiveViewSourceLock = true;
        setPixelDataLock = true;
        if (CheckForStartRequirements())
        {
          MenuMode();
        }
        else
        {
          DisableLEDs();
        }
      }
      catch (Exception ex)
      {
        Log.Error("AtmoLight: g_Player_PlayBackEnded failed.");
        Log.Error("AtmoLight: Exception= {0}", ex.Message);
      }
    }

    /// <summary>
    /// Checks what to do when playback is stopped.
    /// </summary>
    /// <param name="type">Media type.</param>
    /// <param name="stoptime">Media stoptime.</param>
    /// <param name="filename">Media filename.</param>
    void g_Player_PlayBackStopped(g_Player.MediaType type, int stoptime, string filename)
    {
      if (atmoCtrl == null)
      {
        return;
      }
      try
      {
        getAtmoLiveViewSourceLock = true;
        setPixelDataLock = true;
        if (CheckForStartRequirements())
        {
          MenuMode();
        }
        else
        {
          DisableLEDs();
        }
      }
      catch (Exception ex)
      {
        Log.Error("AtmoLight: g_Player_PlayBackStopped failed.");
        Log.Error("AtmoLight: Exception= {0}", ex.Message);
      }
    }

    /// <summary>
    /// Checks what to do when playback is started.
    /// </summary>
    /// <param name="type">Media type.</param>
    /// <param name="filename">Media filename.</param>
    void g_Player_PlayBackStarted(g_Player.MediaType type, string filename)
    {
      if (atmoCtrl == null)
      {
        return;
      }
      try
      {
        if (type == g_Player.MediaType.Video || type == g_Player.MediaType.TV || type == g_Player.MediaType.Recording || type == g_Player.MediaType.Unknown || (type == g_Player.MediaType.Music && filename.Contains(".mkv")))
        {
          Log.Debug("AtmoLight: Video detected.");
          playbackEffect = AtmolightSettings.effectVideo;
        }
        else if (type == g_Player.MediaType.Music)
        {
          // Workaround
          // Enum says we choose MP Live Mode, but it is actually Static color.
          if (AtmolightSettings.effectMusic == ContentEffect.MediaPortalLiveMode)
          {
            AtmolightSettings.effectMusic = ContentEffect.StaticColor;
          }
          playbackEffect = AtmolightSettings.effectMusic;
          Log.Debug("AtmoLight: Music detected.");
        }
        else if (type == g_Player.MediaType.Radio)
        {
          // Workaround
          // Enum says we choose MP Live Mode, but it is actually Static color.
          if (AtmolightSettings.effectRadio == ContentEffect.MediaPortalLiveMode)
          {
            AtmolightSettings.effectRadio = ContentEffect.StaticColor;
          }
          playbackEffect = AtmolightSettings.effectRadio;
          Log.Debug("AtmoLight: Radio detected.");
        }

        if (CheckForStartRequirements())
        {
          PlaybackMode();
        }
        else
        {
          DisableLEDs();
        }
      }
      catch (Exception ex)
      {
        Log.Error("AtmoLight: g_Player_PlayBackStarted failed.");
        Log.Error("AtmoLight: Exception= {0}", ex.Message);
      }
    }

    /// <summary>
    /// Calculates the pixel data that gets send to AtmoWin.
    /// And starts the SetPixelDataThread() threads to send the data.
    /// </summary>
    /// <param name="width">Frame width.</param>
    /// <param name="height">Frame height.</param>
    /// <param name="arWidth">Aspect ratio width.</param>
    /// <param name="arHeight">Aspect ratio height.</param>
    /// <param name="pSurface">Surface.</param>
    private void AtmolightPlugin_OnNewFrame(short width, short height, short arWidth, short arHeight, uint pSurface)
    {
      if (playbackEffect != ContentEffect.MediaPortalLiveMode || atmoOff || atmoCtrl == null || width == 0 || height == 0)
      {
        return;
      }

      if (rgbSurface == null)
      {
        rgbSurface = GUIGraphicsContext.DX9Device.CreateRenderTarget(captureWidth, captureHeight, Format.A8R8G8B8,
          MultiSampleType.None, 0, true);
      }
      unsafe
      {
        try
        {
          if (AtmolightSettings.sbs3dOn)
          {
            VideoSurfaceToRGBSurfaceExt(new IntPtr(pSurface), width / 2, height, (IntPtr)rgbSurface.UnmanagedComPointer,
              captureWidth, captureHeight);
          }
          else
          {
            VideoSurfaceToRGBSurfaceExt(new IntPtr(pSurface), width, height, (IntPtr)rgbSurface.UnmanagedComPointer,
              captureWidth, captureHeight);
          }

          Microsoft.DirectX.GraphicsStream stream = SurfaceLoader.SaveToStream(ImageFileFormat.Bmp, rgbSurface);

          BinaryReader reader = new BinaryReader(stream);
          stream.Position = 0; // ensure that what start at the beginning of the stream. 
          reader.ReadBytes(14); // skip bitmap file info header
          byte[] bmiInfoHeader = reader.ReadBytes(4 + 4 + 4 + 2 + 2 + 4 + 4 + 4 + 4 + 4 + 4);

          int rgbL = (int)(stream.Length - stream.Position);
          int rgb = (int)(rgbL / (captureWidth * captureHeight));

          byte[] pixelData = reader.ReadBytes((int)(stream.Length - stream.Position));

          byte[] h1pixelData = new byte[captureWidth * rgb];
          byte[] h2pixelData = new byte[captureWidth * rgb];
          //now flip horizontally, we do it always to prevent microstudder
          int i;
          for (i = 0; i < ((captureHeight / 2) - 1); i++)
          {
            Array.Copy(pixelData, i * captureWidth * rgb, h1pixelData, 0, captureWidth * rgb);
            Array.Copy(pixelData, (captureHeight - i - 1) * captureWidth * rgb, h2pixelData, 0, captureWidth * rgb);
            Array.Copy(h1pixelData, 0, pixelData, (captureHeight - i - 1) * captureWidth * rgb, captureWidth * rgb);
            Array.Copy(h2pixelData, 0, pixelData, i * captureWidth * rgb, captureWidth * rgb);
          }
          //send scaled and fliped frame to atmowin
          if (!AtmolightSettings.lowCPU ||
              (((Win32API.GetTickCount() - lastFrame) > AtmolightSettings.lowCPUTime) && AtmolightSettings.lowCPU))
          {
            if (AtmolightSettings.lowCPU)
            {
              lastFrame = Win32API.GetTickCount();
            }
            // Create and start a new thread to send the data to AtmoWin.
            // This is done so we can use the TimeoutHandler without halting or disturbing the video playback.
            Thread SetPixelDataThreadHelper = new Thread(() => SetPixelDataThread(bmiInfoHeader, pixelData));
            // Priority gets set higher to ensure that the data gets send as soon as possible.
            SetPixelDataThreadHelper.Priority = ThreadPriority.AboveNormal;
            SetPixelDataThreadHelper.Start();
          }
          stream.Close();
          stream.Dispose();
        }
        catch (Exception ex)
        {
          Log.Error("AtmoLight: Error in AtmolightPlugin_OnNewFrame.");
          Log.Error("AtmoLight: Exception: {0}", ex.Message);

          rgbSurface.Dispose();
          rgbSurface = null;

          // Try to reconnect to AtmoWin.
          // Thread needed to not halt the general playback.
          Thread ReInitializeAtmoWinConnectionHelperThread = new Thread(() => ReInitializeAtmoWinConnection());
          ReInitializeAtmoWinConnectionHelperThread.Start();
        }
      }
    }
    #endregion

    #region Remote Button Events
    /// <summary>
    /// Event handler for remote button presses.
    /// </summary>
    /// <param name="action">Action caused by remote button press.</param>
    public void OnNewAction(MediaPortal.GUI.Library.Action action)
    {
      // Remote Key to open Menu
      if ((action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_YELLOW_BUTTON && AtmolightSettings.menuButton == 2) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_GREEN_BUTTON && AtmolightSettings.menuButton == 1) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_RED_BUTTON && AtmolightSettings.menuButton == 0) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_BLUE_BUTTON && AtmolightSettings.menuButton == 3))
      {
        if (atmoCtrl == null)
        {
          if (DialogYesNo(LanguageLoader.appStrings.ContextMenu_ConnectLine1, LanguageLoader.appStrings.ContextMenu_ConnectLine2))
          {
            if (ReInitializeAtmoWinConnection(true) && !atmoLightPluginStarted)
            {
              Start();
            }
          }
        }
        else
        {
          DialogContextMenu();
        }
      }

      // No connection
      if (atmoCtrl == null)
      {
        return;
      }

      // Remote Key to toggle On/Off
      if ((action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_YELLOW_BUTTON && AtmolightSettings.killButton == 2) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_GREEN_BUTTON && AtmolightSettings.killButton == 1) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_RED_BUTTON && AtmolightSettings.killButton == 0) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_BLUE_BUTTON && AtmolightSettings.killButton == 3))
      {
        if (atmoOff)
        {
          StartLEDs();
        }
        else
        {
          DisableLEDs();
        }
      }

      // Remote Key to change Profiles
      else if ((action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_YELLOW_BUTTON && AtmolightSettings.profileButton == 2) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_GREEN_BUTTON && AtmolightSettings.profileButton == 1) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_RED_BUTTON && AtmolightSettings.profileButton == 0) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_BLUE_BUTTON && AtmolightSettings.profileButton == 3))
      {
        SetColorMode(ComEffectMode.cemColorMode);
      }
    }
    #endregion

    #region Context Menu
    /// <summary>
    /// Prompts the user with an on screen keyboard.
    /// </summary>
    /// <param name="keyboardString">String already in the keyboard when opened.</param>
    /// <returns>String entered by the user.</returns>
    private string GetKeyboardString(string keyboardString)
    {
      VirtualKeyboard Keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
      if (Keyboard == null)
      {
        return null;
      }
      Keyboard.IsSearchKeyboard = true;
      Keyboard.Reset();
      Keyboard.Text = keyboardString;
      Keyboard.DoModal(GUIWindowManager.ActiveWindow);
      if (Keyboard.IsConfirmed)
      {
        return Keyboard.Text;
      }
      return null;
    }

    /// <summary>
    /// Prompts the user with an error dialog.
    /// </summary>
    /// <param name="setLine1">First line.</param>
    /// <param name="setLine2">Second line.</param>
    private void DialogError(string setLine1 = null, string setLine2 = null)
    {
      GUIDialogOK dlgError = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
      if (dlgError != null)
      {
        dlgError.SetHeading(LanguageLoader.appStrings.ContextMenu_Error + "!");
        dlgError.SetLine(1, setLine1);
        dlgError.SetLine(2, setLine2);
        dlgError.DoModal(GUIWindowManager.ActiveWindow);
      }
    }

    /// <summary>
    /// Prompts the user with the context menu.
    /// </summary>
    private void DialogContextMenu()
    {
      int delayTogglePos = 4;
      int delayChangePos = 5;
      int staticColorPos = 4;
      Log.Info("AtmoLight: Opening AtmoLight context menu.");

      // Showing context menu
      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      dlg.Reset();
      dlg.SetHeading("AtmoLight");

      // Toggle On/Off
      if (atmoOff)
      {
        dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_SwitchLEDsON));
      }
      else
      {
        dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_SwitchLEDsOFF));
      }

      // Change Effect
      dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_ChangeEffect));

      // Change AtmoWin Profile
      dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_ChangeAWProfile));

      // Toggle 3D Mode
      if (AtmolightSettings.sbs3dOn)
      {
        dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Switch3DOFF));
      }
      else
      {
        dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Switch3DON));
      }

      // Delay
      if ((g_Player.Playing) && (playbackEffect == ContentEffect.MediaPortalLiveMode))
      {
        // Toggle Delay
        if (AtmolightSettings.delay)
        {
          dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_DelayOFF));
        }
        else
        {
          dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_DelayON));
        }
        staticColorPos++;

        // Change Delay
        if (AtmolightSettings.delay)
        {
          dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_ChangeDelay + " (" + delayRefreshRateDependant + "ms)"));
          staticColorPos++;
        }
        else
        {
          delayChangePos = -1;
        }
      }
      else
      {
        delayTogglePos = -1;
        delayChangePos = -1;
      }

      // Change Static Color
      if (((g_Player.Playing) && (playbackEffect == ContentEffect.StaticColor) && (!atmoOff)) ||
          ((!g_Player.Playing) && (menuEffect == ContentEffect.StaticColor) && (!atmoOff)))
      {
        dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_ChangeStatic));
      }
      else
      {
        staticColorPos = -1;
      }

      dlg.SelectedLabel = 0;
      dlg.DoModal(GUIWindowManager.ActiveWindow);

      // Do stuff
      if (dlg.SelectedLabel == 0)
      {
        if (atmoOff)
        {
          StartLEDs();
        }
        else
        {
          DisableLEDs();
        }
      }
      else if (dlg.SelectedLabel == 1)
      {
        if (g_Player.Playing)
        {
          GUIDialogMenu dlgEffect = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          dlgEffect.Reset();
          dlgEffect.SetHeading(LanguageLoader.appStrings.ContextMenu_ChangeEffect);
          dlgEffect.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_LEDsDisabled));
          dlgEffect.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_MPLive));
          dlgEffect.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_AWLive));
          dlgEffect.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Colorchanger));
          dlgEffect.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_ColorchangerLR));
          dlgEffect.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_StaticColor));
          dlgEffect.SelectedLabel = 0;
          dlgEffect.DoModal(GUIWindowManager.ActiveWindow);

          switch (dlgEffect.SelectedLabel)
          {
            case 0:
              playbackEffect = ContentEffect.LEDsDisabled;
              DisableLEDs();
              break;
            case 1:
              playbackEffect = ContentEffect.MediaPortalLiveMode;
              PlaybackMode();
              break;
            case 2:
              playbackEffect = ContentEffect.AtmoWinLiveMode;
              PlaybackMode();
              break;
            case 3:
              playbackEffect = ContentEffect.Colorchanger;
              PlaybackMode();
              break;
            case 4:
              playbackEffect = ContentEffect.ColorchangerLR;
              PlaybackMode();
              break;
            case 5:
              playbackEffect = ContentEffect.StaticColor;
              PlaybackMode();
              break;
          }
        }
        else
        {
          GUIDialogMenu dlgEffect = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
          dlgEffect.Reset();
          dlgEffect.SetHeading(LanguageLoader.appStrings.ContextMenu_ChangeEffect);
          dlgEffect.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_LEDsDisabled));
          dlgEffect.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_AWLive));
          dlgEffect.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Colorchanger));
          dlgEffect.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_ColorchangerLR));
          dlgEffect.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_StaticColor));
          dlgEffect.SelectedLabel = 0;
          dlgEffect.DoModal(GUIWindowManager.ActiveWindow);

          switch (dlgEffect.SelectedLabel)
          {
            case 0:
              menuEffect = ContentEffect.LEDsDisabled;
              DisableLEDs();
              break;
            case 1:
              menuEffect = ContentEffect.AtmoWinLiveMode;
              MenuMode();
              break;
            case 2:
              menuEffect = ContentEffect.Colorchanger;
              MenuMode();
              break;
            case 3:
              menuEffect = ContentEffect.ColorchangerLR;
              MenuMode();
              break;
            case 4:
              menuEffect = ContentEffect.StaticColor;
              MenuMode();
              break;
          }
        }
      }
      else if (dlg.SelectedLabel == 2)
      {
        SetColorMode(ComEffectMode.cemColorMode);
      }
      else if (dlg.SelectedLabel == 3)
      {
        if (AtmolightSettings.sbs3dOn)
        {
          Log.Info("AtmoLight: Switching SBS 3D mode off.");
          AtmolightSettings.sbs3dOn = false;
        }
        else
        {
          Log.Info("AtmoLight: Switching SBS 3D mode on.");
          AtmolightSettings.sbs3dOn = true;
        }
      }
      else if ((dlg.SelectedLabel == delayTogglePos) && (delayTogglePos != -1))
      {
        if (AtmolightSettings.delay)
        {
          Log.Info("AtmoLight: Switching LED delay off.");
          AtmolightSettings.delay = false;
        }
        else
        {
          Log.Info("AtmoLight: Switching LED delay on.");
          AtmolightSettings.delay = true;
          delayRefreshRateDependant = (int)(((float)AtmolightSettings.delayReferenceRefreshRate / (float)GetRefreshRate()) * (float)AtmolightSettings.delayReferenceTime);
          Log.Debug("AtmoLight: Adding {0}ms delay to the LEDs.", delayRefreshRateDependant);
        }
      }
      else if ((dlg.SelectedLabel == delayChangePos) && (delayChangePos != -1))
      {
        if ((int.TryParse(GetKeyboardString(""), out delayTimeHelper)) && (delayTimeHelper >= 0) && (delayTimeHelper <= 1000))
        {
          Log.Info("AtmoLight: Changing LED delay to {0}ms.", delayTimeHelper);
          delayRefreshRateDependant = delayTimeHelper;
          AtmolightSettings.delayReferenceTime = (int)(((float)delayRefreshRateDependant * (float)GetRefreshRate()) / AtmolightSettings.delayReferenceRefreshRate);
          //AtmolightSettings.SaveSpecificSetting("delayTime", AtmolightSettings.delayReferenceTime.ToString());
        }
        else
        {
          DialogError(LanguageLoader.appStrings.ContextMenu_DelayTimeErrorLine1, LanguageLoader.appStrings.ContextMenu_DelayTimeErrorLine2);
        }
      }
      else if ((dlg.SelectedLabel == staticColorPos) && (staticColorPos != -1))
      {
        GUIDialogMenu dlgStaticColor = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
        dlgStaticColor.Reset();
        dlgStaticColor.SetHeading(LanguageLoader.appStrings.ContextMenu_ChangeStatic);
        dlgStaticColor.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Manual));
        dlgStaticColor.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_SaveColor));
        dlgStaticColor.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_LoadColor));
        dlgStaticColor.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_White));
        dlgStaticColor.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Red));
        dlgStaticColor.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Green));
        dlgStaticColor.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Blue));
        dlgStaticColor.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Cyan));
        dlgStaticColor.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Magenta));
        dlgStaticColor.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Yellow));
        dlgStaticColor.SelectedLabel = 0;
        dlgStaticColor.DoModal(GUIWindowManager.ActiveWindow);

        switch (dlgStaticColor.SelectedLabel)
        {
          case 0:
            DialogRGBManualStaticColorChanger();
            break;
          case 1:
            AtmolightSettings.SaveSpecificSetting("StaticColorRed", staticColor[0].ToString());
            AtmolightSettings.staticColorRed = staticColor[0];
            AtmolightSettings.SaveSpecificSetting("StaticColorGreen", staticColor[1].ToString());
            AtmolightSettings.staticColorGreen = staticColor[1];
            AtmolightSettings.SaveSpecificSetting("StaticColorBlue", staticColor[2].ToString());
            AtmolightSettings.staticColorBlue = staticColor[2];
            break;
          case 2:
            staticColor[0] = AtmolightSettings.staticColorRed;
            staticColor[1] = AtmolightSettings.staticColorGreen;
            staticColor[2] = AtmolightSettings.staticColorBlue;
            break;
          case 3:
            staticColor[0] = 255;
            staticColor[1] = 255;
            staticColor[2] = 255;
            break;
          case 4:
            staticColor[0] = 255;
            staticColor[1] = 0;
            staticColor[2] = 0;
            break;
          case 5:
            staticColor[0] = 0;
            staticColor[1] = 255;
            staticColor[2] = 0;
            break;
          case 6:
            staticColor[0] = 0;
            staticColor[1] = 0;
            staticColor[2] = 255;
            break;
          case 7:
            staticColor[0] = 0;
            staticColor[1] = 255;
            staticColor[2] = 255;
            break;
          case 8:
            staticColor[0] = 255;
            staticColor[1] = 0;
            staticColor[2] = 255;
            break;
          case 9:
            staticColor[0] = 255;
            staticColor[1] = 255;
            staticColor[2] = 0;
            break;
        }
        StartLEDs();
      }
    }

    /// <summary>
    /// Prompts the user with a Yes/No dialog.
    /// </summary>
    /// <param name="setLine1">String for the first line.</param>
    /// <param name="setLine2">String for the second line.</param>
    /// <returns>true for yes and false for no.</returns>
    private bool DialogYesNo(string setLine1, string setLine2 = "")
    {
      Log.Info("AtmoLight: Opening AtmoLight Yes/No dialog.");

      // Showing Yes/No dialog
      GUIDialogYesNo dlgYesNo = (GUIDialogYesNo)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_YES_NO);
      if (dlgYesNo != null)
      {
        dlgYesNo.Reset();
        dlgYesNo.SetHeading("AtmoLight");
        dlgYesNo.SetLine(1, setLine1);
        dlgYesNo.SetLine(2, setLine2);
        dlgYesNo.SetDefaultToYes(true);
        dlgYesNo.DoModal(GUIWindowManager.ActiveWindow);
        return dlgYesNo.IsConfirmed;
      }
      return false;
    }

    /// <summary>
    /// Prompts the user with a context menu to set a static color.
    /// </summary>
    /// <param name="Reset">Parameter to reset the colors.</param>
    /// <param name="StartPosition">Element that should be highlighted.</param>
    private void DialogRGBManualStaticColorChanger(bool Reset = true, int StartPosition = 0)
    {
      if (Reset)
      {
        staticColorTemp = new int[] { -1, -1, -1 };
      }
      GUIDialogMenu dlgRGB = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      dlgRGB.Reset();
      dlgRGB.SetHeading(LanguageLoader.appStrings.ContextMenu_ManualStaticColor);
      dlgRGB.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Red + ": " + (staticColorTemp[0] == -1 ? LanguageLoader.appStrings.ContextMenu_NA : staticColorTemp[0].ToString())));
      dlgRGB.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Green + ": " + (staticColorTemp[1] == -1 ? LanguageLoader.appStrings.ContextMenu_NA : staticColorTemp[1].ToString())));
      dlgRGB.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Blue + ": " + (staticColorTemp[2] == -1 ? LanguageLoader.appStrings.ContextMenu_NA : staticColorTemp[2].ToString())));
      dlgRGB.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Apply));
      dlgRGB.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Cancel));
      dlgRGB.SelectedLabel = StartPosition;
      dlgRGB.DoModal(GUIWindowManager.ActiveWindow);
      switch (dlgRGB.SelectedLabel)
      {
        case -1:
          return;
        case 0:
        case 1:
        case 2:
          if ((int.TryParse(GetKeyboardString((staticColorTemp[dlgRGB.SelectedLabel] == -1 ? "" : staticColorTemp[dlgRGB.SelectedLabel].ToString())), out staticColorHelper)) && (staticColorHelper >= 0) && (staticColorHelper <= 255))
          {
            staticColorTemp[dlgRGB.SelectedLabel] = staticColorHelper;
          }
          else
          {
            DialogError(LanguageLoader.appStrings.ContextMenu_RGBErrorLine1, LanguageLoader.appStrings.ContextMenu_RGBErrorLine2);
          }
          break;
        case 3:
          if (staticColorTemp[0] == -1 || staticColorTemp[1] == -1 || staticColorTemp[2] == -1)
          {
            DialogError(LanguageLoader.appStrings.ContextMenu_RGBErrorLine1, LanguageLoader.appStrings.ContextMenu_RGBErrorLine2);
            break;
          }
          else
          {
            staticColor = staticColorTemp;
            return;
          }
        case 4:
          return;
      }
      // Start the dialog again (without reset) so we can enter the other colors.
      DialogRGBManualStaticColorChanger(false, dlgRGB.SelectedLabel);
    }
    #endregion

    #region ISetupForm impementation
    /// <summary>
    ///  Returns authors name.
    /// </summary>
    /// <returns>Authors name.</returns>
    public string Author()
    {
      return "gemx";
    }

    /// <summary>
    /// Returns if this plugin can be enabled.
    /// </summary>
    /// <returns>true</returns>
    public bool CanEnable()
    {
      return true;
    }

    /// <summary>
    /// Returns if this plugin is enabled by default.
    /// </summary>
    /// <returns>true</returns>
    public bool DefaultEnabled()
    {
      return true;
    }

    /// <summary>
    /// Returns the description of this plugin.
    /// </summary>
    /// <returns>Description</returns>
    public string Description()
    {
      return "Interfaces AtmowinA.exe via COM to control the lights";
    }

    /// <summary>
    /// Standard window plugin method
    /// </summary>
    /// <param name="strButtonText"></param>
    /// <param name="strButtonImage"></param>
    /// <param name="strButtonImageFocus"></param>
    /// <param name="strPictureImage"></param>
    /// <returns>false</returns>
    public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage)
    {
      strButtonText = null;
      strButtonImage = null;
      strButtonImageFocus = null;
      strPictureImage = null;
      return false;
    }

    /// <summary>
    /// Returns plugin window id
    /// </summary>
    /// <returns>-1</returns>
    public int GetWindowId()
    {
      return -1;
    }

    /// <summary>
    /// Returns if this plugin has a setup.
    /// </summary>
    /// <returns>true</returns>
    public bool HasSetup()
    {
      return true;
    }

    /// <summary>
    /// Returns the Plugin name.
    /// </summary>
    /// <returns>AtmoLight</returns>
    public string PluginName()
    {
      return "AtmoLight";
    }

    /// <summary>
    /// Opens the AtmoLight setuo form.
    /// </summary>
    public void ShowPlugin()
    {
      new AtmolightSetupForm().ShowDialog();
    }

    /// <summary>
    /// Returns if this plugin should be shown on home.
    /// </summary>
    /// <returns>false</returns>
    public bool ShowDefaultHome()
    {
      return false;
    }
    #endregion
  }
}
