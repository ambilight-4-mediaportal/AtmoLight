using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Language;

namespace MediaPortal.ProcessPlugins.Atmolight
{
  public partial class AtmolightSetupForm : Form
  {
    public AtmolightSetupForm()
    {
      InitializeComponent();
      UpdateLanguageOnControls();

      lblVersionVal.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
      edFile.Text = AtmolightSettings.atmowinExe;
      cbVideo.SelectedIndex = (int)AtmolightSettings.effectVideo;
      cbMusic.SelectedIndex = (int)AtmolightSettings.effectMusic;
      cbRadio.SelectedIndex = (int)AtmolightSettings.effectRadio;
      cbMenu.SelectedIndex = (int)AtmolightSettings.effectMenu;
      comboBox1.SelectedIndex = (int)AtmolightSettings.killButton;
      comboBox2.SelectedIndex = (int)AtmolightSettings.profileButton;
      cbMenuButton.SelectedIndex = (int)AtmolightSettings.menuButton;
      edExcludeStart.Text = AtmolightSettings.excludeTimeStart.ToString("HH:mm");
      edExcludeEnd.Text = AtmolightSettings.excludeTimeEnd.ToString("HH:mm");
      lowCpuTime.Text = AtmolightSettings.lowCPUTime.ToString();
      tbDelay.Text = AtmolightSettings.delayReferenceTime.ToString();
      tbRefreshRate.Text = AtmolightSettings.delayReferenceRefreshRate.ToString();
      tbRed.Text = AtmolightSettings.staticColorRed.ToString();
      tbGreen.Text = AtmolightSettings.staticColorGreen.ToString();
      tbBlue.Text = AtmolightSettings.staticColorBlue.ToString();

      if (AtmolightSettings.manualMode)
      {
        ckOnMediaStart.Checked = true;
      }
      else
      {
        this.ckOnMediaStart.Checked = false;
      }

      if (AtmolightSettings.lowCPU)
      {
        ckLowCpu.Checked = true;
      }
      else
      {
        this.ckLowCpu.Checked = false;
      }

      if (AtmolightSettings.delay)
      {
        ckDelay.Checked = true;
      }
      else
      {
        ckDelay.Checked = false;
      }

      if (AtmolightSettings.startAtmoWin)
      {
        ckStartAtmoWin.Checked = true;
      }
      else
      {
        ckStartAtmoWin.Checked = false;
      }

      if (AtmolightSettings.exitAtmoWin)
      {
        ckExitAtmoWin.Checked = true;
      }
      else
      {
        ckExitAtmoWin.Checked = false;
      }

      if (AtmolightSettings.disableOnShutdown)
      {
        rbDisableLEDs.Checked = true;
      }
      else
      {
        rbSwitchToLiveView.Checked = true;
      }

      if (AtmolightSettings.restartOnError)
      {
        ckRestartOnError.Checked = true;
      }
      else
      {
        ckRestartOnError.Checked = false;
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
      grpMPClose.Text = LanguageLoader.appStrings.SetupForm_grpMPCloseText;
      rbSwitchToLiveView.Text = LanguageLoader.appStrings.SetupForm_rbSwitchToLiveViewText;
      rbDisableLEDs.Text = LanguageLoader.appStrings.SetupForm_rbDisableLEDsText;
      btnSave.Text = LanguageLoader.appStrings.SetupForm_btnSaveText;
      btnCancel.Text = LanguageLoader.appStrings.SetupForm_btnCancelText;
      btnLanguage.Text = LanguageLoader.appStrings.SetupForm_btnLanguageText;
      lblHint.Text = LanguageLoader.appStrings.SetupForm_lblHintText;
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

      AtmolightSettings.excludeTimeStart = dt;
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

      AtmolightSettings.staticColorRed = StaticColorRed;
      AtmolightSettings.staticColorGreen = StaticColorGreen;
      AtmolightSettings.staticColorBlue = StaticColorBlue;
      AtmolightSettings.lowCPUTime = cTime;
      AtmolightSettings.delayReferenceTime = cDelay;
      AtmolightSettings.delayReferenceRefreshRate = cRefreshRate;
      AtmolightSettings.excludeTimeEnd = dt2;
      AtmolightSettings.atmowinExe = edFile.Text;
      AtmolightSettings.effectVideo = (ContentEffect)cbVideo.SelectedIndex;
      AtmolightSettings.effectMusic = (ContentEffect)cbMusic.SelectedIndex;
      AtmolightSettings.effectRadio = (ContentEffect)cbRadio.SelectedIndex;
      AtmolightSettings.effectMenu = (ContentEffect)cbMenu.SelectedIndex;
      AtmolightSettings.killButton = comboBox1.SelectedIndex;
      AtmolightSettings.profileButton = comboBox2.SelectedIndex;
      AtmolightSettings.menuButton = cbMenuButton.SelectedIndex;
      AtmolightSettings.disableOnShutdown = rbDisableLEDs.Checked;
      AtmolightSettings.enableInternalLiveView = rbSwitchToLiveView.Checked;
      AtmolightSettings.manualMode = ckOnMediaStart.Checked;
      AtmolightSettings.lowCPU = ckLowCpu.Checked;
      AtmolightSettings.delay = ckDelay.Checked;
      AtmolightSettings.startAtmoWin = ckStartAtmoWin.Checked;
      AtmolightSettings.exitAtmoWin = ckExitAtmoWin.Checked;
      AtmolightSettings.restartOnError = ckRestartOnError.Checked;
      AtmolightSettings.SaveSettings();
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
  }
}
