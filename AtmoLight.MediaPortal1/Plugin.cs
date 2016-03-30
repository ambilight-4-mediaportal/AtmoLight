using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using MediaPortal;
using MediaPortal.GUI.Library;
using MediaPortal.Profile;
using MediaPortal.Player;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.DirectX.Direct3D;
using Microsoft.Win32;
using MediaPortal.Dialogs;
using MediaPortal.Configuration;

namespace AtmoLight
{
  [PluginIcons("AtmoLight.Resources.Enabled.png", "AtmoLight.Resources.Disabled.png")]
  public class Plugin : ISetupForm, IPlugin
  {
    #region AtmoDXUtil Import

    [DllImport("AtmoDXUtil.dll", PreserveSig = false, CharSet = CharSet.Auto)]
    private static extern void VideoSurfaceToRGBSurfaceExt(IntPtr src, int srcWidth, int srcHeight, IntPtr dst,
      int dstWidth, int dstHeight);

    #endregion

    #region Variables

    // States
    private ContentEffect playbackEffect = ContentEffect.Undefined; // Effect for current placback
    private ContentEffect menuEffect = ContentEffect.Undefined; // Effect in GUI (no playback)
    private bool AtmoLightDisabledByUser = false; // Toggle AtmoLight on/off

    private List<ContentEffect> supportedEffects;

    // Frame Fields
    private Surface rgbSurface = null; // RGB Surface
    private Int64 lastFrame = 0; // Tick count of the last frame

    // Static Color
    private int[] staticColorTemp = {0, 0, 0}; // Temp array to change static color
    private int staticColorHelper; // Helper var for static color change

    // Delay Feature
    private int delayTimeHelper; // Helper var for delay time change

    // SetupForm
    private SetupForm atmoLightSetupForm;

    // Core object
    private Core coreObject;

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
      Log.Info("Version {0}.{1}.{2}.{3}, build on {4} at {5}.", version.Major, version.Minor, version.Build,
        version.Revision, buildDate.ToShortDateString(), buildDate.ToLongTimeString());
      Log.Debug("Loading settings.");
      Settings.LoadSettings();
      Settings.SaveSettings();

      Log.Debug("Initialising event handler.");

      // PowerModeChanged Handler
      SystemEvents.PowerModeChanged += PowerModeChanged;

      // Mediaportal state Handler
      if (Settings.monitorScreensaverState || Settings.monitorWindowState)
      {
        Thread monitorStateThread = new Thread(monitorMediaportalState);
        monitorStateThread.IsBackground = true;
        monitorStateThread.Start();
      }

      // g_Player Handler
      g_Player.PlayBackStarted += new g_Player.StartedHandler(g_Player_PlayBackStarted);
      g_Player.PlayBackStopped += new g_Player.StoppedHandler(g_Player_PlayBackStopped);
      g_Player.PlayBackEnded += new g_Player.EndedHandler(g_Player_PlayBackEnded);

      // FrameGrabber Handler
      MediaPortal.FrameGrabber.GetInstance().OnNewFrame +=
        new MediaPortal.FrameGrabber.NewFrameHandler(AtmolightPlugin_OnNewFrame);

      // Button Handler
      GUIWindowManager.OnNewAction += new OnActionHandler(OnNewAction);

      // Connection Lost Handler
      Core.OnNewConnectionLost += new Core.NewConnectionLostHandler(OnNewConnectionLost);

      // VU Meter Handler
      Core.OnNewVUMeter += new Core.NewVUMeterHander(OnNewVUMeter);

      staticColorTemp[0] = Settings.staticColorRed;
      staticColorTemp[1] = Settings.staticColorGreen;
      staticColorTemp[2] = Settings.staticColorBlue;

      Log.Debug("Generating new AtmoLight.Core instance.");
      coreObject = Core.GetInstance();

      // General settings
      coreObject.SetDelay(Settings.delayReferenceTime);
      if (Settings.delay)
      {
        coreObject.EnableDelay();
      }
      coreObject.SetGIFPath(Settings.gifFile);
      coreObject.SetReInitOnError(Settings.restartOnError);
      coreObject.SetStaticColor(Settings.staticColorRed, Settings.staticColorGreen, Settings.staticColorBlue);
      coreObject.SetCaptureDimensions(Settings.captureWidth, Settings.captureHeight);
      coreObject.blackbarDetection = Settings.blackbarDetection;
      coreObject.blackbarDetectionTime = Settings.blackbarDetectionTime;
      coreObject.blackbarDetectionThreshold = Settings.blackbarDetectionThreshold;
      coreObject.powerModeChangedDelay = Settings.powerModeChangedDelay;
      coreObject.vuMeterMindB = Settings.vuMeterMindB;
      coreObject.vuMeterMinHue = Settings.vuMeterMinHue;
      coreObject.vuMeterMaxHue = Settings.vuMeterMaxHue;
      coreObject.blackbarDetectionHorizontal = Settings.blackbarDetectionHorizontal;
      coreObject.blackbarDetectionVertical = Settings.blackbarDetectionVertical;
      coreObject.blackbarDetectionLinkAreas = Settings.blackbarDetectionLinkAreas;

      // API
      coreObject.apiServerEnabled = Settings.remoteApiServer;

      // AmbiBox
      coreObject.ambiBoxIP = Settings.ambiBoxIP;
      coreObject.ambiBoxPort = Settings.ambiBoxPort;
      coreObject.ambiBoxMaxReconnectAttempts = Settings.ambiBoxMaxReconnectAttempts;
      coreObject.ambiBoxReconnectDelay = Settings.ambiBoxReconnectDelay;
      coreObject.ambiBoxChangeImageDelay = Settings.ambiBoxChangeImageDelay;
      coreObject.ambiBoxMediaPortalProfile = Settings.ambiBoxMediaPortalProfile;
      coreObject.ambiBoxExternalProfile = Settings.ambiBoxExternalProfile;
      coreObject.ambiBoxPath = Settings.ambiBoxPath;
      coreObject.ambiBoxAutoStart = Settings.ambiBoxAutoStart;
      coreObject.ambiBoxAutoStop = Settings.ambiBoxAutoStop;
      if (Settings.ambiBoxTarget)
      {
        coreObject.AddTarget(Target.AmbiBox);
      }

      // AtmoOrb
      coreObject.atmoOrbBlackThreshold = Settings.atmoOrbBlackThreshold;
      coreObject.atmoOrbBroadcastPort = Settings.atmoOrbBroadcastPort;
      coreObject.atmoOrbGamma = Settings.atmoOrbGamma;
      coreObject.atmoOrbLamps = Settings.atmoOrbLamps;
      coreObject.atmoOrbMinDiversion = Settings.atmoOrbMinDiversion;
      coreObject.atmoOrbSaturation = Settings.atmoOrbSaturation;
      coreObject.atmoOrbThreshold = Settings.atmoOrbThreshold;
      coreObject.atmoOrbUseOverallLightness = Settings.atmoOrbUseOverallLightness;
      coreObject.atmoOrbUseSmoothing = Settings.atmoOrbUseSmoothing;
      coreObject.atmoOrbSmoothThreshold = Settings.atmoOrbSmoothingThreshold;

      if (Settings.atmoOrbTarget)
      {
        coreObject.AddTarget(Target.AtmoOrb);
      }

      // AtmoWin
      coreObject.atmoWinPath = Settings.atmowinExe;
      coreObject.atmoWinAutoStart = Settings.startAtmoWin;
      coreObject.atmoWinAutoStop = Settings.exitAtmoWin;
      coreObject.atmoWakeHelperEnabled = Settings.atmoWakeHelperEnabled;
      coreObject.atmoWakeHelperComPort = Settings.atmoWakeHelperComPort;
      coreObject.atmoWakeHelperResumeDelay = Settings.atmoWakeHelperResumeDelay;
      coreObject.atmoWakeHelperDisconnectDelay = Settings.atmoWakeHelperDisconnectDelay;
      coreObject.atmoWakeHelperConnectDelay = Settings.atmoWakeHelperConnectDelay;
      coreObject.atmoWakeHelperReinitializationDelay = Settings.atmoWakeHelperReinitializationDelay;

      if (Settings.atmoWinTarget)
      {
        coreObject.AddTarget(Target.AtmoWin);
      }

      // Boblight
      coreObject.boblightIP = Settings.boblightIP;
      coreObject.boblightPort = Settings.boblightPort;
      coreObject.boblightMaxFPS = Settings.boblightMaxFPS;
      coreObject.boblightMaxReconnectAttempts = Settings.boblightMaxReconnectAttempts;
      coreObject.boblightReconnectDelay = Settings.boblightReconnectDelay;
      coreObject.boblightSpeed = Settings.boblightSpeed;
      coreObject.boblightAutospeed = Settings.boblightAutospeed;
      coreObject.boblightInterpolation = Settings.boblightInterpolation;
      coreObject.boblightSaturation = Settings.boblightSaturation;
      coreObject.boblightValue = Settings.boblightValue;
      coreObject.boblightThreshold = Settings.boblightThreshold;
      coreObject.boblightGamma = Settings.boblightGamma;
      if (Settings.boblightTarget)
      {
        coreObject.AddTarget(Target.Boblight);
      }

      // Hyperion
      coreObject.hyperionIP = Settings.hyperionIP;
      coreObject.hyperionPort = Settings.hyperionPort;
      coreObject.hyperionPriority = Settings.hyperionPriority;
      coreObject.hyperionReconnectDelay = Settings.hyperionReconnectDelay;
      coreObject.hyperionReconnectAttempts = Settings.hyperionReconnectAttempts;
      coreObject.hyperionPriorityStaticColor = Settings.hyperionPriorityStaticColor;
      coreObject.hyperionLiveReconnect = Settings.hyperionLiveReconnect;
      if (Settings.hyperionTarget)
      {
        coreObject.AddTarget(Target.Hyperion);
      }

      // Hue
      coreObject.huePath = Settings.hueExe;
      coreObject.hueStart = Settings.hueStart;
      coreObject.hueIsRemoteMachine = Settings.hueIsRemoteMachine;
      coreObject.hueIP = Settings.hueIP;
      coreObject.huePort = Settings.huePort;
      coreObject.hueReconnectDelay = Settings.hueReconnectDelay;
      coreObject.hueReconnectAttempts = Settings.hueReconnectAttempts;
      coreObject.hueBridgeEnableOnResume = Settings.hueBridgeEnableOnResume;
      coreObject.hueBridgeDisableOnSuspend = Settings.hueBridgeDisableOnSuspend;
      coreObject.hueMinDiversion = Settings.hueMinDiversion;
      coreObject.hueThreshold = Settings.hueThreshold;
      coreObject.hueBlackThreshold = Settings.hueBlackThreshold;
      coreObject.hueSaturation = Settings.hueSaturation;
      coreObject.hueUseOverallLightness = Settings.hueUseOverallLightness;
      coreObject.hueTheaterEnabled = Settings.hueTheaterEnabled;
      coreObject.hueTheaterRestoreLights = Settings.hueTheaterRestoreLights;

      if (Settings.hueTarget)
      {
        coreObject.AddTarget(Target.Hue);
      }

      // Get the effects that are supported by at least one target
      supportedEffects = coreObject.GetSupportedEffects();

      menuEffect = Settings.effectMenu;
      if (CheckForStartRequirements())
      {
        coreObject.ChangeEffect(menuEffect, true);
        CalculateDelay();
      }
      else
      {
        coreObject.ChangeEffect(ContentEffect.LEDsDisabled, true);
      }

      coreObject.Initialise();
    }

    /// <summary>
    /// Stop point of the plugin.
    /// This method gets called by MediaPortal.
    /// </summary>
    public void Stop()
    {
      MediaPortal.FrameGrabber.GetInstance().OnNewFrame -= new MediaPortal.FrameGrabber.NewFrameHandler(AtmolightPlugin_OnNewFrame);
      SystemEvents.PowerModeChanged -= PowerModeChanged;

      Settings.monitorScreensaverState = false;
      Settings.monitorWindowState = false;

      g_Player.PlayBackStarted -= new g_Player.StartedHandler(g_Player_PlayBackStarted);
      g_Player.PlayBackStopped -= new g_Player.StoppedHandler(g_Player_PlayBackStopped);
      g_Player.PlayBackEnded -= new g_Player.EndedHandler(g_Player_PlayBackEnded);

      GUIWindowManager.OnNewAction -= new OnActionHandler(OnNewAction);

      if (!coreObject.apiOverrideActive)
      {
        coreObject.ChangeEffect(Settings.effectMPExit);
      }

      coreObject.Dispose();

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
      if (Settings.manualMode)
      {
        Log.Debug("LEDs should be deactivated. (Manual Mode)");
        return false;
      }
      if (AtmoLightDisabledByUser)
      {
        Log.Debug("LEDs should be deactivated. (permanently toggled off by user)");
        return false;
      }
      if (coreObject.apiOverrideActive)
      {
        Log.Debug("API override is activated.");
        coreObject.apiStoredPlaybackEffect = playbackEffect;
        return false;
      }

      // If starttime is bigger than endtime, then now has to be smaller than both or bigger than both to deactive the leds 
      else if ((DateTime.Now.TimeOfDay >= Settings.excludeTimeStart.TimeOfDay &&
                DateTime.Now.TimeOfDay <= Settings.excludeTimeEnd.TimeOfDay) ||
               ((Settings.excludeTimeStart.TimeOfDay > Settings.excludeTimeEnd.TimeOfDay) &&
                ((DateTime.Now.TimeOfDay <= Settings.excludeTimeStart.TimeOfDay &&
                  DateTime.Now.TimeOfDay <= Settings.excludeTimeEnd.TimeOfDay) ||
                 (DateTime.Now.TimeOfDay >= Settings.excludeTimeStart.TimeOfDay &&
                  DateTime.Now.TimeOfDay >= Settings.excludeTimeEnd.TimeOfDay))))
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
      if (coreObject.GetCurrentEffect() == ContentEffect.MediaPortalLiveMode && coreObject.IsDelayEnabled())
      {
        coreObject.SetDelay(
          (int)
            (((float) Settings.delayReferenceRefreshRate/(float) GetRefreshRate())*(float) Settings.delayReferenceTime));
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
      double[] dbLevel = new double[] {-100.0, -100.0};
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
      // Wait until MediaPortal is ready to display a dialog
      Process mediaPortalProcess = Process.GetCurrentProcess();
      while (mediaPortalProcess.MainWindowHandle == IntPtr.Zero)
      {
        Thread.Sleep(100);
        mediaPortalProcess.Refresh();
      }
      DialogError(Localization.Translate("ContextMenu", "ConnectionLost").Replace("[Target]", target.ToString()));
    }

    #endregion

    #region g_Player Event Handler

    /// <summary>
    /// Playback started event handler.
    /// This event handler gets called when playback starts.
    /// </summary>
    /// <param name="type">Media type</param>
    /// <param name="filename">Media filename</param>
    private void g_Player_PlayBackStarted(g_Player.MediaType type, string filename)
    {
      try
      {
        if (type == g_Player.MediaType.Video || type == g_Player.MediaType.TV || type == g_Player.MediaType.Recording ||
            type == g_Player.MediaType.Unknown || (type == g_Player.MediaType.Music && filename.Contains(".mkv")))
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
          coreObject.ChangeEffect(playbackEffect);
          CalculateDelay();
        }
        else
        {
          if (!coreObject.apiOverrideActive)
          {
            coreObject.ChangeEffect(ContentEffect.LEDsDisabled);
          }
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
    private void g_Player_PlayBackEnded(g_Player.MediaType type, string filename)
    {
      if (!coreObject.IsConnected())
      {
        return;
      }
      try
      {
        if (CheckForStartRequirements())
        {
          coreObject.ChangeEffect(menuEffect);
          CalculateDelay();
        }
        else
        {
          if (!coreObject.apiOverrideActive)
          {
            coreObject.ChangeEffect(ContentEffect.LEDsDisabled);
          }
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
    private void g_Player_PlayBackStopped(g_Player.MediaType type, int stoptime, string filename)
    {
      if (!coreObject.IsConnected())
      {
        return;
      }
      try
      {
        if (CheckForStartRequirements())
        {
          coreObject.ChangeEffect(menuEffect);
          CalculateDelay();
        }
        else
        {
          if (!coreObject.apiOverrideActive)
          {
            coreObject.ChangeEffect(ContentEffect.LEDsDisabled);
          }
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
    /// <param name="FrameSource">Source of frame, see enums FrameGrabber.FrameSource</param>/// 
    private void AtmolightPlugin_OnNewFrame(short width, short height, short arWidth, short arHeight, uint pSurface, FrameGrabber.FrameSource FrameSource)
    {
      if (coreObject.GetCurrentEffect() != ContentEffect.MediaPortalLiveMode || !coreObject.IsConnected() ||
          !coreObject.IsAtmoLightOn() || width == 0 || height == 0)
      {
        return;
      }
	  
      //Drop any frames?
      if (Settings.trueGrabbing)
      {
          //GUI grabbing while video is minimized activated 
          if (g_Player.Playing && (!GUIGraphicsContext.IsFullScreenVideo) && (FrameSource == FrameGrabber.FrameSource.Video))
          {
            //Drop Video frames when user has set TrueGrabbing & video is playing minimized -> just GUI frames will pass
            return;
          }
      }
      else
      {
          //GUI grabbing while video is minimized deactivated 
          if((g_Player.Playing) && (FrameSource == FrameGrabber.FrameSource.GUI))
          {
              //Drop GUI frames when user has NOT set TrueGrabbing & video is playing -> just video frames will pass
            return;
          }
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
        rgbSurface = GUIGraphicsContext.DX9Device.CreateRenderTarget(coreObject.GetCaptureWidth(),
          coreObject.GetCaptureHeight(), Format.A8R8G8B8,
          MultiSampleType.None, 0, true);
      }
      unsafe
      {
        try
        {
          if (Settings.sbs3dOn)
          {
            VideoSurfaceToRGBSurfaceExt(new IntPtr(pSurface), width/2, height, (IntPtr) rgbSurface.UnmanagedComPointer,
              coreObject.GetCaptureWidth(), coreObject.GetCaptureHeight());
          }
          else
          {
            VideoSurfaceToRGBSurfaceExt(new IntPtr(pSurface), width, height, (IntPtr) rgbSurface.UnmanagedComPointer,
              coreObject.GetCaptureWidth(), coreObject.GetCaptureHeight());
          }

          Microsoft.DirectX.GraphicsStream stream = SurfaceLoader.SaveToStream(ImageFileFormat.Bmp, rgbSurface);

          coreObject.CalculateBitmap(stream);

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
      if ((action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_YELLOW_BUTTON &&
           Settings.menuButton == 2) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_GREEN_BUTTON &&
           Settings.menuButton == 1) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_RED_BUTTON && Settings.menuButton == 0) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_BLUE_BUTTON && Settings.menuButton == 3))
      {
        if (!coreObject.IsConnected())
        {
          if (DialogYesNo(Localization.Translate("ContextMenu", "Connect1"),
            Localization.Translate("ContextMenu", "Connect2")))
          {
            coreObject.ReInitialise();
          }
        }
        else
        {
          DialogContextMenu();
        }
      }

      // No connection
      if (!coreObject.IsConnected())
      {
        return;
      }

      // Remote Key to toggle On/Off
      if ((action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_YELLOW_BUTTON &&
           Settings.killButton == 2) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_GREEN_BUTTON &&
           Settings.killButton == 1) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_RED_BUTTON && Settings.killButton == 0) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_BLUE_BUTTON && Settings.killButton == 3))
      {
        if (!coreObject.IsAtmoLightOn())
        {
          if (g_Player.Playing)
          {
            coreObject.ChangeEffect(playbackEffect);
            CalculateDelay();
          }
          else
          {
            coreObject.ChangeEffect(menuEffect);
            CalculateDelay();
          }
        }
        else
        {
          coreObject.ChangeEffect(ContentEffect.LEDsDisabled);
        }
      }

      // Remote Key to change Profiles
      else if ((action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_YELLOW_BUTTON &&
                Settings.profileButton == 2) ||
               (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_GREEN_BUTTON &&
                Settings.profileButton == 1) ||
               (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_RED_BUTTON &&
                Settings.profileButton == 0) ||
               (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_BLUE_BUTTON &&
                Settings.profileButton == 3))
      {
        coreObject.ChangeProfile();
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
      VirtualKeyboard Keyboard =
        (VirtualKeyboard) GUIWindowManager.GetWindow((int) GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
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
      // Send Thread Callback to let this Dialog be shown by the main thread
      // Will result in problems and error messages otherwise
      GUIWindowManager.SendThreadCallback((p1, p2, o) =>
      {
        GUIDialogOK dlgError = (GUIDialogOK) GUIWindowManager.GetWindow((int) GUIWindow.Window.WINDOW_DIALOG_OK);
        if (dlgError != null)
        {
          dlgError.SetHeading(Localization.Translate("Common", "Error") + "!");
          dlgError.SetLine(1, setLine1);
          dlgError.SetLine(2, setLine2);
          dlgError.DoModal(GUIWindowManager.ActiveWindow);
        }
        return 0;
      }, 0, 0, null);
    }

    /// <summary>
    /// Prompts the user with the context menu.
    /// </summary>
    private void DialogContextMenu()
    {
      Log.Info("Opening AtmoLight context menu.");

      // Showing context menu
      GUIDialogMenu dlg = (GUIDialogMenu) GUIWindowManager.GetWindow((int) GUIWindow.Window.WINDOW_DIALOG_MENU);
      dlg.Reset();
      dlg.SetHeading(Localization.Translate("Common", "AtmoLight"));

      // Toggle On/Off
      if (!coreObject.IsAtmoLightOn())
      {
        dlg.Add(new GUIListItem(Localization.Translate("ContextMenu", "LEDsOn")));
      }
      else
      {
        dlg.Add(new GUIListItem(Localization.Translate("ContextMenu", "LEDsOff")));
      }

      // Change Effect
      dlg.Add(new GUIListItem(Localization.Translate("ContextMenu", "ChangeEffect")));

      // Change Profile
      dlg.Add(new GUIListItem(Localization.Translate("ContextMenu", "ChangeProfile")));

      // Toggle 3D Mode
      if (Settings.sbs3dOn)
      {
        dlg.Add(new GUIListItem(Localization.Translate("ContextMenu", "3DOff")));
      }
      else
      {
        dlg.Add(new GUIListItem(Localization.Translate("ContextMenu", "3DOn")));
      }

      // Toggle Blackbar Detection
      dlg.Add(new GUIListItem(Localization.Translate("ContextMenu", "BlackbarDetection")));

      // Delay
      if (coreObject.GetCurrentEffect() == ContentEffect.MediaPortalLiveMode)
      {
        // Toggle Delay and Change Delay
        if (coreObject.IsDelayEnabled())
        {
          dlg.Add(new GUIListItem(Localization.Translate("ContextMenu", "DelayOff")));
          dlg.Add(
            new GUIListItem(Localization.Translate("ContextMenu", "ChangeDelay") + " (" + coreObject.GetDelayTime() +
                            Localization.Translate("Common", "MS") + ")"));
        }
        else
        {
          dlg.Add(new GUIListItem(Localization.Translate("ContextMenu", "DelayOn")));
        }
      }

      // Change Static Color
      if (coreObject.GetCurrentEffect() == ContentEffect.StaticColor)
      {
        dlg.Add(new GUIListItem(Localization.Translate("ContextMenu", "ChangeStaticColor")));
      }

      // ReInit
      if (!coreObject.AreAllConnected())
      {
        dlg.Add(new GUIListItem(Localization.Translate("ContextMenu", "Reinitialise")));
      }

      // Hue set active liveview group
      if (coreObject.GetTarget(Target.Hue) != null)
      {
        dlg.Add(new GUIListItem(Localization.Translate("Hue", "LiveviewGroup")));
      }

      // Hue set active liveview group
      if (coreObject.GetTarget(Target.Hue) != null)
      {
        dlg.Add(new GUIListItem(Localization.Translate("Hue", "StaticColorGroup")));
      }

      // Toggle On/Off AtmoLight
      if (AtmoLightDisabledByUser)
      {
        // Clear all other items if AtmoLight leds is manually disabled by yser
        dlg.Reset();
        dlg.SetHeading(Localization.Translate("ContextMenu", "AtmoLightDisabled"));

        dlg.Add(new GUIListItem(Localization.Translate("ContextMenu", "AtmoLightOn")));
      }
      else if (!AtmoLightDisabledByUser)
      {
        dlg.Add(new GUIListItem(Localization.Translate("ContextMenu", "AtmoLightOff")));
      }

      dlg.SelectedLabel = 0;
      dlg.DoModal(GUIWindowManager.ActiveWindow);


      // Do stuff
      // Toggle LEDs
      if (dlg.SelectedLabelText == Localization.Translate("ContextMenu", "LEDsOn") ||
          dlg.SelectedLabelText == Localization.Translate("ContextMenu", "LEDsOff"))
      {
        if (!coreObject.IsAtmoLightOn())
        {
          if (g_Player.Playing)
          {
            coreObject.ChangeEffect(playbackEffect);
            CalculateDelay();
          }
          else
          {
            coreObject.ChangeEffect(menuEffect);
            CalculateDelay();
          }
        }
        else
        {
          coreObject.ChangeEffect(ContentEffect.LEDsDisabled);
        }
      }
      // Toggle LEDs for duration of Mediaportal runtime
      else if (dlg.SelectedLabelText == Localization.Translate("ContextMenu", "AtmoLightOn"))
      {
        AtmoLightDisabledByUser = false;

        if (g_Player.Playing)
        {
          coreObject.ChangeEffect(playbackEffect);
        }
        else
        {
          coreObject.ChangeEffect(menuEffect);
        }
        CalculateDelay();
      }
      else if (dlg.SelectedLabelText == Localization.Translate("ContextMenu", "AtmoLightOff"))
      {
        AtmoLightDisabledByUser = true;
        coreObject.ChangeEffect(ContentEffect.LEDsDisabled);
      }
      // Change Effect
      else if (dlg.SelectedLabelText == Localization.Translate("ContextMenu", "ChangeEffect"))
      {
        GUIDialogMenu dlgEffect = (GUIDialogMenu) GUIWindowManager.GetWindow((int) GUIWindow.Window.WINDOW_DIALOG_MENU);
        dlgEffect.Reset();
        dlgEffect.SetHeading(Localization.Translate("ContextMenu", "ChangeEffect"));

        // Only show effects that are support by at least one target
        foreach (ContentEffect effect in Enum.GetValues(typeof (ContentEffect)))
        {
          if (supportedEffects.Contains(effect) && effect != ContentEffect.Undefined)
          {
            if (effect == ContentEffect.VUMeter || effect == ContentEffect.VUMeterRainbow)
            {
              if (g_Player.Playing &&
                  (g_Player.currentMedia == g_Player.MediaType.Music ||
                   g_Player.currentMedia == g_Player.MediaType.Radio))
              {
                dlgEffect.Add(new GUIListItem(Localization.Translate("ContentEffect", effect.ToString())));
              }
            }
            else
            {
              dlgEffect.Add(new GUIListItem(Localization.Translate("ContentEffect", effect.ToString())));
            }
          }
        }
        dlgEffect.SelectedLabel = 0;
        dlgEffect.DoModal(GUIWindowManager.ActiveWindow);

        if (!String.IsNullOrEmpty(dlgEffect.SelectedLabelText))
        {
          ContentEffect temp =
            (ContentEffect)
              Enum.Parse(typeof (ContentEffect),
                Localization.ReverseTranslate("ContentEffect", dlgEffect.SelectedLabelText));

          if (g_Player.Playing)
          {
            playbackEffect = temp;
          }
          else
          {
            menuEffect = temp;
          }
          coreObject.ChangeEffect(temp);
          CalculateDelay();
        }
      }
      // Change Profile
      else if (dlg.SelectedLabelText == Localization.Translate("ContextMenu", "ChangeProfile"))
      {
        coreObject.ChangeProfile();
      }
      // Toggle 3D
      else if (dlg.SelectedLabelText == Localization.Translate("ContextMenu", "3DOn") ||
               dlg.SelectedLabelText == Localization.Translate("ContextMenu", "3DOff"))
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
      else if (dlg.SelectedLabelText == Localization.Translate("ContextMenu", "BlackbarDetection"))
      {
        GUIDialogMenu dlgBlackbarDetection = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
        dlgBlackbarDetection.Reset();
        dlgBlackbarDetection.SetHeading(Localization.Translate("ContextMenu", "BlackbarDetection"));

        if (Settings.blackbarDetection)
        {
          dlgBlackbarDetection.Add(new GUIListItem(Localization.Translate("ContextMenu", "BlackbarDetectionOff")));
        }
        else
        {
          dlgBlackbarDetection.Add(new GUIListItem(Localization.Translate("ContextMenu", "BlackbarDetectionOn")));
        }

        if (coreObject.blackbarDetectionManual)
        {
          dlgBlackbarDetection.Add(new GUIListItem(Localization.Translate("ContextMenu", "BlackbarDetectionDisableManual")));
        }

        dlgBlackbarDetection.Add(new GUIListItem(Localization.Translate("ContextMenu", "BlackbarDetection1.33:1")));
        dlgBlackbarDetection.Add(new GUIListItem(Localization.Translate("ContextMenu", "BlackbarDetection1.78:1")));
        dlgBlackbarDetection.Add(new GUIListItem(Localization.Translate("ContextMenu", "BlackbarDetection1.85:1")));
        dlgBlackbarDetection.Add(new GUIListItem(Localization.Translate("ContextMenu", "BlackbarDetection2.35:1")));
        dlgBlackbarDetection.SelectedLabel = 0;
        dlgBlackbarDetection.DoModal(GUIWindowManager.ActiveWindow);

        if (dlgBlackbarDetection.SelectedLabelText == Localization.Translate("ContextMenu", "BlackbarDetectionOn") ||
             dlgBlackbarDetection.SelectedLabelText == Localization.Translate("ContextMenu", "BlackbarDetectionOff"))
        {
          if (Settings.blackbarDetection)
          {
            Log.Info("Switching blackbar detection off.");
            Settings.blackbarDetection = false;
            coreObject.blackbarDetection = false;
          }
          else
          {
            Log.Info("Switching blackbar detection on.");
            Settings.blackbarDetection = true;
            coreObject.blackbarDetection = true;
          }
        }
        else if (dlgBlackbarDetection.SelectedLabelText == Localization.Translate("ContextMenu", "BlackbarDetectionDisableManual"))
        {
          coreObject.blackbarDetectionManual = false;
        }
        else if (dlgBlackbarDetection.SelectedLabelText == Localization.Translate("ContextMenu", "BlackbarDetection1.33:1"))
        {
          coreObject.blackbarDetectionManual = true;
          coreObject.blackbarDetectionAR = BlackbarDetectionAR._1_33x1;
        }
        else if (dlgBlackbarDetection.SelectedLabelText == Localization.Translate("ContextMenu", "BlackbarDetection1.78:1"))
        {
          coreObject.blackbarDetectionManual = true;
          coreObject.blackbarDetectionAR = BlackbarDetectionAR._1_78x1;
        }
        else if (dlgBlackbarDetection.SelectedLabelText == Localization.Translate("ContextMenu", "BlackbarDetection1.85:1"))
        {
          coreObject.blackbarDetectionManual = true;
          coreObject.blackbarDetectionAR = BlackbarDetectionAR._1_85x1;
        }
        else if (dlgBlackbarDetection.SelectedLabelText == Localization.Translate("ContextMenu", "BlackbarDetection2.35:1"))
        {
          coreObject.blackbarDetectionManual = true;
          coreObject.blackbarDetectionAR = BlackbarDetectionAR._2_35x1;
        }
      }

      // Toggle Delay
      else if (dlg.SelectedLabelText == Localization.Translate("ContextMenu", "DelayOn") ||
               dlg.SelectedLabelText == Localization.Translate("ContextMenu", "DelayOff"))
      {
        if (coreObject.IsDelayEnabled())
        {
          Log.Info("Switching LED delay off.");
          coreObject.DisableDelay();
        }
        else
        {
          coreObject.EnableDelay(
            (int)
              (((float)Settings.delayReferenceRefreshRate / (float)GetRefreshRate()) *
               (float)Settings.delayReferenceTime));
        }
      }
      // Change Delay
      else if (dlg.SelectedLabelText ==
               Localization.Translate("ContextMenu", "ChangeDelay") + " (" + coreObject.GetDelayTime() +
               Localization.Translate("Common", "MS") + ")")
      {
        if ((int.TryParse(GetKeyboardString(""), out delayTimeHelper)) && (delayTimeHelper >= 0) &&
            (delayTimeHelper <= 1000))
        {
          coreObject.SetDelay(delayTimeHelper);
          Settings.delayReferenceTime =
            (int)(((float)delayTimeHelper * (float)GetRefreshRate()) / Settings.delayReferenceRefreshRate);
        }
        else
        {
          DialogError(Localization.Translate("ContextMenu", "DelayError1"),
            Localization.Translate("ContextMenu", "DelayError2"));
        }
      }
      // Change Static Color
      else if (dlg.SelectedLabelText == Localization.Translate("ContextMenu", "ChangeStaticColor"))
      {
        GUIDialogMenu dlgStaticColor =
          (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
        dlgStaticColor.Reset();
        dlgStaticColor.SetHeading(Localization.Translate("ContextMenu", "ChangeStaticColor"));
        dlgStaticColor.Add(new GUIListItem(Localization.Translate("ContextMenu", "Manual")));
        dlgStaticColor.Add(new GUIListItem(Localization.Translate("ContextMenu", "SaveColor")));
        dlgStaticColor.Add(new GUIListItem(Localization.Translate("ContextMenu", "LoadColor")));
        dlgStaticColor.Add(new GUIListItem(Localization.Translate("ContextMenu", "White")));
        dlgStaticColor.Add(new GUIListItem(Localization.Translate("Common", "Red")));
        dlgStaticColor.Add(new GUIListItem(Localization.Translate("Common", "Green")));
        dlgStaticColor.Add(new GUIListItem(Localization.Translate("Common", "Blue")));
        dlgStaticColor.Add(new GUIListItem(Localization.Translate("ContextMenu", "Cyan")));
        dlgStaticColor.Add(new GUIListItem(Localization.Translate("ContextMenu", "Magenta")));
        dlgStaticColor.Add(new GUIListItem(Localization.Translate("Common", "Yellow")));
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
            Settings.SaveSpecificSetting("StaticColorRed", coreObject.GetStaticColor()[0].ToString());
            Settings.staticColorRed = coreObject.GetStaticColor()[0];
            Settings.SaveSpecificSetting("StaticColorGreen", coreObject.GetStaticColor()[1].ToString());
            Settings.staticColorGreen = coreObject.GetStaticColor()[1];
            Settings.SaveSpecificSetting("StaticColorBlue", coreObject.GetStaticColor()[2].ToString());
            Settings.staticColorBlue = coreObject.GetStaticColor()[2];
            break;
          case 2:
            coreObject.SetStaticColor(Settings.staticColorRed, Settings.staticColorGreen,
              Settings.staticColorBlue);
            break;
          case 3:
            coreObject.SetStaticColor(255, 255, 255);
            break;
          case 4:
            coreObject.SetStaticColor(255, 0, 0);
            break;
          case 5:
            coreObject.SetStaticColor(0, 255, 0);
            break;
          case 6:
            coreObject.SetStaticColor(0, 0, 255);
            break;
          case 7:
            coreObject.SetStaticColor(0, 255, 255);
            break;
          case 8:
            coreObject.SetStaticColor(255, 0, 255);
            break;
          case 9:
            coreObject.SetStaticColor(255, 255, 0);
            break;
        }
        coreObject.ChangeEffect(ContentEffect.StaticColor, true);
      }
      else if (dlg.SelectedLabelText == Localization.Translate("ContextMenu", "Reinitialise"))
      {
        coreObject.ReInitialise();
      }

      // Hue set active liveview group
      if (dlg.SelectedLabelText == Localization.Translate("Hue", "LiveviewGroup"))
      {
        GUIDialogMenu dlgHueSetActiveGroup =
          (GUIDialogMenu) GUIWindowManager.GetWindow((int) GUIWindow.Window.WINDOW_DIALOG_MENU);
        dlgHueSetActiveGroup.Reset();
        dlgHueSetActiveGroup.SetHeading(Localization.Translate("Hue", "LiveviewGroup"));
        dlgHueSetActiveGroup.Add(Localization.Translate("Hue", "All"));
        var hueTarget = coreObject.GetTarget(Target.Hue) as AtmoLight.Targets.HueHandler;

        List<string> groups = hueTarget.Loadgroups();

        foreach (string group in groups)
        {
          dlgHueSetActiveGroup.Add(new GUIListItem(group));
        }

        dlgHueSetActiveGroup.Add(Localization.Translate("Hue", "DisableAllGroups"));

        dlgHueSetActiveGroup.SelectedLabel = 0;
        dlgHueSetActiveGroup.DoModal(GUIWindowManager.ActiveWindow);

        if (dlgHueSetActiveGroup.SelectedLabel == 0)
        {
          hueTarget.setActiveGroup(Localization.Translate("Hue", "All"));
        }
        else if (dlgHueSetActiveGroup.SelectedLabel > 0)
        {
          hueTarget.setActiveGroup(dlgHueSetActiveGroup.SelectedLabelText);
        }
      }

      // Hue set static color for group
      if (dlg.SelectedLabelText == Localization.Translate("Hue", "StaticColorGroup"))
      {
        GUIDialogMenu dlgHueSetActiveGroup =
          (GUIDialogMenu) GUIWindowManager.GetWindow((int) GUIWindow.Window.WINDOW_DIALOG_MENU);
        dlgHueSetActiveGroup.Reset();
        dlgHueSetActiveGroup.SetHeading(Localization.Translate("Hue", "StaticColorGroup"));
        dlgHueSetActiveGroup.Add(Localization.Translate("Hue", "All"));
        var hueTarget = coreObject.GetTarget(Target.Hue) as AtmoLight.Targets.HueHandler;

        List<string> groups = hueTarget.Loadgroups();

        foreach (string group in groups)
        {
          dlgHueSetActiveGroup.Add(new GUIListItem(group));
        }

        dlgHueSetActiveGroup.SelectedLabel = 0;
        dlgHueSetActiveGroup.DoModal(GUIWindowManager.ActiveWindow);

        if (dlgHueSetActiveGroup.SelectedLabel >= 0)
        {
          string groupName = dlgHueSetActiveGroup.SelectedLabelText;
          GUIDialogMenu dlgHueSetStaticColorGroup =
            (GUIDialogMenu) GUIWindowManager.GetWindow((int) GUIWindow.Window.WINDOW_DIALOG_MENU);
          dlgHueSetStaticColorGroup.Reset();
          dlgHueSetStaticColorGroup.SetHeading(Localization.Translate("Hue", "SelectStaticColorGroup"));
          dlgHueSetStaticColorGroup.Add(Localization.Translate("Hue", "Off"));

          List<string> staticColors = hueTarget.LoadStaticColors();

          foreach (string staticColor in staticColors)
          {
            dlgHueSetStaticColorGroup.Add(new GUIListItem(staticColor));
          }

          dlgHueSetStaticColorGroup.SelectedLabel = 0;
          dlgHueSetStaticColorGroup.DoModal(GUIWindowManager.ActiveWindow);

          if (dlgHueSetStaticColorGroup.SelectedLabel >= 0)
          {
            string colorName = dlgHueSetStaticColorGroup.SelectedLabelText;
            hueTarget.setGroupStaticColor(groupName, colorName);
          }
        }
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
      GUIDialogYesNo dlgYesNo = (GUIDialogYesNo) GUIWindowManager.GetWindow((int) GUIWindow.Window.WINDOW_DIALOG_YES_NO);
      if (dlgYesNo != null)
      {
        dlgYesNo.Reset();
        dlgYesNo.SetHeading(Localization.Translate("Common", "AtmoLight"));
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
        staticColorTemp = new int[] {-1, -1, -1};
      }
      GUIDialogMenu dlgRGB = (GUIDialogMenu) GUIWindowManager.GetWindow((int) GUIWindow.Window.WINDOW_DIALOG_MENU);
      dlgRGB.Reset();
      dlgRGB.SetHeading(Localization.Translate("ContextMenu", "ManualStaticColor"));
      dlgRGB.Add(
        new GUIListItem(Localization.Translate("Common", "Red") + ": " +
                        (staticColorTemp[0] == -1
                          ? Localization.Translate("Common", "NA")
                          : staticColorTemp[0].ToString())));
      dlgRGB.Add(
        new GUIListItem(Localization.Translate("Common", "Green") + ": " +
                        (staticColorTemp[1] == -1
                          ? Localization.Translate("Common", "NA")
                          : staticColorTemp[1].ToString())));
      dlgRGB.Add(
        new GUIListItem(Localization.Translate("Common", "Blue") + ": " +
                        (staticColorTemp[2] == -1
                          ? Localization.Translate("Common", "NA")
                          : staticColorTemp[2].ToString())));
      dlgRGB.Add(new GUIListItem(Localization.Translate("Common", "Apply")));
      dlgRGB.Add(new GUIListItem(Localization.Translate("Common", "Cancel")));
      dlgRGB.SelectedLabel = StartPosition;
      dlgRGB.DoModal(GUIWindowManager.ActiveWindow);
      switch (dlgRGB.SelectedLabel)
      {
        case -1:
          return;
        case 0:
        case 1:
        case 2:
          if (
            (int.TryParse(
              GetKeyboardString((staticColorTemp[dlgRGB.SelectedLabel] == -1
                ? ""
                : staticColorTemp[dlgRGB.SelectedLabel].ToString())), out staticColorHelper)) &&
            (staticColorHelper >= 0) && (staticColorHelper <= 255))
          {
            staticColorTemp[dlgRGB.SelectedLabel] = staticColorHelper;
          }
          else
          {
            DialogError(Localization.Translate("ContextMenu", "RGBError1"),
              Localization.Translate("ContextMenu", "RGBError2"));
          }
          break;
        case 3:
          if (staticColorTemp[0] == -1 || staticColorTemp[1] == -1 || staticColorTemp[2] == -1)
          {
            DialogError(Localization.Translate("ContextMenu", "RGBError1"),
              Localization.Translate("ContextMenu", "RGBError2"));
            break;
          }
          else
          {
            coreObject.SetStaticColor(staticColorTemp[0], staticColorTemp[1], staticColorTemp[2]);
            return;
          }
        case 4:
          return;
      }
      // Start the dialog again (without reset) so we can enter the other colors.
      DialogRGBManualStaticColorChanger(false, dlgRGB.SelectedLabel);
    }

    #endregion

    #region PowerModeChanged Event

    private void PowerModeChanged(object sender, PowerModeChangedEventArgs powerMode)
    {
      if (powerMode.Mode == PowerModes.Resume)
      {
        if (CheckForStartRequirements())
        {
          coreObject.ChangeEffect(menuEffect, true, true);
        }
        else
        {
          coreObject.ChangeEffect(ContentEffect.LEDsDisabled, true, true);
        }
      }

      Task.Factory.StartNew(() => { coreObject.PowerModeChanged(powerMode.Mode); });
    }
    #endregion

    #region Monitor screensaver and/or window state

    private void monitorMediaportalState()
    {
      Boolean LEDsDisabledByMediaportalState = false;

      while (Settings.monitorScreensaverState || Settings.monitorWindowState)
      {
        try
        {
          // Check for screensaver window and/or Window state, blank screen for screensaver / Suspended for window state
          if ((GUIGraphicsContext.BlankScreen && Settings.monitorScreensaverState) ||
             (GUIGraphicsContext.CurrentState == GUIGraphicsContext.State.SUSPENDING && Settings.monitorWindowState))
          {
            ContentEffect currenContentEffect = coreObject.GetCurrentEffect();

            if (currenContentEffect == ContentEffect.MediaPortalLiveMode ||
                currenContentEffect == ContentEffect.ExternalLiveMode || currenContentEffect == ContentEffect.Undefined)
            {
              coreObject.ChangeEffect(ContentEffect.LEDsDisabled);
              LEDsDisabledByMediaportalState = true;
              Log.Debug("LEDs should be deactivated. (Screensaver detected and/or window state is minimized/suspended)");
            }
          }
          // Restore last known effect when coming back from screensaver and/or suspended window state
          else if (LEDsDisabledByMediaportalState)
          {
            LEDsDisabledByMediaportalState = false;

            if (g_Player.Playing)
            {
              coreObject.ChangeEffect(playbackEffect);
              CalculateDelay();
            }
            else
            {
              coreObject.ChangeEffect(menuEffect);
              CalculateDelay();
            }

            Log.Debug("LEDs should be activated again. (Screensaver and or minimzed/suspended state no longer active)");
          }

          // Sleep for 5 seconds
          int sleepTime = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
          Thread.Sleep(sleepTime);
        }
        catch (Exception)
        {
          // Sleep for 5 seconds
          int sleepTime = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
          Thread.Sleep(sleepTime);
        }
      }
    }
    #endregion

    #region ISetupForm impementation

    /// <summary>
    ///  Returns authors name.
    /// </summary>
    /// <returns>Authors name.</returns>
    public string Author()
    {
      return "gemx, Angie05, Rick164, legnod, azzuro, BassFan, Lightning303, HomeY, Sebastiii";
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
    public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus,
      out string strPictureImage)
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
      if (atmoLightSetupForm == null)
      {
        // Log Handler
        Log.OnNewLog += new Log.NewLogHandler(OnNewLog);

        var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        DateTime buildDate = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTime;
        Log.Info("Version {0}.{1}.{2}.{3}, build on {4} at {5}.", version.Major, version.Minor, version.Build,
          version.Revision, buildDate.ToShortDateString(), buildDate.ToLongTimeString());
        Log.Debug("Loading settings.");
        Settings.LoadSettings();
        atmoLightSetupForm = new SetupForm();
      }
      atmoLightSetupForm.ShowDialog();
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
