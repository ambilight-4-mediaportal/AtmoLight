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
      comboBox1.SelectedIndex = (int)AtmolightSettings.killbutton;
      comboBox2.SelectedIndex = (int)AtmolightSettings.cmbutton;
      cbMenuButton.SelectedIndex = (int)AtmolightSettings.menubutton;
      edExcludeStart.Text = AtmolightSettings.excludeTimeStart.ToString("HH:mm");
      edExcludeEnd.Text = AtmolightSettings.excludeTimeEnd.ToString("HH:mm");
      lowCpuTime.Text = AtmolightSettings.lowCPUTime.ToString();
      tbRed.Text = AtmolightSettings.StaticColorRed.ToString();
      tbGreen.Text = AtmolightSettings.StaticColorGreen.ToString();
      tbBlue.Text = AtmolightSettings.StaticColorBlue.ToString();

      if (AtmolightSettings.OffOnStart)
        ckOnMediaStart.Checked = true;
      else
        this.ckOnMediaStart.Checked = false;

      if (AtmolightSettings.lowCPU)
        ckLowCpu.Checked = true;
      else
        this.ckLowCpu.Checked = false;

      if (AtmolightSettings.startAtmoWin)
        ckStartAtmoWin.Checked = true;
      else
        ckStartAtmoWin.Checked = false;

      if (AtmolightSettings.exitAtmoWin)
        ckExitAtmoWin.Checked = true;
      else
        ckExitAtmoWin.Checked = false;

      if (AtmolightSettings.disableOnShutdown)
        rbDisableLEDs.Checked = true;
      else
        rbSwitchToLiveView.Checked = true;
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
      lblStart.Text = LanguageLoader.appStrings.SetupForm_lblStartText;
      lblEnd.Text = LanguageLoader.appStrings.SetupForm_lblEndText;
      grpDeactivate.Text = LanguageLoader.appStrings.SetupForm_grpDeactivateText;
        lblMenu.Text = LanguageLoader.appStrings.SetupForm_lblMenu;
        grpStaticColor.Text = LanguageLoader.appStrings.SetupForm_grpStaticColor;
        lblRed.Text = LanguageLoader.appStrings.SetupForm_lblRed;
        lblGreen.Text = LanguageLoader.appStrings.SetupForm_lblGreen;
        lblBlue.Text = LanguageLoader.appStrings.SetupForm_lblBlue;
        lblMenuButton.Text = LanguageLoader.appStrings.SetupForm_lblMenuButton;
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
          MessageBox.Show("You have to enter a invalid File, should be AtmoWinA", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        MessageBox.Show("You have to enter a valid start time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      AtmolightSettings.excludeTimeStart = dt;
      DateTime dt2;
      if (!DateTime.TryParse(edExcludeEnd.Text, out dt2))
      {
        MessageBox.Show("You have to enter a valid end time", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      int cTime;
      if (!int.TryParse(lowCpuTime.Text, out cTime))
      {
        MessageBox.Show("You have to enter a valid number of ms", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      int StaticColorRed, StaticColorGreen, StaticColorBlue;
      if ((!int.TryParse(tbRed.Text, out StaticColorRed)) || (StaticColorRed < 0 || StaticColorRed > 255))
      {
          MessageBox.Show("Please enter a number between 0 and 255 for Red.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
      }
      else if ((!int.TryParse(tbGreen.Text, out StaticColorGreen)) || (StaticColorGreen < 0 || StaticColorGreen > 255))
      {
          MessageBox.Show("Please enter a number between 0 and 255 for Green.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
      }
      else if ((!int.TryParse(tbBlue.Text, out StaticColorBlue)) || (StaticColorBlue < 0 || StaticColorBlue > 255))
      {
          MessageBox.Show("Please enter a number between 0 and 255 for Blue.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
      }

      if ((cbMenuButton.SelectedIndex == comboBox1.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
          (cbMenuButton.SelectedIndex == comboBox2.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
          (comboBox1.SelectedIndex == comboBox2.SelectedIndex) && (comboBox1.SelectedIndex != 4))
      {
          MessageBox.Show("You cant use the same remote key for more than one task.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          return;
      }

      AtmolightSettings.StaticColorRed = StaticColorRed;
      AtmolightSettings.StaticColorGreen = StaticColorGreen;
      AtmolightSettings.StaticColorBlue = StaticColorBlue;
      AtmolightSettings.lowCPUTime = cTime;
      AtmolightSettings.excludeTimeEnd = dt2;
      AtmolightSettings.atmowinExe = edFile.Text;
      AtmolightSettings.effectVideo = (ContentEffect)cbVideo.SelectedIndex;
      AtmolightSettings.effectMusic = (ContentEffect)cbMusic.SelectedIndex;
      AtmolightSettings.effectRadio = (ContentEffect)cbRadio.SelectedIndex;
      AtmolightSettings.effectMenu = (ContentEffect)cbMenu.SelectedIndex;
      AtmolightSettings.killbutton = comboBox1.SelectedIndex;
      AtmolightSettings.cmbutton = comboBox2.SelectedIndex;
      AtmolightSettings.menubutton = cbMenuButton.SelectedIndex;
      AtmolightSettings.disableOnShutdown = rbDisableLEDs.Checked;
      AtmolightSettings.enableInternalLiveView = rbSwitchToLiveView.Checked;
      AtmolightSettings.OffOnStart = ckOnMediaStart.Checked;
      AtmolightSettings.lowCPU = ckLowCpu.Checked;
      AtmolightSettings.startAtmoWin = ckStartAtmoWin.Checked;
      AtmolightSettings.exitAtmoWin = ckExitAtmoWin.Checked;
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
