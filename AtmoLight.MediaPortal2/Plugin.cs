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
using MediaPortal.Common.General;
using MediaPortal.Common.Localization;
using MediaPortal.Common.PathManager;
using MediaPortal.Common.Services.Settings;
using MediaPortal.Common.Settings;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Models;
using MediaPortal.UI.Presentation.Screens;
using MediaPortal.UI.Presentation.Players;
using MediaPortal.Common.Logging;
using MediaPortal.Common.PluginManager;
using MediaPortal.UI.SkinEngine;
using MediaPortal.UI.SkinEngine.Players;
using MediaPortal.UI.SkinEngine.SkinManagement;

using MediaPortal.Common.Messaging;
using MediaPortal.Common.Runtime;

// SharpDX
using SharpDX;
using SharpDX.Direct3D9;

// Key binding
using MediaPortal.UI.Control.InputManager;
using MediaPortal.UI.Presentation.Actions;


namespace AtmoLight
{
  public class Plugin : IPluginStateTracker
  {
    #region Fields
    protected AsynchronousMessageQueue messageQueue;
    public Core AtmoLightObject;

    // Settings
    private string atmoWinPath = "C:\\ProgramData\\Team MediaPortal\\MediaPortal\\AtmoWin\\AtmoWinA.exe";
    private bool restartOnError = true;
    private bool startAtmoWin = true;
    private int[] staticColorTemp = { 0, 0, 0 };
    private bool delay = false;
    private int delayReferenceTime = 0;

    // Surfaces
    private SharpDX.Direct3D9.Surface surfaceSkinEngine; // Surface of the whole UI
    private SharpDX.Direct3D9.Surface surfacePlayer; // Surface from the Player
    private SharpDX.Direct3D9.Surface surfaceDestination; // Destination Surface (resized)

    // Player helper
    private IPlayerManager playerManager;
    private ISharpDXVideoPlayer player;

    // Depracted
    //private SharpDX.Direct3D9.Device sharpDXDevice;
    //private int surfacePollingRate = 60;

    #endregion

    #region IPluginStateTracker implementation
    public void Activated(PluginRuntime pluginRuntime)
    {
      // Log Handler
      Log.OnNewLog += new Log.NewLogHandler(OnNewLog);

      var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
      DateTime buildDate = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTime;
      Log.Info("AtmoLight: Version {0}.{1}.{2}.{3}, build on {4} at {5}.", version.Major, version.Minor, version.Build, version.Revision, buildDate.ToShortDateString(), buildDate.ToLongTimeString());

      Log.Debug("AtmoLight: Initialising event handler.");

      messageQueue = new AsynchronousMessageQueue(this, new string[] { PlayerManagerMessaging.CHANNEL });
      messageQueue.MessageReceived += OnMessageReceived;
      messageQueue.Start();

      RegisterKeyBindings();

      Log.Debug("Generating new AtmoLight.Core instance.");
      AtmoLightObject = new Core(atmoWinPath, restartOnError, startAtmoWin, staticColorTemp, delay, delayReferenceTime);

      if (!AtmoLightObject.Initialise())
      {
        Log.Error("Initialising failed.");
        return;
      }

      AtmoLightObject.ChangeEffect(ContentEffect.MediaPortalLiveMode);

      // Handler for UI capture
      SkinContext.DeviceSceneEnd += UICapture;
    }

    public void Stop()
    {
      return;
    }

    public void Shutdown()
    {
      UnregisterKeyBindings();
      AtmoLightObject.ChangeEffect(ContentEffect.LEDsDisabled);
      AtmoLightObject.Disconnect();
      AtmoLightObject.StopAtmoWin();
      SkinContext.DeviceSceneEnd -= UICapture;
      surfaceDestination.Dispose();
      surfaceDestination = null;
      messageQueue.MessageReceived -= OnMessageReceived;
      Log.OnNewLog -= new Log.NewLogHandler(OnNewLog);
      return;
    }

    public void Continue()
    {
      return;
    }

    public bool RequestEnd()
    {
      Shutdown();
      return true;
    }

    #endregion

    #region Message Handler
    void OnMessageReceived(AsynchronousMessageQueue queue, SystemMessage message)
    {
      if (message.ChannelName == PlayerManagerMessaging.CHANNEL)
      {
        PlayerManagerMessaging.MessageType messageType = (PlayerManagerMessaging.MessageType)message.MessageType;
        if (messageType == PlayerManagerMessaging.MessageType.PlayerStarted)
        {
          Log.Info("AtmoLight: Playback started.");
          if (ServiceRegistration.Get<IPlayerContextManager>().IsVideoContextActive)
          {
            Log.Info("AtmoLight: Video started.");
            //AtmoLightObject.ChangeEffect(ContentEffect.MediaPortalLiveMode);
            //Thread SurfacePollingHelper = new Thread(() => SurfacePolling());
            //SurfacePollingHelper.Start();
          }
        }
        else if (messageType == PlayerManagerMessaging.MessageType.PlayerStopped)
        {
          Log.Info("AtmoLight: Playback stopped.");
          //AtmoLightObject.ChangeEffect(ContentEffect.LEDsDisabled);
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
            surfacePlayer = player.Surface;
          });
          surfacePlayer.Device.StretchRectangle(surfacePlayer, null, surfaceDestination, rectangleDestination, SharpDX.Direct3D9.TextureFilter.None);
        }
        else
        {
          surfaceSkinEngine = SkinContext.Device.GetRenderTarget(0);
          surfaceSkinEngine.Device.StretchRectangle(surfaceSkinEngine, null, surfaceDestination, rectangleDestination, SharpDX.Direct3D9.TextureFilter.None);
        }


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
        manager.AddKeyBinding(Key.F3, new VoidKeyActionDlgt(ToggleEffect));
      }
    }

    private void UnregisterKeyBindings()
    {
      IInputManager manager = ServiceRegistration.Get<IInputManager>(false);
      if (manager != null)
      {
        manager.RemoveKeyBinding(Key.F3);
      }
    }

    private void ToggleEffect()
    {
      if (AtmoLightObject.IsAtmoLightOn())
      {
        AtmoLightObject.ChangeEffect(ContentEffect.LEDsDisabled);
      }
      else
      {
        AtmoLightObject.ChangeEffect(ContentEffect.MediaPortalLiveMode);
      }
    }
    #endregion

    #region Old depracted code
    /*
    // Not needed anymore?!
    private void SurfacePolling()
    {
      Rectangle rect = new Rectangle(0, 0, AtmoLightObject.captureWidth, AtmoLightObject.captureHeight);

      playerManager = ServiceRegistration.Get<IPlayerManager>();
      playerManager.ForEach(psc =>
      {
        player = psc.CurrentPlayer as ISharpDXVideoPlayer;
        if (player == null || player.Surface == null)
        {
          return;
        }
        surfaceSkinEngine = player.Surface;
      });

      sharpDXDevice = surfaceSkinEngine.Device;

      while (ServiceRegistration.Get<IPlayerContextManager>().IsVideoContextActive)
      {
        if (surfaceDestination == null)
        {
          surfaceDestination = SharpDX.Direct3D9.Surface.CreateRenderTarget(sharpDXDevice, AtmoLightObject.captureWidth, AtmoLightObject.captureHeight, SharpDX.Direct3D9.Format.A8R8G8B8, SharpDX.Direct3D9.MultisampleType.None, 0, true);
        }
        try
        {
          Stopwatch stopwatch = new Stopwatch();
          stopwatch.Start();
          sharpDXDevice.StretchRectangle(surfaceSkinEngine, null, surfaceDestination, rect, SharpDX.Direct3D9.TextureFilter.None);
          DataStream stream = SharpDX.Direct3D9.Surface.ToStream(surfaceDestination, SharpDX.Direct3D9.ImageFileFormat.Bmp);
          stopwatch.Stop();
          Log.Error("{0}", stopwatch.ElapsedMilliseconds);

          BinaryReader reader = new BinaryReader(stream);
          stream.Position = 0; // ensure that what start at the beginning of the stream. 
          reader.ReadBytes(14); // skip bitmap file info header
          byte[] bmiInfoHeader = reader.ReadBytes(4 + 4 + 4 + 2 + 2 + 4 + 4 + 4 + 4 + 4 + 4);

          int rgbL = (int)(stream.Length - stream.Position);
          int rgb = (int)(rgbL / (AtmoLightObject.captureWidth * AtmoLightObject.captureHeight));

          byte[] pixelData = reader.ReadBytes((int)(stream.Length - stream.Position));

          byte[] h1pixelData = new byte[AtmoLightObject.captureWidth * rgb];
          byte[] h2pixelData = new byte[AtmoLightObject.captureWidth * rgb];
          //now flip horizontally, we do it always to prevent microstudder
          int i;
          for (i = 0; i < ((AtmoLightObject.captureHeight / 2) - 1); i++)
          {
            Array.Copy(pixelData, i * AtmoLightObject.captureWidth * rgb, h1pixelData, 0, AtmoLightObject.captureWidth * rgb);
            Array.Copy(pixelData, (AtmoLightObject.captureHeight - i - 1) * AtmoLightObject.captureWidth * rgb, h2pixelData, 0, AtmoLightObject.captureWidth * rgb);
            Array.Copy(h1pixelData, 0, pixelData, (AtmoLightObject.captureHeight - i - 1) * AtmoLightObject.captureWidth * rgb, AtmoLightObject.captureWidth * rgb);
            Array.Copy(h2pixelData, 0, pixelData, i * AtmoLightObject.captureWidth * rgb, AtmoLightObject.captureWidth * rgb);
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
        System.Threading.Thread.Sleep(1000 / surfacePollingRate);
      }
      surfaceDestination.Dispose();
      surfaceDestination = null;
    }
    */
    #endregion
  }
}
