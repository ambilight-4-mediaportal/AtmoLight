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
//using AtmoWinRemoteControl;
//using System.Drawing;
using System.IO;
using Microsoft.DirectX.Direct3D;
using MediaPortal.Dialogs;
using MediaPortal.Configuration;
using Language;

namespace AtmoLight
{
  [PluginIcons("AtmoLight.Resources.Enabled.png", "AtmoLight.Resources.Disabled.png")]
  public class Plugin : ISetupForm, IPlugin
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
    // States
    private ContentEffect playbackEffect = ContentEffect.Undefined; // Effect for current placback
    private ContentEffect menuEffect = ContentEffect.Undefined; // Effect in GUI (no playback)

    // Timings
    private const int timeoutComInterface = 5000; // Timeout for the COM interface
    private const int delaySetStaticColor = 20; // SEDU workaround delay time
    private const int delayAtmoWinConnecting = 1000; // Delay between starting AtmoWin and connection to it
    private const int delayGetAtmoLiveViewSource = 1000; // Delay between liveview source checks

    // Frame Fields
    private Surface rgbSurface = null; // RGB Surface
    private Int64 lastFrame = 0; // Tick count of the last frame
    
    // Static Color
    private int[] staticColorTemp = { 0, 0, 0 }; // Temp array to change static color
    private int staticColorHelper; // Helper var for static color change
    
    // Delay Feature
    private int delayTimeHelper; // Helper var for delay time change

    public Core AtmoLightObject;
    #endregion

    #region Plugin Ctor/Start/Stop
    /// <summary>
    /// AtmoLight constructor.
    /// Loads the plugin, loads the settings and initializes AtmoWin and the connection.
    /// </summary>
    public Plugin()
    {
      if (MPSettings.Instance.GetValueAsBool("plugins", "AtmoLight", true))
      {
        // Log Handler
        Log.OnNewLog += new Log.NewLogHandler(OnNewLog);

        var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        DateTime buildDate = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTime;
        Log.Info("AtmoLight: Version {0}.{1}.{2}.{3}, build on {4} at {5}.", version.Major, version.Minor, version.Build, version.Revision, buildDate.ToShortDateString(), buildDate.ToLongTimeString());
        Log.Debug("AtmoLight: Loading settings.");
        Settings.LoadSettings();
      }
    }

    /// <summary>
    /// Start point of the plugin.
    /// This method gets called by MediaPortal.
    /// </summary>
    public void Start()
    {
      Log.Debug("AtmoLight: Initialising event handler.");

      // g_Player Handler
      g_Player.PlayBackStarted += new g_Player.StartedHandler(g_Player_PlayBackStarted);
      g_Player.PlayBackStopped += new g_Player.StoppedHandler(g_Player_PlayBackStopped);
      g_Player.PlayBackEnded += new g_Player.EndedHandler(g_Player_PlayBackEnded);

      // FrameGrabber Handler
      MediaPortal.FrameGrabber.GetInstance().OnNewFrame += new MediaPortal.FrameGrabber.NewFrameHandler(AtmolightPlugin_OnNewFrame);

      // Button Handler
      GUIWindowManager.OnNewAction += new OnActionHandler(OnNewAction);

      // Connection Lost Handler

      Core.OnNewConnectionLost += new Core.NewConnectionLostHandler(OnNewConnectionLost);

      // Workaround
      // Enum says we choose MP Live Mode, but it is actually Static color.
      if (Settings.effectMenu == ContentEffect.MediaPortalLiveMode)
      {
        Settings.effectMenu = ContentEffect.StaticColor;
      }
      menuEffect = Settings.effectMenu;

      staticColorTemp[0] = Settings.staticColorRed;
      staticColorTemp[1] = Settings.staticColorGreen;
      staticColorTemp[2] = Settings.staticColorBlue;

      Log.Debug("Generating new AtmoLight.Core instance.");
      AtmoLightObject = new Core(Settings.atmowinExe, Settings.restartOnError, Settings.startAtmoWin, staticColorTemp, Settings.delay, Settings.delayReferenceTime);

      if (!AtmoLightObject.Initialise())
      {
        Log.Error("Initialising failed.");
        return; 
      }

      if (CheckForStartRequirements())
       {
         AtmoLightObject.ChangeEffect(menuEffect);
       }
       else
       {
         AtmoLightObject.ChangeEffect(ContentEffect.LEDsDisabled);
       }
    }

    /// <summary>
    /// Stop point of the plugin.
    /// This method gets called by MediaPortal.
    /// </summary>
    public void Stop()
    {
      MediaPortal.FrameGrabber.GetInstance().OnNewFrame -= new MediaPortal.FrameGrabber.NewFrameHandler(AtmolightPlugin_OnNewFrame);

      g_Player.PlayBackStarted -= new g_Player.StartedHandler(g_Player_PlayBackStarted);
      g_Player.PlayBackStopped -= new g_Player.StoppedHandler(g_Player_PlayBackStopped);
      g_Player.PlayBackEnded -= new g_Player.EndedHandler(g_Player_PlayBackEnded);

      GUIWindowManager.OnNewAction -= new OnActionHandler(OnNewAction);

      if (Settings.disableOnShutdown)
      {
        AtmoLightObject.ChangeEffect(ContentEffect.LEDsDisabled);
      }

      if (Settings.enableInternalLiveView)
      {
        AtmoLightObject.ChangeEffect(ContentEffect.AtmoWinLiveMode);
      }

      AtmoLightObject.Disconnect();

      if (Settings.exitAtmoWin)
      {
        AtmoLightObject.StopAtmoWin();
      }

      Log.Debug("Plugin Stopped.");

      Log.OnNewLog -= new Log.NewLogHandler(OnNewLog);
    }
    #endregion

    #region Utilities
    /// <summary>
    /// Check if LEDs should be activated.
    /// </summary>
    /// <returns>true or false</returns>
    private bool CheckForStartRequirements()
    {
      if (!AtmoLightObject.IsConnected())
      {
        return false;
      }
      if (Settings.manualMode)
      {
        Log.Debug("LEDs should be deactivated. (Manual Mode)");
        return false;
      }
      else if ((DateTime.Now.TimeOfDay >= Settings.excludeTimeStart.TimeOfDay && DateTime.Now.TimeOfDay <= Settings.excludeTimeEnd.TimeOfDay))
      {
        Log.Debug("LEDs should be deactivated. (Timeframe)");
        return false;
      }
      else
      {
        Log.Debug("LEDs can be activated.");
        return true;
      }
    }

    /// <summary>
    /// Return the current refresh rate.
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
    #endregion

    #region Log Event Handler
    /// <summary>
    /// Event Handler for logging.
    /// This event gets called if logging is done from Core or from Plugin.
    /// </summary>
    /// <param name="logLevel">Log level</param>
    /// <param name="format">Format</param>
    /// <param name="args">Arguments</param>
    private void OnNewLog(Log.LogLevel logLevel, string format, params object[] args)
    {
      switch (logLevel)
      {
        case Log.LogLevel.Debug:
          MediaPortal.GUI.Library.Log.Debug(String.Format(format, args));
          break;
        case Log.LogLevel.Warn:
          MediaPortal.GUI.Library.Log.Warn(String.Format(format, args));
          break;
        case Log.LogLevel.Info:
          MediaPortal.GUI.Library.Log.Info(String.Format(format, args));
          break;
        case Log.LogLevel.Error:
          MediaPortal.GUI.Library.Log.Error(String.Format(format, args));
          break;
      }
    }
    #endregion

    #region Connection Lost Handler
    /// <summary>
    /// Connection lost event handler.
    /// This event gets called if connection to AtmoWin is lost and not recoverable.
    /// </summary>
    private void OnNewConnectionLost()
    {
      DialogError(LanguageLoader.appStrings.ContextMenu_AtmoWinConnectionLost);
    }
    #endregion

    #region g_Player Event Handler
    /// <summary>
    /// Playback started event handler.
    /// This event handler gets called when playback starts.
    /// </summary>
    /// <param name="type">Media type</param>
    /// <param name="filename">Media filename</param>
    void g_Player_PlayBackStarted(g_Player.MediaType type, string filename)
    {
      try
      {
        if (type == g_Player.MediaType.Video || type == g_Player.MediaType.TV || type == g_Player.MediaType.Recording || type == g_Player.MediaType.Unknown || (type == g_Player.MediaType.Music && filename.Contains(".mkv")))
        {
          Log.Debug("Video detected.");
          playbackEffect = Settings.effectVideo;
        }
        else if (type == g_Player.MediaType.Music)
        {
          // Workaround
          // Enum says we choose MP Live Mode, but it is actually Static color.
          if (Settings.effectMusic == ContentEffect.MediaPortalLiveMode)
          {
            Settings.effectMusic = ContentEffect.StaticColor;
          }
          playbackEffect = Settings.effectMusic;
          Log.Debug("Music detected.");
        }
        else if (type == g_Player.MediaType.Radio)
        {
          // Workaround
          // Enum says we choose MP Live Mode, but it is actually Static color.
          if (Settings.effectRadio == ContentEffect.MediaPortalLiveMode)
          {
            Settings.effectRadio = ContentEffect.StaticColor;
          }
          playbackEffect = Settings.effectRadio;
          Log.Debug("Radio detected.");
        }

        if (CheckForStartRequirements())
        {
          AtmoLightObject.ChangeEffect(playbackEffect);
        }
        else
        {
          AtmoLightObject.ChangeEffect(ContentEffect.LEDsDisabled);
        }
      }
      catch (Exception ex)
      {
        Log.Error("g_Player_PlayBackStarted failed.");
        Log.Error("Exception= {0}", ex.Message);
      }
    }

    /// <summary>
    /// Checks what to do when playback is ended.
    /// </summary>
    /// <param name="type">Media type.</param>
    /// <param name="filename">Media filename.</param>
    void g_Player_PlayBackEnded(g_Player.MediaType type, string filename)
    {
      if (!AtmoLightObject.IsConnected())
      {
        return;
      }
      try
      {
        if (CheckForStartRequirements())
        {
          AtmoLightObject.ChangeEffect(menuEffect);
        }
        else
        {
          AtmoLightObject.ChangeEffect(ContentEffect.LEDsDisabled);
        }
      }
      catch (Exception ex)
      {
        Log.Error("g_Player_PlayBackEnded failed.");
        Log.Error("Exception= {0}", ex.Message);
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
      if (!AtmoLightObject.IsConnected())
      {
        return;
      }
      try
      {
        if (CheckForStartRequirements())
        {
          AtmoLightObject.ChangeEffect(menuEffect);
        }
        else
        {
          AtmoLightObject.ChangeEffect(ContentEffect.LEDsDisabled);
        }
      }
      catch (Exception ex)
      {
        Log.Error("g_Player_PlayBackStopped failed.");
        Log.Error("Exception= {0}", ex.Message);
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
      if (AtmoLightObject.GetCurrentEffect() != ContentEffect.MediaPortalLiveMode || !AtmoLightObject.IsConnected() || !AtmoLightObject.IsAtmoLightOn() || width == 0 || height == 0)
      {
        return;
      }

      // Low CPU setting.
      // Skip frame if LowCPUTime has not yet passed since last frame.
      if (Settings.lowCPU)
      {
        if ((Win32API.GetTickCount() - lastFrame) < Settings.lowCPUTime)
        {
          return;
        }
        else
        {
          lastFrame = Win32API.GetTickCount();
        }
      }

      if (rgbSurface == null)
      {
        rgbSurface = GUIGraphicsContext.DX9Device.CreateRenderTarget(AtmoLightObject.GetCaptureWidth(), AtmoLightObject.GetCaptureHeight(), Format.A8R8G8B8,
          MultiSampleType.None, 0, true);
      }
      unsafe
      {
        try
        {
          if (Settings.sbs3dOn)
          {
            VideoSurfaceToRGBSurfaceExt(new IntPtr(pSurface), width / 2, height, (IntPtr)rgbSurface.UnmanagedComPointer,
              AtmoLightObject.GetCaptureWidth(), AtmoLightObject.GetCaptureHeight());
          }
          else
          {
            VideoSurfaceToRGBSurfaceExt(new IntPtr(pSurface), width, height, (IntPtr)rgbSurface.UnmanagedComPointer,
              AtmoLightObject.GetCaptureWidth(), AtmoLightObject.GetCaptureHeight());
          }

          Microsoft.DirectX.GraphicsStream stream = SurfaceLoader.SaveToStream(ImageFileFormat.Bmp, rgbSurface);

          BinaryReader reader = new BinaryReader(stream);
          stream.Position = 0; // ensure that what start at the beginning of the stream. 
          reader.ReadBytes(14); // skip bitmap file info header
          byte[] bmiInfoHeader = reader.ReadBytes(4 + 4 + 4 + 2 + 2 + 4 + 4 + 4 + 4 + 4 + 4);

          int rgbL = (int)(stream.Length - stream.Position);
          int rgb = (int)(rgbL / (AtmoLightObject.GetCaptureWidth() * AtmoLightObject.GetCaptureHeight()));

          byte[] pixelData = reader.ReadBytes((int)(stream.Length - stream.Position));

          byte[] h1pixelData = new byte[AtmoLightObject.GetCaptureWidth() * rgb];
          byte[] h2pixelData = new byte[AtmoLightObject.GetCaptureWidth() * rgb];
          //now flip horizontally, we do it always to prevent microstudder
          int i;
          for (i = 0; i < ((AtmoLightObject.GetCaptureHeight() / 2) - 1); i++)
          {
            Array.Copy(pixelData, i * AtmoLightObject.GetCaptureWidth() * rgb, h1pixelData, 0, AtmoLightObject.GetCaptureWidth() * rgb);
            Array.Copy(pixelData, (AtmoLightObject.GetCaptureHeight() - i - 1) * AtmoLightObject.GetCaptureWidth() * rgb, h2pixelData, 0, AtmoLightObject.GetCaptureWidth() * rgb);
            Array.Copy(h1pixelData, 0, pixelData, (AtmoLightObject.GetCaptureHeight() - i - 1) * AtmoLightObject.GetCaptureWidth() * rgb, AtmoLightObject.GetCaptureWidth() * rgb);
            Array.Copy(h2pixelData, 0, pixelData, i * AtmoLightObject.GetCaptureWidth() * rgb, AtmoLightObject.GetCaptureWidth() * rgb);
          }
          //send scaled and fliped frame to atmowin

          if (AtmoLightObject.IsDelayEnabled())
          {
            AtmoLightObject.AddDelayListItem(bmiInfoHeader, pixelData);
          }
          else
          {
            AtmoLightObject.SetPixelData(bmiInfoHeader, pixelData);
          }
          stream.Close();
          stream.Dispose();
        }
        catch (Exception ex)
        {
          Log.Error("Error in AtmolightPlugin_OnNewFrame.");
          Log.Error("Exception: {0}", ex.Message);

          rgbSurface.Dispose();
          rgbSurface = null;
        }
      }
    }
    #endregion

    #region Button Event Handler
    /// <summary>
    /// Event handler for remote button presses.
    /// </summary>
    /// <param name="action">Action caused by remote button press.</param>
    public void OnNewAction(MediaPortal.GUI.Library.Action action)
    {
      // Remote Key to open Menu
      if ((action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_YELLOW_BUTTON && Settings.menuButton == 2) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_GREEN_BUTTON && Settings.menuButton == 1) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_RED_BUTTON && Settings.menuButton == 0) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_BLUE_BUTTON && Settings.menuButton == 3))
      {
        if (!AtmoLightObject.IsConnected())
        {
          if (DialogYesNo(LanguageLoader.appStrings.ContextMenu_ConnectLine1, LanguageLoader.appStrings.ContextMenu_ConnectLine2))
          {
            AtmoLightObject.ReinitialiseThreaded(true);
          }
        }
        else
        {
          DialogContextMenu();
        }
      }

      // No connection
      if (!AtmoLightObject.IsConnected())
      {
        return;
      }

      // Remote Key to toggle On/Off
      if ((action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_YELLOW_BUTTON && Settings.killButton == 2) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_GREEN_BUTTON && Settings.killButton == 1) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_RED_BUTTON && Settings.killButton == 0) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_BLUE_BUTTON && Settings.killButton == 3))
      {
        if (!AtmoLightObject.IsAtmoLightOn())
        {
          if (g_Player.Playing)
          {
            AtmoLightObject.ChangeEffect(playbackEffect);
          }
          else
          {
            AtmoLightObject.ChangeEffect(menuEffect);
          }
        }
        else
        {
          AtmoLightObject.ChangeEffect(ContentEffect.LEDsDisabled);
        }
      }

      // Remote Key to change Profiles
      else if ((action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_YELLOW_BUTTON && Settings.profileButton == 2) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_GREEN_BUTTON && Settings.profileButton == 1) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_RED_BUTTON && Settings.profileButton == 0) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_BLUE_BUTTON && Settings.profileButton == 3))
      {
        AtmoLightObject.ChangeAtmoWinProfile();
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
      Log.Info("Opening AtmoLight context menu.");

      // Showing context menu
      GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
      dlg.Reset();
      dlg.SetHeading("AtmoLight");

      // Toggle On/Off
      if (!AtmoLightObject.IsAtmoLightOn())
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
      if (Settings.sbs3dOn)
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
        // Toggle Delay and Change Delay
        if (AtmoLightObject.IsDelayEnabled())
        {
          dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_DelayOFF));
          dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_ChangeDelay + " (" + AtmoLightObject.GetDelayTime() + "ms)"));
          staticColorPos++;
        }
        else
        {
          dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_DelayON));
          delayChangePos = -1;
        }
        staticColorPos++;
      }
      else
      {
        delayTogglePos = -1;
        delayChangePos = -1;
      }

      // Change Static Color
      if (((g_Player.Playing) && (playbackEffect == ContentEffect.StaticColor) && (AtmoLightObject.IsAtmoLightOn())) ||
          ((!g_Player.Playing) && (menuEffect == ContentEffect.StaticColor) && (AtmoLightObject.IsAtmoLightOn())))
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
        if (!AtmoLightObject.IsAtmoLightOn())
        {
          if (g_Player.Playing)
          {
            AtmoLightObject.ChangeEffect(playbackEffect);
          }
          else
          {
            AtmoLightObject.ChangeEffect(menuEffect);
          }
        }
        else
        {
          AtmoLightObject.ChangeEffect(ContentEffect.LEDsDisabled);
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
              break;
            case 1:
              playbackEffect = ContentEffect.MediaPortalLiveMode;
              break;
            case 2:
              playbackEffect = ContentEffect.AtmoWinLiveMode;
              break;
            case 3:
              playbackEffect = ContentEffect.Colorchanger;
              break;
            case 4:
              playbackEffect = ContentEffect.ColorchangerLR;
              break;
            case 5:
              playbackEffect = ContentEffect.StaticColor;
              break;
          }
          AtmoLightObject.ChangeEffect(playbackEffect);
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
              break;
            case 1:
              menuEffect = ContentEffect.AtmoWinLiveMode;
              break;
            case 2:
              menuEffect = ContentEffect.Colorchanger;
              break;
            case 3:
              menuEffect = ContentEffect.ColorchangerLR;
              break;
            case 4:
              menuEffect = ContentEffect.StaticColor;
              break;
          }
          AtmoLightObject.ChangeEffect(menuEffect);
        }
      }
      else if (dlg.SelectedLabel == 2)
      {
        AtmoLightObject.ChangeAtmoWinProfile();
      }
      else if (dlg.SelectedLabel == 3)
      {
        if (Settings.sbs3dOn)
        {
          Log.Info("Switching SBS 3D mode off.");
          Settings.sbs3dOn = false;
        }
        else
        {
          Log.Info("Switching SBS 3D mode on.");
          Settings.sbs3dOn = true;
        }
      }
      else if ((dlg.SelectedLabel == delayTogglePos) && (delayTogglePos != -1))
      {
        if (AtmoLightObject.IsDelayEnabled())
        {
          Log.Info("Switching LED delay off.");
          AtmoLightObject.DisableDelay();
        }
        else
        {
          AtmoLightObject.EnableDelay((int)(((float)Settings.delayReferenceRefreshRate / (float)GetRefreshRate()) * (float)Settings.delayReferenceTime));
        }
      }
      else if ((dlg.SelectedLabel == delayChangePos) && (delayChangePos != -1))
      {
        if ((int.TryParse(GetKeyboardString(""), out delayTimeHelper)) && (delayTimeHelper >= 0) && (delayTimeHelper <= 1000))
        {
          AtmoLightObject.ChangeDelay(delayTimeHelper);
          Settings.delayReferenceTime = (int)(((float)delayTimeHelper * (float)GetRefreshRate()) / Settings.delayReferenceRefreshRate);
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
            Settings.SaveSpecificSetting("StaticColorRed", AtmoLightObject.GetStaticColor()[0].ToString());
            Settings.staticColorRed = AtmoLightObject.GetStaticColor()[0];
            Settings.SaveSpecificSetting("StaticColorGreen", AtmoLightObject.GetStaticColor()[1].ToString());
            Settings.staticColorGreen = AtmoLightObject.GetStaticColor()[1];
            Settings.SaveSpecificSetting("StaticColorBlue", AtmoLightObject.GetStaticColor()[2].ToString());
            Settings.staticColorBlue = AtmoLightObject.GetStaticColor()[2];
            break;
          case 2:
            AtmoLightObject.ChangeStaticColor(Settings.staticColorRed, Settings.staticColorGreen, Settings.staticColorBlue);
            break;
          case 3:
            AtmoLightObject.ChangeStaticColor(255, 255, 255);
            break;
          case 4:
            AtmoLightObject.ChangeStaticColor(255, 0, 0);
            break;
          case 5:
            AtmoLightObject.ChangeStaticColor(0, 255, 0);
            break;
          case 6:
            AtmoLightObject.ChangeStaticColor(0, 0, 255);
            break;
          case 7:
            AtmoLightObject.ChangeStaticColor(0, 255, 255);
            break;
          case 8:
            AtmoLightObject.ChangeStaticColor(255, 0, 255);
            break;
          case 9:
            AtmoLightObject.ChangeStaticColor(255, 255, 0);
            break;
        }
        AtmoLightObject.ChangeEffect(ContentEffect.StaticColor, true);
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
      Log.Info("Opening AtmoLight Yes/No dialog.");

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
            AtmoLightObject.ChangeStaticColor(staticColorTemp[0], staticColorTemp[1], staticColorTemp[2]);
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
      return "gemx, Angie05, RickDB, legnod, azzuro, BassFan, Lightning303, HomeY, Sebastiii";
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
      return "Interfaces with AtmowinA.exe via COM to control it.";
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
      new SetupForm().ShowDialog();
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
