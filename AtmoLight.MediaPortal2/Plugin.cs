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
    private Core coreObject;

    // Settings
    private AtmoLight.Settings settings;

    private Int64 lastFrame = 0;

    // Surfaces
    private SharpDX.Direct3D9.Surface surfaceSource; // Source Surface
    private SharpDX.Direct3D9.Surface surfaceDestination; // Destination Surface (resized)

    // Player helper
    private ISharpDXVideoPlayer player;
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
      coreObject = Core.GetInstance();

      // General settings
      coreObject.SetDelay(settings.DelayTime);
      coreObject.SetGIFPath(settings.GIFFile);
      coreObject.SetReInitOnError(settings.RestartAtmoWinOnError);
      coreObject.SetStaticColor(settings.StaticColorRed, settings.StaticColorGreen, settings.StaticColorBlue);
      coreObject.SetCaptureDimensions(settings.CaptureWidth, settings.CaptureHeight);
      coreObject.blackbarDetection = settings.BlackbarDetection;
      coreObject.blackbarDetectionTime = settings.BlackbarDetectionTime;
      coreObject.blackbarDetectionThreshold = settings.BlackbarDetectionThreshold;
      coreObject.powerModeChangedDelay = settings.PowerModeChangedDelay;

      // AmbiBox
      coreObject.ambiBoxAutoStart = settings.AmbiBoxAutoStart;
      coreObject.ambiBoxAutoStop = settings.AmbiBoxAutoStop;
      coreObject.ambiBoxExternalProfile = settings.AmbiBoxExternalProfile;
      coreObject.ambiBoxIP = settings.AmbiBoxIP;
      coreObject.ambiBoxMaxReconnectAttempts = settings.AmbiBoxMaxReconnectAttempts;
      coreObject.ambiBoxMediaPortalProfile = settings.AmbiBoxMediaPortalProfile;
      coreObject.ambiBoxPath = settings.AmbiBoxPath;
      coreObject.ambiBoxPort = settings.AmbiBoxPort;
      coreObject.ambiBoxReconnectDelay = settings.AmbiBoxReconnectDelay;
      if (settings.AmbiBoxTarget)
      {
        coreObject.AddTarget(Target.AmbiBox);
      }

      // AtmoWin
      coreObject.atmoWinPath = settings.AtmoWinExe;
      coreObject.atmoWinAutoStart = settings.StartAtmoWinOnStart;
      coreObject.atmoWinAutoStop = settings.StopAtmoWinOnExit;
      if (settings.AtmoWinTarget)
      {
        coreObject.AddTarget(Target.AtmoWin);
      }

      // Boblight
      coreObject.boblightIP = settings.BoblightIP;
      coreObject.boblightPort = settings.BoblightPort;
      coreObject.boblightMaxFPS = settings.BoblightMaxFPS;
      coreObject.boblightMaxReconnectAttempts = settings.BoblightMaxReconnectAttempts;
      coreObject.boblightReconnectDelay = settings.BoblightReconnectDelay;
      coreObject.boblightSpeed = settings.BoblightSpeed;
      coreObject.boblightAutospeed = settings.BoblightAutospeed;
      coreObject.boblightInterpolation = settings.BoblightInterpolation;
      coreObject.boblightSaturation = settings.BoblightSaturation;
      coreObject.boblightValue = settings.BoblightValue;
      coreObject.boblightThreshold = settings.BoblightThreshold;
      coreObject.boblightGamma = settings.BoblightGamma;
      if (settings.BoblightTarget)
      {
        coreObject.AddTarget(Target.Boblight);
      }

      // Hyperion
      coreObject.hyperionIP = settings.HyperionIP;
      coreObject.hyperionPort = settings.HyperionPort;
      coreObject.hyperionPriority = settings.HyperionPriority;
      coreObject.hyperionReconnectDelay = settings.HyperionReconnectDelay;
      coreObject.hyperionReconnectAttempts = settings.HyperionReconnectAttempts;
      coreObject.hyperionPriorityStaticColor = settings.HyperionPriorityStaticColor;
      coreObject.hyperionLiveReconnect = settings.HyperionLiveReconnect;
      if (settings.HyperionTarget)
      {
        coreObject.AddTarget(Target.Hyperion);
      }

      //Hue
      coreObject.huePath = settings.hueExe;
      coreObject.hueStart = settings.hueStart;
      coreObject.hueIsRemoteMachine = settings.hueIsRemoteMachine;
      coreObject.hueIP = settings.HueIP;
      coreObject.huePort = settings.HuePort;
      coreObject.hueReconnectDelay = settings.HueReconnectDelay;
      coreObject.hueReconnectAttempts = settings.HueReconnectAttempts;
      coreObject.hueBridgeEnableOnResume = settings.HueBridgeEnableOnResume;
      coreObject.hueBridgeDisableOnSuspend = settings.HueBridgeDisableOnSuspend;
      coreObject.hueThreshold = settings.HueThreshold;
      coreObject.hueBlackThreshold = settings.HueBlackThreshold;
      coreObject.hueMinDiversion = settings.HueMinDiversion;
      coreObject.hueSaturation = settings.HueSaturation;
      coreObject.hueUseOverallLightness = settings.HueUseOverallLightness;
      if (settings.HueTarget)
      {
        coreObject.AddTarget(Target.Hue);
      }

      if (CheckForStartRequirements())
      {
        coreObject.ChangeEffect(settings.MenuEffect, true);
      }
      else
      {
        coreObject.ChangeEffect(ContentEffect.LEDsDisabled, true);
      }

      coreObject.Initialise();

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
      coreObject.ChangeEffect(settings.MPExitEffect);

      coreObject.Dispose();

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
      if (coreObject.GetCurrentEffect() == ContentEffect.MediaPortalLiveMode && coreObject.IsDelayEnabled())
      {
        int refreshRate = SkinContext.Direct3D.GetAdapterDisplayModeEx(0).RefreshRate;
        coreObject.SetDelay((int)(((float)settings.DelayRefreshRate / (float)refreshRate) * (float)settings.DelayTime));
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
          Log.Info("Playback started.");
          ContentEffect effect;
          // What kind of playback?
          // Video gets highest priority in PiP.
          if (ServiceRegistration.Get<IPlayerContextManager>().IsVideoContextActive)
          {
            effect = settings.VideoEffect;
          }
          else if (ServiceRegistration.Get<IPlayerContextManager>().IsAudioContextActive)
          {
            effect = settings.AudioEffect;
          }
          else
          {
            return;
          }

          // Start the right effect.
          if (CheckForStartRequirements())
          {
            coreObject.ChangeEffect(effect);
            CalculateDelay();
          }
          else
          {
            coreObject.ChangeEffect(ContentEffect.LEDsDisabled);
          }
        }
        else if (messageType == PlayerManagerMessaging.MessageType.PlayerStopped || messageType == PlayerManagerMessaging.MessageType.PlayerEnded)
        {
          Log.Info("Playback stopped.");
          ContentEffect effect;
          // Is there still something playing? (PiP)
          // If yes, what?
          if (ServiceRegistration.Get<IPlayerContextManager>().IsVideoContextActive)
          {
            Log.Info("Another video player is still active (PiP).");
            effect = settings.VideoEffect;
          }
          else if (ServiceRegistration.Get<IPlayerContextManager>().IsAudioContextActive)
          {
            Log.Info("Another audio player is still active (PiP).");
            effect = settings.AudioEffect;
          }
          else
          {
            effect = settings.MenuEffect;
          }

          if (CheckForStartRequirements())
          {
            coreObject.ChangeEffect(effect);
            CalculateDelay();
          }
          else
          {
            coreObject.ChangeEffect(ContentEffect.LEDsDisabled);
          }

        }
      }
    }
    #endregion

    #region UI Capture Event Handler
    public void UICapture(object sender, EventArgs args)
    {
      if (!coreObject.IsConnected() || !coreObject.IsAtmoLightOn() || coreObject.GetCurrentEffect() != ContentEffect.MediaPortalLiveMode)
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

      Rectangle rectangleDestination = new Rectangle(0, 0, coreObject.GetCaptureWidth(), coreObject.GetCaptureHeight());
      try
      {
        if (surfaceDestination == null)
        {
          surfaceDestination = SharpDX.Direct3D9.Surface.CreateRenderTarget(SkinContext.Device, coreObject.GetCaptureWidth(), coreObject.GetCaptureHeight(), SharpDX.Direct3D9.Format.A8R8G8B8, SharpDX.Direct3D9.MultisampleType.None, 0, true);
        }

        // Use the Player Surface if video is playing.
        // This results in lower time to calculate aswell as blackbar removal
        if (ServiceRegistration.Get<IPlayerContextManager>().IsVideoContextActive)
        {
          player = ServiceRegistration.Get<IPlayerContextManager>().PrimaryPlayerContext.CurrentPlayer as ISharpDXVideoPlayer;
          surfaceSource = player.Surface;
        }
        else
        {
          surfaceSource = SkinContext.Device.GetRenderTarget(0);
        }

        surfaceSource.Device.StretchRectangle(surfaceSource, null, surfaceDestination, rectangleDestination, SharpDX.Direct3D9.TextureFilter.None);
        DataStream stream = SharpDX.Direct3D9.Surface.ToStream(surfaceDestination, SharpDX.Direct3D9.ImageFileFormat.Bmp);

        coreObject.CalculateBitmap(stream);

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
      if (coreObject.IsConnected())
      {
        if (coreObject.IsAtmoLightOn())
        {
          coreObject.ChangeEffect(ContentEffect.LEDsDisabled);
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
          coreObject.ChangeEffect(temp);
          CalculateDelay();
        }
      }
      else
      {
        coreObject.ReInitialise();
      }
    }

    private void ChangeAtmoWinProfile()
    {
      if (coreObject.IsConnected())
      {
        coreObject.ChangeProfile();
      }
      else
      {
        coreObject.ReInitialise();
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

    #region Settings Changed Hander
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
          coreObject.ChangeEffect(settings.MenuEffect, true);
        }
        else
        {
          coreObject.ChangeEffect(ContentEffect.LEDsDisabled, true);
        }
      }

      Task.Factory.StartNew(() => { coreObject.PowerModeChanged(powerMode.Mode); });
    }
    #endregion
  }
}