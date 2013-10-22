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
  public class AtmolightPlugin: ISetupForm, IPlugin
  {
    #region Variables
      public static bool Atmo_off = false;
      public int lasteff = 999;
    private IAtmoRemoteControl2 atmoCtrl=null;
    private IAtmoLiveViewControl atmoLiveViewCtrl = null;
    private int captureWidth = 0;
    private int captureHeight = 0;
    private Surface rgbSurface = null;
    private ContentEffect currentEffect = ContentEffect.LEDs_disabled;
    private bool gap = false;
    #endregion

    [DllImport("AtmoDXUtil.dll", PreserveSig = false, CharSet = CharSet.Auto)]
    private static extern void VideoSurfaceToRGBSurfaceExt(IntPtr src,int srcWidth,int srcHeight, IntPtr dst,int dstWidth,int dstHeight);

    public AtmolightPlugin()
    {
      AtmolightSettings.LoadSettings();
      Log.Info("atmolight: Trying to connect to atmowina.exe...");
      if (!ConnectToAtmoWinA())
      {
        Log.Warn("atmolight: AtmoWinA.exe not started. Trying to launch...");
        if (!StartAtmoWinA())
        {
          Log.Error("atmolight: Can't start atmowin.exe. Atmolight control won't work :(");
          return;
        }
        if (!ConnectToAtmoWinA())
        {
          Log.Error("atmolight: AtmoWinA started but still can't connect. Atmolight control won't work :(");
          return;
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
        Log.Error("atmolight: exception= {0}",ex.Message);
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
        Log.Error("atmolight: Failed to switch effect to "+effect.ToString()+Environment.NewLine+ex.Message+Environment.StackTrace);
      }
    }
    private void SetAtmoColor(byte red, byte green, byte blue)
    {
      if (atmoCtrl == null)
        return;
      try
      {
        atmoCtrl.setStaticColor(red, green, blue);
        Log.Info("atmolight: Set static color to RED={0} GREEN={1} BLUE={2}",red,green,blue);
      }
      catch (Exception ex)
      {
        Log.Error("atmolight: Failed to set static color to RED={0} GREEN={1} BLUE={2}"+Environment.NewLine+ex.Message+Environment.NewLine+ex.StackTrace, red, green, blue);
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
      SetAtmoColor(0, 0, 0);
      //SetAtmoEffect(ComEffectMode.cemStaticColor);
      SetAtmoEffect(ComEffectMode.cemDisabled);
    }
    #endregion

    public void OnNewAction(MediaPortal.GUI.Library.Action action)
    {
        if ((action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_YELLOW_BUTTON && AtmolightSettings.killbutton == 2) ||
            (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_GREEN_BUTTON && AtmolightSettings.killbutton == 1) ||
            (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_RED_BUTTON && AtmolightSettings.killbutton == 0) ||
            (action.wID == MediaPortal.GUI.Library.Action.ActionType.ACTION_REMOTE_BLUE_BUTTON && AtmolightSettings.killbutton == 3))
        {
            if (Atmo_off)
            { 
                Atmo_off = false;
                //SetAtmoEffect(ComEffectMode.cemLivePicture);
                if (lasteff == 0)
                {
                    currentEffect = AtmolightSettings.effectVideo;
                }
                if (lasteff == 1)
                {
                    currentEffect = AtmolightSettings.effectMusic;
                }
                if (lasteff == 2)
                {
                    currentEffect = AtmolightSettings.effectRadio;
                }
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
                Atmo_off = true;

                atmoLiveViewCtrl.setLiveViewSource(ComLiveViewSource.lvsGDI);
                SetAtmoEffect(ComEffectMode.cemDisabled);
                SetAtmoColor(0, 0, 0);
                //SetAtmoEffect(ComEffectMode.cemStaticColor);
                SetAtmoEffect(ComEffectMode.cemDisabled);
            }
            return;
        }

        if (action.wID != MediaPortal.GUI.Library.Action.ActionType.ACTION_STOP || g_Player.Playing)
            return;

        if (!AtmolightSettings.HateTheStopThing)
        {
            GUIDialogMenu dlg = (GUIDialogMenu)GUIWindowManager.GetWindow((int)GUIWindow.Window.WINDOW_DIALOG_MENU);
            dlg.Reset();
            dlg.SetHeading("Set Atmolight Mode");
         
            dlg.Add(new GUIListItem("GDI Live-View"));

            if (Atmo_off)
                dlg.Add(new GUIListItem("switch LEDs on"));
            else
                dlg.Add(new GUIListItem("switch All LEDs off"));
            if (AtmolightSettings.SBS_3D_ON)
                dlg.Add(new GUIListItem("switch 3D SBS Mode off"));
            else
                dlg.Add(new GUIListItem("switch 3D SBS Mode on"));

            dlg.SelectedLabel = 0;
            dlg.DoModal(GUIWindowManager.ActiveWindow);
            if (dlg.SelectedLabel == 0)
            {
                EnableLivePictureMode(ComLiveViewSource.lvsGDI);
                SetAtmoEffect(ComEffectMode.cemLivePicture);
            }
            else if (dlg.SelectedLabel == 1)
            {
                // DisableLEDs();
                // SetAtmoEffect(ComEffectMode.cemDisabled);
                // AtmolightSettings.effectVideo = ContentEffect.LEDs_disabled;
                if (Atmo_off)
                { Atmo_off = false; }
                else
                { Atmo_off = true; }

            }
            else if (dlg.SelectedLabel == 2)
            {
                if (AtmolightSettings.SBS_3D_ON)
                { AtmolightSettings.SBS_3D_ON = false; }
                else
                { AtmolightSettings.SBS_3D_ON = true; }
            }
        }
    }
    
    #region Events
    void g_Player_PlayBackEnded(g_Player.MediaType type, string filename)
    {
      DisableLEDs();
    }
    void g_Player_PlayBackStopped(g_Player.MediaType type, int stoptime, string filename)
    {
      DisableLEDs();
    }
    void g_Player_PlayBackStarted(g_Player.MediaType type, string filename)
    {
        if ((DateTime.Now.TimeOfDay >= AtmolightSettings.excludeTimeStart.TimeOfDay && DateTime.Now.TimeOfDay <= AtmolightSettings.excludeTimeEnd.TimeOfDay) )
      {
        Log.Debug("atmolight: Playback st_art_ed received but won't do anything because we are in the time where there is enough daylight :)");
        return;
      }
      if (type == g_Player.MediaType.Video || type == g_Player.MediaType.TV || type == g_Player.MediaType.Recording || type == g_Player.MediaType.Unknown)
      {
        //EnableLivePictureMode(ComLiveViewSource.lvsExternal);
        //currentEffect = ContentEffect.MP_Live_view;
        currentEffect = AtmolightSettings.effectVideo;
        lasteff = 0;
      }
      else if (type == g_Player.MediaType.Music)
      {
        currentEffect = AtmolightSettings.effectMusic;
        lasteff = 1;
      }
      else if (type == g_Player.MediaType.Radio)
      {
        currentEffect = AtmolightSettings.effectRadio;
        lasteff = 2;
      }

      if (Atmo_off == false)
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
        g_Player.PlayBackStarted +=  new g_Player.StartedHandler(g_Player_PlayBackStarted);
        g_Player.PlayBackStopped += new g_Player.StoppedHandler(g_Player_PlayBackStopped);
        g_Player.PlayBackEnded += new g_Player.EndedHandler(g_Player_PlayBackEnded);
        FrameGrabber.GetInstance().OnNewFrame += new FrameGrabber.NewFrameHandler(AtmolightPlugin_OnNewFrame);
        Log.Info("atmolight: Start() waiting for g_player events...");
        DisableLEDs();
       // if (!AtmolightSettings.HateTheStopThing) 
            GUIGraphicsContext.OnNewAction += new OnActionHandler(OnNewAction);
        if (AtmolightSettings.OffOnStart)
        { Atmo_off = true; }
      }
    }

    void AtmolightPlugin_OnNewFrame(short width, short height, short arWidth, short arHeight, uint pSurface)
    {
        //if (gap == false && AtmolightSettings.lowCPU)
        //{
        //    gap = true;
        //    return;
        //}
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
                VideoSurfaceToRGBSurfaceExt(new IntPtr(pSurface), width/2, height, (IntPtr)rgbSurface.UnmanagedComPointer, captureWidth, captureHeight);
                else
                VideoSurfaceToRGBSurfaceExt(new IntPtr(pSurface), width, height, (IntPtr)rgbSurface.UnmanagedComPointer, captureWidth, captureHeight);

                Microsoft.DirectX.GraphicsStream stream = SurfaceLoader.SaveToStream(ImageFileFormat.Bmp, rgbSurface);


                BinaryReader reader = new BinaryReader(stream);
                stream.Position = 0; // ensure that what start at the beginning of the stream. 
                reader.ReadBytes(14); // skip bitmap file info header
                byte[] bmiInfoHeader = reader.ReadBytes(4 + 4 + 4 + 2 + 2 + 4 + 4 + 4 + 4 + 4 + 4);

                int rgbL = (int)(stream.Length - stream.Position);
                int rgb =  (int)(rgbL / (captureWidth * captureHeight));
      
                //byte[] fpixelData = new byte[rgbL];

                byte[] pixelData = reader.ReadBytes((int)(stream.Length - stream.Position));

                byte[] h1pixelData = new byte[captureWidth * rgb];
                byte[] h2pixelData = new byte[captureWidth * rgb];

                int i;
                for (i = 0; i < ((captureHeight / 2) - 1); i++)
                {

                    Array.Copy(pixelData, i * captureWidth * rgb, h1pixelData, 0, captureWidth * rgb);
                    Array.Copy(pixelData, (captureHeight - i - 1) * captureWidth * rgb, h2pixelData, 0, captureWidth * rgb);

                    Array.Copy(h1pixelData, 0, pixelData, (captureHeight - i - 1) * captureWidth * rgb, captureWidth * rgb);
                    Array.Copy(h2pixelData, 0, pixelData, i * captureWidth * rgb, captureWidth * rgb);
                }
               // Array.Copy(pixelData, 0, fpixelData, 0, rgbL);

                if ((gap == false && AtmolightSettings.lowCPU) || !AtmolightSettings.lowCPU)
                {

                    atmoLiveViewCtrl.setPixelData(bmiInfoHeader, pixelData);
                    gap = true;
                    //return;
                }
                else gap = false;

                //atmoLiveViewCtrl.setPixelData(bmiInfoHeader, pixelData);
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
        //gap = false;
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
        //if (!AtmolightSettings.HateTheStopThing) 
            GUIGraphicsContext.OnNewAction -= new OnActionHandler(OnNewAction); 

        if (AtmolightSettings.disableOnShutdown)
          DisableLEDs();
        if (AtmolightSettings.enableInternalLiveView)
          EnableLivePictureMode(ComLiveViewSource.lvsGDI);
        atmoCtrl = null;
      }
    }
#endregion
}
}
