using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32;
using MediaPortal.Common;
using MediaPortal.UI.Presentation.Players;
using MediaPortal.UI.Presentation.UiNotifications;
using MediaPortal.Common.Logging;
using MediaPortal.Common.PluginManager;
using MediaPortal.UI.SkinEngine.Players;
using MediaPortal.UI.SkinEngine.SkinManagement;
using MediaPortal.Common.Messaging;
using MediaPortal.Common.Runtime;
using MediaPortal.UI.Control.InputManager;
using MediaPortal.UI.Presentation.Actions;
using SharpDX;
using SharpDX.Direct3D9;


namespace AtmoLight
{
  public class Plugin : IPluginStateTracker
  {
    #region Fields
    protected AsynchronousMessageQueue messageQueue;
    private Core AtmoLightObject;

    // Settings
    private AtmoLight.Settings settings;
    private ContentEffect menuEffect = ContentEffect.Undefined;
    private ContentEffect playbackEffect = ContentEffect.Undefined;

    private Int64 lastFrame = 0;

    // Surfaces
    private SharpDX.Direct3D9.Surface surfaceSource; // Source Surface
    private SharpDX.Direct3D9.Surface surfaceDestination; // Destination Surface (resized)

    // Player helper
    private IPlayerManager playerManager;
    private ISharpDXVideoPlayer player;
    #endregion

    #region Win32API
    public sealed class Win32API
    {
      [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
      public static extern Int64 GetTickCount();
    }
    #endregion

    #region IPluginStateTracker implementation
    public void Activated(PluginRuntime pluginRuntime)
    {
      messageQueue = new AsynchronousMessageQueue(this, new string[] { SystemMessaging.CHANNEL, PlayerManagerMessaging.CHANNEL });
      messageQueue.MessageReceived += OnMessageReceived;
      messageQueue.Start();
    }

    public void Stop()
    {
      return;
    }

    public void Shutdown()
    {
      Dispose();
      return;
    }

    public void Continue()
    {
      return;
    }

    public bool RequestEnd()
    {
      Dispose();
      return true;
    }

    #endregion

    #region Initialise
    private void Initialise()
    {
      // Log Handler
      Log.OnNewLog += new Log.NewLogHandler(OnNewLog);

      // Version Infos
      var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
      DateTime buildDate = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTime;
      Log.Info("Version {0}.{1}.{2}.{3}, build on {4} at {5}.", version.Major, version.Minor, version.Build, version.Revision, buildDate.ToShortDateString(), buildDate.ToLongTimeString());

      // Settings
      Log.Debug("Loading settings.");
      settings = new AtmoLight.Settings();
      settings.LoadAll();
      settings.SaveAll();

      // AtmoLight object creation
      Log.Debug("Generating new AtmoLight.Core instance.");
      AtmoLightObject = Core.GetInstance();

      // AtmoWin
      if (settings.AtmoWinTarget)
      {
        AtmoLightObject.AddTarget(Target.AtmoWin);
      }
      AtmoLightObject.atmoWinPath = settings.AtmoWinExe;
      AtmoLightObject.atmoWinAutoStart = settings.StartAtmoWinOnStart;
      AtmoLightObject.atmoWinAutoStop = settings.StopAtmoWinOnExit;

      // Boblight
      if (settings.BoblightTarget)
      {
        AtmoLightObject.AddTarget(Target.Boblight);
      }
      AtmoLightObject.boblightIP = settings.BoblightIP;
      AtmoLightObject.boblightPort = settings.BoblightPort;
      AtmoLightObject.boblightMaxFPS = settings.BoblightMaxFPS;
      AtmoLightObject.boblightMaxReconnectAttempts = settings.BoblightMaxReconnectAttempts;
      AtmoLightObject.boblightReconnectDelay = settings.BoblightReconnectDelay;
      AtmoLightObject.boblightSpeed = settings.BoblightSpeed;
      AtmoLightObject.boblightAutospeed = settings.BoblightAutospeed;
      AtmoLightObject.boblightInterpolation = settings.BoblightInterpolation;
      AtmoLightObject.boblightSaturation = settings.BoblightSaturation;
      AtmoLightObject.boblightValue = settings.BoblightValue;
      AtmoLightObject.boblightThreshold = settings.BoblightThreshold;
      AtmoLightObject.boblightGamma = settings.BoblightGamma;

      // Hyperion
      if (settings.HyperionTarget)
      {
        AtmoLightObject.AddTarget(Target.Hyperion);
      }
      AtmoLightObject.hyperionIP = settings.HyperionIP;
      AtmoLightObject.hyperionPort = settings.HyperionPort;
      AtmoLightObject.hyperionPriority = settings.HyperionPriority;
      AtmoLightObject.hyperionReconnectDelay = settings.HyperionReconnectDelay;
      AtmoLightObject.hyperionReconnectAttempts = settings.HyperionReconnectAttempts;
      AtmoLightObject.hyperionPriorityStaticColor = settings.HyperionPriorityStaticColor;
      AtmoLightObject.hyperionLiveReconnect = settings.HyperionLiveReconnect;

      //Hue
      if (settings.HueTarget)
      {
        AtmoLightObject.AddTarget(Target.Hue);
      }
      AtmoLightObject.huePath = settings.hueExe;
      AtmoLightObject.hueStart = settings.hueStart;
      AtmoLightObject.hueIsRemoteMachine = settings.hueIsRemoteMachine;
      AtmoLightObject.hueIP = settings.HueIP;
      AtmoLightObject.huePort = settings.HuePort;
      AtmoLightObject.hueReconnectDelay = settings.HueReconnectDelay;
      AtmoLightObject.hueReconnectAttempts = settings.HueReconnectAttempts;
      AtmoLightObject.hueMinimalColorDifference = settings.HueMinimalColorDifference;
      AtmoLightObject.hueBridgeEnableOnResume = settings.HueBridgeEnableOnResume;
      AtmoLightObject.hueBridgeDisableOnSuspend = settings.HueBridgeDisableOnSuspend;

      // General settings
      AtmoLightObject.SetDelay(settings.DelayTime);
      AtmoLightObject.SetGIFPath(settings.GIFFile);
      AtmoLightObject.SetReInitOnError(settings.RestartAtmoWinOnError);
      AtmoLightObject.SetStaticColor(settings.StaticColorRed, settings.StaticColorGreen, settings.StaticColorBlue);
      AtmoLightObject.SetCaptureDimensions(settings.CaptureWidth, settings.CaptureHeight);
      AtmoLightObject.blackbarDetection = settings.BlackbarDetection;
      AtmoLightObject.blackbarDetectionTime = settings.BlackbarDetectionTime;
      AtmoLightObject.blackbarDetectionThreshold = settings.BlackbarDetectionThreshold;
      AtmoLightObject.powerModeChangedDelay = settings.PowerModeChangedDelay;

      menuEffect = settings.MenuEffect;
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

      // Handlers
      Core.OnNewConnectionLost += new Core.NewConnectionLostHandler(OnNewConnectionLost);
      Core.OnNewVUMeter += new Core.NewVUMeterHander(OnNewVUMeter);
      AtmoLight.Configuration.OnOffButton.ButtonsChanged += new Configuration.OnOffButton.ButtonsChangedHandler(ReregisterKeyBindings);
      AtmoLight.Configuration.ProfileButton.ButtonsChanged += new Configuration.ProfileButton.ButtonsChangedHandler(ReregisterKeyBindings);
      SkinContext.DeviceSceneEnd += UICapture;
      SystemEvents.PowerModeChanged += PowerModeChanged;
      RegisterSettingsChangedHandler();
      RegisterKeyBindings();
    }

    private void Dispose()
    {
      // Unregister handlers
      UnregisterKeyBindings();

      SkinContext.DeviceSceneEnd -= UICapture;
      if (surfaceDestination != null)
      {
        surfaceDestination.Dispose();
        surfaceDestination = null;
      }

      messageQueue.MessageReceived -= OnMessageReceived;

      // Dispose of the AtmoLight Core
      AtmoLightObject.ChangeEffect(settings.MPExitEffect);

      AtmoLightObject.Dispose();

      // Unregister Log Handler
      Log.OnNewLog -= new Log.NewLogHandler(OnNewLog);
      Core.OnNewConnectionLost -= new Core.NewConnectionLostHandler(OnNewConnectionLost);
      Core.OnNewVUMeter -= new Core.NewVUMeterHander(OnNewVUMeter);
      AtmoLight.Configuration.OnOffButton.ButtonsChanged -= new Configuration.OnOffButton.ButtonsChangedHandler(ReregisterKeyBindings);
      AtmoLight.Configuration.ProfileButton.ButtonsChanged -= new Configuration.ProfileButton.ButtonsChangedHandler(ReregisterKeyBindings);
      SystemEvents.PowerModeChanged -= PowerModeChanged;
      UnregisterSettingsChangedHandler();
    }
    #endregion

    #region Utilities
    /// <summary>
    /// Check if LEDs should be activated.
    /// </summary>
    /// <returns>true or false</returns>
    private bool CheckForStartRequirements()
    {
      if (settings.ManualMode)
      {
        Log.Debug("LEDs should be deactivated. (Manual Mode)");
        return false;
      }
      else if ((DateTime.Now >= Convert.ToDateTime(settings.ExcludeTimeStart) && DateTime.Now <= Convert.ToDateTime(settings.ExcludeTimeEnd)) ||
              ((Convert.ToDateTime(settings.ExcludeTimeStart) > Convert.ToDateTime(settings.ExcludeTimeEnd)) &&
              ((DateTime.Now <= Convert.ToDateTime(settings.ExcludeTimeStart) && DateTime.Now <= Convert.ToDateTime(settings.ExcludeTimeEnd)) ||
              (DateTime.Now >= Convert.ToDateTime(settings.ExcludeTimeStart) && DateTime.Now >= Convert.ToDateTime(settings.ExcludeTimeEnd)))))
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
    /// Calculates the delay dependent on the refresh rate.
    /// </summary>
    private void CalculateDelay()
    {
      if (AtmoLightObject.GetCurrentEffect() == ContentEffect.MediaPortalLiveMode && AtmoLightObject.IsDelayEnabled())
      {
        int refreshRate = SkinContext.Direct3D.GetAdapterDisplayModeEx(0).RefreshRate;
        AtmoLightObject.SetDelay((int)(((float)settings.DelayRefreshRate / (float)refreshRate) * (float)settings.DelayTime));
      }
    }
    #endregion

    #region Message Handler
    void OnMessageReceived(AsynchronousMessageQueue queue, SystemMessage message)
    {
      if (message.ChannelName == SystemMessaging.CHANNEL)
      {
        SystemMessaging.MessageType messageType = (SystemMessaging.MessageType)message.MessageType;
        if (messageType == SystemMessaging.MessageType.SystemStateChanged)
        {
          SystemState newState = (SystemState)message.MessageData[SystemMessaging.NEW_STATE];
          if (newState == SystemState.Running)
          {
            Initialise();
          }
        }
      }
      else if (message.ChannelName == PlayerManagerMessaging.CHANNEL)
      {
        PlayerManagerMessaging.MessageType messageType = (PlayerManagerMessaging.MessageType)message.MessageType;
        if (messageType == PlayerManagerMessaging.MessageType.PlayerStarted)
        {
          // What kind of playback?
          if (ServiceRegistration.Get<IPlayerContextManager>().IsVideoContextActive)
          {
            Log.Info("Video started.");
            playbackEffect = settings.VideoEffect;
          }
          else if (ServiceRegistration.Get<IPlayerContextManager>().IsAudioContextActive)
          {
            Log.Info("Audio started.");
            playbackEffect = settings.AudioEffect;
          }

          // Start the right effect.
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
        else if (messageType == PlayerManagerMessaging.MessageType.PlayerStopped)
        {
          Log.Info("Playback stopped.");
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
      }
    }
    #endregion

    #region UI Capture Event Handler
    public void UICapture(object sender, EventArgs args)
    {
      if (!AtmoLightObject.IsConnected() || !AtmoLightObject.IsAtmoLightOn() || AtmoLightObject.GetCurrentEffect() != ContentEffect.MediaPortalLiveMode)
      {
        return;
      }

      // Low CPU setting.
      // Skip frame if LowCPUTime has not yet passed since last frame.
      if (settings.LowCPU)
      {
        if ((Win32API.GetTickCount() - lastFrame) < settings.LowCPUTime)
        {
          return;
        }
        else
        {
          lastFrame = Win32API.GetTickCount();
        }
      }

      Rectangle rectangleDestination = new Rectangle(0, 0, AtmoLightObject.GetCaptureWidth(), AtmoLightObject.GetCaptureHeight());
      try
      {
        if (surfaceDestination == null)
        {
          surfaceDestination = SharpDX.Direct3D9.Surface.CreateRenderTarget(SkinContext.Device, AtmoLightObject.GetCaptureWidth(), AtmoLightObject.GetCaptureHeight(), SharpDX.Direct3D9.Format.A8R8G8B8, SharpDX.Direct3D9.MultisampleType.None, 0, true);
        }

        // Use the Player Surface if video is playing.
        // This results in lower time to calculate aswell as blackbar removal
        if (ServiceRegistration.Get<IPlayerContextManager>().IsVideoContextActive)
        {
          playerManager = ServiceRegistration.Get<IPlayerManager>();
          playerManager.ForEach(psc =>
          {
            player = psc.CurrentPlayer as ISharpDXVideoPlayer;
            if (player == null || player.Surface == null)
            {
              return;
            }
            surfaceSource = player.Surface;
          });
        }
        else
        {
          surfaceSource = SkinContext.Device.GetRenderTarget(0);
        }

        surfaceSource.Device.StretchRectangle(surfaceSource, null, surfaceDestination, rectangleDestination, SharpDX.Direct3D9.TextureFilter.None);
        DataStream stream = SharpDX.Direct3D9.Surface.ToStream(surfaceDestination, SharpDX.Direct3D9.ImageFileFormat.Bmp);

        AtmoLightObject.CalculateBitmap(stream);

        stream.Close();
        stream.Dispose();
      }
      catch (Exception ex)
      {
        surfaceDestination.Dispose();
        surfaceDestination = null;
        Log.Error("Exception: {0}", ex.Message);
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
          ServiceRegistration.Get<ILogger>().Debug(String.Format(format, args));
          break;
        case Log.LogLevel.Warn:
          ServiceRegistration.Get<ILogger>().Warn(String.Format(format, args));
          break;
        case Log.LogLevel.Info:
          ServiceRegistration.Get<ILogger>().Info(String.Format(format, args));
          break;
        case Log.LogLevel.Error:
          ServiceRegistration.Get<ILogger>().Error(String.Format(format, args));
          break;
      }
    }
    #endregion

    #region Key Bindings
    private void ReregisterKeyBindings()
    {
      UnregisterKeyBindings();
      settings.LoadAll();
      settings.SaveAll();
      RegisterKeyBindings();
    }

    private void RegisterKeyBindings()
    {
      IInputManager manager = ServiceRegistration.Get<IInputManager>(false);
      if (manager != null)
      {
        if (settings.OnOffButton == 1)
        {
          manager.AddKeyBinding(Key.Red, new VoidKeyActionDlgt(ToggleEffectOnOff));
        }
        else if (settings.OnOffButton == 2)
        {
          manager.AddKeyBinding(Key.Green, new VoidKeyActionDlgt(ToggleEffectOnOff));
        }
        else if (settings.OnOffButton == 3)
        {
          manager.AddKeyBinding(Key.Yellow, new VoidKeyActionDlgt(ToggleEffectOnOff));
        }
        else if (settings.OnOffButton == 4)
        {
          manager.AddKeyBinding(Key.Blue, new VoidKeyActionDlgt(ToggleEffectOnOff));
        }

        if (settings.ProfileButton == 1)
        {
          manager.AddKeyBinding(Key.Red, new VoidKeyActionDlgt(ChangeAtmoWinProfile));
        }
        else if (settings.ProfileButton == 2)
        {
          manager.AddKeyBinding(Key.Green, new VoidKeyActionDlgt(ChangeAtmoWinProfile));
        }
        else if (settings.ProfileButton == 3)
        {
          manager.AddKeyBinding(Key.Yellow, new VoidKeyActionDlgt(ChangeAtmoWinProfile));
        }
        else if (settings.ProfileButton == 4)
        {
          manager.AddKeyBinding(Key.Blue, new VoidKeyActionDlgt(ChangeAtmoWinProfile));
        }
      }
    }

    private void UnregisterKeyBindings()
    {
      IInputManager manager = ServiceRegistration.Get<IInputManager>(false);
      if (manager != null)
      {
        manager.RemoveKeyBinding(Key.Red);
        manager.RemoveKeyBinding(Key.Green);
        manager.RemoveKeyBinding(Key.Yellow);
        manager.RemoveKeyBinding(Key.Blue);
      }
    }

    private void ToggleEffectOnOff()
    {
      if (AtmoLightObject.IsConnected())
      {
        if (AtmoLightObject.IsAtmoLightOn())
        {
          AtmoLightObject.ChangeEffect(ContentEffect.LEDsDisabled);
        }
        else
        {
          ContentEffect temp;
          if (ServiceRegistration.Get<IPlayerContextManager>().IsVideoContextActive)
          {
            temp = settings.VideoEffect;
          }
          else if (ServiceRegistration.Get<IPlayerContextManager>().IsAudioContextActive)
          {
            temp = settings.AudioEffect;
          }
          else
          {
            temp = settings.MenuEffect;
          }
          AtmoLightObject.ChangeEffect(temp);
          CalculateDelay();
        }
      }
      else
      {
        AtmoLightObject.ReInitialise();
      }
    }

    private void ChangeAtmoWinProfile()
    {
      if (AtmoLightObject.IsConnected())
      {
        AtmoLightObject.ChangeProfile();
      }
      else
      {
        AtmoLightObject.ReInitialise();
      }
    }
    #endregion

    #region Connection Lost Handler
    /// <summary>
    /// Connection lost event handler.
    /// This event gets called if connection to AtmoWin is lost and not recoverable.
    /// </summary>
    private void OnNewConnectionLost(Target target)
    {
      ServiceRegistration.Get<INotificationService>().EnqueueNotification(NotificationType.Error, "[AtmoLight.Name]", MediaPortal.Common.Localization.LocalizationHelper.Translate("[AtmoLight.AtmoWinConnectionLost]").Replace("[Target]", target.ToString()), true);
    }
    #endregion

    #region Settings Lost Hander
    private void RegisterSettingsChangedHandler()
    {
      AtmoLight.Configuration.VideoEffect.SettingsChanged += new Configuration.VideoEffect.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.AudioEffect.SettingsChanged += new Configuration.AudioEffect.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.MenuEffect.SettingsChanged += new Configuration.MenuEffect.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.MPExitEffect.SettingsChanged += new Configuration.MPExitEffect.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.ManualMode.SettingsChanged += new Configuration.ManualMode.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.SBS3D.SettingsChanged += new Configuration.SBS3D.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.LowCPU.SettingsChanged += new Configuration.LowCPU.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.LowCPUTime.SettingsChanged += new Configuration.LowCPUTime.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.DelayTime.SettingsChanged += new Configuration.DelayTime.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.DelayRefreshRate.SettingsChanged += new Configuration.DelayRefreshRate.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.ExcludeTimeStartHour.SettingsChanged += new Configuration.ExcludeTimeStartHour.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.ExcludeTimeEndHour.SettingsChanged += new Configuration.ExcludeTimeEndHour.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.ExcludeTimeStartMinutes.SettingsChanged += new Configuration.ExcludeTimeStartMinutes.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.ExcludeTimeEndMinutes.SettingsChanged += new Configuration.ExcludeTimeEndMinutes.SettingsChangedHandler(ReloadSettings);
    }

    private void UnregisterSettingsChangedHandler()
    {
      AtmoLight.Configuration.VideoEffect.SettingsChanged -= new Configuration.VideoEffect.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.AudioEffect.SettingsChanged -= new Configuration.AudioEffect.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.MenuEffect.SettingsChanged -= new Configuration.MenuEffect.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.MPExitEffect.SettingsChanged -= new Configuration.MPExitEffect.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.ManualMode.SettingsChanged -= new Configuration.ManualMode.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.SBS3D.SettingsChanged -= new Configuration.SBS3D.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.LowCPU.SettingsChanged -= new Configuration.LowCPU.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.LowCPUTime.SettingsChanged -= new Configuration.LowCPUTime.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.DelayTime.SettingsChanged -= new Configuration.DelayTime.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.DelayRefreshRate.SettingsChanged -= new Configuration.DelayRefreshRate.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.ExcludeTimeStartHour.SettingsChanged -= new Configuration.ExcludeTimeStartHour.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.ExcludeTimeEndHour.SettingsChanged -= new Configuration.ExcludeTimeEndHour.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.ExcludeTimeStartMinutes.SettingsChanged -= new Configuration.ExcludeTimeStartMinutes.SettingsChangedHandler(ReloadSettings);
      AtmoLight.Configuration.ExcludeTimeEndMinutes.SettingsChanged -= new Configuration.ExcludeTimeEndMinutes.SettingsChangedHandler(ReloadSettings);
    }

    private void ReloadSettings()
    {
      settings.LoadAll();
    }

    #endregion

    #region VU Meter Event Handler
    private double[] OnNewVUMeter()
    {
      double[] dbLevel = new double[] { -200.0, -200.0 };

      IPlayerContextManager playerContextManager = ServiceRegistration.Get<IPlayerContextManager>(false);
      if (playerContextManager == null)
      {
        return dbLevel;
      }

      IPlayerContext playerContext = playerContextManager.GetPlayerContext(PlayerChoice.PrimaryPlayer);
      if (playerContext == null)
      {
        return dbLevel;
      }

      IAudioPlayerAnalyze player = (playerContext.CurrentPlayer as IAudioPlayerAnalyze);
      if (player == null)
      {
        return dbLevel;
      }

      double dbLevelL;
      double dbLevelR;
      if (player.GetChannelLevel(out dbLevelL, out dbLevelR))
      {
        dbLevel[0] = dbLevelL;
        dbLevel[1] = dbLevelR;
      }
      return dbLevel;
    }
    #endregion

    #region PowerModeChanged Event
    private void PowerModeChanged(object sender, PowerModeChangedEventArgs powerMode)
    {
      if (powerMode.Mode == PowerModes.Resume)
      {
        if (CheckForStartRequirements())
        {
          AtmoLightObject.SetInitialEffect(menuEffect);
        }
        else
        {
          AtmoLightObject.SetInitialEffect(ContentEffect.LEDsDisabled);
        }
      }

      Task.Factory.StartNew(() => { AtmoLightObject.PowerModeChanged(powerMode.Mode); });
    }
    #endregion
  }
}