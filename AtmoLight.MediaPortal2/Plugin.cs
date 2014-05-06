using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
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
    public static Core AtmoLightObject;

    // Settings
    AtmoLight.Settings settings;
    private int[] staticColorTemp = { 0, 0, 0 };
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
      Log.Info("AtmoLight: Version {0}.{1}.{2}.{3}, build on {4} at {5}.", version.Major, version.Minor, version.Build, version.Revision, buildDate.ToShortDateString(), buildDate.ToLongTimeString());

      // Settings
      Log.Debug("Loading settings.");
      settings = new AtmoLight.Settings();
      settings.LoadAll();
      settings.SaveAll();
      staticColorTemp[0] = settings.StaticColorRed;
      staticColorTemp[1] = settings.StaticColorGreen;
      staticColorTemp[2] = settings.StaticColorBlue;
      menuEffect = settings.MenuEffect;

      // AtmoLight object creation
      Log.Debug("Generating new AtmoLight.Core instance.");
      AtmoLightObject = new Core(settings.AtmoWinExe, settings.RestartAtmoWinOnError, settings.StartAtmoWinOnStart, staticColorTemp, settings.Delay, settings.DelayTime);

      // Handlers
      Core.OnNewConnectionLost += new Core.NewConnectionLostHandler(OnNewConnectionLost);
      SkinContext.DeviceSceneEnd += UICapture;
      RegisterKeyBindings();

      // AtmoLight initialisation
      if (!AtmoLightObject.Initialise())
      {
        Log.Error("Initialising failed.");
        return;
      }

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

      // Disconnect from AtmoWin
      if (settings.DisableLEDsOnExit)
      {
        AtmoLightObject.ChangeEffect(ContentEffect.LEDsDisabled);
      }
      else if (settings.EnableLiveviewOnExit)
      {
        AtmoLightObject.ChangeEffect(ContentEffect.AtmoWinLiveMode);
      }
      AtmoLightObject.Disconnect();

      if (settings.StopAtmoWinOnExit)
      {
        AtmoLightObject.StopAtmoWin();
      }

      // Unregister Log Handler
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
      if (settings.ManualMode)
      {
        Log.Debug("LEDs should be deactivated. (Manual Mode)");
        return false;
      }
      else if ((DateTime.Now >= Convert.ToDateTime(settings.ExcludeTimeStart) && DateTime.Now <= Convert.ToDateTime(settings.ExcludeTimeEnd)))
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
        AtmoLightObject.ChangeDelay((int)(((float)settings.DelayRefreshRate / (float)refreshRate) * (float)settings.DelayTime));
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

        AtmoLightObject.SetPixelData(bmiInfoHeader, pixelData);

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
    private void RegisterKeyBindings()
    {
      IInputManager manager = ServiceRegistration.Get<IInputManager>(false);
      if (manager != null)
      {
        if (settings.MenuButton == "Red")
        {
          manager.AddKeyBinding(Key.Red, new VoidKeyActionDlgt(ContextMenu));
        }
        else if (settings.MenuButton == "Green")
        {
          manager.AddKeyBinding(Key.Green, new VoidKeyActionDlgt(ContextMenu));
        }
        else if (settings.MenuButton == "Yellow")
        {
          manager.AddKeyBinding(Key.Yellow, new VoidKeyActionDlgt(ContextMenu));
        }
        else if (settings.MenuButton == "Blue")
        {
          manager.AddKeyBinding(Key.Blue, new VoidKeyActionDlgt(ContextMenu));
        }

        if (settings.OnOffButton == "Red")
        {
          manager.AddKeyBinding(Key.Red, new VoidKeyActionDlgt(ToggleEffectOnOff));
        }
        else if (settings.OnOffButton == "Green")
        {
          manager.AddKeyBinding(Key.Green, new VoidKeyActionDlgt(ToggleEffectOnOff));
        }
        else if (settings.OnOffButton == "Yellow")
        {
          manager.AddKeyBinding(Key.Yellow, new VoidKeyActionDlgt(ToggleEffectOnOff));
        }
        else if (settings.OnOffButton == "Blue")
        {
          manager.AddKeyBinding(Key.Blue, new VoidKeyActionDlgt(ToggleEffectOnOff));
        }

        if (settings.ProfileButton == "Red")
        {
          manager.AddKeyBinding(Key.Red, new VoidKeyActionDlgt(ChangeAtmoWinProfile));
        }
        else if (settings.ProfileButton == "Green")
        {
          manager.AddKeyBinding(Key.Green, new VoidKeyActionDlgt(ChangeAtmoWinProfile));
        }
        else if (settings.ProfileButton == "Yellow")
        {
          manager.AddKeyBinding(Key.Yellow, new VoidKeyActionDlgt(ChangeAtmoWinProfile));
        }
        else if (settings.ProfileButton == "Blue")
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
        AtmoLightObject.ReinitialiseThreaded();
      }
    }

    private void ChangeAtmoWinProfile()
    {
      if (AtmoLightObject.IsConnected())
      {
        AtmoLightObject.ChangeAtmoWinProfile();
      }
      else
      {
        AtmoLightObject.ReinitialiseThreaded();
      }
    }

    private void ContextMenu()
    {

    }
    #endregion

    #region Connection Lost Handler
    /// <summary>
    /// Connection lost event handler.
    /// This event gets called if connection to AtmoWin is lost and not recoverable.
    /// </summary>
    private void OnNewConnectionLost()
    {
      ServiceRegistration.Get<INotificationService>().EnqueueNotification(NotificationType.Error, "[AtmoLight.Name]", "[AtmoLight.AtmoWinConnectionLost]", true);
    }
    #endregion
  }
}
