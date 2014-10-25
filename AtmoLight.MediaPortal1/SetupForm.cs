using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Language;

namespace AtmoLight
{
  public partial class SetupForm : Form
  {
    public SetupForm()
    {
      InitializeComponent();
      UpdateLanguageOnControls();

      lblVersionVal.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
      edFile.Text = Settings.atmowinExe;
      cbVideo.SelectedIndex = (int)Settings.effectVideo;
      cbMusic.SelectedIndex = (int)Settings.effectMusic;
      cbRadio.SelectedIndex = (int)Settings.effectRadio;
      cbMenu.SelectedIndex = (int)Settings.effectMenu;
      cbMPExit.SelectedIndex = (Settings.effectMPExit == ContentEffect.StaticColor ? 4 : (int)Settings.effectMPExit);
      comboBox1.SelectedIndex = (int)Settings.killButton;
      comboBox2.SelectedIndex = (int)Settings.profileButton;
      cbMenuButton.SelectedIndex = (int)Settings.menuButton;
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
      tbHyperionPriorityStaticColor.Text = Settings.HyperionPriorityStaticColor.ToString();
      tbCaptureWidth.Text = Settings.captureWidth.ToString();
      tbCaptureHeight.Text = Settings.captureHeight.ToString();

      if (Settings.manualMode)
      {
        ckOnMediaStart.Checked = true;
      }
      else
      {
        this.ckOnMediaStart.Checked = false;
      }

      if (Settings.lowCPU)
      {
        ckLowCpu.Checked = true;
      }
      else
      {
        this.ckLowCpu.Checked = false;
      }

      if (Settings.delay)
      {
        ckDelay.Checked = true;
      }
      else
      {
        ckDelay.Checked = false;
      }

      if (Settings.startAtmoWin)
      {
        ckStartAtmoWin.Checked = true;
      }
      else
      {
        ckStartAtmoWin.Checked = false;
      }

      if (Settings.exitAtmoWin)
      {
        ckExitAtmoWin.Checked = true;
      }
      else
      {
        ckExitAtmoWin.Checked = false;
      }

      if (Settings.restartOnError)
      {
        ckRestartOnError.Checked = true;
      }
      else
      {
        ckRestartOnError.Checked = false;
      }

      if (Settings.blackbarDetection)
      {
        ckBlackbarDetection.Checked = true;
      }
      else
      {
        ckBlackbarDetection.Checked = false;
      }
      if (Settings.atmoWinTarget)
      {
        ckAtmowinEnabled.Checked = true;
      }
      else
      {
        ckAtmowinEnabled.Checked = false;
      }
      if (Settings.hyperionTarget)
      {
          ckHyperionEnabled.Checked = true;
      }
      else
      {
          ckHyperionEnabled.Checked = false;
      }
      if (Settings.HyperionLiveReconnect)
      {
        ckHyperionLiveReconnect.Checked = true;
      }
      else
      {
        ckHyperionLiveReconnect.Checked = false;
      }
    }

    private void UpdateLanguageOnControls()
    {
      // this function places language specific text on all "skin-able" text items.
      lblPathInfo.Text = LanguageLoader.appStrings.SetupForm_lblPathInfoText;
      grpMode.Text = LanguageLoader.appStrings.SetupForm_grpModeText;
      grpPluginOption.Text = LanguageLoader.appStrings.SetupForm_grpPluginOptionText;
      lblVidTvRec.Text = LanguageLoader.appStrings.SetupForm_lblVidTvRecText;
      lblMusic.Text = LanguageLoader.appStrings.SetupForm_lblMusicText;
      lblRadio.Text = LanguageLoader.appStrings.SetupForm_lblRadioText;
      lblLedsOnOff.Text = LanguageLoader.appStrings.SetupForm_lblLedsOnOffText;
      lblProfile.Text = LanguageLoader.appStrings.SetupForm_lblProfileText;
      ckOnMediaStart.Text = LanguageLoader.appStrings.SetupForm_ckOnMediaStartText;
      ckLowCpu.Text = LanguageLoader.appStrings.SetupForm_ckLowCpuText;
      ckDelay.Text = LanguageLoader.appStrings.SetupForm_ckDelayText;
      ckStartAtmoWin.Text = LanguageLoader.appStrings.SetupForm_ckStartAtmoWinText;
      ckExitAtmoWin.Text = LanguageLoader.appStrings.SetupForm_ckExitAtmoWinText;
      btnSave.Text = LanguageLoader.appStrings.SetupForm_btnSaveText;
      btnCancel.Text = LanguageLoader.appStrings.SetupForm_btnCancelText;
      btnLanguage.Text = LanguageLoader.appStrings.SetupForm_btnLanguageText;
      lblHintMenuButtons.Text = LanguageLoader.appStrings.SetupForm_lblHintText;
      lblHintHardware.Text = LanguageLoader.appStrings.SetupForm_lblHintHardware;
      lblHintCaptureDimensions.Text = LanguageLoader.appStrings.SetupForm_lblHintCaptureDimensions;
      lblFrames.Text = LanguageLoader.appStrings.SetupForm_lblFramesText;
      lblDelay.Text = LanguageLoader.appStrings.SetupForm_lblDelay;
      lblStart.Text = LanguageLoader.appStrings.SetupForm_lblStartText;
      lblEnd.Text = LanguageLoader.appStrings.SetupForm_lblEndText;
      grpDeactivate.Text = LanguageLoader.appStrings.SetupForm_grpDeactivateText;
      lblMenu.Text = LanguageLoader.appStrings.SetupForm_lblMenu;
      grpStaticColor.Text = LanguageLoader.appStrings.SetupForm_grpStaticColor;
      lblRed.Text = LanguageLoader.appStrings.SetupForm_lblRed;
      lblGreen.Text = LanguageLoader.appStrings.SetupForm_lblGreen;
      lblBlue.Text = LanguageLoader.appStrings.SetupForm_lblBlue;
      lblMenuButton.Text = LanguageLoader.appStrings.SetupForm_lblMenuButton;
      ckRestartOnError.Text = LanguageLoader.appStrings.SetupForm_ckRestartOnError;
      lblRefreshRate.Text = LanguageLoader.appStrings.SetupForm_lblRefreshRate;
      ckBlackbarDetection.Text = LanguageLoader.appStrings.SetupForm_ckBlackbarDetection;
      grpGIF.Text = LanguageLoader.appStrings.SetupForm_grpGIF;
      lblHyperionIP.Text = LanguageLoader.appStrings.SetupForm_lblHyperionIP;
      lblHyperionPort.Text = LanguageLoader.appStrings.SetupForm_lblHyperionPort;
      lblHyperionPriority.Text = LanguageLoader.appStrings.SetupForm_lblHyperionPriorty;
      lblHyperionReconnectDelay.Text = LanguageLoader.appStrings.SetupForm_lblHyperionReconnectDelay;
      lblHyperionReconnectAttempts.Text = LanguageLoader.appStrings.SetupForm_lblHyperionReconnectAttempts;
      lblHyperionPriorityStaticColor.Text = LanguageLoader.appStrings.SetupForm_lblHyperionPriorityStaticColor;
      ckHyperionLiveReconnect.Text = LanguageLoader.appStrings.SetupForm_ckHyperionLiveReconnect;
      lblCaptureWidth.Text = LanguageLoader.appStrings.SetupForm_lblCaptureWidth;
      lblCaptureHeight.Text = LanguageLoader.appStrings.SetupForm_lblCaptureHeight;
      tabPageGeneric.Text = LanguageLoader.appStrings.SetupForm_tabPageGeneric;
      grpTargets.Text = LanguageLoader.appStrings.SetupForm_grpTargets;
      grpAtmowinSettings.Text = LanguageLoader.appStrings.SetupForm_grpAtmowinSettings;
      grpHyperionNetworkSettings.Text = LanguageLoader.appStrings.SetupForm_grpHyperionNetworkSettings;
      grpHyperionPrioritySettings.Text = LanguageLoader.appStrings.SetupForm_grpHyperionPrioritySettings;
      grpCaptureDimensions.Text = LanguageLoader.appStrings.SetupForm_grpCaptureDimensions;
    }

    private void btnSelectFile_Click(object sender, EventArgs e)
    {
      if (openFileDialog1.ShowDialog() == DialogResult.OK)
      {
        string filenameNoExtension = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
        string filename = filenameNoExtension.ToLower();
        if (filename == "atmowina")
        {
          edFile.Text = openFileDialog1.FileName;
        }
        else
        {
          MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorAtmoWinA, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
          edFile.Text = "";
          return;
        }
      }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      DateTime dt;
      if (!DateTime.TryParse(edExcludeStart.Text, out dt))
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorStartTime, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      Settings.excludeTimeStart = dt;
      DateTime dt2;
      if (!DateTime.TryParse(edExcludeEnd.Text, out dt2))
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorEndTime, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      int cTime;
      if (!int.TryParse(lowCpuTime.Text, out cTime) || cTime < 0)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorMiliseconds, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      int cDelay;
      if (!int.TryParse(tbDelay.Text, out cDelay) || cDelay < 0)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorMiliseconds, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      int cRefreshRate;
      if (!int.TryParse(tbRefreshRate.Text, out cRefreshRate) || cRefreshRate <= 0)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorRefreshRate, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      int cBlackbarDetectionTime;
      if (!int.TryParse(tbBlackbarDetectionTime.Text, out cBlackbarDetectionTime) || cBlackbarDetectionTime < 0)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorMiliseconds, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      int StaticColorRed, StaticColorGreen, StaticColorBlue;
      if ((!int.TryParse(tbRed.Text, out StaticColorRed)) || (StaticColorRed < 0 || StaticColorRed > 255))
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorRed, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      else if ((!int.TryParse(tbGreen.Text, out StaticColorGreen)) || (StaticColorGreen < 0 || StaticColorGreen > 255))
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorGreen, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      else if ((!int.TryParse(tbBlue.Text, out StaticColorBlue)) || (StaticColorBlue < 0 || StaticColorBlue > 255))
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorBlue, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      if ((cbMenuButton.SelectedIndex == comboBox1.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
          (cbMenuButton.SelectedIndex == comboBox2.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
          (comboBox1.SelectedIndex == comboBox2.SelectedIndex) && (comboBox1.SelectedIndex != 4))
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorRemoteButtons, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      Settings.staticColorRed = StaticColorRed;
      Settings.staticColorGreen = StaticColorGreen;
      Settings.staticColorBlue = StaticColorBlue;
      Settings.lowCPUTime = cTime;
      Settings.delayReferenceTime = cDelay;
      Settings.delayReferenceRefreshRate = cRefreshRate;
      Settings.excludeTimeEnd = dt2;
      Settings.atmowinExe = edFile.Text;
      Settings.effectVideo = (ContentEffect)cbVideo.SelectedIndex;
      Settings.effectMusic = (ContentEffect)cbMusic.SelectedIndex;
      Settings.effectRadio = (ContentEffect)cbRadio.SelectedIndex;
      Settings.effectMenu = (ContentEffect)cbMenu.SelectedIndex;
      Settings.effectMPExit = (ContentEffect)(cbMPExit.SelectedIndex == 4 ? 5 : cbMPExit.SelectedIndex);
      Settings.killButton = comboBox1.SelectedIndex;
      Settings.profileButton = comboBox2.SelectedIndex;
      Settings.menuButton = cbMenuButton.SelectedIndex;
      Settings.manualMode = ckOnMediaStart.Checked;
      Settings.lowCPU = ckLowCpu.Checked;
      Settings.delay = ckDelay.Checked;
      Settings.startAtmoWin = ckStartAtmoWin.Checked;
      Settings.exitAtmoWin = ckExitAtmoWin.Checked;
      Settings.restartOnError = ckRestartOnError.Checked;
      Settings.blackbarDetection = ckBlackbarDetection.Checked;
      Settings.blackbarDetectionTime = cBlackbarDetectionTime;
      Settings.gifFile = tbGIF.Text;
      Settings.hyperionIP = tbHyperionIP.Text;
      Settings.hyperionPort = int.Parse(tbHyperionPort.Text);
      Settings.hyperionPriority = int.Parse(tbHyperionPriority.Text);
      Settings.hyperionReconnectDelay = int.Parse(tbHyperionReconnectDelay.Text);
      Settings.hyperionReconnectAttempts = int.Parse(tbHyperionReconnectAttempts.Text);
      Settings.HyperionPriorityStaticColor = int.Parse(tbHyperionPriorityStaticColor.Text);
      Settings.HyperionLiveReconnect = ckHyperionLiveReconnect.Checked;
      Settings.captureWidth = int.Parse(tbCaptureWidth.Text);
      Settings.captureHeight = int.Parse(tbCaptureHeight.Text);
      Settings.atmoWinTarget = ckAtmowinEnabled.Checked;
      Settings.hyperionTarget = ckHyperionEnabled.Checked;

      Settings.SaveSettings();
      this.DialogResult = DialogResult.OK;
    }

    private void btnLanguage_Click(object sender, EventArgs e)
    {
      openFileDialog2.InitialDirectory = Path.GetDirectoryName(LanguageLoader.strCurrentLanguageFile);
      if (openFileDialog2.ShowDialog() == DialogResult.OK)
      {
        LanguageLoader.LoadLanguageFile(openFileDialog2.FileName);
        LanguageLoader.strCurrentLanguageFile = openFileDialog2.FileName;
        UpdateLanguageOnControls();
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
  }
}
