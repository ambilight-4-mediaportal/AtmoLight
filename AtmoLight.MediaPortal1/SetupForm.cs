using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Globalization;

namespace AtmoLight
{
  public partial class SetupForm : Form
  {
    private Core coreObject = Core.GetInstance();
    private Boolean atmoOrbSelectedIndexChangedHelper = false;

    #region Init

    public SetupForm()
    {
      InitializeComponent();
      UpdateLanguageOnControls();
      //Settings.LoadSettings();

      // AmbiBox
      if (Settings.ambiBoxTarget)
      {
        coreObject.AddTarget(Target.AmbiBox);
      }
      // AtmoWin
      if (Settings.atmoWinTarget)
      {
        coreObject.AddTarget(Target.AtmoWin);
      }
      // Boblight
      if (Settings.boblightTarget)
      {
        coreObject.AddTarget(Target.Boblight);
      }
      // Hyperion
      if (Settings.hyperionTarget)
      {
        coreObject.AddTarget(Target.Hyperion);
      }
      // Hue
      if (Settings.hueTarget)
      {
        coreObject.AddTarget(Target.Hue);
      }

      if (Settings.atmoOrbTarget)
      {
        coreObject.AddTarget(Target.AtmoOrb);
      }

      UpdateEffectComboBoxes();
      UpdateRemoteButtonComboBoxes();

      lblVersionVal.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
      edFileAtmoWin.Text = Settings.atmowinExe;
      comboBox1.SelectedIndex = (int) Settings.killButton;
      comboBox2.SelectedIndex = (int) Settings.profileButton;
      cbMenuButton.SelectedIndex = (int) Settings.menuButton;
      edExcludeStart.Text = Settings.excludeTimeStart.ToString("HH:mm");
      edExcludeEnd.Text = Settings.excludeTimeEnd.ToString("HH:mm");
      lowCpuTime.Text = Settings.lowCPUTime.ToString();
      tbDelay.Text = Settings.delayReferenceTime.ToString();
      tbRefreshRate.Text = Settings.delayReferenceRefreshRate.ToString();
      tbRed.Text = Settings.staticColorRed.ToString();
      tbGreen.Text = Settings.staticColorGreen.ToString();
      tbBlue.Text = Settings.staticColorBlue.ToString();
      tbBlackbarDetectionTime.Text = Settings.blackbarDetectionTime.ToString();
      tbGIF.Text = Settings.gifFile;
      tbHyperionIP.Text = Settings.hyperionIP;
      tbHyperionPort.Text = Settings.hyperionPort.ToString();
      tbHyperionReconnectDelay.Text = Settings.hyperionReconnectDelay.ToString();
      tbHyperionReconnectAttempts.Text = Settings.hyperionReconnectAttempts.ToString();
      tbHyperionPriority.Text = Settings.hyperionPriority.ToString();
      tbHyperionPriorityStaticColor.Text = Settings.hyperionPriorityStaticColor.ToString();
      tbCaptureWidth.Text = Settings.captureWidth.ToString();
      tbCaptureHeight.Text = Settings.captureHeight.ToString();
      edFileHue.Text = Settings.hueExe;
      ckStartHue.Checked = Settings.hueStart;
      ckhueIsRemoteMachine.Checked = Settings.hueIsRemoteMachine;
      tbHueIP.Text = Settings.hueIP;
      tbHuePort.Text = Settings.huePort.ToString();
      tbHueReconnectDelay.Text = Settings.hueReconnectDelay.ToString();
      tbHueReconnectAttempts.Text = Settings.hueReconnectAttempts.ToString();
      ckHueBridgeEnableOnResume.Checked = Settings.hueBridgeEnableOnResume;
      ckHueBridgeDisableOnSuspend.Checked = Settings.hueBridgeDisableOnSuspend;
      ckHueTheaterEnabled.Checked = Settings.hueTheaterEnabled;
      ckHueTheaterRestoreLights.Checked = Settings.hueTheaterRestoreLights;
      ckOnMediaStart.Checked = Settings.manualMode;
      ckLowCpu.Checked = Settings.lowCPU;
      ckDelay.Checked = Settings.delay;
      ckStartAtmoWin.Checked = Settings.startAtmoWin;
      ckExitAtmoWin.Checked = Settings.exitAtmoWin;
      ckAtmoWakeHelperEnabled.Checked = Settings.atmoWakeHelperEnabled;
      cbAtmoWakeHelperComPort.Text = Settings.atmoWakeHelperComPort;
      tbAtmoWakeHelperResumeDelay.Text = Settings.atmoWakeHelperResumeDelay.ToString();
      tbAtmoWakeHelperDisconnectDelay.Text = Settings.atmoWakeHelperDisconnectDelay.ToString();
      tbAtmoWakeHelperConnectDelay.Text = Settings.atmoWakeHelperConnectDelay.ToString();
      tbAtmoWakeHelperReinitializationDelay.Text = Settings.atmoWakeHelperReinitializationDelay.ToString();
      ckRestartOnError.Checked = Settings.restartOnError;
      ckTrueGrabbing.Checked = Settings.trueGrabbing;
      ckBlackbarDetection.Checked = Settings.blackbarDetection;
      ckAtmowinEnabled.Checked = Settings.atmoWinTarget;
      ckHyperionEnabled.Checked = Settings.hyperionTarget;
      ckHueEnabled.Checked = Settings.hueTarget;
      ckHyperionLiveReconnect.Checked = Settings.hyperionLiveReconnect;
      ckBoblightEnabled.Checked = Settings.boblightTarget;
      tbBoblightIP.Text = Settings.boblightIP;
      tbBoblightPort.Text = Settings.boblightPort.ToString();
      tbBoblightMaxReconnectAttempts.Text = Settings.boblightMaxReconnectAttempts.ToString();
      tbBoblightReconnectDelay.Text = Settings.boblightReconnectDelay.ToString();
      tbBoblightMaxFPS.Text = Settings.boblightMaxFPS.ToString();
      tbarBoblightSpeed.Value = (int) Settings.boblightSpeed;
      tbarBoblightAutospeed.Value = (int) Settings.boblightAutospeed;
      tbarBoblightSaturation.Value = (int) Settings.boblightSaturation;
      tbarBoblightValue.Value = (int) Settings.boblightValue;
      tbarBoblightThreshold.Value = Settings.boblightThreshold;
      tbarBoblightGamma.Value = (int) (Settings.boblightGamma*10);
      ckBoblightInterpolation.Checked = Settings.boblightInterpolation;
      tbBoblightSpeed.Text = Settings.boblightSpeed.ToString();
      tbBoblightAutospeed.Text = Settings.boblightAutospeed.ToString();
      tbBoblightSaturation.Text = Settings.boblightSaturation.ToString();
      tbBoblightValue.Text = Settings.boblightValue.ToString();
      tbBoblightThreshold.Text = Settings.boblightThreshold.ToString();
      tbBoblightGamma.Text = Settings.boblightGamma.ToString();
      tbBlackbarDetectionThreshold.Text = Settings.blackbarDetectionThreshold.ToString();
      tbpowerModeChangedDelay.Text = Settings.powerModeChangedDelay.ToString();
      ckAmbiBoxEnabled.Checked = Settings.ambiBoxTarget;
      tbAmbiBoxPath.Text = Settings.ambiBoxPath;
      cbAmbiBoxAutoStart.Checked = Settings.ambiBoxAutoStart;
      cbAmbiBoxAutoStop.Checked = Settings.ambiBoxAutoStop;
      tbAmbiBoxIP.Text = Settings.ambiBoxIP;
      tbAmbiBoxPort.Text = Settings.ambiBoxPort.ToString();
      tbAmbiBoxMaxReconnectAttempts.Text = Settings.ambiBoxMaxReconnectAttempts.ToString();
      tbAmbiBoxReconnectDelay.Text = Settings.ambiBoxReconnectDelay.ToString();
      tbAmbiboxChangeImageDelay.Text = Settings.ambiBoxChangeImageDelay.ToString();
      tbAmbiBoxMediaPortalProfile.Text = Settings.ambiBoxMediaPortalProfile;
      tbAmbiBoxExternalProfile.Text = Settings.ambiBoxExternalProfile;
      tbAtmoOrbBlackThreshold.Text = Settings.atmoOrbBlackThreshold.ToString();
      tbAtmoOrbBroadcastPort.Text = Settings.atmoOrbBroadcastPort.ToString();
      tbAtmoOrbGamma.Text = Settings.atmoOrbGamma.ToString();
      tbAtmoOrbMinDiversion.Text = Settings.atmoOrbMinDiversion.ToString();
      tbAtmoOrbSaturation.Text = Settings.atmoOrbSaturation.ToString();
      tbAtmoOrbThreshold.Text = Settings.atmoOrbThreshold.ToString();
      ckAtmoOrbEnabled.Checked = Settings.atmoOrbTarget;
      cbAtmoOrbUseOverallLightness.Checked = Settings.atmoOrbUseOverallLightness;
      cbAtmoOrbUseSmoothing.Checked = Settings.atmoOrbUseSmoothing;
      tbAtmoOrbSmoothingThreshold.Text = Settings.atmoOrbSmoothingThreshold.ToString();

      for (int i = 0; i < Settings.atmoOrbLamps.Count; i++)
      {
        if (!string.IsNullOrEmpty(Settings.atmoOrbLamps[i].Split(',')[0]))
        {
          lbAtmoOrbLamps.Items.Add(Settings.atmoOrbLamps[i].Split(',')[0]);
        }
      }
      tbVUMeterMaxHue.Text = Settings.vuMeterMaxHue.ToString();
      tbVUMeterMindB.Text = Settings.vuMeterMindB.ToString();
      tbVUMeterMinHue.Text = Settings.vuMeterMinHue.ToString();
      tbHueMinDiversion.Text = Settings.hueMinDiversion.ToString();
      tbHueThreshold.Text = Settings.hueThreshold.ToString();
      tbHueBlackThreshold.Text = Settings.hueBlackThreshold.ToString();
      tbHueSaturation.Text = Settings.hueSaturation.ToString();
      cbHueOverallLightness.Checked = Settings.hueUseOverallLightness;
      ckMonitorScreensaverState.Checked = Settings.monitorScreensaverState;
      ckMonitorWindowState.Checked = Settings.monitorWindowState;
      cbBlackbarDetectionLinkAreas.Checked = Settings.blackbarDetectionLinkAreas;
      cbRemoteApiServer.Checked = Settings.remoteApiServer;
      cbBlackbarDetectionHorizontal.Checked = Settings.blackbarDetectionHorizontal;
      cbBlackbarDetectionVertical.Checked = Settings.blackbarDetectionVertical;
    }

    private void UpdateLanguageOnControls()
    {
      // Tabs
      tabPageGeneric.Text = Localization.Translate("Common", "GeneralSettings");
      tabPageAmbiBox.Text = Localization.Translate("AmbiBox", "AmbiBox");
      tabPageAtmoOrb.Text = Localization.Translate("AtmoOrb", "AtmoOrb");
      tabPageAtmowin.Text = Localization.Translate("AtmoWin", "AtmoWin");
      tabPageBoblight.Text = Localization.Translate("Boblight", "Boblight");
      tabPageHue.Text = Localization.Translate("Hue", "Hue");
      tabPageHyperion.Text = Localization.Translate("Hyperion", "Hyperion");

      // Buttons
      btnSave.Text = Localization.Translate("Common", "Save");
      btnCancel.Text = Localization.Translate("Common", "Cancel");
      btnLanguage.Text = Localization.Translate("SetupForm", "LoadLanguage");

      // Targets
      grpTargets.Text = Localization.Translate("SetupForm", "SelectTargets");
      ckAmbiBoxEnabled.Text = Localization.Translate("AmbiBox", "AmbiBox");
      ckAtmoOrbEnabled.Text = Localization.Translate("AtmoOrb", "AtmoOrb");
      ckAtmowinEnabled.Text = Localization.Translate("AtmoWin", "AtmoWin");
      ckBoblightEnabled.Text = Localization.Translate("Boblight", "Boblight");
      ckHueEnabled.Text = Localization.Translate("Hue", "Hue");
      ckHyperionEnabled.Text = Localization.Translate("Hyperion", "Hyperion");

      // Effects
      grpMode.Text = Localization.Translate("SetupForm", "EffectSettings");
      lblVidTvRec.Text = Localization.Translate("SetupForm", "VideoTVRec");
      lblMusic.Text = Localization.Translate("SetupForm", "Music");
      lblRadio.Text = Localization.Translate("SetupForm", "Radio");
      lblMenu.Text = Localization.Translate("SetupForm", "Menu");
      lblMPExit.Text = Localization.Translate("SetupForm", "MediaPortalExit");
      grpGIF.Text = Localization.Translate("ContentEffect", "GIFReader");
      grpStaticColor.Text = Localization.Translate("ContentEffect", "StaticColor");
      lblRed.Text = Localization.Translate("Common", "Red") + ":";
      lblGreen.Text = Localization.Translate("Common", "Green") + ":";
      lblBlue.Text = Localization.Translate("Common", "Blue") + ":";
      grpVUMeter.Text = Localization.Translate("ContentEffect", "VUMeter") + " / " +
                        Localization.Translate("ContentEffect", "VUMeterRainbow");
      lblVUMeterMaxHue.Text = Localization.Translate("SetupForm", "VUMeterMaxHue");
      lblVUMeterMindB.Text = Localization.Translate("SetupForm", "VUMeterMindB");
      lblVUMeterMinHue.Text = Localization.Translate("SetupForm", "VUMeterMinHue");

      // Plugin options
      grpPluginOption.Text = Localization.Translate("SetupForm", "PluginSettings");
      lblLedsOnOff.Text = Localization.Translate("SetupForm", "ToggleRemoteButton");
      lblProfile.Text = Localization.Translate("SetupForm", "ProfileRemoteButton");
      lblMenuButton.Text = Localization.Translate("SetupForm", "MenuRemoteButton");
      ckOnMediaStart.Text = Localization.Translate("SetupForm", "ManualMode");
      ckLowCpu.Text = Localization.Translate("SetupForm", "LowCPU");
      ckDelay.Text = Localization.Translate("SetupForm", "Delay");
      ckRestartOnError.Text = Localization.Translate("SetupForm", "RestartOnError");
      ckTrueGrabbing.Text = Localization.Translate("SetupForm", "TrueGrabbing");
      lblRefreshRate.Text = Localization.Translate("SetupForm", "RefreshRate");
      ckBlackbarDetection.Text = Localization.Translate("SetupForm", "BlackbarDetection");
      lblBlackarDetectionMS.Text = Localization.Translate("SetupForm", "BlackbarDetectionThreshold");
      lblpowerModeChangedDelay.Text = Localization.Translate("SetupForm", "ResumeDelay");
      lblFrames.Text = Localization.Translate("SetupForm", "FramesBetween");
      lblDelay.Text = Localization.Translate("SetupForm", "DelayAt");
      lblpowerModeChangedDelayMS.Text = Localization.Translate("Common", "MS");
      ckMonitorScreensaverState.Text = Localization.Translate("SetupForm", "MonitorScreensaverState");
      ckMonitorWindowState.Text = Localization.Translate("SetupForm", "MonitorWindowState");
      cbBlackbarDetectionLinkAreas.Text = Localization.Translate("SetupForm", "BlackbarDetectionLinkAreas");
      cbRemoteApiServer.Text = Localization.Translate("SetupForm", "RemoteApiServer");
      grpAdvancedOptions.Text = Localization.Translate("SetupForm", "AdvancedOptions");
      cbBlackbarDetectionHorizontal.Text = Localization.Translate("SetupForm", "BlackbarDetectionHorizontal");
      cbBlackbarDetectionVertical.Text = Localization.Translate("SetupForm", "BlackbarDetectionVertical");

      // Capture dimension
      grpCaptureDimensions.Text = Localization.Translate("SetupForm", "CaptureDimension");
      lblCaptureWidth.Text = Localization.Translate("SetupForm", "Width");
      lblCaptureHeight.Text = Localization.Translate("SetupForm", "Height");

      // Deactivate time
      lblStart.Text = Localization.Translate("SetupForm", "Start");
      lblEnd.Text = Localization.Translate("SetupForm", "End");
      grpDeactivate.Text = Localization.Translate("SetupForm", "DisableBetween");

      // Hints
      lblHintMenuButtons.Text = Localization.Translate("SetupForm", "MenuHint");
      lblHintHardware.Text = Localization.Translate("SetupForm", "TargetHint");
      lblHintCaptureDimensions.Text = Localization.Translate("SetupForm", "CaptureDimensionHint");

      // AmbiBox
      lblAmbiBoxExternalProfile.Text = Localization.Translate("AmbiBox", "ExternalProfile");
      lblAmbiBoxIP.Text = Localization.Translate("Common", "IP");
      lblAmbiBoxMaxReconnectAttempts.Text = Localization.Translate("Common", "ReconnectAttempts");
      lblAmbiBoxMediaPortalProfile.Text = Localization.Translate("AmbiBox", "MediaPortalProfile");
      lblAmbiBoxPath.Text = Localization.Translate("Common", "Path").Replace("[Filename]", "AmbiBox.exe");
      lblAmbiBoxPort.Text = Localization.Translate("Common", "Port");
      lblAmbiBoxReconnectDelay.Text = Localization.Translate("Common", "ReconnectDelay");
      lblAmbiboxChangeImageDelay.Text = Localization.Translate("AmbiBox", "ChangeImageDelay");
      cbAmbiBoxAutoStart.Text = Localization.Translate("Common", "StartTargetWithMP")
        .Replace("[Target]", Localization.Translate("AmbiBox", "AmbiBox"));
      cbAmbiBoxAutoStop.Text = Localization.Translate("Common", "StopTargetWithMP")
        .Replace("[Target]", Localization.Translate("AmbiBox", "AmbiBox"));
      grpAmbiBoxLocal.Text = Localization.Translate("Common", "GeneralSettings");
      grpAmbiBoxNetwork.Text = Localization.Translate("Common", "NetworkSettings");

      // AtmoOrb
      lblAtmoOrbBlackThreshold.Text = Localization.Translate("Common", "BlackThreshold");
      lblAtmoOrbBroadcastPort.Text = Localization.Translate("AtmoOrb", "BroadcastPort");
      lblAtmoOrbConnection.Text = Localization.Translate("AtmoOrb", "ConnectionType");
      lblAtmoOrbProtocol.Text = Localization.Translate("AtmoOrb", "Protocol");
      lblAtmoOrbGamma.Text = Localization.Translate("Common", "Gamma");
      lblAtmoOrbHScan.Text = Localization.Translate("AtmoOrb", "HScan");
      lblAtmoOrbHScanTo.Text = Localization.Translate("AtmoOrb", "ScanTo");
      lblAtmoOrbID.Text = Localization.Translate("Common", "ID");
      lblAtmoOrbIP.Text = Localization.Translate("Common", "IP");
      lblAtmoOrbMinDiversion.Text = Localization.Translate("Common", "MinDiversion");
      lblAtmoOrbPort.Text = Localization.Translate("Common", "Port");
      lblAtmoOrbLedCount.Text = Localization.Translate("AtmoOrb", "LedCount");
      lblAtmoOrbSaturation.Text = Localization.Translate("Common", "Saturation");
      lblAtmoOrbThreshold.Text = Localization.Translate("Common", "Threshold");
      lblAtmoOrbVScan.Text = Localization.Translate("AtmoOrb", "VScan");
      lblAtmoOrbVScanTo.Text = Localization.Translate("AtmoOrb", "ScanTo");
      grpAtmoOrbBasicSettings.Text = Localization.Translate("Common", "GeneralSettings");
      grpAtmoOrbLamps.Text = Localization.Translate("AtmoOrb", "LampSettings");
      rbAtmoOrbTCP.Text = Localization.Translate("AtmoOrb", "TCP");
      rbAtmoOrbUDP.Text = Localization.Translate("AtmoOrb", "UDP");
      cbAtmoOrbInvertZone.Text = Localization.Translate("AtmoOrb", "InvertZone");
      cbAtmoOrbUseOverallLightness.Text = Localization.Translate("Common", "OverallLightness");
      cbAtmoOrbUseSmoothing.Text = Localization.Translate("AtmoOrb", "InternalSmoothing");
      btnAtmoOrbAdd.Text = Localization.Translate("Common", "Add");
      btnAtmoOrbRemove.Text = Localization.Translate("Common", "Remove");
      btnAtmoOrbUpdate.Text = Localization.Translate("Common", "Update");
      lblAtmoOrbSmoothingThreshold.Text = Localization.Translate("AtmoOrb", "SmoothingThreshold");

      // AtmoWin
      lblPathInfoAtmoWin.Text = Localization.Translate("Common", "Path").Replace("[Filename]", "AtmoWinA.exe");
      grpAtmowinSettings.Text = Localization.Translate("Common", "GeneralSettings");
      ckStartAtmoWin.Text = Localization.Translate("Common", "StartTargetWithMP")
        .Replace("[Target]", Localization.Translate("AtmoWin", "AtmoWin"));
      ckExitAtmoWin.Text = Localization.Translate("Common", "StopTargetWithMP")
        .Replace("[Target]", Localization.Translate("AtmoWin", "AtmoWin"));
      grpAtmowinWakeHelper.Text = Localization.Translate("AtmoWin", "AtmoWakeHelperDescription");
      ckAtmoWakeHelperEnabled.Text = Localization.Translate("AtmoWin", "AtmoWakeHelperEnabled");
      lblAtmoWakeHelperComPort.Text = Localization.Translate("AtmoWin", "AtmoWakeHelperComPort");
      lblAtmoWakeHelperResumeDelay.Text = Localization.Translate("AtmoWin", "AtmoWakeHelperResumeDelay");
      lblAtmoWakeHelperDisconnectDelay.Text = Localization.Translate("AtmoWin", "AtmoWakeHelperDisconnectDelay");
      lblAtmoWakeHelperConnectDelay.Text = Localization.Translate("AtmoWin", "AtmoWakeHelperConnectDelay");
      lblAtmoWakeHelperReinitializationDelay.Text = Localization.Translate("AtmoWin", "AtmoWakeHelperReinitializationDelay");

      // Boblight
      lblBoblightIP.Text = Localization.Translate("Common", "IP");
      lblBoblightPort.Text = Localization.Translate("Common", "Port");
      lblBoblightMaxReconnectAttempts.Text = Localization.Translate("Common", "ReconnectAttempts");
      lblBoblightReconnectDelay.Text = Localization.Translate("Common", "ReconnectDelay");
      lblBoblightMaxFPS.Text = Localization.Translate("Boblight", "MaxFPS");
      lblBoblightSpeed.Text = Localization.Translate("Boblight", "Speed");
      lblBoblightAutospeed.Text = Localization.Translate("Boblight", "Autospeed");
      lblBoblightSaturation.Text = Localization.Translate("Common", "Saturation");
      lblBoblightValue.Text = Localization.Translate("Boblight", "Value");
      lblBoblightThreshold.Text = Localization.Translate("Common", "Threshold");
      ckBoblightInterpolation.Text = Localization.Translate("Boblight", "Interpolation");
      grpBoblightGeneral.Text = Localization.Translate("Common", "GeneralSettings");
      grpBoblightSettings.Text = Localization.Translate("Boblight", "BoblightSettings");
      lblBoblightGamma.Text = Localization.Translate("Common", "Gamma");

      // Hue
      lblPathInfoHue.Text = Localization.Translate("Common", "Path").Replace("[Filename]", "AtmoHue.exe");
      ckStartHue.Text = Localization.Translate("Common", "StartTargetWithMP")
        .Replace("[Target]", Localization.Translate("Hue", "Hue"));
      ckhueIsRemoteMachine.Text = Localization.Translate("Hue", "RemoteMachine");
      lblHueIP.Text = Localization.Translate("Common", "IP");
      lblHuePort.Text = Localization.Translate("Common", "Port");
      lblHueReconnectDelay.Text = Localization.Translate("Common", "ReconnectDelay");
      lblHueReconnectAttempts.Text = Localization.Translate("Common", "ReconnectAttempts");
      ckHueBridgeEnableOnResume.Text = Localization.Translate("Hue", "EnableBridgeOnResume");
      ckHueBridgeDisableOnSuspend.Text = Localization.Translate("Hue", "DisableBridgeonSuspend");
      ckHueTheaterEnabled.Text = Localization.Translate("Hue", "HueTheaterEnabled");
      ckHueTheaterRestoreLights.Text = Localization.Translate("Hue", "HueTheaterRestoreLights");

      lblHueMinDiversion.Text = Localization.Translate("Common", "MinDiversion");
      lblHueThreshold.Text = Localization.Translate("Common", "Threshold");
      lblHueBlackThreshold.Text = Localization.Translate("Common", "BlackThreshold");
      lblHueSaturation.Text = Localization.Translate("Common", "Saturation");
      cbHueOverallLightness.Text = Localization.Translate("Common", "OverallLightness");
      grpHueAverageColor.Text = Localization.Translate("Hue", "AverageColor");
      grpHueGeneralSettings.Text = Localization.Translate("Common", "GeneralSettings");
      grpHueNetworkSettings.Text = Localization.Translate("Common", "NetworkSettings");
      lblHintHue.Text = Localization.Translate("Hue", "Hint");
      lblHintHueTheaterMode.Text = Localization.Translate("Hue", "HintTheaterMode");

      // Hyperion
      lblHyperionIP.Text = Localization.Translate("Hyperion", "IPHostname");
      lblHyperionPort.Text = Localization.Translate("Common", "Port");
      lblHyperionPriority.Text = Localization.Translate("Hyperion", "PriorityLive");
      lblHyperionReconnectDelay.Text = Localization.Translate("Common", "ReconnectDelay");
      lblHyperionReconnectAttempts.Text = Localization.Translate("Common", "ReconnectAttempts");
      lblHyperionPriorityStaticColor.Text = Localization.Translate("Hyperion", "PriorityStaticColor");
      ckHyperionLiveReconnect.Text = Localization.Translate("Hyperion", "LiveReconnect");
      grpHyperionNetworkSettings.Text = Localization.Translate("Common", "NetworkSettings");
      grpHyperionPrioritySettings.Text = Localization.Translate("Hyperion", "Priority");
    }

    #endregion

    #region General Button Events

    private void btnCancel_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      //Validate user input


      //Time excluded Start
      if (validatorDateTime(edExcludeStart.Text) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorStartTime") + " - [" + lblStart.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Time excluded Stop
      if (validatorDateTime(edExcludeEnd.Text) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorEndTime") + " - [" + lblEnd.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Low CPU
      if (validatorInt(lowCpuTime.Text, 1, 0, false) == false)
      {
        if (ckLowCpu.Checked)
        {
          MessageBox.Show(Localization.Translate("Common", "ErrorMiliseconds") + " - [" + ckLowCpu.Text + "]",
            Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
        else

        {
          //Didn't pass validation so save cleanly with default value even if option isn't used
          lowCpuTime.Text = "0";
        }
      }

      //LED delay
      if (validatorInt(tbDelay.Text, 1, 0, false) == false)
      {
        if (ckDelay.Checked)
        {
          MessageBox.Show(Localization.Translate("Common", "ErrorMiliseconds") + " - [" + ckDelay.Text + "]",
            Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
        else
        {
          //Didn't pass validation so save cleanly with default value even if option isn't used
          tbDelay.Text = "0";
        }
      }

      //Refresh rate
      if (validatorInt(tbRefreshRate.Text, 1, 0, false) == false)
      {
        if (ckDelay.Checked)
        {
          MessageBox.Show(Localization.Translate("Common", "ErrorMiliseconds") + " - [" + lblRefreshRate.Text + "]",
            Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
        else
        {
          //Didn't pass validation so save cleanly with default value even if option isn't used
          tbRefreshRate.Text = "50";
        }
      }

      //Black bar detection
      if (validatorInt(tbBlackbarDetectionTime.Text, 1, 0, false) == false)
      {
        if (ckBlackbarDetection.Checked)
        {
          MessageBox.Show(
            Localization.Translate("Common", "ErrorMiliseconds") + " - [" + ckBlackbarDetection.Text + "]",
            Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
        else
        {
          //Didn't pass validation so save cleanly with default value even if option isn't used
          tbBlackbarDetectionTime.Text = "0";
        }
      }

      //Static color RED
      if (validatorInt(tbRed.Text, 0, 255, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorColor").Replace("[Color]", Localization.Translate("Common", "Red")) +
          " - [" + lblRed.Text + "]", Localization.Translate("Common", "Error"), MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        return;
      }

      //Static color GREEN
      if (validatorInt(tbGreen.Text, 0, 255, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorColor").Replace("[Color]", Localization.Translate("Common", "Green")) +
          " - [" + lblGreen.Text + "]", Localization.Translate("Common", "Error"), MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        return;
      }

      //Static color BLUE
      if (validatorInt(tbBlue.Text, 0, 255, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorColor").Replace("[Color]", Localization.Translate("Common", "Blue")) +
          " - [" + lblBlue.Text + "]", Localization.Translate("Common", "Error"), MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        return;
      }

      //Menu buttons
      if ((cbMenuButton.SelectedIndex == comboBox1.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
          (cbMenuButton.SelectedIndex == comboBox2.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
          (comboBox1.SelectedIndex == comboBox2.SelectedIndex) && (comboBox1.SelectedIndex != 4))
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorRemoteButton"), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //GIF path
      if (validatorPath(tbGIF.Text) == false && string.IsNullOrEmpty(tbGIF.Text) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorFileGeneric") + " - [" + grpGIF.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Atmowin path
      if (validatorPath(edFileAtmoWin.Text) == false && string.IsNullOrEmpty(edFileAtmoWin.Text) == false)
      {
        if (ckAtmowinEnabled.Checked)
        {
          MessageBox.Show(
            Localization.Translate("Common", "ErrorFileGeneric") + " - [" + lblPathInfoAtmoWin.Text + "]",
            Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
      }

      //Hyperion IP
      if (string.IsNullOrEmpty(tbHyperionIP.Text))
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorIP") + " - [" + lblHyperionIP.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      //Hue IP
      if (validatorIPAdress(tbHueIP.Text) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorIP") + " - [" + lblHueIP.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      /*
       * Settings with specific Integer restrictions
       */

      int minValue = 0;
      int maxValue = 0;

      //Capture width
      minValue = 1;
      maxValue = 0;
      if (validatorInt(tbCaptureWidth.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumber").Replace("[minInteger]", minValue.ToString()) + " - [" +
          lblCaptureWidth.Text + "]", Localization.Translate("Common", "Error"), MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        return;
      }

      //Capture height
      minValue = 1;
      maxValue = 0;
      if (validatorInt(tbCaptureHeight.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumber").Replace("[minInteger]", minValue.ToString()) + " - [" +
          lblCaptureHeight.Text + "]", Localization.Translate("Common", "Error"), MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        return;
      }

      //AtmoWin wake helper resume delay
      minValue = 0;
      maxValue = 999999;
      if (validatorInt(tbAtmoWakeHelperResumeDelay.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorInvalidNumberRange").Replace("[minInteger]", minValue.ToString()).Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoWakeHelperResumeDelay.Text + "]", Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //AtmoWin wake helper disconnect delay
      minValue = 0;
      maxValue = 999999;
      if (validatorInt(tbAtmoWakeHelperDisconnectDelay.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorInvalidNumberRange").Replace("[minInteger]", minValue.ToString()).Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoWakeHelperDisconnectDelay.Text + "]", Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //AtmoWin wake helper connect delay
      minValue = 0;
      maxValue = 999999;
      if (validatorInt(tbAtmoWakeHelperConnectDelay.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorInvalidNumberRange").Replace("[minInteger]", minValue.ToString()).Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoWakeHelperConnectDelay.Text + "]", Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //AtmoWin wake helper reconnect delay
      minValue = 0;
      maxValue = 999999;
      if (validatorInt(tbAtmoWakeHelperReinitializationDelay.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorInvalidNumberRange").Replace("[minInteger]", minValue.ToString()).Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoWakeHelperReinitializationDelay.Text + "]", Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      
      //Hyperion port
      minValue = 1;
      maxValue = 65535;
      if (validatorInt(tbHyperionPort.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblHyperionPort.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Hyperion reconnect attempts
      minValue = 1;
      maxValue = 0;
      if (validatorInt(tbHyperionReconnectAttempts.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumber").Replace("[minInteger]", minValue.ToString()) + " - [" +
          lblHyperionReconnectAttempts.Text + "]", Localization.Translate("Common", "Error"), MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        return;
      }

      //Hyperion reconnect delay
      minValue = 100;
      maxValue = 999999;
      if (validatorInt(tbHyperionReconnectDelay.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblHyperionReconnectDelay.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Hyperion priority
      minValue = 1;
      maxValue = 0;
      if (validatorInt(tbHyperionPriority.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumber").Replace("[minInteger]", minValue.ToString()) + " - [" +
          lblHyperionPriority.Text + "]", Localization.Translate("Common", "Error"), MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        return;
      }

      //Hyperion priority static color
      minValue = 1;
      maxValue = 0;
      if (validatorInt(tbHyperionPriorityStaticColor.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumber").Replace("[minInteger]", minValue.ToString()) + " - [" +
          lblHyperionPriorityStaticColor.Text + "]", Localization.Translate("Common", "Error"), MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        return;
      }

      //Hue path
      if (validatorPath(edFileHue.Text) == false && string.IsNullOrEmpty(edFileHue.Text) == false)
      {
        if (ckHueEnabled.Checked)
        {
          MessageBox.Show(Localization.Translate("Common", "ErrorFileGeneric") + " - [" + lblPathInfoHue.Text + "]",
            Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
      }

      //Hue port
      minValue = 1;
      maxValue = 65535;
      if (validatorInt(tbHuePort.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblHueIP.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Hue reconnect attempts
      minValue = 1;
      maxValue = 0;
      if (validatorInt(tbHueReconnectAttempts.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumber").Replace("[minInteger]", minValue.ToString()) + " - [" +
          lblHueReconnectAttempts.Text + "]", Localization.Translate("Common", "Error"), MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        return;
      }

      //Hue reconnect delay
      minValue = 100;
      maxValue = 999999;
      if (validatorInt(tbHueReconnectDelay.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblHueReconnectDelay.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // Boblight IP
      if (validatorIPAdress(tbBoblightIP.Text) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorIP") + " - [" + tbBoblightIP.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // Boblight Port
      minValue = 1;
      maxValue = 65535;
      if (validatorInt(tbBoblightPort.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblBoblightPort.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // Boblight MaxReconnectAttempts
      minValue = 1;
      maxValue = 9999;
      if (validatorInt(tbBoblightMaxReconnectAttempts.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblBoblightMaxReconnectAttempts.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // Boblight ReconnectDelay
      minValue = 100;
      maxValue = 999999;
      if (validatorInt(tbBoblightReconnectDelay.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblBoblightSaturation.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // Boblight MaxFPS
      minValue = 1;
      maxValue = 144;
      if (validatorInt(tbBoblightMaxFPS.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblBoblightMaxFPS.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // Blackbar Detection Threshold
      minValue = 0;
      maxValue = 255;
      if (validatorInt(tbBlackbarDetectionThreshold.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + ckBlackbarDetection.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // Power mode change delay
      minValue = 0;
      maxValue = 999999;
      if (validatorInt(tbpowerModeChangedDelay.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblpowerModeChangedDelay.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // AmbiBox IP
      if (validatorIPAdress(tbAmbiBoxIP.Text) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorIP") + " - [" + tbAmbiBoxIP.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // AmbiBox Port
      minValue = 1;
      maxValue = 65535;
      if (validatorInt(tbAmbiBoxPort.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAmbiBoxPort.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // AmbiBox Reconnect Attempts
      minValue = 1;
      maxValue = 9999;
      if (validatorInt(tbAmbiBoxMaxReconnectAttempts.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAmbiBoxMaxReconnectAttempts.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // AmbiBox Reconnect Delay
      minValue = 1;
      maxValue = 99999;
      if (validatorInt(tbAmbiBoxReconnectDelay.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAmbiBoxReconnectDelay.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // AmbiBox Change Image Delay
      minValue = 0;
      maxValue = 99999;
      if (validatorInt(tbAmbiboxChangeImageDelay.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAmbiboxChangeImageDelay.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // AtmoOrb Broadcast Port
      minValue = 1;
      maxValue = 65535;
      if (validatorInt(tbAtmoOrbBroadcastPort.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoOrbBroadcastPort.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // AtmoOrb Min Diversion
      minValue = 0;
      maxValue = 255;
      if (validatorInt(tbAtmoOrbMinDiversion.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoOrbMinDiversion.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // AtmoOrb Threshold
      minValue = 0;
      maxValue = 255;
      if (validatorInt(tbAtmoOrbThreshold.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoOrbThreshold.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // AtmoOrb Black Threshold
      minValue = 0;
      maxValue = 255;
      if (validatorInt(tbAtmoOrbBlackThreshold.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoOrbBlackThreshold.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // AtmoOrb Saturation
      double doubleMinValue = -1.0;
      double doubleMaxValue = 1.0;
      if (validatorDouble(tbAtmoOrbSaturation.Text, doubleMinValue, doubleMaxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", doubleMinValue.ToString())
            .Replace("[maxInteger]", doubleMaxValue.ToString()) + " - [" + lblAtmoOrbSaturation.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // AtmoOrb Gamma
      doubleMinValue = 0.0;
      doubleMaxValue = 5.0;
      if (validatorDouble(tbAtmoOrbGamma.Text, doubleMinValue, doubleMaxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", doubleMinValue.ToString())
            .Replace("[maxInteger]", doubleMaxValue.ToString()) + " - [" + lblAtmoOrbGamma.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // AtmoOrb SMoothing Threshold
      minValue = 0;
      maxValue = 255;
      if (validatorInt(tbAtmoOrbSmoothingThreshold.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + tbAtmoOrbSmoothingThreshold.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // VUMeter Min dB
      minValue = -100;
      maxValue = 0;
      if (validatorInt(tbVUMeterMindB.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblVUMeterMindB.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // VUMeter Min Hue
      doubleMinValue = 0.0;
      doubleMaxValue = 1.0;
      if (validatorDouble(tbVUMeterMinHue.Text, doubleMinValue, doubleMaxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", doubleMinValue.ToString())
            .Replace("[maxInteger]", doubleMaxValue.ToString()) + " - [" + lblVUMeterMinHue.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // VUMeter Max Hue
      doubleMinValue = 0.0;
      doubleMaxValue = 1.0;
      if (validatorDouble(tbVUMeterMaxHue.Text, doubleMinValue, doubleMaxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", doubleMinValue.ToString())
            .Replace("[maxInteger]", doubleMaxValue.ToString()) + " - [" + lblVUMeterMaxHue.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // Hue Min Diversion
      minValue = 0;
      maxValue = 255;
      if (validatorInt(tbHueMinDiversion.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblHueMinDiversion.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // Hue Threshold
      minValue = 0;
      maxValue = 255;
      if (validatorInt(tbHueThreshold.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblHueThreshold.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      // Hue Black Threshold
      minValue = 0;
      maxValue = 255;
      if (validatorInt(tbHueBlackThreshold.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblHueBlackThreshold.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Hue Saturation
      doubleMinValue = -1.0;
      doubleMaxValue = 1.0;
      if (validatorDouble(tbHueSaturation.Text, doubleMinValue, doubleMaxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", doubleMinValue.ToString())
            .Replace("[maxInteger]", doubleMaxValue.ToString()) + " - [" + lblHueSaturation.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      Settings.staticColorRed = int.Parse(tbRed.Text);
      Settings.staticColorGreen = int.Parse(tbGreen.Text);
      Settings.staticColorBlue = int.Parse(tbBlue.Text);
      Settings.atmowinExe = edFileAtmoWin.Text;
      Settings.excludeTimeStart = DateTime.Parse(edExcludeStart.Text);
      Settings.excludeTimeEnd = DateTime.Parse(edExcludeEnd.Text);
      Settings.killButton = comboBox1.SelectedIndex;
      Settings.profileButton = comboBox2.SelectedIndex;
      Settings.menuButton = cbMenuButton.SelectedIndex;
      Settings.manualMode = ckOnMediaStart.Checked;
      Settings.lowCPU = ckLowCpu.Checked;
      Settings.lowCPUTime = int.Parse(lowCpuTime.Text);
      Settings.delay = ckDelay.Checked;
      Settings.startAtmoWin = ckStartAtmoWin.Checked;
      Settings.exitAtmoWin = ckExitAtmoWin.Checked;
      Settings.atmoWakeHelperEnabled = ckAtmoWakeHelperEnabled.Checked;
      Settings.atmoWakeHelperComPort = cbAtmoWakeHelperComPort.Text;
      Settings.atmoWakeHelperResumeDelay = int.Parse(tbAtmoWakeHelperResumeDelay.Text);
      Settings.atmoWakeHelperDisconnectDelay = int.Parse(tbAtmoWakeHelperDisconnectDelay.Text);
      Settings.atmoWakeHelperConnectDelay = int.Parse(tbAtmoWakeHelperConnectDelay.Text);
      Settings.atmoWakeHelperReinitializationDelay = int.Parse(tbAtmoWakeHelperReinitializationDelay.Text);
      Settings.restartOnError = ckRestartOnError.Checked;
      Settings.trueGrabbing = ckTrueGrabbing.Checked;
      Settings.blackbarDetection = ckBlackbarDetection.Checked;
      Settings.blackbarDetectionTime = int.Parse(tbBlackbarDetectionTime.Text);
      Settings.delayReferenceRefreshRate = int.Parse(tbRefreshRate.Text);
      Settings.delayReferenceTime = int.Parse(tbDelay.Text);
      Settings.gifFile = tbGIF.Text;
      Settings.hyperionIP = tbHyperionIP.Text;
      Settings.hyperionPort = int.Parse(tbHyperionPort.Text);
      Settings.hyperionPriority = int.Parse(tbHyperionPriority.Text);
      Settings.hyperionReconnectDelay = int.Parse(tbHyperionReconnectDelay.Text);
      Settings.hyperionReconnectAttempts = int.Parse(tbHyperionReconnectAttempts.Text);
      Settings.hyperionPriorityStaticColor = int.Parse(tbHyperionPriorityStaticColor.Text);
      Settings.hyperionLiveReconnect = ckHyperionLiveReconnect.Checked;
      Settings.captureWidth = int.Parse(tbCaptureWidth.Text);
      Settings.captureHeight = int.Parse(tbCaptureHeight.Text);
      Settings.hueExe = edFileHue.Text;
      Settings.hueStart = ckStartHue.Checked;
      Settings.hueIsRemoteMachine = ckhueIsRemoteMachine.Checked;
      Settings.hueIP = tbHueIP.Text;
      Settings.huePort = int.Parse(tbHuePort.Text);
      Settings.hueReconnectDelay = int.Parse(tbHueReconnectDelay.Text);
      Settings.hueReconnectAttempts = int.Parse(tbHueReconnectAttempts.Text);
      Settings.hueBridgeEnableOnResume = ckHueBridgeEnableOnResume.Checked;
      Settings.hueBridgeDisableOnSuspend = ckHueBridgeDisableOnSuspend.Checked;
      Settings.hueTheaterEnabled = ckHueTheaterEnabled.Checked;
      Settings.hueTheaterRestoreLights = ckHueTheaterRestoreLights.Checked;
      Settings.atmoWinTarget = ckAtmowinEnabled.Checked;
      Settings.hueTarget = ckHueEnabled.Checked;
      Settings.hyperionTarget = ckHyperionEnabled.Checked;
      Settings.boblightTarget = ckBoblightEnabled.Checked;
      Settings.boblightIP = tbBoblightIP.Text;
      Settings.boblightPort = int.Parse(tbBoblightPort.Text);
      Settings.boblightMaxReconnectAttempts = int.Parse(tbBoblightMaxReconnectAttempts.Text);
      Settings.boblightReconnectDelay = int.Parse(tbBoblightReconnectDelay.Text);
      Settings.boblightMaxFPS = int.Parse(tbBoblightMaxFPS.Text);
      Settings.boblightSpeed = tbarBoblightSpeed.Value;
      Settings.boblightAutospeed = tbarBoblightAutospeed.Value;
      Settings.boblightSaturation = tbarBoblightSaturation.Value;
      Settings.boblightValue = tbarBoblightValue.Value;
      Settings.boblightThreshold = tbarBoblightThreshold.Value;
      Settings.boblightInterpolation = ckBoblightInterpolation.Checked;
      Settings.boblightGamma = (double) tbarBoblightGamma.Value/10;
      Settings.blackbarDetectionThreshold = int.Parse(tbBlackbarDetectionThreshold.Text);
      Settings.powerModeChangedDelay = int.Parse(tbpowerModeChangedDelay.Text);
      Settings.ambiBoxTarget = ckAmbiBoxEnabled.Checked;
      Settings.ambiBoxPath = tbAmbiBoxPath.Text;
      Settings.ambiBoxAutoStart = cbAmbiBoxAutoStart.Checked;
      Settings.ambiBoxAutoStop = cbAmbiBoxAutoStop.Checked;
      Settings.ambiBoxIP = tbAmbiBoxIP.Text;
      Settings.ambiBoxPort = int.Parse(tbAmbiBoxPort.Text);
      Settings.ambiBoxMaxReconnectAttempts = int.Parse(tbAmbiBoxMaxReconnectAttempts.Text);
      Settings.ambiBoxReconnectDelay = int.Parse(tbAmbiBoxReconnectDelay.Text);
      Settings.ambiBoxChangeImageDelay = int.Parse(tbAmbiboxChangeImageDelay.Text);
      Settings.ambiBoxMediaPortalProfile = tbAmbiBoxMediaPortalProfile.Text;
      Settings.ambiBoxExternalProfile = tbAmbiBoxExternalProfile.Text;
      Settings.atmoOrbBlackThreshold = int.Parse(tbAtmoOrbBlackThreshold.Text);
      Settings.atmoOrbBroadcastPort = int.Parse(tbAtmoOrbBroadcastPort.Text);
      Settings.atmoOrbGamma = Double.Parse(tbAtmoOrbGamma.Text.Replace(",", "."),
        CultureInfo.InvariantCulture.NumberFormat);
      Settings.atmoOrbMinDiversion = int.Parse(tbAtmoOrbMinDiversion.Text);
      Settings.atmoOrbSaturation = Double.Parse(tbAtmoOrbSaturation.Text.Replace(",", "."),
        CultureInfo.InvariantCulture.NumberFormat);
      Settings.atmoOrbThreshold = int.Parse(tbAtmoOrbThreshold.Text);
      Settings.atmoOrbUseOverallLightness = cbAtmoOrbUseOverallLightness.Checked;
      Settings.atmoOrbUseSmoothing = cbAtmoOrbUseSmoothing.Checked;
      Settings.atmoOrbSmoothingThreshold = int.Parse(tbAtmoOrbSmoothingThreshold.Text);

      Settings.atmoOrbTarget = ckAtmoOrbEnabled.Checked;
      Settings.vuMeterMaxHue = Double.Parse(tbVUMeterMaxHue.Text.Replace(",", "."),
        CultureInfo.InvariantCulture.NumberFormat);
      Settings.vuMeterMindB = int.Parse(tbVUMeterMindB.Text);
      Settings.vuMeterMinHue = Double.Parse(tbVUMeterMinHue.Text.Replace(",", "."),
        CultureInfo.InvariantCulture.NumberFormat);
      Settings.hueMinDiversion = int.Parse(tbHueMinDiversion.Text);
      Settings.hueThreshold = int.Parse(tbHueThreshold.Text);
      Settings.hueBlackThreshold = int.Parse(tbHueBlackThreshold.Text);
      Settings.hueSaturation = Double.Parse(tbHueSaturation.Text.Replace(",", "."),
        CultureInfo.InvariantCulture.NumberFormat);
      Settings.hueUseOverallLightness = cbHueOverallLightness.Checked;
      Settings.monitorScreensaverState = ckMonitorScreensaverState.Checked;
      Settings.monitorWindowState = ckMonitorWindowState.Checked;
      Settings.blackbarDetectionLinkAreas = cbBlackbarDetectionLinkAreas.Checked;
      Settings.remoteApiServer = cbRemoteApiServer.Checked;
      Settings.blackbarDetectionHorizontal = cbBlackbarDetectionHorizontal.Checked;
      Settings.blackbarDetectionVertical = cbBlackbarDetectionVertical.Checked;

      Settings.effectVideo =
        (ContentEffect) Enum.Parse(typeof (ContentEffect), Localization.ReverseTranslate("ContentEffect", cbVideo.Text));
      Settings.effectMusic =
        (ContentEffect) Enum.Parse(typeof (ContentEffect), Localization.ReverseTranslate("ContentEffect", cbMusic.Text));
      Settings.effectRadio =
        (ContentEffect) Enum.Parse(typeof (ContentEffect), Localization.ReverseTranslate("ContentEffect", cbRadio.Text));
      Settings.effectMenu =
        (ContentEffect) Enum.Parse(typeof (ContentEffect), Localization.ReverseTranslate("ContentEffect", cbMenu.Text));
      Settings.effectMPExit =
        (ContentEffect)
          Enum.Parse(typeof (ContentEffect), Localization.ReverseTranslate("ContentEffect", cbMPExit.Text));

      if (validatorString(cbMusic.Text, 1))
      {
        Settings.effectMusic = (ContentEffect)Enum.Parse(typeof(ContentEffect), Localization.ReverseTranslate("ContentEffect", cbMusic.Text));
      }

      if (validatorString(cbRadio.Text, 1))
      {
        Settings.effectRadio = (ContentEffect)Enum.Parse(typeof(ContentEffect), Localization.ReverseTranslate("ContentEffect", cbRadio.Text));
      }

      if (validatorString(cbMenu.Text, 1))
      {
        Settings.effectMenu = (ContentEffect)Enum.Parse(typeof(ContentEffect), Localization.ReverseTranslate("ContentEffect", cbMenu.Text));
      }

      if (validatorString(cbMPExit.Text, 1))
      {
        Settings.effectMPExit = (ContentEffect)Enum.Parse(typeof(ContentEffect), Localization.ReverseTranslate("ContentEffect", cbMPExit.Text));
      }

      Settings.SaveSettings();
      this.DialogResult = DialogResult.OK;
    }

    private void btnLanguage_Click(object sender, EventArgs e)
    {
      openFileDialog2.InitialDirectory = Path.GetDirectoryName(Settings.currentLanguageFile);
      if (openFileDialog2.ShowDialog() == DialogResult.OK)
      {
        Settings.currentLanguageFile = openFileDialog2.FileName;

        Localization.Load(Settings.currentLanguageFile);

        UpdateLanguageOnControls();
        UpdateEffectComboBoxes();
        UpdateRemoteButtonComboBoxes();
        openFileDialog2.FileName = "";
      }
    }

    private void btnSelectGIF_Click(object sender, EventArgs e)
    {
      if (openFileDialog3.ShowDialog() == DialogResult.OK)
      {
        tbGIF.Text = openFileDialog3.FileName;
      }
    }

    #endregion

    #region Input Validators

    private Boolean validatorInt(string input, int minValue, int maxValue, Boolean validateMaxValue)
    {
      Boolean IsValid = false;
      Int32 value;
      bool IsInteger = Int32.TryParse(input, out value);

      if (IsInteger)
      {
        //Only check minValue
        if (validateMaxValue == false && value >= minValue)
        {
          IsValid = true;
        }
        //Check both min/max values
        else
        {
          if (value >= minValue && value <= maxValue)
          {
            IsValid = true;
          }
        }
      }
      return IsValid;
    }

    private bool validatorDouble(string input, double minValue, double maxValue, bool validateMaxValue)
    {
      double value;

      if (!double.TryParse(input, out value))
      {
        return false;
      }

      if ((value >= minValue && value <= maxValue && validateMaxValue) || (value >= minValue && !validateMaxValue))
      {
        return true;
      }
      return false;
    }

    private Boolean validatorIPAdress(string input)
    {
      Boolean IsValid = false;

      System.Net.IPAddress address;
      if (System.Net.IPAddress.TryParse(input, out address))
      {
        switch (address.AddressFamily)
        {
          case System.Net.Sockets.AddressFamily.InterNetwork:
            // we have IPv4
            IsValid = true;
            break;
          case System.Net.Sockets.AddressFamily.InterNetworkV6:
            // we have IPv6
            break;
          default:
            // do something else
            break;
        }
      }
      return IsValid;
    }

    private Boolean validatorDateTime(string input)
    {
      DateTime dt;
      Boolean IsValid = false;
      bool isDateTime = DateTime.TryParse(input, out dt);
      if (isDateTime)
      {
        IsValid = true;
      }

      return IsValid;
    }

    private Boolean validatorPath(string input)
    {
      Boolean IsValid = false;

      try
      {
        if (File.Exists(input))
        {
          IsValid = true;
        }
      }
      catch
      {
      }
      ;

      return IsValid;
    }

    private Boolean validatorRGB(string color, string range)
    {

      Boolean IsValid = false;
      return IsValid;
    }
    private Boolean validatorString(string input, double minLength)
    {
      Boolean IsValid = false;

      try
      {
        if (String.IsNullOrEmpty(input) == false && input.Length >= minLength)
        {
          IsValid = true;
        }
      }
      catch { };

      return IsValid;
    }
    #endregion

    #region Validation Events

    private void lowCpuTime_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 0;
      if (validatorInt(lowCpuTime.Text, minValue, maxValue, false) == false)
      {
        if (ckLowCpu.Checked)
        {
          MessageBox.Show(Localization.Translate("Common", "ErrorMiliseconds"),
            Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }

    private void tbDelay_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 0;
      if (validatorInt(tbDelay.Text, minValue, maxValue, false) == false)
      {
        if (ckDelay.Checked)
        {
          MessageBox.Show(Localization.Translate("Common", "ErrorMiliseconds"),
            Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }

    private void tbRefreshRate_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 0;
      if (validatorInt(tbRefreshRate.Text, minValue, maxValue, false) == false)
      {
        if (ckDelay.Checked)
        {
          MessageBox.Show(Localization.Translate("Common", "ErrorRefreshRate"),
            Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }

    private void tbBlackbarDetectionTime_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 0;
      if (validatorInt(tbBlackbarDetectionTime.Text, minValue, maxValue, false) == false)
      {
        if (ckBlackbarDetection.Checked)
        {
          MessageBox.Show(Localization.Translate("Common", "ErrorMiliseconds"),
            Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }

    private void tbCaptureWidth_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 0;
      if (validatorInt(tbCaptureWidth.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumber").Replace("[minInteger]", minValue.ToString()),
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbCaptureHeight_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 0;
      if (validatorInt(tbCaptureHeight.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumber").Replace("[minInteger]", minValue.ToString()),
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void edExcludeStart_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (validatorDateTime(edExcludeStart.Text) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorStartTime"), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

    }

    private void edExcludeEnd_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (validatorDateTime(edExcludeEnd.Text) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorEndTime"), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }


    private void tbRed_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 255;
      if (validatorInt(tbRed.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorColor").Replace("[Color]", Localization.Translate("Common", "Red")),
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

    }

    private void tbGreen_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 255;
      if (validatorInt(tbGreen.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorColor").Replace("[Color]", Localization.Translate("Common", "Green")),
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbBlue_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 255;
      if (validatorInt(tbBlue.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorColor").Replace("[Color]", Localization.Translate("Common", "Blue")),
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbGIF_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (validatorPath(tbGIF.Text) == false && string.IsNullOrEmpty(tbGIF.Text) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorFileGeneric"), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void cbMenuButton_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if ((cbMenuButton.SelectedIndex == comboBox1.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
          (cbMenuButton.SelectedIndex == comboBox2.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
          (comboBox1.SelectedIndex == comboBox2.SelectedIndex) && (comboBox1.SelectedIndex != 4))
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorRemoteButton"), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

    }

    private void comboBox1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if ((cbMenuButton.SelectedIndex == comboBox1.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
          (cbMenuButton.SelectedIndex == comboBox2.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
          (comboBox1.SelectedIndex == comboBox2.SelectedIndex) && (comboBox1.SelectedIndex != 4))
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorRemoteButton"), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

    }

    private void comboBox2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if ((cbMenuButton.SelectedIndex == comboBox1.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
          (cbMenuButton.SelectedIndex == comboBox2.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
          (comboBox1.SelectedIndex == comboBox2.SelectedIndex) && (comboBox1.SelectedIndex != 4))
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorRemoteButton"), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

    }

    // AtmoWin

    private void edFile_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (validatorPath(edFileAtmoWin.Text) == false && string.IsNullOrEmpty(edFileAtmoWin.Text) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorFileGeneric"), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
    private void tbAtmoWakeHelperResumeDelay_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 999999;
      if (validatorInt(tbAtmoWakeHelperResumeDelay.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorMiliseconds"), Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }


    private void tbAtmoWakeHelperDisconnectDelay_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 999999;
      if (validatorInt(tbAtmoWakeHelperDisconnectDelay.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorMiliseconds"), Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbAtmoWakeHelperConnectDelay_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 999999;
      if (validatorInt(tbAtmoWakeHelperConnectDelay.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorMiliseconds"), Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbAtmoWakeHelperReinitializationDelay_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 999999;
      if (validatorInt(tbAtmoWakeHelperReinitializationDelay.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorMiliseconds"), Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    // Hyperion

    private void tbHyperionIP_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (string.IsNullOrEmpty(tbHyperionIP.Text))
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorIP"), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbHyperionPort_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 65535;
      if (validatorInt(tbHyperionPort.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbHyperionReconnectDelay_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 10;
      int maxValue = 999999;
      if (validatorInt(tbHyperionReconnectDelay.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumber").Replace("[minInteger]", minValue.ToString()),
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

    }

    private void tbHyperionReconnectAttempts_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 0;
      if (validatorInt(tbHyperionReconnectAttempts.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumber").Replace("[minInteger]", minValue.ToString()),
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbHyperionPriority_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 0;
      if (validatorInt(tbHyperionPriority.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumber").Replace("[minInteger]", minValue.ToString()),
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbHyperionPriorityStaticColor_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 0;
      if (validatorInt(tbHyperionPriorityStaticColor.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumber").Replace("[minInteger]", minValue.ToString()),
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbHueIP_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (validatorIPAdress(tbHueIP.Text) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorIP"), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbHuePort_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 65535;
      if (validatorInt(tbHuePort.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbHueReconnectDelay_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 100;
      int maxValue = 999999;
      if (validatorInt(tbHueReconnectDelay.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbHueReconnectAttempts_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 9999;
      if (validatorInt(tbHueReconnectAttempts.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    // Boblight
    private void tbBoblightIP_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (validatorIPAdress(tbBoblightIP.Text) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorIP"), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbBoblightPort_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 65535;
      if (validatorInt(tbBoblightPort.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbBoblightMaxReconnectAttempts_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 9999;
      if (validatorInt(tbBoblightMaxReconnectAttempts.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbBoblightReconnectDelay_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 100;
      int maxValue = 999999;
      if (validatorInt(tbBoblightReconnectDelay.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbBoblightMaxFPS_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 144;
      if (validatorInt(tbBoblightMaxFPS.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbBlackbarDetectionThreshold_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 255;
      if (validatorInt(tbBlackbarDetectionThreshold.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbpowerModeChangedDelay_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 999999;
      if (validatorInt(tbpowerModeChangedDelay.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbarBoblightSpeed_ValueChanged(Object sender, EventArgs e)
    {
      tbBoblightSpeed.Text = tbarBoblightSpeed.Value.ToString();
    }

    private void tbarBoblightAutospeed_ValueChanged(Object sender, EventArgs e)
    {
      tbBoblightAutospeed.Text = tbarBoblightAutospeed.Value.ToString();
    }

    private void tbarBoblightSaturation_ValueChanged(Object sender, EventArgs e)
    {
      tbBoblightSaturation.Text = tbarBoblightSaturation.Value.ToString();
    }

    private void tbarBoblightValue_ValueChanged(Object sender, EventArgs e)
    {
      tbBoblightValue.Text = tbarBoblightValue.Value.ToString();
    }

    private void tbarBoblightThreshold_ValueChanged(Object sender, EventArgs e)
    {
      tbBoblightThreshold.Text = tbarBoblightThreshold.Value.ToString();
    }

    private void tbarBoblightGamma_ValueChanged(Object sender, EventArgs e)
    {
      tbBoblightGamma.Text = ((double) tbarBoblightGamma.Value/10).ToString();
    }

    // AmbiBox
    private void tbAmbiBoxIP_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (validatorIPAdress(tbAmbiBoxIP.Text) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorIP"), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbAmbiBoxPort_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 65535;
      if (validatorInt(tbAmbiBoxPort.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbAmbiBoxMaxReconnectAttempts_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 9999;
      if (validatorInt(tbAmbiBoxMaxReconnectAttempts.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbAmbiBoxReconnectDelay_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 99999;
      if (validatorInt(tbAmbiBoxReconnectDelay.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    // AtmoOrb
    private void tbAtmoOrbVScanEnd_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 100;
      if (validatorInt(tbAtmoOrbVScanEnd.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbAtmoOrbVScanStart_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 100;
      if (validatorInt(tbAtmoOrbVScanStart.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbAtmoOrbHScanEnd_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 100;
      if (validatorInt(tbAtmoOrbHScanEnd.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbAtmoOrbHScanStart_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 100;
      if (validatorInt(tbAtmoOrbHScanStart.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbAtmoOrbPort_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (rbAtmoOrbUDP.Checked)
      {
        return;
      }
      int minValue = 1;
      int maxValue = 65535;
      if (validatorInt(tbAtmoOrbPort.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbAtmoOrbIP_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (rbAtmoOrbUDP.Checked)
      {
        return;
      }
      if (validatorIPAdress(tbAtmoOrbIP.Text) == false)
      {
        MessageBox.Show(Localization.Translate("Common", "ErrorIP"), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbAtmoOrbGamma_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      double minValue = 0.0;
      double maxValue = 5.0;
      if (validatorDouble(tbAtmoOrbGamma.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbAtmoOrbSaturation_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      double minValue = -1.0;
      double maxValue = 1.0;
      if (validatorDouble(tbAtmoOrbSaturation.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbAtmoOrbBlackThreshold_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 255;
      if (validatorInt(tbAtmoOrbBlackThreshold.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbAtmoOrbThreshold_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 255;
      if (validatorInt(tbAtmoOrbThreshold.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbAtmoOrbMinDiversion_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 255;
      if (validatorInt(tbAtmoOrbMinDiversion.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbAtmoOrbBroadcastPort_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 65535;
      if (validatorInt(tbAtmoOrbBroadcastPort.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbVUMeterMindB_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = -100;
      int maxValue = 0;
      if (validatorInt(tbVUMeterMindB.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbVUMeterMinHue_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      double minValue = 0;
      double maxValue = 1.0;
      if (validatorDouble(tbVUMeterMinHue.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbVUMeterMaxHue_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      double minValue = 0;
      double maxValue = 1.0;
      if (validatorDouble(tbVUMeterMaxHue.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbHueSaturation_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      double minValue = -1.0;
      double maxValue = 1.0;
      if (validatorDouble(tbHueSaturation.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbHueMinDiversion_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 255;
      if (validatorDouble(tbHueMinDiversion.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbHueThreshold_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 255;
      if (validatorDouble(tbHueThreshold.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbHueBlackThreshold_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 255;
      if (validatorDouble(tbHueBlackThreshold.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()), Localization.Translate("Common", "Error"),
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    #endregion

    #region Dynamic Effect ComboBoxes

    public void UpdateEffectComboBoxes()
    {
      List<ContentEffect> supportedEffects = coreObject.GetSupportedEffects();

      cbVideo.Items.Clear();
      cbMusic.Items.Clear();
      cbRadio.Items.Clear();
      cbMenu.Items.Clear();
      cbMPExit.Items.Clear();

      foreach (ContentEffect effect in Enum.GetValues(typeof (ContentEffect)))
      {
        if (supportedEffects.Contains(effect) && effect != ContentEffect.Undefined)
        {
          // Cases in which all effects are possible
          cbMusic.Items.Add(Localization.Translate("ContentEffect", effect.ToString()));
          cbRadio.Items.Add(Localization.Translate("ContentEffect", effect.ToString()));

          // Cases in which VU Meter is not possible
          if (effect != ContentEffect.VUMeter && effect != ContentEffect.VUMeterRainbow)
          {
            cbVideo.Items.Add(Localization.Translate("ContentEffect", effect.ToString()));
            cbMenu.Items.Add(Localization.Translate("ContentEffect", effect.ToString()));

            // Cases in which Vu Meter, MPLiveView and GifReader are not possible
            if (effect != ContentEffect.MediaPortalLiveMode && effect != ContentEffect.GIFReader)
            {
              cbMPExit.Items.Add(Localization.Translate("ContentEffect", effect.ToString()));
            }
          }
        }
      }

      cbVideo.Text = Localization.Translate("ContentEffect", Settings.effectVideo.ToString());
      cbMusic.Text = Localization.Translate("ContentEffect", Settings.effectMusic.ToString());
      cbRadio.Text = Localization.Translate("ContentEffect", Settings.effectRadio.ToString());
      cbMenu.Text = Localization.Translate("ContentEffect", Settings.effectMenu.ToString());
      cbMPExit.Text = Localization.Translate("ContentEffect", Settings.effectMPExit.ToString());
    }

    private void ckAtmowinEnabled_CheckedChanged(Object sender, EventArgs e)
    {
      if (ckAtmowinEnabled.Checked)
      {
        coreObject.AddTarget(Target.AtmoWin);
      }
      else
      {
        coreObject.RemoveTarget(Target.AtmoWin);
      }
      UpdateEffectComboBoxes();
    }

    private void ckBoblightEnabled_CheckedChanged(Object sender, EventArgs e)
    {
      if (ckBoblightEnabled.Checked)
      {
        coreObject.AddTarget(Target.Boblight);
      }
      else
      {
        coreObject.RemoveTarget(Target.Boblight);
      }
      UpdateEffectComboBoxes();
    }

    private void ckHyperionEnabled_CheckedChanged(Object sender, EventArgs e)
    {
      if (ckHyperionEnabled.Checked)
      {
        coreObject.AddTarget(Target.Hyperion);
      }
      else
      {
        coreObject.RemoveTarget(Target.Hyperion);
      }
      UpdateEffectComboBoxes();
    }

    private void ckHueEnabled_CheckedChanged(Object sender, EventArgs e)
    {
      if (ckHueEnabled.Checked)
      {
        coreObject.AddTarget(Target.Hue);
      }
      else
      {
        coreObject.RemoveTarget(Target.Hue);
      }
      UpdateEffectComboBoxes();
    }

    private void ckAmbiBoxEnabled_CheckedChanged(Object sender, EventArgs e)
    {
      if (ckAmbiBoxEnabled.Checked)
      {
        coreObject.AddTarget(Target.AmbiBox);
      }
      else
      {
        coreObject.RemoveTarget(Target.AmbiBox);
      }
      UpdateEffectComboBoxes();
    }

    private void ckAtmoOrbEnabled_CheckedChanged(Object sender, EventArgs e)
    {
      if (ckAtmoOrbEnabled.Checked)
      {
        coreObject.AddTarget(Target.AtmoOrb);
      }
      else
      {
        coreObject.RemoveTarget(Target.AtmoOrb);
      }
      UpdateEffectComboBoxes();
    }

    #endregion

    #region Remote Button ComboBoxes

    public void UpdateRemoteButtonComboBoxes()
    {
      comboBox1.Items.Clear();
      comboBox2.Items.Clear();
      cbMenuButton.Items.Clear();

      comboBox1.Items.Add(Localization.Translate("Common", "Red"));
      comboBox2.Items.Add(Localization.Translate("Common", "Red"));
      cbMenuButton.Items.Add(Localization.Translate("Common", "Red"));

      comboBox1.Items.Add(Localization.Translate("Common", "Green"));
      comboBox2.Items.Add(Localization.Translate("Common", "Green"));
      cbMenuButton.Items.Add(Localization.Translate("Common", "Green"));

      comboBox1.Items.Add(Localization.Translate("Common", "Yellow"));
      comboBox2.Items.Add(Localization.Translate("Common", "Yellow"));
      cbMenuButton.Items.Add(Localization.Translate("Common", "Yellow"));

      comboBox1.Items.Add(Localization.Translate("Common", "Blue"));
      comboBox2.Items.Add(Localization.Translate("Common", "Blue"));
      cbMenuButton.Items.Add(Localization.Translate("Common", "Blue"));

      comboBox1.Items.Add(Localization.Translate("Common", "None"));
      comboBox2.Items.Add(Localization.Translate("Common", "None"));
      cbMenuButton.Items.Add(Localization.Translate("Common", "None"));

      comboBox1.SelectedIndex = Settings.killButton;
      comboBox2.SelectedIndex = Settings.profileButton;
      cbMenuButton.SelectedIndex = Settings.menuButton;
    }

    #endregion

    #region Effect Changing Events

    private void cbVideo_SelectedIndexChanged(object sender, EventArgs e)
    {
      Settings.effectVideo =
        (ContentEffect) Enum.Parse(typeof (ContentEffect), Localization.ReverseTranslate("ContentEffect", cbVideo.Text));
    }

    private void cbMusic_SelectedIndexChanged(object sender, EventArgs e)
    {
      Settings.effectMusic =
        (ContentEffect) Enum.Parse(typeof (ContentEffect), Localization.ReverseTranslate("ContentEffect", cbMusic.Text));
    }

    private void cbRadio_SelectedIndexChanged(object sender, EventArgs e)
    {
      Settings.effectRadio =
        (ContentEffect) Enum.Parse(typeof (ContentEffect), Localization.ReverseTranslate("ContentEffect", cbRadio.Text));
    }

    private void cbMenu_SelectedIndexChanged(object sender, EventArgs e)
    {
      Settings.effectMenu =
        (ContentEffect) Enum.Parse(typeof (ContentEffect), Localization.ReverseTranslate("ContentEffect", cbMenu.Text));
    }

    private void cbMPExit_SelectedIndexChanged(object sender, EventArgs e)
    {
      Settings.effectMPExit =
        (ContentEffect)
          Enum.Parse(typeof (ContentEffect), Localization.ReverseTranslate("ContentEffect", cbMPExit.Text));
    }

    #endregion

    #region AtmoWin Button Event

    private void btnSelectFile_Click(object sender, EventArgs e)
    {
      if (openFileDialog1.ShowDialog() == DialogResult.OK)
      {
        string filenameNoExtension = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
        string filename = filenameNoExtension.ToLower();
        if (filename == "atmowina")
        {
          edFileAtmoWin.Text = openFileDialog1.FileName;
        }
        else
        {
          MessageBox.Show(Localization.Translate("Common", "ErrorFile").Replace("[Filename]", "AtmoWinA.exe"),
            Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
          edFileAtmoWin.Text = "";
          return;
        }
      }
    }

    #endregion

    #region AmbiBox Button Event

    private void btnSelectFileAmbiBox_Click(object sender, EventArgs e)
    {
      if (openFileDialog5.ShowDialog() == DialogResult.OK)
      {
        tbAmbiBoxPath.Text = openFileDialog5.FileName;
      }
    }

    #endregion

    #region Hue Button Event

    private void btnSelectFileHue_Click(object sender, EventArgs e)
    {
      if (openFileDialog4.ShowDialog() == DialogResult.OK)
      {
        string filenameNoExtension = Path.GetFileNameWithoutExtension(openFileDialog4.FileName);
        string filename = filenameNoExtension.ToLower();
        if (filename == "atmohue")
        {
          edFileHue.Text = openFileDialog4.FileName;
        }
        else
        {
          MessageBox.Show(Localization.Translate("Common", "ErrorFile").Replace("[Filename]", "AtmoHue.exe"),
            Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
          edFileHue.Text = "";
          return;
        }
      }
    }

    #endregion

    #region AtmoOrb ListBox

    private void btnAtmoOrbAdd_Click(object sender, EventArgs e)
    {
      if (lbAtmoOrbLamps.Items.Contains(tbAtmoOrbID.Text))
      {
        btnAtmoOrbUpdate_Click(sender, e);
        return;
      }
      int minValue, maxValue;
      string lampString = "";
      lampString += tbAtmoOrbID.Text + ",";
      if (rbAtmoOrbTCP.Checked)
      {
        if (validatorIPAdress(tbAtmoOrbIP.Text) == false)
        {
          MessageBox.Show(Localization.Translate("Common", "ErrorIP") + " - [" + lblAtmoOrbIP.Text + "]",
            Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
        minValue = 1;
        maxValue = 65535;
        if (validatorInt(tbAtmoOrbPort.Text, minValue, maxValue, true) == false)
        {
          MessageBox.Show(
            Localization.Translate("Common", "ErrorInvalidNumberRange")
              .Replace("[minInteger]", minValue.ToString())
              .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoOrbPort.Text + "]",
            Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
        lampString += "TCP," + tbAtmoOrbIP.Text + "," + tbAtmoOrbPort.Text + ",";
      }
      else if (rbAtmoOrbUDP.Checked)
      {
        if (cbAtmoOrbProtocol.Text == "IP" || cbAtmoOrbProtocol.Text == "Multicast")
        {
          if (validatorIPAdress(tbAtmoOrbIP.Text) == false)
          {
            MessageBox.Show(Localization.Translate("Common", "ErrorIP") + " - [" + lblAtmoOrbIP.Text + "]",
              Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
          }

          minValue = 1;
          maxValue = 65535;
          if (validatorInt(tbAtmoOrbPort.Text, minValue, maxValue, true) == false)
          {
            MessageBox.Show(
              Localization.Translate("Common", "ErrorInvalidNumberRange")
                .Replace("[minInteger]", minValue.ToString())
                .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoOrbPort.Text + "]",
              Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
          }
          lampString += String.Format("UDP_{0}", cbAtmoOrbProtocol.Text) + "," +tbAtmoOrbIP.Text + "," + tbAtmoOrbPort.Text + ",";
        }
        else if (cbAtmoOrbProtocol.Text == "Broadcast")
        {
          lampString += string.Format("UDP_{0}", cbAtmoOrbProtocol.Text) + ",";
        }
      }

      if (validatorString(cbAtmoOrbProtocol.Text, 1) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidString") + " - [" + lblAtmoOrbPort.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      minValue = 1;
      maxValue = 255;
      if (validatorInt(tbAtmoOrbID.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoOrbID.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      minValue = 0;
      maxValue = 100;
      if (validatorInt(tbAtmoOrbHScanStart.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoOrbHScan.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      if (validatorInt(tbAtmoOrbHScanEnd.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoOrbHScan.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      if (validatorInt(tbAtmoOrbVScanStart.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoOrbVScan.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      if (validatorInt(tbAtmoOrbVScanEnd.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoOrbVScan.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      if (validatorInt(tbAtmoOrbLedCount.Text, 0, 5000, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoOrbLedCount.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      lampString += tbAtmoOrbHScanStart.Text + "," + tbAtmoOrbHScanEnd.Text + "," + tbAtmoOrbVScanStart.Text + "," +
                    tbAtmoOrbVScanEnd.Text + ",";
      lampString += cbAtmoOrbInvertZone.Checked + ",";
      lampString += tbAtmoOrbLedCount.Text + ",";
      lampString += cbAtmoOrbProtocol.Text;
      Settings.atmoOrbLamps.Add(lampString);
      lbAtmoOrbLamps.Items.Add(tbAtmoOrbID.Text);
    }

    private void btnAtmoOrbRemove_Click(object sender, EventArgs e)
    {
      if (lbAtmoOrbLamps.SelectedIndex >= 0)
      {
        Settings.atmoOrbLamps.RemoveAt(lbAtmoOrbLamps.Items.IndexOf(lbAtmoOrbLamps.SelectedItem.ToString()));
        lbAtmoOrbLamps.Items.RemoveAt(lbAtmoOrbLamps.Items.IndexOf(lbAtmoOrbLamps.SelectedItem.ToString()));
      }
    }

    private void btnAtmoOrbUpdate_Click(object sender, EventArgs e)
    {
      if (lbAtmoOrbLamps.Items.IndexOf(tbAtmoOrbID.Text) < 0)
      {
        btnAtmoOrbAdd_Click(sender, e);
        return;
      }
      int minValue, maxValue;
      string lampString = "";
      lampString += tbAtmoOrbID.Text + ",";

      if (validatorString(cbAtmoOrbProtocol.Text, 1) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidString") + " - [" + lblAtmoOrbPort.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      if (rbAtmoOrbTCP.Checked)
      {
        if (validatorIPAdress(tbAtmoOrbIP.Text) == false)
        {
          MessageBox.Show(Localization.Translate("Common", "ErrorIP") + " - [" + lblAtmoOrbIP.Text + "]",
            Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
        minValue = 1;
        maxValue = 65535;
        if (validatorInt(tbAtmoOrbPort.Text, minValue, maxValue, true) == false)
        {
          MessageBox.Show(
            Localization.Translate("Common", "ErrorInvalidNumberRange")
              .Replace("[minInteger]", minValue.ToString())
              .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoOrbPort.Text + "]",
            Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
        }
        lampString += "TCP," + tbAtmoOrbIP.Text + "," + tbAtmoOrbPort.Text + ",";
      }
      else if (rbAtmoOrbUDP.Checked)
      {
        if (cbAtmoOrbProtocol.Text == "IP" || cbAtmoOrbProtocol.Text == "Multicast")
        {
          if (validatorIPAdress(tbAtmoOrbIP.Text) == false)
          {
            MessageBox.Show(Localization.Translate("Common", "ErrorIP") + " - [" + lblAtmoOrbIP.Text + "]",
              Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
          }

          minValue = 1;
          maxValue = 65535;
          if (validatorInt(tbAtmoOrbPort.Text, minValue, maxValue, true) == false)
          {
            MessageBox.Show(
              Localization.Translate("Common", "ErrorInvalidNumberRange")
                .Replace("[minInteger]", minValue.ToString())
                .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoOrbPort.Text + "]",
              Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
          }
          lampString += String.Format("UDP_{0}", cbAtmoOrbProtocol.Text) + "," + tbAtmoOrbIP.Text + "," + tbAtmoOrbPort.Text + ",";
        }
        else if (cbAtmoOrbProtocol.Text == "Broadcast")
        {
          lampString += string.Format("UDP_{0}", cbAtmoOrbProtocol.Text) + ",";
        }
      }
      minValue = 1;
      maxValue = 255;
      if (validatorInt(tbAtmoOrbID.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoOrbID.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      minValue = 0;
      maxValue = 100;
      if (validatorInt(tbAtmoOrbHScanStart.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoOrbHScan.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      if (validatorInt(tbAtmoOrbHScanEnd.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoOrbHScan.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      if (validatorInt(tbAtmoOrbVScanStart.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoOrbVScan.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      if (validatorInt(tbAtmoOrbVScanEnd.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(
          Localization.Translate("Common", "ErrorInvalidNumberRange")
            .Replace("[minInteger]", minValue.ToString())
            .Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblAtmoOrbVScan.Text + "]",
          Localization.Translate("Common", "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      lampString += tbAtmoOrbHScanStart.Text + "," + tbAtmoOrbHScanEnd.Text + "," + tbAtmoOrbVScanStart.Text + "," +
                    tbAtmoOrbVScanEnd.Text + ",";
      lampString += cbAtmoOrbInvertZone.Checked + ",";
      lampString += tbAtmoOrbLedCount.Text + ",";
      lampString += cbAtmoOrbProtocol.Text;
      Settings.atmoOrbLamps[lbAtmoOrbLamps.Items.IndexOf(tbAtmoOrbID.Text)] = lampString;
    }

    private void lbAtmoOrbLamps_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (lbAtmoOrbLamps.SelectedIndex >= 0)
      {
        atmoOrbSelectedIndexChangedHelper = true;
        string[] lampSettings = Settings.atmoOrbLamps[lbAtmoOrbLamps.SelectedIndex].Split(',');
        tbAtmoOrbID.Text = lampSettings[0];
        if (lampSettings[1] == "UDP_IP")
        {
          rbAtmoOrbUDP.Checked = true;
          rbAtmoOrbTCP.Checked = false;
          tbAtmoOrbIP.ReadOnly = false;
          tbAtmoOrbPort.ReadOnly = false;
          tbAtmoOrbIP.Text = lampSettings[2];
          tbAtmoOrbPort.Text = lampSettings[3];
          tbAtmoOrbHScanStart.Text = lampSettings[4];
          tbAtmoOrbHScanEnd.Text = lampSettings[5];
          tbAtmoOrbVScanStart.Text = lampSettings[6];
          tbAtmoOrbVScanEnd.Text = lampSettings[7];
          cbAtmoOrbInvertZone.Checked = bool.Parse(lampSettings[8]);

          // Legacy checks
          try
          {
            tbAtmoOrbLedCount.Text = lampSettings[9];
            cbAtmoOrbProtocol.Text = lampSettings[10];
          }
          catch (Exception)
          {
            tbAtmoOrbLedCount.Text = "24";
            cbAtmoOrbProtocol.Text = "IP";
          }
        }
        if (lampSettings[1] == "UDP_Multicast")
        {
          rbAtmoOrbUDP.Checked = true;
          rbAtmoOrbTCP.Checked = false;
          tbAtmoOrbIP.ReadOnly = false;
          tbAtmoOrbPort.ReadOnly = false;
          tbAtmoOrbIP.Text = lampSettings[2];
          tbAtmoOrbPort.Text = lampSettings[3];
          tbAtmoOrbHScanStart.Text = lampSettings[4];
          tbAtmoOrbHScanEnd.Text = lampSettings[5];
          tbAtmoOrbVScanStart.Text = lampSettings[6];
          tbAtmoOrbVScanEnd.Text = lampSettings[7];
          cbAtmoOrbInvertZone.Checked = bool.Parse(lampSettings[8]);

          // Legacy checks
          try
          {
            tbAtmoOrbLedCount.Text = lampSettings[9];
            cbAtmoOrbProtocol.Text = lampSettings[10];
          }
          catch (Exception)
          {
            tbAtmoOrbLedCount.Text = "24";
            cbAtmoOrbProtocol.Text = "Multicast";
          }
        }
        if (lampSettings[1] == "UDP_Broadcast")
        {
          rbAtmoOrbUDP.Checked = true;
          rbAtmoOrbTCP.Checked = false;
          tbAtmoOrbIP.ReadOnly = true;
          tbAtmoOrbPort.ReadOnly = true;
          tbAtmoOrbIP.Text = "";
          tbAtmoOrbPort.Text = "";
          tbAtmoOrbHScanStart.Text = lampSettings[2];
          tbAtmoOrbHScanEnd.Text = lampSettings[3];
          tbAtmoOrbVScanStart.Text = lampSettings[4];
          tbAtmoOrbVScanEnd.Text = lampSettings[5];
          cbAtmoOrbInvertZone.Checked = bool.Parse(lampSettings[6]);
          tbAtmoOrbLedCount.Text = lampSettings[7];
          cbAtmoOrbProtocol.Text = lampSettings[8];

          // Legacy checks
          try
          {
            tbAtmoOrbLedCount.Text = lampSettings[7];
            cbAtmoOrbProtocol.Text = lampSettings[8];
          }
          catch (Exception)
          {
            tbAtmoOrbLedCount.Text = "24";
            cbAtmoOrbProtocol.Text = "Broadcast";
          }
        }
        else if (lampSettings[1] == "TCP")
        {
          rbAtmoOrbUDP.Checked = false;
          rbAtmoOrbTCP.Checked = true;
          tbAtmoOrbIP.ReadOnly = false;
          tbAtmoOrbPort.ReadOnly = false;
          tbAtmoOrbIP.Text = lampSettings[2];
          tbAtmoOrbPort.Text = lampSettings[3];
          tbAtmoOrbHScanStart.Text = lampSettings[4];
          tbAtmoOrbHScanEnd.Text = lampSettings[5];
          tbAtmoOrbVScanStart.Text = lampSettings[6];
          tbAtmoOrbVScanEnd.Text = lampSettings[7];
          cbAtmoOrbInvertZone.Checked = bool.Parse(lampSettings[8]);

          // Legacy checks
          try
          {
            tbAtmoOrbLedCount.Text = lampSettings[9];
            cbAtmoOrbProtocol.Text = lampSettings[10];
          }
          catch (Exception)
          {
            tbAtmoOrbLedCount.Text = "24";
            cbAtmoOrbProtocol.Text = "IP";
          }
        }
      }
      atmoOrbSelectedIndexChangedHelper = false;
    }

    private void rbAtmoOrbUDPTCP_CheckedChanged(object sender, EventArgs e)
    {
      RadioButton rb = sender as RadioButton;
      if (rb != null)
      {
        if (rb.Checked)
        {
          if (rb.Text == "TCP")
          {
            tbAtmoOrbIP.ReadOnly = false;
            tbAtmoOrbPort.ReadOnly = false;
            cbAtmoOrbProtocol.Text = "IP";
            cbAtmoOrbProtocol.Enabled = false;
            tbAtmoOrbLedCount.Text = "24";
            tbAtmoOrbHScanStart.Text = "0";
            tbAtmoOrbHScanEnd.Text = "100";
            tbAtmoOrbVScanStart.Text = "0";
            tbAtmoOrbVScanEnd.Text = "100";
          }
          else if (rb.Text == "UDP")
          {
            if (cbAtmoOrbProtocol.Text == "IP")
            {
              tbAtmoOrbIP.ReadOnly = false;
              tbAtmoOrbPort.ReadOnly = false;
              cbAtmoOrbProtocol.Enabled = true;
              tbAtmoOrbLedCount.Text = "24";
              tbAtmoOrbHScanStart.Text = "0";
              tbAtmoOrbHScanEnd.Text = "100";
              tbAtmoOrbVScanStart.Text = "0";
              tbAtmoOrbVScanEnd.Text = "100";
            }
            else if (cbAtmoOrbProtocol.Text == "Broadcast")
            {
              tbAtmoOrbIP.ReadOnly = true;
              tbAtmoOrbPort.ReadOnly = true;
              cbAtmoOrbProtocol.Enabled = true;
              tbAtmoOrbIP.Text = "";
              tbAtmoOrbPort.Text = "";
              tbAtmoOrbLedCount.Text = "24";
              tbAtmoOrbHScanStart.Text = "0";
              tbAtmoOrbHScanEnd.Text = "100";
              tbAtmoOrbVScanStart.Text = "0";
              tbAtmoOrbVScanEnd.Text = "100";
            }
            else if (cbAtmoOrbProtocol.Text == "Multicast")
            {
              tbAtmoOrbIP.ReadOnly = false;
              tbAtmoOrbPort.ReadOnly = false;
              cbAtmoOrbProtocol.Enabled = true;
              tbAtmoOrbIP.Text = "239.15.18.2";
              tbAtmoOrbPort.Text = "49692";
              tbAtmoOrbLedCount.Text = "24";
              tbAtmoOrbHScanStart.Text = "0";
              tbAtmoOrbHScanEnd.Text = "100";
              tbAtmoOrbVScanStart.Text = "0";
              tbAtmoOrbVScanEnd.Text = "100";
            }
          }
        }
      }
    }
    private void cbAtmoOrbProtocolType_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (atmoOrbSelectedIndexChangedHelper)
      {
        return;
      }
      if (cbAtmoOrbProtocol.Text == "IP")
      {
        tbAtmoOrbIP.ReadOnly = false;
        tbAtmoOrbPort.ReadOnly = false;
        tbAtmoOrbLedCount.Text = "24";
        tbAtmoOrbHScanStart.Text = "0";
        tbAtmoOrbHScanEnd.Text = "100";
        tbAtmoOrbVScanStart.Text = "0";
        tbAtmoOrbVScanEnd.Text = "100";
      }
      else if (cbAtmoOrbProtocol.Text == "Broadcast")
      {
        tbAtmoOrbIP.ReadOnly = true;
        tbAtmoOrbPort.ReadOnly = true;
        tbAtmoOrbIP.Text = "";
        tbAtmoOrbPort.Text = "";
        tbAtmoOrbLedCount.Text = "24";
        tbAtmoOrbHScanStart.Text = "0";
        tbAtmoOrbHScanEnd.Text = "100";
        tbAtmoOrbVScanStart.Text = "0";
        tbAtmoOrbVScanEnd.Text = "100";
      }
      else if (cbAtmoOrbProtocol.Text == "Multicast")
      {
        tbAtmoOrbIP.ReadOnly = false;
        tbAtmoOrbPort.ReadOnly = false;
        tbAtmoOrbIP.Text = "239.15.18.2";
        tbAtmoOrbPort.Text = "49692";
        tbAtmoOrbLedCount.Text = "24";
        tbAtmoOrbHScanStart.Text = "0";
        tbAtmoOrbHScanEnd.Text = "100";
        tbAtmoOrbVScanStart.Text = "0";
        tbAtmoOrbVScanEnd.Text = "100";
      }
    }

    private void cbAtmoOrbProtocolType_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {

    }
    #endregion

  }
}
