using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using MediaPortal.GUI.Library;
using MediaPortal.Profile;
using MediaPortal.Player;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.DirectX.Direct3D;
using MediaPortal.Dialogs;
using MediaPortal.Configuration;
using Language;
using ProcessPlugins.ViewModeSwitcher;

namespace AtmoLight
{
  [PluginIcons("AtmoLight.Resources.Enabled.png", "AtmoLight.Resources.Disabled.png")]
  public class Plugin : ISetupForm, IPlugin
  {
    #region class Win32API
    public sealed class Win32API
    {
      [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
      public static extern Int64 GetTickCount();
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

    // Frame Fields
    private Surface rgbSurface = null; // RGB Surface
    private Int64 lastFrame = 0; // Tick count of the last frame

    // Blackbar detection
    private object blackbarAnalyzerClass; // Helper for reflection
    private Assembly blackbarAnalyzerAssembly; // Helper for reflection
    private Type blackbarAnalyzerType; // Helper for reflection
    private MethodInfo blackbarAnalyzerMethodInfo; // Helper for reflection
    private Rectangle blackbarDetectionRect = new Rectangle(); // Rectangle with the dimensions of the picture (without blackbars)
    private Int64 blackbarDetectionLastTime = 0; // Last time blackbar detection was run

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
    /// </summary>
    public Plugin()
    {
    }

    /// <summary>
    /// Start point of the plugin.
    /// This method gets called by MediaPortal.
    /// </summary>
    public void Start()
    {
      // Log Handler
      Log.OnNewLog += new Log.NewLogHandler(OnNewLog);

      var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
      DateTime buildDate = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTime;
      Log.Info("Version {0}.{1}.{2}.{3}, build on {4} at {5}.", version.Major, version.Minor, version.Build, version.Revision, buildDate.ToShortDateString(), buildDate.ToLongTimeString());
      Log.Debug("Loading settings.");
      Settings.LoadSettings();

      Log.Debug("Initialising event handler.");

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

      // VU Meter Handler
      Core.OnNewVUMeter += new Core.NewVUMeterHander(OnNewVUMeter);

      // Frameanalyzer
      // Reflection needed to access the FrameAnalyzer as it is an internal class
      blackbarAnalyzerAssembly = Assembly.LoadFrom("plugins\\process\\ProcessPlugins.dll");
      blackbarAnalyzerClass = blackbarAnalyzerAssembly.CreateInstance("ProcessPlugins.ViewModeSwitcher.FrameAnalyzer");
      blackbarAnalyzerType = blackbarAnalyzerClass.GetType();
      blackbarAnalyzerMethodInfo = blackbarAnalyzerType.GetMethod("FindBounds");


      staticColorTemp[0] = Settings.staticColorRed;
      staticColorTemp[1] = Settings.staticColorGreen;
      staticColorTemp[2] = Settings.staticColorBlue;

      Log.Debug("Generating new AtmoLight.Core instance.");
      AtmoLightObject = Core.GetInstance();
      if (Settings.atmoWinTarget)
      {
        AtmoLightObject.AddTarget(Target.AtmoWin);
        AtmoLightObject.atmoWinPath = Settings.atmowinExe;
        AtmoLightObject.atmoWinAutoStart = Settings.startAtmoWin;
        AtmoLightObject.atmoWinAutoStop = Settings.exitAtmoWin;
      }
      if (Settings.hyperionTarget)
      {
        AtmoLightObject.AddTarget(Target.Hyperion);
        AtmoLightObject.hyperionIP = Settings.hyperionIP;
        AtmoLightObject.hyperionPort = Settings.hyperionPort;
        AtmoLightObject.hyperionPriority = Settings.hyperionPriority;
        AtmoLightObject.hyperionReconnectDelay = Settings.hyperionReconnectDelay;
        AtmoLightObject.hyperionReconnectAttempts = Settings.hyperionReconnectAttempts;
        AtmoLightObject.hyperionPriorityStaticColor = Settings.HyperionPriorityStaticColor;
        AtmoLightObject.hyperionLiveReconnect = Settings.HyperionLiveReconnect;
      }
      AtmoLightObject.SetDelay(Settings.delayReferenceTime);
      AtmoLightObject.SetGIFPath(Settings.gifFile);
      AtmoLightObject.SetReInitOnError(Settings.restartOnError);
      AtmoLightObject.SetStaticColor(Settings.staticColorRed, Settings.staticColorGreen, Settings.staticColorBlue);
      AtmoLightObject.SetCaptureDimensions(Settings.captureWidth, Settings.captureHeight);

      menuEffect = Settings.effectMenu;
      if (CheckForStartRequirements())
      {
        AtmoLightObject.SetInitialEffect(menuEffect);
      }
      else
      {
        AtmoLightObject.SetInitialEffect(ContentEffect.LEDsDisabled);
      }

      if (!AtmoLightObject.Initialise())
      {
        Log.Error("Initialising failed.");
        return;
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

      AtmoLightObject.ChangeEffect(Settings.effectMPExit);

      AtmoLightObject.Dispose();

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
      // If starttime is bigger than endtime, then now has to be smaller than both or bigger than both to deactive the leds 
      else if ((DateTime.Now.TimeOfDay >= Settings.excludeTimeStart.TimeOfDay && DateTime.Now.TimeOfDay <= Settings.excludeTimeEnd.TimeOfDay) ||
              ((Settings.excludeTimeStart.TimeOfDay > Settings.excludeTimeEnd.TimeOfDay) &&
              ((DateTime.Now.TimeOfDay <= Settings.excludeTimeStart.TimeOfDay && DateTime.Now.TimeOfDay <= Settings.excludeTimeEnd.TimeOfDay) ||
              (DateTime.Now.TimeOfDay >= Settings.excludeTimeStart.TimeOfDay && DateTime.Now.TimeOfDay >= Settings.excludeTimeEnd.TimeOfDay))))
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

    /// <summary>
    /// Calculates the delay dependent on the refresh rate.
    /// </summary>
    private void CalculateDelay()
    {
      if (AtmoLightObject.GetCurrentEffect() == ContentEffect.MediaPortalLiveMode && AtmoLightObject.IsDelayEnabled())
      {
        AtmoLightObject.SetDelay((int)(((float)Settings.delayReferenceRefreshRate / (float)GetRefreshRate()) * (float)Settings.delayReferenceTime));
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

    #region VU Meter Event Handler
    private double[] OnNewVUMeter()
    {
      double[] dbLevel = new double[] { -100.0, -100.0 };
      if (BassMusicPlayer.Initialized)
      {
        if (BassMusicPlayer.Player.Playing)
        {
          BassMusicPlayer.Player.RMS(out dbLevel[0], out dbLevel[1]);
        }
      }
      return dbLevel;
    }
    #endregion

    #region Connection Lost Handler
    /// <summary>
    /// Connection lost event handler.
    /// This event gets called if connection to AtmoWin is lost and not recoverable.
    /// </summary>
    private void OnNewConnectionLost(Target target)
    {
      DialogError(LanguageLoader.appStrings.ContextMenu_AtmoWinConnectionLost.Replace("[Target]", target.ToString()));
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
          playbackEffect = Settings.effectMusic;
          Log.Debug("Music detected.");
        }
        else if (type == g_Player.MediaType.Radio)
        {
          playbackEffect = Settings.effectRadio;
          Log.Debug("Radio detected.");
        }

        if (CheckForStartRequirements())
        {
          AtmoLightObject.ChangeEffect(playbackEffect);
          CalculateDelay();
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
                CalculateDelay();
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
          CalculateDelay();
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

          if (Settings.blackbarDetection)
          {
            if (Win32API.GetTickCount() >= (blackbarDetectionLastTime + Settings.blackbarDetectionTime))
            {
              blackbarDetectionLastTime = Win32API.GetTickCount();

              // Analyzing the frame for black bars.
              // Has to be done in low res, as it would be to cpu heavy otherwise (0-2ms vs. 1000ms).
              Bitmap streamBitmap = new Bitmap(stream);
              object[] arguments = new Object[] { streamBitmap, null };

              // Call FindBounds.
              bool blackbarblackbarAnalyzerSuccess = (bool)blackbarAnalyzerMethodInfo.Invoke(blackbarAnalyzerClass, arguments);
              streamBitmap.Dispose();

              if (blackbarblackbarAnalyzerSuccess)
              {
                // Retrieving the bounds.
                blackbarDetectionRect = (System.Drawing.Rectangle)arguments[1];
              }
            }

            // New bitmap that has to have to dimensions AtmoWin expects.
            Bitmap target = new Bitmap(AtmoLightObject.GetCaptureWidth(), AtmoLightObject.GetCaptureHeight());

            using (Graphics g = Graphics.FromImage(target))
            {
              // Cropping and resizing the original bitmap
              g.DrawImage(new Bitmap(stream), new Rectangle(0, 0, target.Width, target.Height), blackbarDetectionRect, GraphicsUnit.Pixel);
            }

            // Saving cropped and resized bitmap to stream
            target.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
            target.Dispose();
          }
          AtmoLightObject.CalculateBitmap(stream);

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
            AtmoLightObject.ReInitialise();
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
            CalculateDelay();
          }
          else
          {
            AtmoLightObject.ChangeEffect(menuEffect);
            CalculateDelay();
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
        AtmoLightObject.ChangeProfile();
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

      // Change Profile
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

      // Toggle Blackbar Detection
      if (Settings.blackbarDetection)
      {
        dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_SwitchBlackbarDetectionOFF));
      }
      else
      {
        dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_SwitchBlackbarDetectionON));
      }

      // Delay
      if (AtmoLightObject.GetCurrentEffect() == ContentEffect.MediaPortalLiveMode)
      {
        // Toggle Delay and Change Delay
        if (AtmoLightObject.IsDelayEnabled())
        {
          dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_DelayOFF));
          dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_ChangeDelay + " (" + AtmoLightObject.GetDelayTime() + "ms)"));
        }
        else
        {
          dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_DelayON));
        }
      }

      // Change Static Color
      if (AtmoLightObject.GetCurrentEffect() == ContentEffect.StaticColor)
      {
        dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_ChangeStatic));
      }

      // ReInit
      if (!AtmoLightObject.AreAllConnected())
      {
        dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_ReInitialise));
      }

      dlg.SelectedLabel = 0;
      dlg.DoModal(GUIWindowManager.ActiveWindow);

      // Do stuff
      // Toggle LEDs
      if (dlg.SelectedLabelText == LanguageLoader.appStrings.ContextMenu_SwitchLEDsON || dlg.SelectedLabelText == LanguageLoader.appStrings.ContextMenu_SwitchLEDsOFF)
      {
        if (!AtmoLightObject.IsAtmoLightOn())
        {
          if (g_Player.Playing)
          {
            AtmoLightObject.ChangeEffect(playbackEffect);
            CalculateDelay();
          }
          else
          {
            AtmoLightObject.ChangeEffect(menuEffect);
            CalculateDelay();
          }
        }
        else
        {
          AtmoLightObject.ChangeEffect(ContentEffect.LEDsDisabled);
        }
      }
      // Change Effect
      else if (dlg.SelectedLabelText == LanguageLoader.appStrings.ContextMenu_ChangeEffect)
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
        dlgEffect.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_GIFReader));
        if (g_Player.Playing && (g_Player.currentMedia == g_Player.MediaType.Music || g_Player.currentMedia == g_Player.MediaType.Radio))
        {
          dlgEffect.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_VUMeter));
          dlgEffect.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_VUMeterRainbow));
        }
        dlgEffect.SelectedLabel = 0;
        dlgEffect.DoModal(GUIWindowManager.ActiveWindow);

        ContentEffect temp = ContentEffect.Undefined;

        switch (dlgEffect.SelectedLabel)
        {
          case -1:
            return;
          case 0:
            temp = ContentEffect.LEDsDisabled;
            break;
          case 1:
            temp = ContentEffect.MediaPortalLiveMode;
            break;
          case 2:
            temp = ContentEffect.AtmoWinLiveMode;
            break;
          case 3:
            temp = ContentEffect.Colorchanger;
            break;
          case 4:
            temp = ContentEffect.ColorchangerLR;
            break;
          case 5:
            temp = ContentEffect.StaticColor;
            break;
          case 6:
            temp = ContentEffect.GIFReader;
            break;
          case 7:
            temp = ContentEffect.VUMeter;
            break;
          case 8:
            temp = ContentEffect.VUMeterRainbow;
            break;
        }
        if (g_Player.Playing)
        {
          playbackEffect = temp;
        }
        else
        {
          menuEffect = temp;
        }
        AtmoLightObject.ChangeEffect(temp);
        CalculateDelay();
      }
      // Change Profile
      else if (dlg.SelectedLabelText == LanguageLoader.appStrings.ContextMenu_ChangeAWProfile)
      {
        AtmoLightObject.ChangeProfile();
      }
      // Toggle 3D
      else if (dlg.SelectedLabelText == LanguageLoader.appStrings.ContextMenu_Switch3DOFF || dlg.SelectedLabelText == LanguageLoader.appStrings.ContextMenu_Switch3DON)
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
      // Blackbar detection
      else if (dlg.SelectedLabelText == LanguageLoader.appStrings.ContextMenu_SwitchBlackbarDetectionOFF || dlg.SelectedLabelText == LanguageLoader.appStrings.ContextMenu_SwitchBlackbarDetectionON)
      {
        if (Settings.blackbarDetection)
        {
          Log.Info("Switching blackbar detection off.");
          Settings.blackbarDetection = false;
        }
        else
        {
          Log.Info("Switching blackbar detection on.");
          Settings.blackbarDetection = true;
        }
      }
      // Toggle Delay
      else if (dlg.SelectedLabelText == LanguageLoader.appStrings.ContextMenu_DelayOFF || dlg.SelectedLabelText == LanguageLoader.appStrings.ContextMenu_DelayON)
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
      // Change Delay
      else if (dlg.SelectedLabelText == LanguageLoader.appStrings.ContextMenu_ChangeDelay + " (" + AtmoLightObject.GetDelayTime() + "ms)")
      {
        if ((int.TryParse(GetKeyboardString(""), out delayTimeHelper)) && (delayTimeHelper >= 0) && (delayTimeHelper <= 1000))
        {
          AtmoLightObject.SetDelay(delayTimeHelper);
          Settings.delayReferenceTime = (int)(((float)delayTimeHelper * (float)GetRefreshRate()) / Settings.delayReferenceRefreshRate);
        }
        else
        {
          DialogError(LanguageLoader.appStrings.ContextMenu_DelayTimeErrorLine1, LanguageLoader.appStrings.ContextMenu_DelayTimeErrorLine2);
        }
      }
      // Change Static Color
      else if (dlg.SelectedLabelText == LanguageLoader.appStrings.ContextMenu_ChangeStatic)
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
          case -1:
            return;
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
            AtmoLightObject.SetStaticColor(Settings.staticColorRed, Settings.staticColorGreen, Settings.staticColorBlue);
            break;
          case 3:
            AtmoLightObject.SetStaticColor(255, 255, 255);
            break;
          case 4:
            AtmoLightObject.SetStaticColor(255, 0, 0);
            break;
          case 5:
            AtmoLightObject.SetStaticColor(0, 255, 0);
            break;
          case 6:
            AtmoLightObject.SetStaticColor(0, 0, 255);
            break;
          case 7:
            AtmoLightObject.SetStaticColor(0, 255, 255);
            break;
          case 8:
            AtmoLightObject.SetStaticColor(255, 0, 255);
            break;
          case 9:
            AtmoLightObject.SetStaticColor(255, 255, 0);
            break;
        }
        AtmoLightObject.ChangeEffect(ContentEffect.StaticColor, true);
      }
      else if (dlg.SelectedLabelText == LanguageLoader.appStrings.ContextMenu_ReInitialise)
      {
        AtmoLightObject.ReInitialise();
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
            AtmoLightObject.SetStaticColor(staticColorTemp[0], staticColorTemp[1], staticColorTemp[2]);
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
      // Log Handler
      Log.OnNewLog += new Log.NewLogHandler(OnNewLog);

      var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
      DateTime buildDate = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTime;
      Log.Info("Version {0}.{1}.{2}.{3}, build on {4} at {5}.", version.Major, version.Minor, version.Build, version.Revision, buildDate.ToShortDateString(), buildDate.ToLongTimeString());
      Log.Debug("Loading settings.");
      Settings.LoadSettings();

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
