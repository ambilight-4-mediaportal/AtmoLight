﻿using System;
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
using Microsoft.Win32;

namespace AtmoLight
{
  class AtmoWinHandler : ITargets
  {
    #region Fields
    public Target Name { get { return Target.AtmoWin; } }
    public TargetType Type { get { return TargetType.Local; } }
    public bool AllowDelay { get { return true; } }
    public List<ContentEffect> SupportedEffects
    {
      get
      {
        return new List<ContentEffect> {  ContentEffect.ExternalLiveMode,
                                          ContentEffect.AtmoWinColorchanger,
                                          ContentEffect.AtmoWinColorchangerLR,
                                          ContentEffect.GIFReader,
                                          ContentEffect.LEDsDisabled,
                                          ContentEffect.MediaPortalLiveMode,
                                          ContentEffect.StaticColor,
                                          ContentEffect.VUMeter,
                                          ContentEffect.VUMeterRainbow
        };
      }
    }

    // Threads
    private Thread reinitialiseThreadHelper;
    private Thread initialiseThreadHelper;
    private Thread getAtmoLiveViewSourceThreadHelper;

    // Locks
    private volatile bool reInitialiseLock = false;
    private volatile bool initialiseLock = false;
    private volatile bool getAtmoLiveViewSourceLock = false;

    // Com  Objects
    private IAtmoRemoteControl atmoRemoteControl = null; // Com Object to control AtmoWin
    private IAtmoLiveViewControl atmoLiveViewControl = null; // Com Object to control AtmoWins liveview
    private ComLiveViewSource atmoLiveViewSource; // Current liveview source

    // Timings
    private const int timeoutComInterface = 5000; // Timeout for the COM interface
    private const int delaySetStaticColor = 20; // SEDU workaround delay time
    private const int delayGetAtmoLiveViewSource = 1000; // Delay between liveview source checks

    // Core Object
    private Core coreObject;

    // Other Fields
    private int captureWidth;
    private int captureHeight;
    private bool staticColorSentDisabledEffect = false;
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
      if (atmoRemoteControl == null || atmoLiveViewControl == null || initialiseLock || reInitialiseLock)
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

      if (effect != ContentEffect.StaticColor)
      {
        staticColorSentDisabledEffect = false;
      }

      StopGetAtmoLiveViewSourceThread();
      switch (effect)
      {
        case ContentEffect.ExternalLiveMode:
          if (!SetAtmoEffect(ComEffectMode.cemLivePicture)) return false;
          if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsGDI)) return false;
          break;
        case ContentEffect.AtmoWinColorchanger:
          if (!SetAtmoEffect(ComEffectMode.cemColorChange)) return false;
          break;
        case ContentEffect.AtmoWinColorchangerLR:
          if (!SetAtmoEffect(ComEffectMode.cemLrColorChange)) return false;
          break;
        case ContentEffect.MediaPortalLiveMode:
          if (!SetAtmoEffect(ComEffectMode.cemLivePicture)) return false;
          if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal)) return false;

          StartGetAtmoLiveViewSourceThread();
          break;
        case ContentEffect.StaticColor:
          if (!staticColorSentDisabledEffect)
          {
            if (!SetAtmoEffect(ComEffectMode.cemDisabled)) return false;
            staticColorSentDisabledEffect = true;
          }
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
        case ContentEffect.LEDsDisabled:
        case ContentEffect.Undefined:
        default:
          if (!SetAtmoEffect(ComEffectMode.cemDisabled)) return false;
          // Workaround for SEDU.
          // Without the sleep it would not change to color.
          System.Threading.Thread.Sleep(delaySetStaticColor);
          if (!SetAtmoColor(0, 0, 0)) return false;
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
    public void ChangeProfile(string profileName)
    {
      if (!IsConnected())
      {
        return;
      }
      if (!SetColorMode(ComEffectMode.cemColorMode)) return;

      // Change the effect to the desired effect.
      // Needed for AtmoWin 1.0.0.5+
      if (!ChangeEffect(coreObject.GetCurrentEffect())) return;
    }

    public void 
      PowerModeChanged(PowerModes powerMode)
    {
      if (powerMode == PowerModes.Resume)
      {
        //AtmoWakeHelper
        if (coreObject.atmoWakeHelperEnabled)
        {
          // Set lock to prevent it from reinitializing during COM reconnection
          reInitialiseLock = true;
          Log.Debug("AtmoWinHandler - [AtmoWakeHelper] locking reinit during reconnection of COM port");
          Disconnect();
          StopAtmoWin();

          // Reconnect COM ports and setting user configured delays
          AtmoLight.Targets.AtmoWin.AtmoWakeHelper.disconnectDelay = coreObject.atmoWakeHelperDisconnectDelay;
          AtmoLight.Targets.AtmoWin.AtmoWakeHelper.connectDelay = coreObject.atmoWakeHelperConnectDelay;

          Log.Debug("AtmoWinHandler - [AtmoWakeHelper] reconnecting COM port");
          AtmoLight.Targets.AtmoWin.AtmoWakeHelper.reconnectCOM(coreObject.atmoWakeHelperComPort, coreObject.atmoWakeHelperResumeDelay);
          Log.Debug("AtmoWinHandler - [AtmoWakeHelper] reconnected COM ports and releasing reinit lock");
          Thread.Sleep(AtmoLight.Targets.AtmoWin.AtmoWakeHelper.connectDelay = coreObject.atmoWakeHelperReinitializationDelay);
          reInitialiseLock = false;
          Log.Debug("AtmoWinHandler - [AtmoWakeHelper] started reinit");
          ReInitialise(true);
        }
        else
        {
          ChangeEffect(coreObject.GetCurrentEffect());
        }

      }
      else if (powerMode == PowerModes.Suspend)
      {
        StopGetAtmoLiveViewSourceThread();
        ChangeEffect(ContentEffect.LEDsDisabled);
      }
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

      Log.Info("AtmoWinHandler - Initialising successfull.");
      initialiseLock = false;

      ChangeEffect(coreObject.GetCurrentEffect());

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

      if (!Disconnect() || !StopAtmoWin() ||  !InitialiseThreaded(force))
      {
        Disconnect();
        StopAtmoWin();
        Log.Error("AtmoWinHandler - Reinitialising failed.");
        reInitialiseLock = false;
        coreObject.NewConnectionLost(Name);
        return;
      }

      Log.Info("AtmoWinHandler - Reinitialising successfull.");
      reInitialiseLock = false;

      ChangeEffect(coreObject.GetCurrentEffect());
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
      if (!SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal, true)) return false;
      if (!GetAtmoLiveViewRes()) return false;

      Log.Info("AtmoWinHandler - Successfully connected to AtmoWin.");
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

      // Sleep timer to avoid Windows being to quick upon COM port unlocking
      Thread.Sleep(1500);

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
        AtmoWinA.WaitForInputIdle();
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
      Log.Debug("AtmoWinHandler - Trying to stop AtmoWin.");
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
      if (TimeoutHandler(() => atmoRemoteControl = (IAtmoRemoteControl)Marshal.GetActiveObject("AtmoRemoteControl.1")))
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
    private bool SetAtmoEffect(ComEffectMode effect, bool force = false)
    {
      if (!IsConnected() && !force)
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
    private bool SetAtmoLiveViewSource(ComLiveViewSource viewSource, bool force = false)
    {
      if (!IsConnected() && !force)
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
      if (atmoRemoteControl == null)
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
      getAtmoLiveViewSourceThreadHelper.IsBackground = true;
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
  }
}
