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
      tbHueIP.Text = Settings.hueIP;
      tbHuePort.Text = Settings.huePort.ToString();

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
      if (Settings.hueTarget)
      {
        ckHueEnabled.Checked = true;
      }
      else
      {
        ckHueEnabled.Checked = false;
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
      lblHintHue.Text = LanguageLoader.appStrings.SetupForm_lblHintHue;
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
      lblHueIP.Text = LanguageLoader.appStrings.SetupForm_lblHueIP;
      lblHuePort.Text = LanguageLoader.appStrings.SetupForm_lblHuePort;
      lblMPExit.Text = LanguageLoader.appStrings.SetupForm_lblMPExit;
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
      //Validate user input


      //Time excluded Start
      if (validatorDateTime(edExcludeStart.Text) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorStartTime + " - ["+lblStart.Text+"]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Time excluded Stop
      if (validatorDateTime(edExcludeEnd.Text) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorEndTime + " - [" + lblEnd.Text + "]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Low CPU
      if(validatorInt(lowCpuTime.Text,1,0,false) == false)
      {
        if (ckLowCpu.Checked)
        {
          MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorMiliseconds + " - [" + ckLowCpu.Text + "]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
          MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorMiliseconds + " - [" + ckDelay.Text + "]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
          MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorMiliseconds + " - [" + lblRefreshRate.Text + "]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
          MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorMiliseconds + " - [" + ckBlackbarDetection.Text + "]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorRed + " - [" + lblRed.Text + "]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Static color GREEN
      if (validatorInt(tbGreen.Text, 0, 255, true) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorGreen + " - [" + lblGreen.Text + "]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Static color BLUE
      if (validatorInt(tbBlue.Text, 0, 255, true) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorBlue + " - [" + lblBlue.Text + "]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Menu buttons
      if ((cbMenuButton.SelectedIndex == comboBox1.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
          (cbMenuButton.SelectedIndex == comboBox2.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
          (comboBox1.SelectedIndex == comboBox2.SelectedIndex) && (comboBox1.SelectedIndex != 4))
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorRemoteButtons, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //GIF path
      if (validatorPath(tbGIF.Text) == false && string.IsNullOrEmpty(tbGIF.Text) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidPath + " - [" + grpGIF.Text + "]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Atmowin path
      if (validatorPath(edFile.Text) == false && string.IsNullOrEmpty(edFile.Text) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidPath + " - [" + lblPathInfo.Text + "]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Hyperion IP
      if (validatorIPAdress(tbHyperionIP.Text) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIP + " - [" + lblHyperionIP.Text + "]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }
      //Hue IP
      if (validatorIPAdress(tbHueIP.Text) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIP + " - [" + lblHueIP.Text + "]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIntegerStarting.Replace("[minInteger]", minValue.ToString()) + " - [" + lblCaptureWidth.Text + "]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Capture height
      minValue = 1;
      maxValue = 0;
      if (validatorInt(tbCaptureHeight.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIntegerStarting.Replace("[minInteger]", minValue.ToString()) + " - [" + lblCaptureHeight.Text + "]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Hyperion port
      minValue = 1;
      maxValue = 65535;
      if (validatorInt(tbHyperionPort.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIntegerBetween.Replace("[minInteger]", minValue.ToString()).Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblHyperionPort.Text + "]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Hyperion reconnect attempts
      minValue = 1;
      maxValue = 0;
      if (validatorInt(tbHyperionReconnectAttempts.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIntegerStarting.Replace("[minInteger]", minValue.ToString()) + " - ["+ lblHyperionReconnectAttempts.Text +"]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Hyperion priority
      minValue = 1;
      maxValue = 0;
      if (validatorInt(tbHyperionPriority.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIntegerStarting.Replace("[minInteger]", minValue.ToString()) + " - ["+ lblHyperionPriority.Text +"]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Hyperion priority static color
      minValue = 1;
      maxValue = 0;
      if (validatorInt(tbHyperionPriorityStaticColor.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIntegerStarting.Replace("[minInteger]", minValue.ToString()) + " - ["+ lblHyperionPriorityStaticColor.Text +"]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      //Hue port
      minValue = 1;
      maxValue = 65535;
      if (validatorInt(tbHuePort.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIntegerBetween.Replace("[minInteger]", minValue.ToString()).Replace("[maxInteger]", maxValue.ToString()) + " - [" + lblHueIP.Text + "]", LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }


      Settings.staticColorRed = int.Parse(tbRed.Text);
      Settings.staticColorGreen = int.Parse(tbGreen.Text);
      Settings.staticColorBlue = int.Parse(tbBlue.Text);
      Settings.atmowinExe = edFile.Text;
      Settings.effectVideo = (ContentEffect)cbVideo.SelectedIndex;
      Settings.effectMusic = (ContentEffect)cbMusic.SelectedIndex;
      Settings.effectRadio = (ContentEffect)cbRadio.SelectedIndex;
      Settings.effectMenu = (ContentEffect)cbMenu.SelectedIndex;
      Settings.effectMPExit = (ContentEffect)(cbMPExit.SelectedIndex == 4 ? 5 : cbMPExit.SelectedIndex);
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
      Settings.restartOnError = ckRestartOnError.Checked;
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
      Settings.HyperionPriorityStaticColor = int.Parse(tbHyperionPriorityStaticColor.Text);
      Settings.HyperionLiveReconnect = ckHyperionLiveReconnect.Checked;
      Settings.captureWidth = int.Parse(tbCaptureWidth.Text);
      Settings.captureHeight = int.Parse(tbCaptureHeight.Text);
      Settings.hueIP = tbHueIP.Text;
      Settings.huePort = int.Parse(tbHuePort.Text);
      Settings.atmoWinTarget = ckAtmowinEnabled.Checked;
      Settings.hueTarget = ckHueEnabled.Checked;
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

    #region input validators
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
      catch { };

      return IsValid;
    }
    private Boolean validatorRGB(string color, string range)
    {

      Boolean IsValid = false;
      return IsValid;
    }
    #endregion

    private void lowCpuTime_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 0;
      if (validatorInt(lowCpuTime.Text, minValue, maxValue, false) == false)
      {
        if (ckLowCpu.Checked)
        {
          MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorMiliseconds, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
          MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorMiliseconds, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
          MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorRefreshRate, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
          MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorMiliseconds, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
    }

    private void tbCaptureWidth_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 0;
      if (validatorInt(tbCaptureWidth.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIntegerStarting.Replace("[minInteger]", minValue.ToString()), LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbCaptureHeight_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 0;
      if (validatorInt(tbCaptureHeight.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIntegerStarting.Replace("[minInteger]", minValue.ToString()), LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void edExcludeStart_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (validatorDateTime(edExcludeStart.Text) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorStartTime, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

    }

    private void edExcludeEnd_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (validatorDateTime(edExcludeEnd.Text) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorEndTime, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }


    private void tbRed_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 255;
      if (validatorInt(tbRed.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorRed, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

    }

    private void tbGreen_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 255;
      if (validatorInt(tbGreen.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorGreen, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbBlue_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 0;
      int maxValue = 255;
      if (validatorInt(tbBlue.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorBlue, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbGIF_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (validatorPath(tbGIF.Text) == false && string.IsNullOrEmpty(tbGIF.Text) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidPath, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
    private void cbMenuButton_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if ((cbMenuButton.SelectedIndex == comboBox1.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
    (cbMenuButton.SelectedIndex == comboBox2.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
    (comboBox1.SelectedIndex == comboBox2.SelectedIndex) && (comboBox1.SelectedIndex != 4))
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorRemoteButtons, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

    }

    private void comboBox1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if ((cbMenuButton.SelectedIndex == comboBox1.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
    (cbMenuButton.SelectedIndex == comboBox2.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
    (comboBox1.SelectedIndex == comboBox2.SelectedIndex) && (comboBox1.SelectedIndex != 4))
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorRemoteButtons, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

    }

    private void comboBox2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if ((cbMenuButton.SelectedIndex == comboBox1.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
    (cbMenuButton.SelectedIndex == comboBox2.SelectedIndex) && (cbMenuButton.SelectedIndex != 4) ||
    (comboBox1.SelectedIndex == comboBox2.SelectedIndex) && (comboBox1.SelectedIndex != 4))
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorRemoteButtons, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

    }

    private void edFile_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (validatorPath(edFile.Text) == false && string.IsNullOrEmpty(edFile.Text) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidPath, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbHyperionIP_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (validatorIPAdress(tbHyperionIP.Text) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIP, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbHyperionPort_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 65535;
      if (validatorInt(tbHyperionPort.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIntegerBetween.Replace("[minInteger]",minValue.ToString()).Replace("[maxInteger]", maxValue.ToString()), LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbHyperionReconnectDelay_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 10;
      int maxValue = 0;
      if (validatorInt(tbHyperionReconnectDelay.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIntegerStarting.Replace("[minInteger]",minValue.ToString()), LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

    }

    private void tbHyperionReconnectAttempts_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 0;
      if (validatorInt(tbHyperionReconnectAttempts.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIntegerStarting.Replace("[minInteger]", minValue.ToString()), LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbHyperionPriority_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 0;
      if (validatorInt(tbHyperionPriority.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIntegerStarting.Replace("[minInteger]", minValue.ToString()), LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbHyperionPriorityStaticColor_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 0;
      if (validatorInt(tbHyperionPriorityStaticColor.Text, minValue, maxValue, false) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIntegerStarting.Replace("[minInteger]", minValue.ToString()), LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbHueIP_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (validatorIPAdress(tbHueIP.Text) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIP, LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void tbHuePort_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      int minValue = 1;
      int maxValue = 65535;
      if (validatorInt(tbHuePort.Text, minValue, maxValue, true) == false)
      {
        MessageBox.Show(LanguageLoader.appStrings.SetupForm_ErrorInvalidIntegerBetween.Replace("[minInteger]", minValue.ToString()).Replace("[maxInteger]", maxValue.ToString()), LanguageLoader.appStrings.SetupForm_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

    }
  }
}
