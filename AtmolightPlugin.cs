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
      atmoLiveViewCtrl.setLiveViewSource(ComLiveViewSource.lvsGDI);
      SetAtmoEffect(ComEffectMode.cemDisabled);
      System.Threading.Thread.Sleep(10);
      SetAtmoColor(0, 0, 0);
      SetAtmoEffect(ComEffectMode.cemDisabled);
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
            MenuEffect = AtmolightSettings.effectMenu;
            switch (MenuEffect)
            {
                case ContentEffect.AtmoWin_GDI_Live_view:
                    EnableLivePictureMode(ComLiveViewSource.lvsGDI);
                    break;
                case ContentEffect.Colorchanger:
                    EnableLivePictureMode(ComLiveViewSource.lvsGDI);
                    SetAtmoEffect(ComEffectMode.cemColorChange);
                    break;
                case ContentEffect.Colorchanger_LR:
                    EnableLivePictureMode(ComLiveViewSource.lvsGDI);
                    SetAtmoEffect(ComEffectMode.cemLrColorChange);
                    break;
                case ContentEffect.LEDs_disabled:
                    DisableLEDs();
                    Atmo_off = true;
                    break;
                // Effect is called "MP_Live_view" but it actually is "Static Color".
                case ContentEffect.MP_Live_view:
                case ContentEffect.ColorMode:
                    atmoLiveViewCtrl.setLiveViewSource(ComLiveViewSource.lvsGDI);
                    SetAtmoEffect(ComEffectMode.cemDisabled);
                    SetAtmoColor((byte)AtmolightSettings.StaticColorRed, (byte)AtmolightSettings.StaticColorGreen, (byte)AtmolightSettings.StaticColorBlue);
                    break;
            }
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
            if (g_Player.Playing)
            {
                if (Atmo_off)
                {
                    Atmo_off = false;
                    switch (currentEffect)
                    {
                        case ContentEffect.AtmoWin_GDI_Live_view:
                            EnableLivePictureMode(ComLiveViewSource.lvsGDI);
                            break;
                        case ContentEffect.Colorchanger:
                            EnableLivePictureMode(ComLiveViewSource.lvsGDI);
                            SetAtmoEffect(ComEffectMode.cemColorChange);
                            break;
                        case ContentEffect.Colorchanger_LR:
                            EnableLivePictureMode(ComLiveViewSource.lvsGDI);
                            SetAtmoEffect(ComEffectMode.cemLrColorChange);
                            break;
                        case ContentEffect.LEDs_disabled:
                            DisableLEDs();
                            break;
                        case ContentEffect.MP_Live_view:
                            EnableLivePictureMode(ComLiveViewSource.lvsExternal);
                            break;
                        case ContentEffect.ColorMode:
                            atmoLiveViewCtrl.setLiveViewSource(ComLiveViewSource.lvsGDI);
                            SetAtmoEffect(ComEffectMode.cemDisabled);
                            SetAtmoColor((byte)AtmolightSettings.StaticColorRed, (byte)AtmolightSettings.StaticColorGreen, (byte)AtmolightSettings.StaticColorBlue);
                            break;
                    }
                }
                else
                {
                    Atmo_off = true;
                    DisableLEDs();
                }
                return;
            }
            else
            {
                if (Atmo_off)
                {
                    Atmo_off = false;
                    MenuMode();
                }
                else
                {
                    Atmo_off = true;
                    DisableLEDs();
                }
              
            }
        }
        // Remote Key to change Profiles
        else if ((action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_YELLOW_BUTTON && AtmolightSettings.cmbutton == 2) ||
            (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_GREEN_BUTTON && AtmolightSettings.cmbutton == 1) ||
            (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_RED_BUTTON && AtmolightSettings.cmbutton == 0) ||
            (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_BLUE_BUTTON && AtmolightSettings.cmbutton == 3))
        {
            SetColorMode(ComEffectMode.cemColorMode);
            return;
        }
        // Remote Key to open Menu
        else if ((action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_YELLOW_BUTTON && AtmolightSettings.menubutton == 2) ||
            (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_GREEN_BUTTON && AtmolightSettings.menubutton == 1) ||
            (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_RED_BUTTON && AtmolightSettings.menubutton == 0) ||
            (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_BLUE_BUTTON && AtmolightSettings.menubutton == 3))
        {
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);

            dlg.Reset();
            dlg.SetHeading("AtmoLight");
            if (Atmo_off)
            {
                dlg.Add(new GUIListItem("Switch LEDs on"));
            }
            else
            {
                dlg.Add(new GUIListItem("Switch LEDs off"));
            }

            dlg.Add(new GUIListItem("Choose Effect"));

            if (AtmolightSettings.SBS_3D_ON)
            {
                dlg.Add(new GUIListItem("Switch 3D SBS Mode off"));
            }
            else
            {
                dlg.Add(new GUIListItem("Switch 3D SBS Mode on"));
            }
            dlg.SelectedLabel = 0;
            dlg.DoModal(GUIWindowManager.ActiveWindow);

            if (dlg.SelectedLabel == 0)
            {
                if (Atmo_off)
                {
                    Atmo_off = false;
                    MenuMode();
                }
                else
                {
                    Atmo_off = true;
                    DisableLEDs();
                }
            }
            else if (dlg.SelectedLabel == 1)
            {
                if (g_Player.Playing)
                {
                    GUIDialogMenu dlgEffect = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    dlgEffect.Reset();
                    dlgEffect.SetHeading("Set Menu Effect");
                    dlgEffect.Add(new GUIListItem("LEDs disabled"));
                    dlgEffect.Add(new GUIListItem("MediaPortal Live Mode"));
                    dlgEffect.Add(new GUIListItem("AtmoWin Live Mode"));
                    dlgEffect.Add(new GUIListItem("Colorchanger"));
                    dlgEffect.Add(new GUIListItem("Colorchanger LR"));
                    dlgEffect.Add(new GUIListItem("Static Color"));
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
                            EnableLivePictureMode(ComLiveViewSource.lvsExternal);
                            break;
                        case 2:
                            currentEffect = ContentEffect.AtmoWin_GDI_Live_view;
                            EnableLivePictureMode(ComLiveViewSource.lvsGDI);
                            break;
                        case 3:
                            currentEffect = ContentEffect.Colorchanger;
                            EnableLivePictureMode(ComLiveViewSource.lvsGDI);
                            SetAtmoEffect(ComEffectMode.cemColorChange);
                            break;
                        case 4:
                            currentEffect = ContentEffect.Colorchanger_LR;
                            EnableLivePictureMode(ComLiveViewSource.lvsGDI);
                            SetAtmoEffect(ComEffectMode.cemLrColorChange);
                            break;
                        case 5:
                            currentEffect = ContentEffect.ColorMode;
                            atmoLiveViewCtrl.setLiveViewSource(ComLiveViewSource.lvsGDI);
                            SetAtmoEffect(ComEffectMode.cemDisabled);
                            SetAtmoColor((byte)AtmolightSettings.StaticColorRed, (byte)AtmolightSettings.StaticColorGreen, (byte)AtmolightSettings.StaticColorBlue);
                            break;
                    }
                }
                else
                {
                    GUIDialogMenu dlgEffect = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
                    dlgEffect.Reset();
                    dlgEffect.SetHeading("Set Menu Effect");
                    dlgEffect.Add(new GUIListItem("LEDs disabled"));
                    dlgEffect.Add(new GUIListItem("AtmoWin Live Mode"));
                    dlgEffect.Add(new GUIListItem("Colorchanger"));
                    dlgEffect.Add(new GUIListItem("Colorchanger LR"));
                    dlgEffect.Add(new GUIListItem("Static Color"));
                    dlgEffect.SelectedLabel = 0;
                    dlgEffect.DoModal(GUIWindowManager.ActiveWindow);

                    switch (dlgEffect.SelectedLabel)
                    {
                        case 0:
                            AtmolightSettings.effectMenu = ContentEffect.LEDs_disabled;
                            DisableLEDs();
                            break;
                        case 1:
                            AtmolightSettings.effectMenu = ContentEffect.AtmoWin_GDI_Live_view;
                            MenuMode();
                            break;
                        case 2:
                            AtmolightSettings.effectMenu = ContentEffect.Colorchanger;
                            MenuMode();
                            break;
                        case 3:
                            AtmolightSettings.effectMenu = ContentEffect.Colorchanger_LR;
                            MenuMode();
                            break;
                        case 4:
                            AtmolightSettings.effectMenu = ContentEffect.ColorMode;
                            MenuMode();
                            break;
                    }
                }
            }
            else if (dlg.SelectedLabel == 2)
            {
                if (AtmolightSettings.SBS_3D_ON)
                {
                    AtmolightSettings.SBS_3D_ON = false;
                }
                else
                {
                    AtmolightSettings.SBS_3D_ON = true;
                }
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
            currentEffect = AtmolightSettings.effectMusic;
            Log.Debug("atmolight: Music detected)");
        }
        else if (type == g_Player.MediaType.Radio)
        {
            currentEffect = AtmolightSettings.effectRadio;
            Log.Debug("atmolight: Radio detected)");
        }

        if (CheckForStartRequirements())
        {
            switch (currentEffect)
            {
                case ContentEffect.AtmoWin_GDI_Live_view:
                    EnableLivePictureMode(ComLiveViewSource.lvsGDI);
                    break;
                case ContentEffect.Colorchanger:
                    EnableLivePictureMode(ComLiveViewSource.lvsGDI);
                    SetAtmoEffect(ComEffectMode.cemColorChange);
                    break;
                case ContentEffect.Colorchanger_LR:
                    EnableLivePictureMode(ComLiveViewSource.lvsGDI);
                    SetAtmoEffect(ComEffectMode.cemLrColorChange);
                    break;
                case ContentEffect.LEDs_disabled:
                    DisableLEDs();
                    break;
                case ContentEffect.MP_Live_view:
                    EnableLivePictureMode(ComLiveViewSource.lvsExternal);
                    break;
            }
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
        
        if (CheckForStartRequirements())
        {
            MenuMode();
        }
        else
        {
            DisableLEDs();
        }

        GUIGraphicsContext.OnNewAction += new OnActionHandler(OnNewAction);
        if (AtmolightSettings.OffOnStart)
        { 
          Atmo_off = true; 
        }
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

        GUIGraphicsContext.OnNewAction -= new OnActionHandler(OnNewAction);

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
