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
using Language;

namespace MediaPortal.ProcessPlugins.Atmolight
{
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
    public static bool atmoOff = false;
    public Int64 tickCount = 0;
    public Int64 lastFrame = 0;
    private IAtmoRemoteControl2 atmoCtrl = null;
    private IAtmoLiveViewControl atmoLiveViewCtrl = null;
    private int captureWidth = 0;
    private int captureHeight = 0;
    private Surface rgbSurface = null;
    private ContentEffect currentEffect = ContentEffect.LEDsDisabled;
    private ContentEffect menuEffect = ContentEffect.LEDsDisabled;
    private int[] staticColor = { 0, 0, 0 };
    private int[] staticColorTemp = { 0, 0, 0 };
    private int staticColorHelper;
    private const int comInterfaceTimeout = 1000;
    private const int delaySetStaticColor = 20;
    private const int delayAtmoWinConnecting = 1000;
    private bool reInitializeLock = false;
    private bool getAtmoLiveViewSourceLock = true;
    private ComLiveViewSource atmoLiveViewSource;
    #endregion

    #region Initialize AtmoLight
    public AtmolightPlugin()
    {
      if (MPSettings.Instance.GetValueAsBool("plugins", "Atmolight", true))
      {
        Log.Debug("AtmoLight: Loading Settings.");
        AtmolightSettings.LoadSettings();
        InitializeAtmoWinConnection();
      }
    }

    private bool InitializeAtmoWinConnection()
    {
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
            return ConnectToAtmoWinA();
          }
          else
          {
            Log.Error("AtmoLight: AtmoWinA is not running.");
          }
        }
      }
      return ConnectToAtmoWinA();
    }

    private bool ConnectToAtmoWinA()
    {
      try
      {
        if (atmoCtrl != null)
        {
          Marshal.ReleaseComObject(atmoCtrl);
          atmoCtrl = null;
        }
        atmoCtrl = (IAtmoRemoteControl2)Marshal.GetActiveObject("AtmoRemoteControl.1");
      }
      catch (Exception ex)
      {
        Log.Error("AtmoLight: Failed to connect to AtmoWin.");
        Log.Error("AtmoLight: Exception: {0}", ex.Message);
        if (atmoCtrl != null)
        {
          Marshal.ReleaseComObject(atmoCtrl);
        }
        atmoCtrl = null;
        return false;
      }

      Log.Info("AtmoLight: Successfully connected to AtmoWin.");

      SetAtmoEffect(ComEffectMode.cemLivePicture);
      atmoLiveViewCtrl = (IAtmoLiveViewControl)Marshal.GetActiveObject("AtmoRemoteControl.1");
      SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal);
      GetAtmoLiveViewRes();

      DisableLEDs();

      return true;
    }
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

    private void KillAtmoWinA()
    {
      Log.Debug("AtmoLight: Stopping AtmoWinA.exe.");
      foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("atmowina")))
      {
        process.Kill();
        Win32API.RefreshTrayArea();
        Log.Debug("AtmoLight: AtmoWinA.exe stopped.");
      }
    }

    private bool ReInitializeAtmoWinConnection()
    {
      if (reInitializeLock)
      {
        return false;
      }

      if (!AtmolightSettings.restartOnError)
      {
        if (atmoCtrl != null)
        {
          Marshal.ReleaseComObject(atmoCtrl);
          atmoCtrl = null;
        }
        atmoOff = true;
        DialogError(LanguageLoader.appStrings.ContextMenu_AtmoWinConnectionLost);
        return false;
      }

      reInitializeLock = true;
      Log.Debug("AtmoLight: Trying to restart AtmoWin and reconnect to it.");
      if (atmoCtrl != null)
      {
        Marshal.ReleaseComObject(atmoCtrl);
        atmoCtrl = null;
      }
      KillAtmoWinA();
      if (!InitializeAtmoWinConnection())
      {
        atmoOff = true;
        Log.Error("AtmoLight: Reconnecting to AtmoWin failed.");
        DialogError(LanguageLoader.appStrings.ContextMenu_AtmoWinConnectionLost);
        reInitializeLock = false;
        return false;
      }
      else
      {
        StartLEDs();
        reInitializeLock = false;
        return true;
      }
    }

    public void Start()
    {
      if (atmoCtrl != null)
      {
        Log.Debug("AtmoLight: Plugin started.");

        g_Player.PlayBackStarted += new g_Player.StartedHandler(g_Player_PlayBackStarted);
        g_Player.PlayBackStopped += new g_Player.StoppedHandler(g_Player_PlayBackStopped);
        g_Player.PlayBackEnded += new g_Player.EndedHandler(g_Player_PlayBackEnded);

        FrameGrabber.GetInstance().OnNewFrame += new FrameGrabber.NewFrameHandler(AtmolightPlugin_OnNewFrame);

        // Workaround
        if (AtmolightSettings.effectMenu == ContentEffect.MediaPortalLiveMode)
        {
          AtmolightSettings.effectMenu = ContentEffect.StaticColor;
        }

        menuEffect = AtmolightSettings.effectMenu;

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

        GUIWindowManager.OnNewAction += new OnActionHandler(OnNewAction);
      }
    }

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
    private void TimeoutHandler(System.Action method)
    {
      var tokenSource = new CancellationTokenSource();
      CancellationToken token = tokenSource.Token;
      var task = Task.Factory.StartNew(() => method(), token);

      if (!task.Wait(comInterfaceTimeout, token))
      {
        StackTrace trace = new StackTrace();
        Log.Error("AtmoLight: {0} timed out!", trace.GetFrame(1).GetMethod().Name);

        // Try to reconnect to AtmoWin
        ReInitializeAtmoWinConnection();
      }

    }

    private void SetColorMode(ComEffectMode effect)
    {
      if (atmoCtrl == null)
      {
        return;
      }

      try
      {
        Log.Debug("AtmoLight: Changing AtmoWin profile.");
        ComEffectMode oldEffect;
        TimeoutHandler(() => atmoCtrl.setEffect(effect, out oldEffect));
      }
      catch (Exception ex)
      {
        Log.Error("AtmoLight: Failed to change AtmoWin profile.");
        Log.Error("AtmoLight: Exception: {0}", ex.Message);
        ReInitializeAtmoWinConnection();
        return;
      }
      Log.Info("AtmoLight: Successfully changed AtmoWin profile.");
    }

    private void SetAtmoEffect(ComEffectMode effect)
    {
      if (atmoCtrl == null)
      {
        return;
      }

      try
      {
        Log.Debug("AtmoLight: Changing AtmoWin effect to: {0}", effect.ToString());
        ComEffectMode oldEffect;
        TimeoutHandler(() => atmoCtrl.setEffect(effect, out oldEffect));
      }
      catch (Exception ex)
      {
        Log.Error("AtmoLight: Failed changing effect to: {0}", effect.ToString());
        Log.Error("AtmoLight: Exception: {0}", ex.Message);
        ReInitializeAtmoWinConnection();
        return;
      }
      Log.Info("AtmoLight: Successfully changed AtmoWin effect to: {0}", effect.ToString());
    }

    private void SetAtmoColor(byte red, byte green, byte blue)
    {
      if (atmoCtrl == null)
      {
        return;
      }

      try
      {
        Log.Debug("AtmoLight: Setting static color to R:{0} G:{1} B:{2}.", red, green, blue);
        TimeoutHandler(() => atmoCtrl.setStaticColor(red, green, blue));
      }
      catch (Exception ex)
      {
        Log.Error("AtmoLight: Failed setting static color to R:{0} G:{1} B:{2}.", red, green, blue);
        Log.Error("AtmoLight: Exception: {0}", ex.Message);
        ReInitializeAtmoWinConnection();
        return;
      }
      Log.Info("AtmoLight: Successfully set static color to R:{0} G:{1} B:{2}.", red, green, blue);
    }

    private void SetAtmoLiveViewSource(ComLiveViewSource viewSource)
    {
      if (atmoCtrl == null)
      {
        return;
      }

      try
      {
        Log.Debug("AtmoLight: Changing AtmoWin Liveview Source to: {0}", viewSource.ToString());
        TimeoutHandler(() => atmoLiveViewCtrl.setLiveViewSource(viewSource));
      }
      catch (Exception ex)
      {
        Log.Error("AtmoLight: Failed changing AtmoWin Liveview Source to: {0}", viewSource.ToString());
        Log.Error("AtmoLight: Exception: {0}", ex.Message);
        ReInitializeAtmoWinConnection();
        return;
      }
      Log.Info("AtmoLight: Successfully changed AtmoWin Liveview Source to: {0}", viewSource.ToString());
    }

    private void GetAtmoLiveViewSource()
    {
      if (atmoCtrl == null)
      {
        return;
      }

      try
      {
        TimeoutHandler(() => atmoLiveViewCtrl.getCurrentLiveViewSource(out atmoLiveViewSource));
      }
      catch (Exception ex)
      {
        Log.Error("AtmoLight: Error in GetAtmoLiveViewSource.");
        Log.Error("AtmoLight: Exception: {0}", ex.Message);
        ReInitializeAtmoWinConnection();
        return;
      }
    }

    private void GetAtmoLiveViewRes()
    {
      if (atmoCtrl == null)
      {
        return;
      }

      try
      {
        Log.Debug("AtmoLight: Getting Liveview Resolution.");
        TimeoutHandler(() => atmoCtrl.getLiveViewRes(out captureWidth, out captureHeight));
      }
      catch (Exception ex)
      {
        Log.Error("AtmoLight: Failed to get Liveview Resolution.");
        Log.Error("AtmoLight: Exception: {0}", ex.Message);
        ReInitializeAtmoWinConnection();
        return;
      }
      Log.Debug("AtmoLight: Liveview capture resolution is {0}x{1}. Screenshot will be resized to this dimensions.", captureWidth, captureHeight);
    }
    #endregion

    #region Control LEDs
    private void DisableLEDs()
    {
      atmoOff = true;
      try
      {
        Log.Debug("AtmoLight: Disabling LEDs.");
        SetAtmoLiveViewSource(ComLiveViewSource.lvsGDI);
        SetAtmoEffect(ComEffectMode.cemDisabled);
        // Workaround for SEDU
        System.Threading.Thread.Sleep(delaySetStaticColor);
        SetAtmoColor(0, 0, 0);
      }
      catch (Exception ex)
      {
        Log.Error("AtmoLight: Failed to disable LEDs.");
        Log.Error("AtmoLight: Exception: {0}", ex.Message);
        return;
      }
      Log.Info("AtmoLight: Successfully disabled LEDs.");
    }

    private void MenuMode()
    {
      Log.Info("AtmoLight: Changing AtmoLight effect to: {0}", menuEffect.ToString());
      switch (menuEffect)
      {
        case ContentEffect.AtmoWinLiveMode:
          atmoOff = false;
          SetAtmoEffect(ComEffectMode.cemLivePicture);
          SetAtmoLiveViewSource(ComLiveViewSource.lvsGDI);
          break;
        case ContentEffect.Colorchanger:
          atmoOff = false;
          SetAtmoEffect(ComEffectMode.cemLivePicture);
          SetAtmoLiveViewSource(ComLiveViewSource.lvsGDI);
          SetAtmoEffect(ComEffectMode.cemColorChange);
          break;
        case ContentEffect.ColorchangerLR:
          atmoOff = false;
          SetAtmoEffect(ComEffectMode.cemLivePicture);
          SetAtmoLiveViewSource(ComLiveViewSource.lvsGDI);
          SetAtmoEffect(ComEffectMode.cemLrColorChange);
          break;
        case ContentEffect.LEDsDisabled:
          DisableLEDs();
          break;
        // Effect can be called "MP_Live_view" but it actually is "Static Color".
        // This should not happen anymore, but the case for it stays in for now.
        case ContentEffect.MediaPortalLiveMode:
        case ContentEffect.StaticColor:
          atmoOff = false;
          SetAtmoLiveViewSource(ComLiveViewSource.lvsGDI);
          SetAtmoEffect(ComEffectMode.cemDisabled);
          SetAtmoColor((byte)staticColor[0], (byte)staticColor[1], (byte)staticColor[2]);
          // Workaround for SEDU
          System.Threading.Thread.Sleep(delaySetStaticColor);
          SetAtmoColor((byte)staticColor[0], (byte)staticColor[1], (byte)staticColor[2]);
          break;
      }
    }

    private void PlaybackMode()
    {

      Log.Info("AtmoLight: Changing AtmoLight effect to: {0}", currentEffect.ToString());
      switch (currentEffect)
      {
        case ContentEffect.AtmoWinLiveMode:
          atmoOff = false;
          SetAtmoEffect(ComEffectMode.cemLivePicture);
          SetAtmoLiveViewSource(ComLiveViewSource.lvsGDI);
          break;
        case ContentEffect.Colorchanger:
          atmoOff = false;
          SetAtmoEffect(ComEffectMode.cemLivePicture);
          SetAtmoLiveViewSource(ComLiveViewSource.lvsGDI);
          SetAtmoEffect(ComEffectMode.cemColorChange);
          break;
        case ContentEffect.ColorchangerLR:
          atmoOff = false;
          SetAtmoEffect(ComEffectMode.cemLivePicture);
          SetAtmoLiveViewSource(ComLiveViewSource.lvsGDI);
          SetAtmoEffect(ComEffectMode.cemLrColorChange);
          break;
        case ContentEffect.LEDsDisabled:
          DisableLEDs();
          break;
        case ContentEffect.MediaPortalLiveMode:
          atmoOff = false;
          SetAtmoEffect(ComEffectMode.cemLivePicture);
          SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal);
          break;
        case ContentEffect.StaticColor:
          atmoOff = false;
          SetAtmoLiveViewSource(ComLiveViewSource.lvsGDI);
          SetAtmoEffect(ComEffectMode.cemDisabled);
          SetAtmoColor((byte)staticColor[0], (byte)staticColor[1], (byte)staticColor[2]);
          // Workaround for SEDU
          System.Threading.Thread.Sleep(delaySetStaticColor);
          SetAtmoColor((byte)staticColor[0], (byte)staticColor[1], (byte)staticColor[2]);
          break;
      }
    }

    private void StartLEDs()
    {
      if (g_Player.Playing)
      {
        PlaybackMode();
      }
      else
      {
        MenuMode();
      }
    }

    private bool CheckForStartRequirements()
    {
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

    private void DoDelay(byte[] bmiInfoHeader, byte[] pixelData)
    {
      try
      {
        System.Threading.Thread.Sleep(AtmolightSettings.delayTime);
        if (reInitializeLock || atmoCtrl == null)
        {
          return;
        }
        atmoLiveViewCtrl.setPixelData(bmiInfoHeader, pixelData);
      }
      catch (Exception ex)
      {
        Log.Error("AtmoLight: Could not send data to AtmoWin (Delayed).");
        Log.Error("AtmoLight: Exception: {0}", ex.Message);

        // Try to reconnect to AtmoWin.
        ReInitializeAtmoWinConnection();
      }
    }
    #endregion

    #region Context Menu
    private string GetKeyboardString(string KeyboardString)
    {
      VirtualKeyboard Keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
      if (Keyboard == null)
      {
        return null;
      }
      Keyboard.IsSearchKeyboard = true;
      Keyboard.Reset();
      Keyboard.Text = KeyboardString;
      Keyboard.DoModal(GUIWindowManager.ActiveWindow);
      if (Keyboard.IsConfirmed)
      {
        return Keyboard.Text;
      }
      return null;
    }

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
      DialogRGBManualStaticColorChanger(false, dlgRGB.SelectedLabel);
    }
    #endregion

    #region Remote Button Events
    public void OnNewAction(MediaPortal.GUI.Library.Action action)
    {
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
      // Remote Key to open Menu
      else if ((action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_YELLOW_BUTTON && AtmolightSettings.menuButton == 2) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_GREEN_BUTTON && AtmolightSettings.menuButton == 1) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_RED_BUTTON && AtmolightSettings.menuButton == 0) ||
          (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_BLUE_BUTTON && AtmolightSettings.menuButton == 3))
      {
        Log.Info("AtmoLight: Opening AtmoLight context menu.");
        GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);

        dlg.Reset();
        dlg.SetHeading("AtmoLight");
        if (atmoOff)
        {
          dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_SwitchLEDsON));
        }
        else
        {
          dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_SwitchLEDsOFF));
        }

        dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_ChangeEffect));
        dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_ChangeAWProfile));

        if (AtmolightSettings.sbs3dOn)
        {
          dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Switch3DOFF));
        }
        else
        {
          dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Switch3DON));
        }
        if (((g_Player.Playing) && (currentEffect == ContentEffect.StaticColor) && (!atmoOff)) ||
            ((!g_Player.Playing) && (menuEffect == ContentEffect.StaticColor) && (!atmoOff)))
        {
          dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_ChangeStatic));
        }

        dlg.SelectedLabel = 0;
        dlg.DoModal(GUIWindowManager.ActiveWindow);

        switch (dlg.SelectedLabel)
        {
          case 0:
            if (atmoOff)
            {
              StartLEDs();
            }
            else
            {
              DisableLEDs();
            }
            break;
          case 1:
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
                  currentEffect = ContentEffect.LEDsDisabled;
                  DisableLEDs();
                  break;
                case 1:
                  currentEffect = ContentEffect.MediaPortalLiveMode;
                  PlaybackMode();
                  break;
                case 2:
                  currentEffect = ContentEffect.AtmoWinLiveMode;
                  PlaybackMode();
                  break;
                case 3:
                  currentEffect = ContentEffect.Colorchanger;
                  PlaybackMode();
                  break;
                case 4:
                  currentEffect = ContentEffect.ColorchangerLR;
                  PlaybackMode();
                  break;
                case 5:
                  currentEffect = ContentEffect.StaticColor;
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
            break;
          case 2:
            SetColorMode(ComEffectMode.cemColorMode);
            break;
          case 3:
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
            break;
          case 4:
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
            break;
        }
      }
    }
    #endregion

    #region g_player Events
    void g_Player_PlayBackEnded(g_Player.MediaType type, string filename)
    {
      try
      {
        getAtmoLiveViewSourceLock = true;
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

    void g_Player_PlayBackStopped(g_Player.MediaType type, int stoptime, string filename)
    {
      try
      {
        getAtmoLiveViewSourceLock = true;
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
          currentEffect = AtmolightSettings.effectVideo;
        }
        else if (type == g_Player.MediaType.Music)
        {
          // Workaround
          if (AtmolightSettings.effectMusic == ContentEffect.MediaPortalLiveMode)
          {
            AtmolightSettings.effectMusic = ContentEffect.StaticColor;
          }
          currentEffect = AtmolightSettings.effectMusic;
          Log.Debug("AtmoLight: Music detected.");
        }
        else if (type == g_Player.MediaType.Radio)
        {
          // Workaround
          if (AtmolightSettings.effectRadio == ContentEffect.MediaPortalLiveMode)
          {
            AtmolightSettings.effectRadio = ContentEffect.StaticColor;
          }
          currentEffect = AtmolightSettings.effectRadio;
          Log.Debug("AtmoLight: Radio detected.");
        }

        if (CheckForStartRequirements())
        {
          PlaybackMode();
          if (AtmolightSettings.delay)
          {
            Log.Debug("AtmoLight: Adding {0}ms delay to the LEDs.", AtmolightSettings.delayTime.ToString());
          }
        }
        else
        {
          DisableLEDs();
        }
        getAtmoLiveViewSourceLock = false;
      }
      catch (Exception ex)
      {
        Log.Error("AtmoLight: g_Player_PlayBackStarted failed.");
        Log.Error("AtmoLight: Exception= {0}", ex.Message);
      }
    }

    private void AtmolightPlugin_OnNewFrame(short width, short height, short arWidth, short arHeight, uint pSurface)
    {
      if (currentEffect != ContentEffect.MediaPortalLiveMode || atmoOff || atmoCtrl == null || width == 0 || height == 0)
      {
        return;
      }
      if (!getAtmoLiveViewSourceLock)
      {
        GetAtmoLiveViewSource();
        if (atmoLiveViewSource != ComLiveViewSource.lvsExternal)
        {
          Log.Error("AtmoLight: AtmoWin Liveview Source is not lvsExternal");
          SetAtmoLiveViewSource(ComLiveViewSource.lvsExternal);
        }
      }

      if (AtmolightSettings.lowCPU)
      {
        tickCount = Win32API.GetTickCount();
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
            if (!AtmolightSettings.delay)
            {
              atmoLiveViewCtrl.setPixelData(bmiInfoHeader, pixelData);
            }
            else
            {
              Thread DelayHelperThread = new Thread(() => DoDelay(bmiInfoHeader, pixelData));
              DelayHelperThread.Start();
            }
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

    #region ISetupForm impementation
    public string Author()
    {
      return "gemx";
    }

    public bool CanEnable()
    {
      return true;
    }

    public bool DefaultEnabled()
    {
      return true;
    }

    public string Description()
    {
      return "Interfaces AtmowinA.exe via COM to control the lights";
    }

    public bool GetHome(out string strButtonText, out string strButtonImage, out string strButtonImageFocus, out string strPictureImage)
    {
      strButtonText = null;
      strButtonImage = null;
      strButtonImageFocus = null;
      strPictureImage = null;
      return false;
    }

    public int GetWindowId()
    {
      return -1;
    }

    public bool HasSetup()
    {
      return true;
    }

    public string PluginName()
    {
      return "AtmoLight";
    }

    public void ShowPlugin()
    {
      new AtmolightSetupForm().ShowDialog();
    }
    #endregion

    #region IShowPlugin Member

    public bool ShowDefaultHome()
    {
      return false;
    }

    #endregion
  }
}
