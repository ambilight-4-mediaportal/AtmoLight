using System;
using System.Collections.Generic;
using System.Text;
using MediaPortal.GUI.Library;
using MediaPortal.Profile;
using MediaPortal.Player;
using System.Diagnostics;
using System.Threading;
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

    #region Variables
    public static bool Atmo_off = false;
    public Int64 tickCount = 0;
    public Int64 lastFrame = 0;
    private IAtmoRemoteControl2 atmoCtrl = null;
    private IAtmoLiveViewControl atmoLiveViewCtrl = null;
    private int captureWidth = 0;
    private int captureHeight = 0;
    private Surface rgbSurface = null;
    private ContentEffect currentEffect = ContentEffect.LEDs_disabled;
    private ContentEffect MenuEffect = ContentEffect.LEDs_disabled;
    private int[] StaticColor = { 0, 0, 0 };
    private int[] StaticColorTemp = { 0, 0, 0 };
    private int StaticColorHelper;
    #endregion

    [DllImport("AtmoDXUtil.dll", PreserveSig = false, CharSet = CharSet.Auto)]
    private static extern void VideoSurfaceToRGBSurfaceExt(IntPtr src, int srcWidth, int srcHeight, IntPtr dst, int dstWidth, int dstHeight);

    public AtmolightPlugin()
    {
      if (MPSettings.Instance.GetValueAsBool("plugins", "Atmolight", true))
      {
        AtmolightSettings.LoadSettings();
        if (!Win32API.IsProcessRunning("configuration.exe"))
        {
          if (AtmolightSettings.startAtmoWin)
          {
            Log.Info("atmolight: Checking for atmowina.exe process");
            if (!Win32API.IsProcessRunning("atmowina.exe"))
            {
              Log.Info("atmolight: AtmoWinA.exe not started. Trying to launch...");
              if (!StartAtmoWinA())
              {
                Log.Error("atmolight: Can't start atmowin.exe. Atmolight control won't work :(");
                return;
              }
              else
              {
                if (!ConnectToAtmoWinA())
                {
                  Log.Error("atmolight: AtmoWinA started but still can't connect. Atmolight control won't work :(");
                  return;
                }
              }
            }
            else
            {
              if (!ConnectToAtmoWinA())
              {
                Log.Error("atmolight: AtmoWinA started but still can't connect. Atmolight control won't work :(");
                return;
              }
            }
          }
          else
          {
            Log.Info("atmolight: Checking for atmowina.exe process");
            if (Win32API.IsProcessRunning("atmowina.exe"))
            {
              if (!ConnectToAtmoWinA())
              {
                Log.Error("atmolight: AtmoWinA started but still can't connect. Atmolight control won't work :(");
                return;
              }
            }
          }
        }
      }
    }

    #region Utilities
    private bool ConnectToAtmoWinA()
    {
        // Testcode
        // Waiting 1 second for atmowin to start properly.
        System.Threading.Thread.Sleep(1000);
      try
      {
        atmoCtrl = (IAtmoRemoteControl2)Marshal.GetActiveObject("AtmoRemoteControl.1");
      }
      catch (Exception ex)
      {
        Log.Error("atmolight: exception= {0}", ex.Message);
        atmoCtrl = null;
        return false;
      }
      ComEffectMode oldEffect;
      
      atmoCtrl.setEffect(ComEffectMode.cemLivePicture, out oldEffect);
      atmoLiveViewCtrl = (IAtmoLiveViewControl)Marshal.GetActiveObject("AtmoRemoteControl.1");
      atmoLiveViewCtrl.setLiveViewSource(ComLiveViewSource.lvsExternal);
      atmoCtrl.getLiveViewRes(out captureWidth, out captureHeight);
      
      DisableLEDs();
      
      Log.Info("atmolight: successfully connected to AtmoWinA.exe :)");
      Log.Info("atmolight: live view capture resolution is {0}x{1}. Screenshot will be resized to this dimensions.", captureWidth, captureHeight);

      return true;
    }
    private bool StartAtmoWinA()
    {
      if (!System.IO.File.Exists(AtmolightSettings.atmowinExe))
      {
        Log.Error("atmolight: atmowina.exe not found.");
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
        return false;
      }
      System.Threading.Thread.Sleep(1000);
      return ret;
    }
    
    private void SetColorMode(ComEffectMode effect)
    {
      if (atmoCtrl == null)
        return;
      try
      {
        ComEffectMode oldEffect;
        atmoCtrl.setEffect(effect, out oldEffect);
        Log.Info("atmolight: Switching Profile ");
      }
      catch (Exception ex)
      {
        Log.Error("atmolight: Failed to switch profile " + Environment.NewLine + ex.Message + Environment.StackTrace);
      }
    }
    
    private void SetAtmoEffect(ComEffectMode effect)
    {
      if (atmoCtrl == null)
        return;
      try
      {
        ComEffectMode oldEffect;
        atmoCtrl.setEffect(effect, out oldEffect);
        Log.Info("atmolight: Set atmoeffect to " + effect.ToString());
      }
      catch (Exception ex)
      {
        Log.Error("atmolight: Failed to switch effect to " + effect.ToString() + Environment.NewLine + ex.Message + Environment.StackTrace);
      }
    }
    
    private void SetAtmoColor(byte red, byte green, byte blue)
    {
      if (atmoCtrl == null)
        return;
      try
      {
        atmoCtrl.setStaticColor(red, green, blue);
        Log.Info("atmolight: Set static color to RED={0} GREEN={1} BLUE={2}", red, green, blue);
      }
      catch (Exception ex)
      {
        Log.Error("atmolight: Failed to set static color to RED={0} GREEN={1} BLUE={2}" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace, red, green, blue);
      }
    }
    
    private void EnableLivePictureMode(ComLiveViewSource viewSource)
    {
      SetAtmoEffect(ComEffectMode.cemLivePicture);
      atmoLiveViewCtrl.setLiveViewSource(viewSource);
    }
    
    private void DisableLEDs()
    {
        Atmo_off = true;
        atmoLiveViewCtrl.setLiveViewSource(ComLiveViewSource.lvsGDI);
        SetAtmoEffect(ComEffectMode.cemDisabled);
        // Workaround for SEDU
        System.Threading.Thread.Sleep(10);
        SetAtmoColor(0, 0, 0);
    }

    private bool CheckForStartRequirements()
    {
        if (AtmolightSettings.OffOnStart)
        {
            Log.Debug("atmolight: LEDs should be deactivated. (Manual Mode)");
            Atmo_off = true;
            return false;
        }
        else if ((DateTime.Now.TimeOfDay >= AtmolightSettings.excludeTimeStart.TimeOfDay && DateTime.Now.TimeOfDay <= AtmolightSettings.excludeTimeEnd.TimeOfDay))
        {
            Log.Debug("atmolight: LEDs should be deactivated. (Timeframe)");
            Atmo_off = true;
            return false;
        }
        else
        {
            Log.Debug("atmolight: LEDs should be activated.");
            Atmo_off = false;
            return true;
        }
    }

    private void MenuMode()
    {
            switch (MenuEffect)
            {
                case ContentEffect.AtmoWin_GDI_Live_view:
                    Atmo_off = false;
                    EnableLivePictureMode(ComLiveViewSource.lvsGDI);
                    break;
                case ContentEffect.Colorchanger:
                    Atmo_off = false;
                    EnableLivePictureMode(ComLiveViewSource.lvsGDI);
                    SetAtmoEffect(ComEffectMode.cemColorChange);
                    break;
                case ContentEffect.Colorchanger_LR:
                    Atmo_off = false;
                    EnableLivePictureMode(ComLiveViewSource.lvsGDI);
                    SetAtmoEffect(ComEffectMode.cemLrColorChange);
                    break;
                case ContentEffect.LEDs_disabled:
                    DisableLEDs();
                    break;
                // Effect can be called "MP_Live_view" but it actually is "Static Color".
                // This should not happen anymore, but the case for it stays in for now.
                case ContentEffect.MP_Live_view:
                case ContentEffect.StaticColor:
                    Atmo_off = false;
                    atmoLiveViewCtrl.setLiveViewSource(ComLiveViewSource.lvsGDI);
                    SetAtmoEffect(ComEffectMode.cemDisabled);
                    SetAtmoColor((byte)StaticColor[0], (byte)StaticColor[1], (byte)StaticColor[2]);
                    // Workaround for SEDU
                    System.Threading.Thread.Sleep(20);
                    SetAtmoColor((byte)StaticColor[0], (byte)StaticColor[1], (byte)StaticColor[2]);
                    break;
            }
    }

    private void PlaybackMode()
    {
        switch (currentEffect)
        {
            case ContentEffect.AtmoWin_GDI_Live_view:
                Atmo_off = false;
                EnableLivePictureMode(ComLiveViewSource.lvsGDI);
                break;
            case ContentEffect.Colorchanger:
                Atmo_off = false;
                EnableLivePictureMode(ComLiveViewSource.lvsGDI);
                SetAtmoEffect(ComEffectMode.cemColorChange);
                break;
            case ContentEffect.Colorchanger_LR:
                Atmo_off = false;
                EnableLivePictureMode(ComLiveViewSource.lvsGDI);
                SetAtmoEffect(ComEffectMode.cemLrColorChange);
                break;
            case ContentEffect.LEDs_disabled:
                DisableLEDs();
                break;
            case ContentEffect.MP_Live_view:
                Atmo_off = false;
                EnableLivePictureMode(ComLiveViewSource.lvsExternal);
                break;
            case ContentEffect.StaticColor:
                Atmo_off = false;
                atmoLiveViewCtrl.setLiveViewSource(ComLiveViewSource.lvsGDI);
                SetAtmoEffect(ComEffectMode.cemDisabled);
                SetAtmoColor((byte)StaticColor[0], (byte)StaticColor[1], (byte)StaticColor[2]);
                // Workaround for SEDU
                System.Threading.Thread.Sleep(20);
                SetAtmoColor((byte)StaticColor[0], (byte)StaticColor[1], (byte)StaticColor[2]);
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

    private string GetKeyboardString(string KeyboardString)
    {
        VirtualKeyboard Keyboard = (VirtualKeyboard)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_VIRTUAL_KEYBOARD);
        if (Keyboard == null)
        {
            return null;
        }
        Keyboard.Reset();
        Keyboard.Text = KeyboardString;
        Keyboard.DoModal(GUIWindowManager.ActiveWindow);
        if (Keyboard.IsConfirmed)
        {
            return Keyboard.Text;
        }
        return null;
    }

    private void DialogRGBError()
    {
        GUIDialogOK dlgError = (GUIDialogOK)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_OK);
        if (dlgError != null)
        {
            dlgError.SetHeading(LanguageLoader.appStrings.ContextMenu_Error + "!");
            dlgError.SetLine(1, LanguageLoader.appStrings.ContextMenu_RGBErrorLine1);
            dlgError.SetLine(2, LanguageLoader.appStrings.ContextMenu_RGBErrorLine2);
            dlgError.DoModal(GUIWindowManager.ActiveWindow);
        }
    }

    private void DialogRGBManualStaticColorChanger(bool Reset = true, int StartPosition = 0)
    {
        if (Reset)
        {
            StaticColorTemp = new int[] { -1, -1, -1 };
        }
        GUIDialogMenu dlgRGB = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
        dlgRGB.Reset();
        dlgRGB.SetHeading(LanguageLoader.appStrings.ContextMenu_ManualStaticColor);
        dlgRGB.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Red + ": " + (StaticColorTemp[0] == -1 ? LanguageLoader.appStrings.ContextMenu_NA : StaticColorTemp[0].ToString())));
        dlgRGB.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Green + ": " + (StaticColorTemp[1] == -1 ? LanguageLoader.appStrings.ContextMenu_NA : StaticColorTemp[1].ToString())));
        dlgRGB.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Blue + ": " + (StaticColorTemp[2] == -1 ? LanguageLoader.appStrings.ContextMenu_NA : StaticColorTemp[2].ToString())));
        dlgRGB.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Apply));
        dlgRGB.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Cancel));
        dlgRGB.SelectedLabel = StartPosition;
        dlgRGB.DoModal(GUIWindowManager.ActiveWindow);
        switch (dlgRGB.SelectedLabel)
        {
            case 0:
            case 1:
            case 2:
                if ((int.TryParse(GetKeyboardString((StaticColorTemp[dlgRGB.SelectedLabel] == -1 ? "" : StaticColorTemp[dlgRGB.SelectedLabel].ToString())), out StaticColorHelper)) && (StaticColorHelper >= 0) && (StaticColorHelper <= 255))
                {
                    StaticColorTemp[dlgRGB.SelectedLabel] = StaticColorHelper;
                }
                else
                {
                    DialogRGBError();
                }
                break;
            case 3:
                if (StaticColorTemp[0] == -1 || StaticColorTemp[1] == -1 || StaticColorTemp[2] == -1)
                {
                    DialogRGBError();
                    break;
                }
                else
                {
                    StaticColor = StaticColorTemp;
                    return;
                }
            case 4:
                return;
        }
        DialogRGBManualStaticColorChanger(false, dlgRGB.SelectedLabel);
    }
    #endregion

    #region Events

    public void OnNewAction(MediaPortal.GUI.Library.Action action)
    {
        // Remote Key to toggle On/Off
        if ((action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_YELLOW_BUTTON && AtmolightSettings.killbutton == 2) ||
            (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_GREEN_BUTTON && AtmolightSettings.killbutton == 1) ||
            (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_RED_BUTTON && AtmolightSettings.killbutton == 0) ||
            (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_BLUE_BUTTON && AtmolightSettings.killbutton == 3))
        {
            if (Atmo_off)
            {
                StartLEDs();
            }
            else
            {
                DisableLEDs();
            }
        }
        // Remote Key to change Profiles
        else if ((action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_YELLOW_BUTTON && AtmolightSettings.cmbutton == 2) ||
            (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_GREEN_BUTTON && AtmolightSettings.cmbutton == 1) ||
            (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_RED_BUTTON && AtmolightSettings.cmbutton == 0) ||
            (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_BLUE_BUTTON && AtmolightSettings.cmbutton == 3))
        {
            SetColorMode(ComEffectMode.cemColorMode);
        }
        // Remote Key to open Menu
        else if ((action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_YELLOW_BUTTON && AtmolightSettings.menubutton == 2) ||
            (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_GREEN_BUTTON && AtmolightSettings.menubutton == 1) ||
            (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_RED_BUTTON && AtmolightSettings.menubutton == 0) ||
            (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_BLUE_BUTTON && AtmolightSettings.menubutton == 3) ||
            (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_STOP && AtmolightSettings.menubutton == 4 && !g_Player.Playing))
        {
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);

            dlg.Reset();
            dlg.SetHeading("AtmoLight");
            if (Atmo_off)
            {
                dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_SwitchLEDsON));
            }
            else
            {
                dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_SwitchLEDsOFF));
            }

            dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_ChangeEffect));
            dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_ChangeAWProfile));

            if (AtmolightSettings.SBS_3D_ON)
            {
                dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Switch3DOFF));
            }
            else
            {
                dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_Switch3DON));
            }
            if (((g_Player.Playing) && (currentEffect == ContentEffect.StaticColor) && (!Atmo_off)) ||
                ((!g_Player.Playing) && (MenuEffect == ContentEffect.StaticColor) && (!Atmo_off)))
            {
                dlg.Add(new GUIListItem(LanguageLoader.appStrings.ContextMenu_ChangeStatic));
            }

            dlg.SelectedLabel = 0;
            dlg.DoModal(GUIWindowManager.ActiveWindow);

            switch (dlg.SelectedLabel)
            {
                case 0:
                    if (Atmo_off)
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
                                currentEffect = ContentEffect.LEDs_disabled;
                                DisableLEDs();
                                break;
                            case 1:
                                currentEffect = ContentEffect.MP_Live_view;
                                PlaybackMode();
                                break;
                            case 2:
                                currentEffect = ContentEffect.AtmoWin_GDI_Live_view;
                                PlaybackMode();
                                break;
                            case 3:
                                currentEffect = ContentEffect.Colorchanger;
                                PlaybackMode();
                                break;
                            case 4:
                                currentEffect = ContentEffect.Colorchanger_LR;
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
                                MenuEffect = ContentEffect.LEDs_disabled;
                                DisableLEDs();
                                break;
                            case 1:
                                MenuEffect = ContentEffect.AtmoWin_GDI_Live_view;
                                MenuMode();
                                break;
                            case 2:
                                MenuEffect = ContentEffect.Colorchanger;
                                MenuMode();
                                break;
                            case 3:
                                MenuEffect = ContentEffect.Colorchanger_LR;
                                MenuMode();
                                break;
                            case 4:
                                MenuEffect = ContentEffect.StaticColor;
                                MenuMode();
                                break;
                        }
                    }
                    break;
                case 2:
                    SetColorMode(ComEffectMode.cemColorMode);
                    break;
                case 3:
                    if (AtmolightSettings.SBS_3D_ON)
                    {
                        AtmolightSettings.SBS_3D_ON = false;
                    }
                    else
                    {
                        AtmolightSettings.SBS_3D_ON = true;
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
                            AtmolightSettings.SaveSpecificSettingInteger("StaticColorRed", StaticColor[0]);
							AtmolightSettings.StaticColorRed = StaticColor[0];
                            AtmolightSettings.SaveSpecificSettingInteger("StaticColorGreen", StaticColor[1]);
							AtmolightSettings.StaticColorGreen = StaticColor[1];
                            AtmolightSettings.SaveSpecificSettingInteger("StaticColorBlue", StaticColor[2]);
							AtmolightSettings.StaticColorBlue = StaticColor[2];
                            break;
                        case 2:
                            StaticColor[0] = AtmolightSettings.StaticColorRed;
                            StaticColor[1] = AtmolightSettings.StaticColorGreen;
                            StaticColor[2] = AtmolightSettings.StaticColorBlue;
                            break;
                        case 3:
                            StaticColor[0] = 255;
                            StaticColor[1] = 255;
                            StaticColor[2] = 255;
                            break;
                        case 4:
                            StaticColor[0] = 255;
                            StaticColor[1] = 0;
                            StaticColor[2] = 0;
                            break;
                        case 5:
                            StaticColor[0] = 0;
                            StaticColor[1]  = 255;
                            StaticColor[2] = 0;
                            break;
                        case 6:
                            StaticColor[0] = 0;
                            StaticColor[1] = 0;
                            StaticColor[2] = 255;
                            break;
                        case 7:
                            StaticColor[0] = 0;
                            StaticColor[1] = 255;
                            StaticColor[2] = 255;
                            break;
                        case 8:
                            StaticColor[0] = 255;
                            StaticColor[1] = 0;
                            StaticColor[2] = 255;
                            break;
                        case 9:
                            StaticColor[0] = 255;
                            StaticColor[1] = 255;
                            StaticColor[2] = 0;
                            break;
                    }
                    StartLEDs();
                    break;
            }
        }
    }


    void g_Player_PlayBackEnded(g_Player.MediaType type, string filename)
    {
        if (CheckForStartRequirements())
        {
            MenuMode();
        }
        else
        {
            DisableLEDs();
        }
    }
    
    void g_Player_PlayBackStopped(g_Player.MediaType type, int stoptime, string filename)
    {
        if (CheckForStartRequirements())
        {
            MenuMode();
        }
        else
        {
            DisableLEDs();
        }
    }
    
    void g_Player_PlayBackStarted(g_Player.MediaType type, string filename)
    {
        if (type == g_Player.MediaType.Video || type == g_Player.MediaType.TV || type == g_Player.MediaType.Recording || type == g_Player.MediaType.Unknown || (type == g_Player.MediaType.Music && filename.Contains(".mkv")))
        {
            Log.Debug("atmolight: Video detected)");
            currentEffect = AtmolightSettings.effectVideo;
        }
        else if (type == g_Player.MediaType.Music)
        {
            // Workaround
            if (AtmolightSettings.effectMusic == ContentEffect.MP_Live_view)
            {
                AtmolightSettings.effectMusic = ContentEffect.StaticColor;
            }
            currentEffect = AtmolightSettings.effectMusic;
            Log.Debug("atmolight: Music detected)");
        }
        else if (type == g_Player.MediaType.Radio)
        {
            // Workaround
            if (AtmolightSettings.effectRadio == ContentEffect.MP_Live_view)
            {
                AtmolightSettings.effectRadio = ContentEffect.StaticColor;
            }
            currentEffect = AtmolightSettings.effectRadio;
            Log.Debug("atmolight: Radio detected)");
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
      return "Atmolight";
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

    #region IPlugin implementation
    public void Start()
    {
      if (atmoCtrl != null)
      {
        g_Player.PlayBackStarted += new g_Player.StartedHandler(g_Player_PlayBackStarted);
        g_Player.PlayBackStopped += new g_Player.StoppedHandler(g_Player_PlayBackStopped);
        g_Player.PlayBackEnded += new g_Player.EndedHandler(g_Player_PlayBackEnded);

        FrameGrabber.GetInstance().OnNewFrame += new FrameGrabber.NewFrameHandler(AtmolightPlugin_OnNewFrame);
        
        Log.Info("atmolight: Start() waiting for g_player events...");

        // Workaround
        if (AtmolightSettings.effectMenu == ContentEffect.MP_Live_view)
        {
            AtmolightSettings.effectMenu = ContentEffect.StaticColor;
        }

        MenuEffect = AtmolightSettings.effectMenu;

        StaticColor[0] = AtmolightSettings.StaticColorRed;
        StaticColor[1] = AtmolightSettings.StaticColorGreen;
        StaticColor[2] = AtmolightSettings.StaticColorBlue;

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

    void AtmolightPlugin_OnNewFrame(short width, short height, short arWidth, short arHeight, uint pSurface)
    {
      if (AtmolightSettings.lowCPU) tickCount = Win32API.GetTickCount();

      if (currentEffect != ContentEffect.MP_Live_view || Atmo_off)
      {
        return;
      }

      if (width == 0 || height == 0)
        return;

      if (rgbSurface == null)
        rgbSurface = GUIGraphicsContext.DX9Device.CreateRenderTarget(captureWidth, captureHeight, Format.A8R8G8B8, MultiSampleType.None, 0, true);
      unsafe
      {
        try
        {
          if (AtmolightSettings.SBS_3D_ON)
            VideoSurfaceToRGBSurfaceExt(new IntPtr(pSurface), width / 2, height, (IntPtr)rgbSurface.UnmanagedComPointer, captureWidth, captureHeight);
          else
            VideoSurfaceToRGBSurfaceExt(new IntPtr(pSurface), width, height, (IntPtr)rgbSurface.UnmanagedComPointer, captureWidth, captureHeight);

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
          if (!AtmolightSettings.lowCPU || (((Win32API.GetTickCount() - lastFrame) > AtmolightSettings.lowCPUTime) && AtmolightSettings.lowCPU))
          {
            if (AtmolightSettings.lowCPU) lastFrame = Win32API.GetTickCount();
            atmoLiveViewCtrl.setPixelData(bmiInfoHeader, pixelData);
          }
          stream.Close();
          stream.Dispose();
        }
        catch (Exception ex)
        {
          Log.Error(ex);
          rgbSurface.Dispose();
          rgbSurface = null;
        }
      }
    }

    public void Stop()
    {
      Log.Debug("atmolight: Stop()");
      if (atmoCtrl != null)
      {
        FrameGrabber.GetInstance().OnNewFrame += new FrameGrabber.NewFrameHandler(AtmolightPlugin_OnNewFrame);
        
        g_Player.PlayBackStarted -= new g_Player.StartedHandler(g_Player_PlayBackStarted);
        g_Player.PlayBackStopped -= new g_Player.StoppedHandler(g_Player_PlayBackStopped);
        g_Player.PlayBackEnded -= new g_Player.EndedHandler(g_Player_PlayBackEnded);

        GUIWindowManager.OnNewAction -= new OnActionHandler(OnNewAction);

        if (AtmolightSettings.disableOnShutdown)
          DisableLEDs();
        
        if (AtmolightSettings.enableInternalLiveView)
          EnableLivePictureMode(ComLiveViewSource.lvsGDI);

        atmoCtrl = null;

        if (AtmolightSettings.exitAtmoWin)
        {
          foreach (var process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension("atmowina")))
          {
            process.Kill();
            Win32API.RefreshTrayArea();
          }
        }
      }
    }
    #endregion
  }
}
